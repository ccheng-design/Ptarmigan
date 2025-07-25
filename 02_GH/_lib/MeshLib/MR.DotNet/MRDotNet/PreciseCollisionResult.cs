using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class PreciseCollisionResult : IDisposable
{
	internal struct MRVectorEdgeTri
	{
		public IntPtr data;

		public ulong size;

		public IntPtr reserved;

		public MRVectorEdgeTri()
		{
			data = IntPtr.Zero;
			size = 0uL;
			reserved = IntPtr.Zero;
		}
	}

	private bool disposed;

	private List<EdgeTri>? edgesAtrisB_;

	private List<EdgeTri>? edgesBtrisA_;

	internal IntPtr nativeResult_;

	public ReadOnlyCollection<EdgeTri> EdgesAtrisB
	{
		get
		{
			if (edgesAtrisB_ == null)
			{
				MRVectorEdgeTri mRVectorEdgeTri = mrPreciseCollisionResultEdgesAtrisB(nativeResult_);
				int num = Marshal.SizeOf(typeof(EdgeTri));
				edgesAtrisB_ = new List<EdgeTri>((int)mRVectorEdgeTri.size);
				for (int i = 0; i < (int)mRVectorEdgeTri.size; i++)
				{
					edgesAtrisB_.Add(Marshal.PtrToStructure<EdgeTri>(IntPtr.Add(mRVectorEdgeTri.data, i * num)));
				}
			}
			return edgesAtrisB_.AsReadOnly();
		}
	}

	public ReadOnlyCollection<EdgeTri> EdgesBtrisA
	{
		get
		{
			if (edgesBtrisA_ == null)
			{
				MRVectorEdgeTri mRVectorEdgeTri = mrPreciseCollisionResultEdgesBtrisA(nativeResult_);
				int num = Marshal.SizeOf(typeof(EdgeTri));
				edgesBtrisA_ = new List<EdgeTri>((int)mRVectorEdgeTri.size);
				for (int i = 0; i < (int)mRVectorEdgeTri.size; i++)
				{
					edgesBtrisA_.Add(Marshal.PtrToStructure<EdgeTri>(IntPtr.Add(mRVectorEdgeTri.data, i * num)));
				}
			}
			return edgesBtrisA_.AsReadOnly();
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern bool mrEdgeTriEq(ref EdgeTri a, ref EdgeTri b);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVectorEdgeTri mrPreciseCollisionResultEdgesAtrisB(IntPtr result);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVectorEdgeTri mrPreciseCollisionResultEdgesBtrisA(IntPtr result);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrPreciseCollisionResultFree(IntPtr result);

	internal PreciseCollisionResult(IntPtr nativeResult)
	{
		nativeResult_ = nativeResult;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (nativeResult_ != IntPtr.Zero)
			{
				mrPreciseCollisionResultFree(nativeResult_);
			}
			disposed = true;
		}
	}

	~PreciseCollisionResult()
	{
		Dispose(disposing: false);
	}
}
