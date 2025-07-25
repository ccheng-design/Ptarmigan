namespace MR.DotNet;

public struct NamedMesh
{
	public string name;

	public Mesh? mesh;

	public AffineXf3f? xf;

	public int skippedFaceCount;

	public int duplicatedVertexCount;

	public NamedMesh()
	{
		name = "";
		mesh = null;
		xf = null;
		skippedFaceCount = 0;
		duplicatedVertexCount = 0;
	}
}
