using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshFixer
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrFindHoleComplicatingFaces(IntPtr mesh);

	public static BitSet FindHoleComplicatingFaces(Mesh mesh)
	{
		return new BitSet(mrFindHoleComplicatingFaces(mesh.mesh_));
	}
}
