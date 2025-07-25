using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class NamedMeshList : List<NamedMesh>, IDisposable
{
	private IntPtr nativeList_;

	private bool disposed_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrVectorMeshLoadNamedMeshFree(IntPtr vector);

	internal NamedMeshList(IntPtr nativeList)
	{
		nativeList_ = nativeList;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed_)
		{
			if (nativeList_ != IntPtr.Zero)
			{
				mrVectorMeshLoadNamedMeshFree(nativeList_);
				nativeList_ = IntPtr.Zero;
			}
			disposed_ = true;
		}
	}

	~NamedMeshList()
	{
		Dispose(disposing: false);
	}
}
