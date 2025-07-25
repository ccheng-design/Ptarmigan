using System;

namespace MR.DotNet;

internal struct MRTriangulation
{
	public IntPtr data;

	public ulong size;

	public IntPtr reserved;

	public MRTriangulation()
	{
		data = IntPtr.Zero;
		size = 0uL;
		reserved = IntPtr.Zero;
	}
}
