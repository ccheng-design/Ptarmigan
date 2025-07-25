namespace MR.DotNet;

public struct UndirectedEdgeId
{
	public int Id;

	public UndirectedEdgeId(int id = -1)
	{
		Id = id;
	}

	public bool Valid()
	{
		return Id >= 0;
	}
}
