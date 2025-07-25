namespace MR.DotNet;

public struct BooleanParameters
{
	public BooleanResultMapper? mapper;

	public AffineXf3f? rigidB2A;

	public bool mergeAllNonIntersectingComponents;

	public BooleanParameters()
	{
		mapper = null;
		rigidB2A = null;
		mergeAllNonIntersectingComponents = false;
	}
}
