using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class EdgeLoops : List<List<EdgeId>>, IDisposable
{
	private bool disposed;

	internal IntPtr mrLoops_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MREdgeLoop mrEdgeLoopsGet(IntPtr loops, ulong index);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ulong mrEdgeLoopsSize(IntPtr loops);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrEdgeLoopsFree(IntPtr loops);

	public EdgeLoops(IntPtr mrLoops)
	{
		mrLoops_ = mrLoops;
		int num = (int)mrEdgeLoopsSize(mrLoops);
		for (int i = 0; i < num; i++)
		{
			Add(new List<EdgeId>());
			MREdgeLoop mREdgeLoop = mrEdgeLoopsGet(mrLoops, (ulong)i);
			for (int j = 0; j < (int)mREdgeLoop.size; j++)
			{
				base[i].Add(new EdgeId(Marshal.ReadInt32(IntPtr.Add(mREdgeLoop.data, j * 4))));
			}
		}
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
			_ = mrLoops_;
			mrEdgeLoopsFree(mrLoops_);
			disposed = true;
		}
	}

	~EdgeLoops()
	{
		Dispose(disposing: false);
	}
}
