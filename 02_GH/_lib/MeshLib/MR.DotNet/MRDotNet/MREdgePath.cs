using System;

namespace MR.DotNet;

internal struct MREdgePath
{
	public IntPtr data;

	public ulong size;

	public IntPtr reserved;

	public MREdgePath()
	{
		data = IntPtr.Zero;
		size = 0uL;
		reserved = IntPtr.Zero;
	}
}
