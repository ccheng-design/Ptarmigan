using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class ConvexHull
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrMakeConvexHullFromMesh(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrMakeConvexHullFromPointCloud(IntPtr pointCloud);

	public static Mesh MakeConvexHull(Mesh mesh)
	{
		return new Mesh(mrMakeConvexHullFromMesh(mesh.mesh_));
	}

	public static Mesh MakeConvexHull(PointCloud pointCloud)
	{
		return new Mesh(mrMakeConvexHullFromPointCloud(pointCloud.pc_));
	}
}
