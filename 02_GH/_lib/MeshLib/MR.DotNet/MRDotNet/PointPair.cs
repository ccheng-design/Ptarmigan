namespace MR.DotNet;

public struct PointPair
{
	public Vector3f srcPoint;

	public Vector3f srcNorm;

	public Vector3f tgtPoint;

	public Vector3f tgtNorm;

	public float distSq;

	public float weight;

	public VertId srcVertId;

	public VertId tgtCloseVert;

	public float normalsAngleCos;

	public bool tgtOnBd;

	public PointPair()
	{
		srcVertId = default(VertId);
		tgtCloseVert = default(VertId);
		srcPoint = new Vector3f();
		srcNorm = new Vector3f();
		tgtPoint = new Vector3f();
		tgtNorm = new Vector3f();
		distSq = 0f;
		weight = 1f;
		normalsAngleCos = 1f;
		tgtOnBd = false;
	}

	private ICP.MRPointPair ToNative()
	{
		ICP.MRICPPairData iCPPairData = new ICP.MRICPPairData();
		iCPPairData.srcPoint = srcPoint.vec_;
		iCPPairData.srcNorm = srcNorm.vec_;
		iCPPairData.tgtPoint = tgtPoint.vec_;
		iCPPairData.tgtNorm = tgtNorm.vec_;
		iCPPairData.distSq = distSq;
		iCPPairData.weight = weight;
		ICP.MRPointPair result = new ICP.MRPointPair();
		result.ICPPairData = iCPPairData;
		result.srcVertId = srcVertId;
		result.tgtCloseVert = tgtCloseVert;
		result.normalsAngleCos = normalsAngleCos;
		result.tgtOnBd = (tgtOnBd ? ((byte)1) : ((byte)0));
		return result;
	}
}
