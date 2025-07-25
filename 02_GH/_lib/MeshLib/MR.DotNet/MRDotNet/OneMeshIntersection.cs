namespace MR.DotNet;

public struct OneMeshIntersection
{
	public VariantIndex variantIndex;

	public int index;

	public Vector3f coordinate;

	public OneMeshIntersection(VariantIndex variantIndex, int index, Vector3f coordinate)
	{
		this.variantIndex = variantIndex;
		this.index = index;
		this.coordinate = coordinate;
	}
}
