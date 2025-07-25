namespace MR.DotNet;

public struct MeshRegions
{
	public BitSet faces;

	public int numRegions;

	public MeshRegions(BitSet faces, int numRegions)
	{
		this.numRegions = 0;
		this.faces = faces;
		this.numRegions = numRegions;
	}
}
