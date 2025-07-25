using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshCollidePrecise
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrFindCollidingEdgeTrisPrecise(ref MRMeshPart a, ref MRMeshPart b, IntPtr conv, IntPtr rigidB2A, bool anyIntersection);

	public static PreciseCollisionResult FindCollidingEdgeTrisPrecise(MeshPart meshA, MeshPart meshB, CoordinateConverters conv, AffineXf3f? rigidB2A = null, bool anyIntersection = false)
	{
		return new PreciseCollisionResult(mrFindCollidingEdgeTrisPrecise(ref meshA.mrMeshPart, ref meshB.mrMeshPart, conv.GetConvertToIntVector(), rigidB2A?.XfAddr() ?? IntPtr.Zero, anyIntersection));
	}
}
