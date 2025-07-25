namespace MR.DotNet;

public struct OffsetParameters
{
	public float voxelSize;

	public SignDetectionMode signDetectionMode;

	public bool memoryEfficient;

	public OffsetParameters()
	{
		voxelSize = 0f;
		signDetectionMode = SignDetectionMode.OpenVDB;
		memoryEfficient = false;
	}
}
