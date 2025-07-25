using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class BooleanResultMapper : IDisposable
{
	private bool disposed;

	private IntPtr mapper_;

	private BooleanMaps?[]? maps_;

	internal IntPtr Mapper => mapper_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrBooleanResultMapperNew();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrBooleanResultMapperMapFaces(IntPtr mapper, IntPtr oldBS, MapObject obj);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrBooleanResultMapperMapVerts(IntPtr mapper, IntPtr oldBS, MapObject obj);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrBooleanResultMapperNewFaces(IntPtr mapper);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrBooleanResultMapperFilteredOldFaceBitSet(IntPtr mapper, IntPtr oldBS, MapObject obj);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrBooleanResultMapperGetMaps(IntPtr mapper, MapObject index);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrBooleanResultMapperFree(IntPtr mapper);

	public BooleanResultMapper()
	{
		mapper_ = mrBooleanResultMapperNew();
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
			if (mapper_ != IntPtr.Zero)
			{
				mrBooleanResultMapperFree(mapper_);
			}
			disposed = true;
		}
	}

	~BooleanResultMapper()
	{
		Dispose(disposing: false);
	}

	public BitSet FaceMap(BitSet oldBS, MapObject obj)
	{
		if (maps_ == null)
		{
			maps_ = new BooleanMaps[2];
		}
		if (maps_[(int)obj] == null)
		{
			maps_[(int)obj] = new BooleanMaps(mrBooleanResultMapperGetMaps(mapper_, obj));
		}
		return new BitSet(mrBooleanResultMapperMapFaces(mapper_, oldBS.bs_, obj));
	}

	public BitSet VertMap(BitSet oldBS, MapObject obj)
	{
		if (maps_ == null)
		{
			maps_ = new BooleanMaps[2];
		}
		if (maps_[(int)obj] == null)
		{
			maps_[(int)obj] = new BooleanMaps(mrBooleanResultMapperGetMaps(mapper_, obj));
		}
		return new BitSet(mrBooleanResultMapperMapVerts(mapper_, oldBS.bs_, obj));
	}

	public BitSet NewFaces()
	{
		return new BitSet(mrBooleanResultMapperNewFaces(mapper_));
	}

	public BooleanMaps GetMaps(MapObject obj)
	{
		if (maps_ == null)
		{
			maps_ = new BooleanMaps[2];
		}
		BooleanMaps booleanMaps = maps_[(int)obj];
		if (booleanMaps == null)
		{
			booleanMaps = (maps_[(int)obj] = new BooleanMaps(mrBooleanResultMapperGetMaps(mapper_, obj)));
		}
		return booleanMaps;
	}

	public BitSet FilteredOldFaceBitSet(BitSet oldBS, MapObject obj)
	{
		if (maps_ == null)
		{
			maps_ = new BooleanMaps[2];
		}
		if (maps_[(int)obj] == null)
		{
			maps_[(int)obj] = new BooleanMaps(mrBooleanResultMapperGetMaps(mapper_, obj));
		}
		return new BitSet(mrBooleanResultMapperFilteredOldFaceBitSet(mapper_, oldBS.bs_, obj));
	}
}
