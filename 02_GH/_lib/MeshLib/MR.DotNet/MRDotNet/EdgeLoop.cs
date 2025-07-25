using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class EdgeLoop : List<EdgeId>, IDisposable
{
	private bool disposed;

	internal unsafe MREdgeLoop* mrLoop_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private unsafe static extern void mrEdgePathFree(MREdgeLoop* loop);

	internal unsafe EdgeLoop(MREdgeLoop* mrLoop)
	{
		mrLoop_ = mrLoop;
		for (int i = 0; i < (int)mrLoop->size; i++)
		{
			Add(new EdgeId(Marshal.ReadInt32(IntPtr.Add(mrLoop->data, i * 4))));
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected unsafe virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (mrLoop_ != null)
			{
				mrEdgePathFree(mrLoop_);
			}
			disposed = true;
		}
	}

	~EdgeLoop()
	{
		Dispose(disposing: false);
	}
}
