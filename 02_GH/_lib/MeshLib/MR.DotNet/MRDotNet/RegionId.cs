namespace MR.DotNet;

public struct RegionId
{
	public int Id;

	public RegionId(int id = -1)
	{
		Id = id;
	}

	public bool Valid()
	{
		return Id >= 0;
	}
}
