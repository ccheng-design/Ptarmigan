using System;

namespace MR.DotNet;

internal struct MRMeshPart
{
	public IntPtr mesh;

	public IntPtr region;

	public MRMeshPart()
	{
		mesh = IntPtr.Zero;
		region = IntPtr.Zero;
	}
}
