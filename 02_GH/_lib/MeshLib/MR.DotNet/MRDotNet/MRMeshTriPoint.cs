namespace MR.DotNet;

internal struct MRMeshTriPoint
{
	public EdgeId e;

	public MRTriPointf bary;

	public MRMeshTriPoint()
	{
		e = default(EdgeId);
		bary = default(MRTriPointf);
	}
}
