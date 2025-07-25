namespace MR.DotNet;

public struct FillHoleParams
{
	public FillHoleMetric Metric;

	public BitSet? OutNewFaces;

	public MultipleEdgesResolveMode MultipleEdgesResolveMode;

	public bool MakeDegenerateBand;

	public int MaxPolygonSubdivisions;

	public bool? StopBeforeBadTriangulation;

	public FillHoleParams()
	{
		Metric = new FillHoleMetric();
		OutNewFaces = null;
		MultipleEdgesResolveMode = MultipleEdgesResolveMode.Simple;
		MakeDegenerateBand = false;
		MaxPolygonSubdivisions = 20;
		StopBeforeBadTriangulation = null;
	}
}
