namespace MR.DotNet;

public struct VertId
{
	public int Id;

	public VertId(int id = -1)
	{
		Id = id;
	}

	public bool Valid()
	{
		return Id >= 0;
	}
}
