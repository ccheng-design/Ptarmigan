namespace MR.DotNet;

public struct NamedMeshXf
{
	public string name;

	public AffineXf3f toWorld;

	public Mesh? mesh;

	public NamedMeshXf()
	{
		name = "";
		toWorld = new AffineXf3f();
		mesh = null;
	}
}
