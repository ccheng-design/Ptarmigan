using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public abstract class BitSetReadOnly
{
	internal IntPtr bs_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrBitSetSize(IntPtr bs);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrBitSetCount(IntPtr bs);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool mrBitSetEq(IntPtr a, IntPtr b);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool mrBitSetTest(IntPtr bs, ulong index);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrBitSetFindFirst(IntPtr bs);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrBitSetFindLast(IntPtr bs);

	public bool Test(int i)
	{
		return mrBitSetTest(bs_, (ulong)i);
	}

	public int FindFirst()
	{
		return (int)mrBitSetFindFirst(bs_);
	}

	public int FindLast()
	{
		return (int)mrBitSetFindLast(bs_);
	}

	public int Size()
	{
		return (int)mrBitSetSize(bs_);
	}

	public int Count()
	{
		return (int)mrBitSetCount(bs_);
	}

	public abstract BitSetReadOnly Clone();

	public static bool operator ==(BitSetReadOnly a, BitSetReadOnly b)
	{
		return mrBitSetEq(a.bs_, b.bs_);
	}

	public static bool operator !=(BitSetReadOnly a, BitSetReadOnly b)
	{
		return !mrBitSetEq(a.bs_, b.bs_);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is BitSetReadOnly))
		{
			return false;
		}
		return this == (BitSetReadOnly)obj;
	}

	public override int GetHashCode()
	{
		throw new NotImplementedException();
	}
}
