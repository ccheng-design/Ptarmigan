using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshNormals
{
	internal struct MRFaceNormals
	{
		public IntPtr data;

		public ulong size;

		public IntPtr reserved;

		public MRFaceNormals()
		{
			data = IntPtr.Zero;
			size = 0uL;
			reserved = IntPtr.Zero;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern MRFaceNormals* mrComputePerFaceNormals(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern MRFaceNormals* mrComputePerVertNormals(IntPtr mesh);

	public unsafe static List<Vector3f> ComputePerVertNormals(Mesh mesh)
	{
		MRFaceNormals mRFaceNormals = *mrComputePerVertNormals(mesh.mesh_);
		List<Vector3f> list = new List<Vector3f>();
		int num = Marshal.SizeOf(typeof(Vector3f.MRVector3f));
		IntPtr data = mRFaceNormals.data;
		for (int i = 0; i < (int)mRFaceNormals.size; i++)
		{
			Vector3f.MRVector3f vec = Marshal.PtrToStructure<Vector3f.MRVector3f>(IntPtr.Add(data, i * num));
			list.Add(new Vector3f(vec));
		}
		return list;
	}

	public unsafe static List<Vector3f> ComputePerFaceNormals(Mesh mesh)
	{
		MRFaceNormals mRFaceNormals = *mrComputePerFaceNormals(mesh.mesh_);
		List<Vector3f> list = new List<Vector3f>();
		int num = Marshal.SizeOf(typeof(Vector3f.MRVector3f));
		IntPtr data = mRFaceNormals.data;
		for (int i = 0; i < (int)mRFaceNormals.size; i++)
		{
			Vector3f.MRVector3f vec = Marshal.PtrToStructure<Vector3f.MRVector3f>(IntPtr.Add(data, i * num));
			list.Add(new Vector3f(vec));
		}
		return list;
	}
}
