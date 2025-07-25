namespace MR.DotNet;

public struct MultiwayICPSamplingParameters
{
	public enum CascadeMode
	{
		Sequential,
		AABBTreeBased
	}

	public float samplingVoxelSize;

	public int maxGroupSize;

	public CascadeMode cascadeMode;

	public MultiwayICPSamplingParameters()
	{
		samplingVoxelSize = 0f;
		maxGroupSize = 64;
		cascadeMode = CascadeMode.AABBTreeBased;
	}
}
