namespace MR.DotNet;

internal struct MRMeshProjectionResult
{
	public MRPointOnFace proj;

	public MRMeshTriPoint mtp;

	public float distSq;

	public MRMeshProjectionResult()
	{
		proj = default(MRPointOnFace);
		mtp = default(MRMeshTriPoint);
		distSq = 0f;
	}
}
