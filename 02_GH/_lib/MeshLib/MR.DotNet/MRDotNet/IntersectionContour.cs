using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class IntersectionContour
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrOrderIntersectionContours(IntPtr topologyA, IntPtr topologyB, IntPtr intersections);

	public static ContinousContours OrderIntersectionContours(Mesh meshA, Mesh meshB, PreciseCollisionResult intersections)
	{
		return new ContinousContours(mrOrderIntersectionContours(meshA.meshTopology_, meshB.meshTopology_, intersections.nativeResult_));
	}
}
