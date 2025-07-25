using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MultiwayICP
{
	internal struct MRMultiwayICPSamplingParameters
	{
		public float samplingVoxelSize;

		public int maxGroupSize;

		public MultiwayICPSamplingParameters.CascadeMode cascadeMode;

		public IntPtr cb;

		public MRMultiwayICPSamplingParameters()
		{
			samplingVoxelSize = 0f;
			maxGroupSize = 64;
			cascadeMode = MultiwayICPSamplingParameters.CascadeMode.AABBTreeBased;
			cb = IntPtr.Zero;
		}
	}

	internal struct MRVectorAffineXf3f
	{
		public IntPtr data;

		public ulong size;

		public IntPtr reserved;

		public MRVectorAffineXf3f()
		{
			data = IntPtr.Zero;
			size = 0uL;
			reserved = IntPtr.Zero;
		}
	}

	private bool disposed;

	private IntPtr icp_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrMultiwayICPNew(IntPtr objects, ulong objectsNum, ref MRMultiwayICPSamplingParameters samplingParams);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern MRVectorAffineXf3f* mrMultiwayICPCalculateTransformations(IntPtr mwicp, IntPtr cb);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern bool mrMultiwayICPResamplePoints(IntPtr mwicp, ref MRMultiwayICPSamplingParameters samplingParams);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern bool mrMultiwayICPUpdateAllPointPairs(IntPtr mwicp, IntPtr cb);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrMultiwayICPSetParams(IntPtr mwicp, ref ICP.MRICPProperties prop);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern float mrMultiWayICPGetMeanSqDistToPoint(IntPtr mwicp, double* value);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern float mrMultiWayICPGetMeanSqDistToPlane(IntPtr mwicp, double* value);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrMultiWayICPGetNumSamples(IntPtr mwicp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrMultiWayICPGetNumActivePairs(IntPtr mwicp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrMultiwayICPFree(IntPtr mwicp);

	public MultiwayICP(List<MeshOrPointsXf> objs, MultiwayICPSamplingParameters samplingParams)
	{
		int num = Marshal.SizeOf(typeof(IntPtr));
		IntPtr intPtr = Marshal.AllocHGlobal(objs.Count * num);
		try
		{
			for (int i = 0; i < objs.Count; i++)
			{
				Marshal.StructureToPtr(objs[i].mrMeshOrPointsXf_, IntPtr.Add(intPtr, i * num), fDeleteOld: false);
			}
			MRMultiwayICPSamplingParameters samplingParams2 = new MRMultiwayICPSamplingParameters
			{
				samplingVoxelSize = samplingParams.samplingVoxelSize,
				maxGroupSize = samplingParams.maxGroupSize,
				cascadeMode = samplingParams.cascadeMode,
				cb = IntPtr.Zero
			};
			icp_ = mrMultiwayICPNew(intPtr, (ulong)objs.Count, ref samplingParams2);
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
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
			if (icp_ != IntPtr.Zero)
			{
				mrMultiwayICPFree(icp_);
			}
			disposed = true;
		}
	}

	~MultiwayICP()
	{
		Dispose(disposing: false);
	}

	public unsafe List<AffineXf3f> CalculateTransformations()
	{
		int num = Marshal.SizeOf(typeof(MRAffineXf3f));
		MRVectorAffineXf3f mRVectorAffineXf3f = *mrMultiwayICPCalculateTransformations(icp_, IntPtr.Zero);
		List<AffineXf3f> list = new List<AffineXf3f>();
		for (int i = 0; i < (int)mRVectorAffineXf3f.size; i++)
		{
			MRAffineXf3f xf = Marshal.PtrToStructure<MRAffineXf3f>(IntPtr.Add(mRVectorAffineXf3f.data, i * num));
			list.Add(new AffineXf3f(xf));
		}
		return list;
	}

	public void ResamplePoints(MultiwayICPSamplingParameters samplingParams)
	{
		MRMultiwayICPSamplingParameters samplingParams2 = new MRMultiwayICPSamplingParameters();
		samplingParams2.samplingVoxelSize = samplingParams.samplingVoxelSize;
		samplingParams2.maxGroupSize = samplingParams.maxGroupSize;
		samplingParams2.cascadeMode = samplingParams.cascadeMode;
		samplingParams2.cb = IntPtr.Zero;
		mrMultiwayICPResamplePoints(icp_, ref samplingParams2);
	}

	[return: MarshalAs(UnmanagedType.I1)]
	public bool UpdateAllPointPairs()
	{
		return mrMultiwayICPUpdateAllPointPairs(icp_, IntPtr.Zero);
	}

	public void SetParams(ICPProperties props)
	{
		ICP.MRICPProperties prop = props.ToNative();
		mrMultiwayICPSetParams(icp_, ref prop);
	}

	public unsafe float GetMeanSqDistToPoint()
	{
		return mrMultiWayICPGetMeanSqDistToPoint(icp_, null);
	}

	public unsafe float GetMeanSqDistToPoint(double value)
	{
		return mrMultiWayICPGetMeanSqDistToPoint(icp_, &value);
	}

	public unsafe float GetMeanSqDistToPlane()
	{
		return mrMultiWayICPGetMeanSqDistToPlane(icp_, null);
	}

	public unsafe float GetMeanSqDistToPlane(double value)
	{
		return mrMultiWayICPGetMeanSqDistToPlane(icp_, &value);
	}

	public int GetNumSamples()
	{
		return (int)mrMultiWayICPGetNumSamples(icp_);
	}

	public int GetNumActivePairs()
	{
		return (int)mrMultiWayICPGetNumActivePairs(icp_);
	}
}
