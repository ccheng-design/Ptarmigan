using System;

namespace MR.DotNet;

internal struct MRVertMap
{
	public IntPtr data;

	public ulong size;

	public IntPtr reserved;

	public MRVertMap()
	{
		data = IntPtr.Zero;
		size = 0uL;
		reserved = IntPtr.Zero;
	}
}
