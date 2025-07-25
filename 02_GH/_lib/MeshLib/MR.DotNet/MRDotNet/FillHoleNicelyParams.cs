using System;

namespace MR.DotNet;

public struct FillHoleNicelyParams
{
	public FillHoleParams triangulationParams;

	public bool TriangulateOnly;

	public float MaxEdgeLen;

	public int MaxEdgeSplits;

	public float MaxAngleChangeAfterFlip;

	public bool SmoothCurvature;

	public bool NaturalSmooth;

	public EdgeWeights EdgeWeights;

	public FillHoleNicelyParams()
	{
		EdgeWeights = EdgeWeights.Unit;
		triangulationParams = new FillHoleParams();
		TriangulateOnly = false;
		MaxEdgeLen = 0f;
		MaxEdgeSplits = 1000;
		MaxAngleChangeAfterFlip = MathF.PI / 6f;
		SmoothCurvature = true;
		NaturalSmooth = false;
	}
}
