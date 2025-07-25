namespace MR.DotNet;

internal struct MRMakeTorusParameters
{
	public float primaryRadius;

	public float secondaryRadius;

	public int primaryResolution;

	public int secondaryResolution;

	public MRMakeTorusParameters()
	{
		primaryRadius = 1f;
		secondaryRadius = 0.5f;
		primaryResolution = 32;
		secondaryResolution = 32;
	}
}
