namespace MR.DotNet;

public struct FaceId
{
	public int Id;

	public FaceId(int id = -1)
	{
		Id = id;
	}

	public bool Valid()
	{
		return Id >= 0;
	}
}
