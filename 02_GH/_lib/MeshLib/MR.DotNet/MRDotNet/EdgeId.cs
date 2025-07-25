namespace MR.DotNet;

public struct EdgeId
{
	public int Id;

	public EdgeId(int id = -1)
	{
		Id = id;
	}

	public bool Valid()
	{
		return Id >= 0;
	}
}
