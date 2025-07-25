using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshLoad
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRMeshLoadNamedMesh mrVectorMeshLoadNamedMeshGet(IntPtr vector, ulong index);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ulong mrVectorMeshLoadNamedMeshSize(IntPtr vector);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshLoadFromSceneObjFile(string file, bool combineAllObjects, ref MRMeshLoadObjLoadSettings settings, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrStringData(IntPtr str);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private unsafe static extern IntPtr mrMeshLoadFromAnySupportedFormat(string file, IntPtr* errorStr);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrLoadIOExtras();

	public unsafe static Mesh FromAnySupportedFormat(string path)
	{
		mrLoadIOExtras();
		IntPtr intPtr = default(IntPtr);
		IntPtr mesh = mrMeshLoadFromAnySupportedFormat(path, &intPtr);
		if (intPtr != IntPtr.Zero)
		{
			throw new SystemException(Marshal.PtrToStringAnsi(mrStringData(intPtr)));
		}
		return new Mesh(mesh);
	}

	public static NamedMeshList FromSceneObjFile(string path, bool combineAllObjects, ObjLoadSettings settings)
	{
		MRMeshLoadObjLoadSettings settings2 = new MRMeshLoadObjLoadSettings();
		settings2.customXf = settings.customXf;
		settings2.countSkippedFaces = settings.countSkippedFaces;
		settings2.callback = IntPtr.Zero;
		IntPtr errorString = default(IntPtr);
		IntPtr intPtr = mrMeshLoadFromSceneObjFile(path, combineAllObjects, ref settings2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new SystemException(Marshal.PtrToStringAnsi(mrStringData(errorString)));
		}
		int num = (int)mrVectorMeshLoadNamedMeshSize(intPtr);
		NamedMeshList namedMeshList = new NamedMeshList(intPtr);
		for (int i = 0; i < num; i++)
		{
			MRMeshLoadNamedMesh mRMeshLoadNamedMesh = mrVectorMeshLoadNamedMeshGet(intPtr, (ulong)i);
			NamedMesh item = new NamedMesh();
			item.name = Marshal.PtrToStringAnsi(mRMeshLoadNamedMesh.name);
			item.mesh = new Mesh(mRMeshLoadNamedMesh.mesh);
			item.mesh.SkipDisposingAtFinalize();
			item.xf = new AffineXf3f(mRMeshLoadNamedMesh.xf);
			item.skippedFaceCount = mRMeshLoadNamedMesh.skippedFaceCount;
			item.duplicatedVertexCount = mRMeshLoadNamedMesh.duplicatedVertexCount;
			namedMeshList.Add(item);
		}
		return namedMeshList;
	}
}
