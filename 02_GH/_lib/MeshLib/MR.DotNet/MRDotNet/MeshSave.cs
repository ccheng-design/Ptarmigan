using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshSave
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrStringData(IntPtr str);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshSaveSceneToObj(IntPtr objects, ulong objectsNum, string file, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrLoadIOExtras();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private unsafe static extern void mrMeshSaveToAnySupportedFormat(IntPtr mesh, string file, IntPtr* errorStr);

	public unsafe static void ToAnySupportedFormat(Mesh mesh, string path)
	{
		mrLoadIOExtras();
		IntPtr intPtr = default(IntPtr);
		mrMeshSaveToAnySupportedFormat(mesh.mesh_, path, &intPtr);
		if (intPtr != IntPtr.Zero)
		{
			throw new SystemException(Marshal.PtrToStringAnsi(mrStringData(intPtr)));
		}
	}

	public static void SceneToObj(List<NamedMeshXf> meshes, string file)
	{
		int num = Marshal.SizeOf(typeof(MRMeshSaveNamedXfMesh));
		IntPtr intPtr = Marshal.AllocHGlobal(meshes.Count * num);
		try
		{
			for (int i = 0; i < meshes.Count; i++)
			{
				MRMeshSaveNamedXfMesh structure = new MRMeshSaveNamedXfMesh();
				structure.name = meshes[i].name;
				structure.toWorld = meshes[i].toWorld.xf_;
				Mesh mesh = meshes[i].mesh;
				if (mesh != null)
				{
					structure.mesh = mesh.mesh_;
				}
				Marshal.StructureToPtr(structure, IntPtr.Add(intPtr, i * num), fDeleteOld: false);
			}
			IntPtr errorString = default(IntPtr);
			mrMeshSaveSceneToObj(intPtr, (ulong)meshes.Count, file, ref errorString);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}
}
