namespace MR.DotNet;

internal struct MRPointOnFace
{
	public FaceId face;

	public Vector3f.MRVector3f point;

	public MRPointOnFace()
	{
		face = default(FaceId);
		point = default(Vector3f.MRVector3f);
	}
}
