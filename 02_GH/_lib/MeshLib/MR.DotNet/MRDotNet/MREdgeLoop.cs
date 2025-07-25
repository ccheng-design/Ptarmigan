using System;

namespace MR.DotNet;

internal struct MREdgeLoop
{
	public IntPtr data;

	public ulong size;

	public IntPtr reserved;

	public MREdgeLoop()
	{
		data = IntPtr.Zero;
		size = 0uL;
		reserved = IntPtr.Zero;
	}
}
