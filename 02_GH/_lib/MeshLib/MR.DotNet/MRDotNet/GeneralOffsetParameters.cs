namespace MR.DotNet;

public struct GeneralOffsetParameters
{
	public float minNewVertDev;

	public float maxNewRank2VertDev;

	public float maxNewRank3VertDev;

	public float maxOldVertPosCorrection;

	public GeneralOffsetMode mode;

	public GeneralOffsetParameters()
	{
		minNewVertDev = 0.04f;
		maxNewRank2VertDev = 5f;
		maxNewRank3VertDev = 2f;
		maxOldVertPosCorrection = 0.5f;
		mode = GeneralOffsetMode.Standard;
	}
}
