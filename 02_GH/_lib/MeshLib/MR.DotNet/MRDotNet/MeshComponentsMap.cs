using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshComponentsMap : List<RegionId>, IDisposable
{
	internal MeshComponents.MRMeshComponentsMap mrMap_;

	private bool disposed_;

	public int NumComponents;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern void mrMeshComponentsAllComponentsMapFree(MeshComponents.MRMeshComponentsMap* map);

	internal unsafe MeshComponentsMap(MeshComponents.MRMeshComponentsMap mrMap)
	{
		mrMap_ = mrMap;
		NumComponents = mrMap.numComponents;
		for (int i = 0; i < (int)mrMap.faceMap->size; i++)
		{
			Add(new RegionId(Marshal.ReadInt32(IntPtr.Add(mrMap.faceMap->data, i * 4))));
		}
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected unsafe virtual void Dispose(bool disposing)
	{
		if (disposed_)
		{
			return;
		}
		if (mrMap_.faceMap->data != IntPtr.Zero)
		{
			fixed (MeshComponents.MRMeshComponentsMap* map = &mrMap_)
			{
				mrMeshComponentsAllComponentsMapFree(map);
			}
		}
		disposed_ = true;
	}

	~MeshComponentsMap()
	{
		Dispose(disposing: false);
	}
}
