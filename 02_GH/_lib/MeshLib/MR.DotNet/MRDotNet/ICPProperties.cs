using System;

namespace MR.DotNet;

public struct ICPProperties
{
	public ICPMethod method;

	public float p2plAngleLimit;

	public float p2plScaleLimit;

	public float cosThreshold;

	public float distThresholdSq;

	public float farDistFactor;

	public ICPMode icpMode;

	public Vector3f fixedRotationAxis;

	public int iterLimit;

	public int badIterStopCount;

	public float exitVal;

	public bool mutualClosest;

	public ICPProperties()
	{
		method = ICPMethod.PointToPlane;
		p2plAngleLimit = MathF.PI / 6f;
		p2plScaleLimit = 2f;
		cosThreshold = 0.7f;
		distThresholdSq = 1f;
		farDistFactor = 3f;
		icpMode = ICPMode.AnyRigidXf;
		fixedRotationAxis = new Vector3f();
		iterLimit = 10;
		badIterStopCount = 3;
		exitVal = 0f;
		mutualClosest = false;
	}

	internal ICP.MRICPProperties ToNative()
	{
		ICP.MRICPProperties result = new ICP.MRICPProperties();
		result.method = method;
		result.p2plAngleLimit = p2plAngleLimit;
		result.p2plScaleLimit = p2plScaleLimit;
		result.cosThreshold = cosThreshold;
		result.distThresholdSq = distThresholdSq;
		result.farDistFactor = farDistFactor;
		result.icpMode = icpMode;
		result.fixedRotationAxis = fixedRotationAxis.vec_;
		result.iterLimit = iterLimit;
		result.badIterStopCount = badIterStopCount;
		result.exitVal = exitVal;
		result.mutualClosest = (mutualClosest ? ((byte)1) : ((byte)0));
		return result;
	}
}
