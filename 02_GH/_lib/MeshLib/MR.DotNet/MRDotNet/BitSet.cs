using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class BitSet : BitSetReadOnly, IDisposable
{
	private bool needDispose;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrBitSetCopy(IntPtr bs);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrBitSetNew(ulong numBits, bool fillValue);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrBitSetSet(IntPtr bs, ulong index, bool value);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrBitSetResize(IntPtr bs, ulong size, bool value);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrBitSetAutoResizeSet(IntPtr bs, ulong pos, bool value);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrBitSetSub(IntPtr a, IntPtr b);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrBitSetFree(IntPtr bs);

	public BitSet()
		: this(0, fillValue: false)
	{
	}

	internal BitSet(IntPtr bs)
	{
		bs_ = bs;
	}

	public BitSet(int size)
		: this(size, fillValue: false)
	{
	}

	public BitSet(int size, bool fillValue)
	{
		bs_ = mrBitSetNew((ulong)size, fillValue);
		needDispose = true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (needDispose)
		{
			if (bs_ != IntPtr.Zero)
			{
				Console.WriteLine("mrBitSetFree start");
				mrBitSetFree(bs_);
				Console.WriteLine("mrBitSetFree end");
				bs_ = IntPtr.Zero;
			}
			needDispose = false;
		}
	}

	~BitSet()
	{
		Dispose(disposing: false);
	}

	public void Set(int index)
	{
		mrBitSetSet(bs_, (ulong)index, value: true);
	}

	public void Set(int index, bool value)
	{
		mrBitSetSet(bs_, (ulong)index, value);
	}

	public void Resize(int size)
	{
		mrBitSetResize(bs_, (ulong)size, value: false);
	}

	public void Resize(int size, bool value)
	{
		mrBitSetResize(bs_, (ulong)size, value);
	}

	public void AutoResizeSet(int pos)
	{
		mrBitSetAutoResizeSet(bs_, (ulong)pos, value: true);
	}

	public void AutoResizeSet(int pos, bool value)
	{
		mrBitSetAutoResizeSet(bs_, (ulong)pos, value);
	}

	public override BitSetReadOnly Clone()
	{
		return new BitSet(mrBitSetCopy(bs_));
	}

	public static BitSet operator -(BitSet a, BitSet b)
	{
		return new BitSet(mrBitSetSub(a.bs_, b.bs_));
	}
}
