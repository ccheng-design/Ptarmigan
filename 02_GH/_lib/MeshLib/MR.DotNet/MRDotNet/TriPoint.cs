namespace MR.DotNet;

public struct TriPoint
{
	public float a;

	public float b;

	public Vector3f Interpolate(Vector3f p0, Vector3f p1, Vector3f p2)
	{
		return p0 * (1f - a - b) + a * p1 + b * p2;
	}
}
