namespace MR.DotNet;

public struct VariableEdgeTri
{
	public EdgeId edge;

	public FaceId tri;

	public bool isEdgeATriB;

	public VariableEdgeTri()
	{
		edge = default(EdgeId);
		tri = default(FaceId);
		isEdgeATriB = false;
	}
}
