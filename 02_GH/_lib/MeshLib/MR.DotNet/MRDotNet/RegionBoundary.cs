using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class RegionBoundary
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrFindRightBoundary(IntPtr topology, IntPtr region);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private unsafe static extern MREdgeLoop* mrTrackRightBoundaryLoop(IntPtr topology, EdgeId e0, IntPtr region);

	public unsafe static EdgeLoop TrackRightBoundaryLoop(Mesh mesh, EdgeId e0, BitSet? region = null)
	{
		return new EdgeLoop(mrTrackRightBoundaryLoop(mesh.meshTopology_, e0, region?.bs_ ?? ((IntPtr)(void*)null)));
	}

	public unsafe static EdgeLoops FindRightBoundary(Mesh mesh, BitSet? region = null)
	{
		return new EdgeLoops(mrFindRightBoundary(mesh.meshTopology_, region?.bs_ ?? ((IntPtr)(void*)null)));
	}
}
