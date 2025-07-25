using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshOrPointsXf
{
	public MeshOrPoints obj;

	public AffineXf3f xf;

	internal IntPtr mrMeshOrPointsXf_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshOrPointsXfFromMesh(IntPtr mesh, ref MRAffineXf3f xf);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshOrPointsXfFromPointCloud(IntPtr pc, ref MRAffineXf3f xf);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshOrPointsXfFree(IntPtr mp);

	public MeshOrPointsXf(MeshOrPoints obj, AffineXf3f xf)
	{
		this.obj = obj;
		this.xf = xf;
		if (obj is Mesh mesh)
		{
			mrMeshOrPointsXf_ = mrMeshOrPointsXfFromMesh(mesh.mesh_, ref xf.xf_);
		}
		if (obj is PointCloud pointCloud)
		{
			mrMeshOrPointsXf_ = mrMeshOrPointsXfFromPointCloud(pointCloud.pc_, ref xf.xf_);
		}
	}

	~MeshOrPointsXf()
	{
		mrMeshOrPointsXfFree(mrMeshOrPointsXf_);
	}
}
