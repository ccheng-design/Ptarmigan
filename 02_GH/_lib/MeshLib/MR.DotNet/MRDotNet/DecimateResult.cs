namespace MR.DotNet;

public struct DecimateResult
{
	public int vertsDeleted;

	public int facesDeleted;

	public float errorIntroduced;

	public DecimateResult()
	{
		vertsDeleted = 0;
		facesDeleted = 0;
		errorIntroduced = 0f;
	}
}
