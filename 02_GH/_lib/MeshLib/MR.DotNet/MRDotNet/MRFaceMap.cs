using System;

namespace MR.DotNet;

internal struct MRFaceMap
{
	public IntPtr data;

	public ulong size;

	public IntPtr reserved;

	public MRFaceMap()
	{
		data = IntPtr.Zero;
		size = 0uL;
		reserved = IntPtr.Zero;
	}
}
