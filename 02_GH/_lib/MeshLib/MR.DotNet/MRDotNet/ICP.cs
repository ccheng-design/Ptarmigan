using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class ICP : IDisposable
{
	internal struct MRICPPairData
	{
		public Vector3f.MRVector3f srcPoint;

		public Vector3f.MRVector3f srcNorm;

		public Vector3f.MRVector3f tgtPoint;

		public Vector3f.MRVector3f tgtNorm;

		public float distSq;

		public float weight;

		public MRICPPairData()
		{
			srcPoint = default(Vector3f.MRVector3f);
			srcNorm = default(Vector3f.MRVector3f);
			tgtPoint = default(Vector3f.MRVector3f);
			tgtNorm = default(Vector3f.MRVector3f);
			distSq = 0f;
			weight = 0f;
		}
	}

	internal struct MRPointPair
	{
		public MRICPPairData ICPPairData;

		public VertId srcVertId;

		public VertId tgtCloseVert;

		public float normalsAngleCos;

		public byte tgtOnBd;

		public MRPointPair()
		{
			ICPPairData = default(MRICPPairData);
			srcVertId = default(VertId);
			tgtCloseVert = default(VertId);
			normalsAngleCos = 0f;
			tgtOnBd = 0;
		}
	}

	internal struct MRICPProperties
	{
		public ICPMethod method;

		public float p2plAngleLimit;

		public float p2plScaleLimit;

		public float cosThreshold;

		public float distThresholdSq;

		public float farDistFactor;

		public ICPMode icpMode;

		public Vector3f.MRVector3f fixedRotationAxis;

		public int iterLimit;

		public int badIterStopCount;

		public float exitVal;

		public byte mutualClosest;

		public MRICPProperties()
		{
			fixedRotationAxis = default(Vector3f.MRVector3f);
			method = ICPMethod.PointToPlane;
			p2plAngleLimit = MathF.PI / 6f;
			p2plScaleLimit = 2f;
			cosThreshold = 0.7f;
			distThresholdSq = 1f;
			farDistFactor = 3f;
			icpMode = ICPMode.AnyRigidXf;
			iterLimit = 10;
			badIterStopCount = 3;
			exitVal = 0f;
			mutualClosest = 0;
		}
	}

	private bool disposed;

	internal IntPtr mrICP_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ref MRICPPairData mrIPointPairsGet(IntPtr pp, ulong idx);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrIPointPairsSize(IntPtr pp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ref MRICPPairData mrIPointPairsGetRef(IntPtr pp, ulong idx);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrICPNew(IntPtr fltObj, IntPtr refObj, float samplingVoxelSize);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrICPNewFromSamples(IntPtr fltObj, IntPtr refObj, IntPtr fltSamples, IntPtr refSamples);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrICPSetParams(IntPtr icp, ref MRICPProperties prop);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrICPSamplePoints(IntPtr icp, float samplingVoxelSize);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRAffineXf3f mrICPAutoSelectFloatXf(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrICPUpdatePointPairs(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrICPGetStatusInfo(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrICPGetNumSamples(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrICPGetNumActivePairs(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern float mrICPGetMeanSqDistToPoint(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern float mrICPGetMeanSqDistToPlane(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrICPGetFlt2RefPairs(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrICPGetRef2FltPairs(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRAffineXf3f mrICPCalculateTransformation(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrICPFree(IntPtr icp);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrStringData(IntPtr str);

	public ICP(MeshOrPointsXf fltObj, MeshOrPointsXf refObj, float samplingVoxelSize)
	{
		mrICP_ = mrICPNew(fltObj.mrMeshOrPointsXf_, refObj.mrMeshOrPointsXf_, samplingVoxelSize);
	}

	public ICP(MeshOrPointsXf fltObj, MeshOrPointsXf refObj, BitSet fltSamples, BitSet refSamples)
	{
		mrICP_ = mrICPNewFromSamples(fltObj.mrMeshOrPointsXf_, refObj.mrMeshOrPointsXf_, fltSamples.bs_, refSamples.bs_);
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
			if (mrICP_ != IntPtr.Zero)
			{
				mrICPFree(mrICP_);
			}
			disposed = true;
		}
	}

	~ICP()
	{
		mrICPFree(mrICP_);
	}

	public void SetParams(ICPProperties prop)
	{
		MRICPProperties prop2 = prop.ToNative();
		mrICPSetParams(mrICP_, ref prop2);
	}

	public void SamplePoints(float sampleVoxelSize)
	{
		mrICPSamplePoints(mrICP_, sampleVoxelSize);
	}

	public void AutoSelectFloatXf()
	{
		mrICPAutoSelectFloatXf(mrICP_);
	}

	public void UpdatePointPairs()
	{
		mrICPUpdatePointPairs(mrICP_);
	}

	public string GetStatusInfo()
	{
		return Marshal.PtrToStringAnsi(mrStringData(mrICPGetStatusInfo(mrICP_)));
	}

	public int GetNumSamples()
	{
		return (int)mrICPGetNumSamples(mrICP_);
	}

	public int GetNumActivePairs()
	{
		return (int)mrICPGetNumActivePairs(mrICP_);
	}

	public float GetMeanSqDistToPoint()
	{
		return mrICPGetMeanSqDistToPoint(mrICP_);
	}

	public float GetMeanSqDistToPlane()
	{
		return mrICPGetMeanSqDistToPlane(mrICP_);
	}

	public PointPairs GetFlt2RefPairs()
	{
		IntPtr pp = mrICPGetFlt2RefPairs(mrICP_);
		int num = (int)mrIPointPairsSize(pp);
		PointPairs result = new PointPairs
		{
			pairs = new List<PointPair>(num)
		};
		for (int i = 0; i < num; i++)
		{
			MRICPPairData mRICPPairData = mrIPointPairsGet(pp, (ulong)i);
			PointPair item = new PointPair();
			item.srcPoint = new Vector3f(mRICPPairData.srcPoint);
			item.srcNorm = new Vector3f(mRICPPairData.srcNorm);
			item.tgtPoint = new Vector3f(mRICPPairData.tgtPoint);
			item.tgtNorm = new Vector3f(mRICPPairData.tgtNorm);
			item.weight = mRICPPairData.weight;
			item.distSq = mRICPPairData.distSq;
			result.pairs.Add(item);
		}
		return result;
	}

	public PointPairs GetRef2FltPairs()
	{
		IntPtr pp = mrICPGetRef2FltPairs(mrICP_);
		int num = (int)mrIPointPairsSize(pp);
		PointPairs result = new PointPairs
		{
			pairs = new List<PointPair>(num)
		};
		for (int i = 0; i < num; i++)
		{
			MRICPPairData mRICPPairData = mrIPointPairsGet(pp, (ulong)i);
			PointPair item = new PointPair();
			item.srcPoint = new Vector3f(mRICPPairData.srcPoint);
			item.srcNorm = new Vector3f(mRICPPairData.srcNorm);
			item.tgtPoint = new Vector3f(mRICPPairData.tgtPoint);
			item.tgtNorm = new Vector3f(mRICPPairData.tgtNorm);
			item.weight = mRICPPairData.weight;
			item.distSq = mRICPPairData.distSq;
			result.pairs.Add(item);
		}
		return result;
	}

	public AffineXf3f CalculateTransformation()
	{
		return new AffineXf3f(mrICPCalculateTransformation(mrICP_));
	}
}
