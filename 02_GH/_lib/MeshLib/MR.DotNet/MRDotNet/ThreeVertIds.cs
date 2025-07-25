namespace MR.DotNet;

public struct ThreeVertIds
{
	public VertId v0;

	public VertId v1;

	public VertId v2;

	public ThreeVertIds(VertId v0_, VertId v1_, VertId v2_)
	{
		v0 = v0_;
		v1 = v1_;
		v2 = v2_;
	}

	public ThreeVertIds(int v0_, int v1_, int v2_)
	{
		v0 = new VertId(v0_);
		v1 = new VertId(v1_);
		v2 = new VertId(v2_);
	}
}
