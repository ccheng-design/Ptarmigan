using System;

namespace MR.DotNet;

internal struct MRDecimateParameters
{
	public DecimateStrategy strategy;

	public float maxError;

	public float maxEdgeLen;

	public float maxBdShift;

	public float maxTriangleAspectRatio;

	public float criticalTriAspectRatio;

	public float tinyEdgeLength;

	public float stabilizer;

	public byte optimizeVertexPos;

	public int maxDeletedVertices;

	public int maxDeletedFaces;

	public IntPtr region;

	public byte collapseNearNotFlippable;

	public byte touchNearBdEdges;

	public byte touchBdVerts;

	public float maxAngleChange;

	public byte packMesh;

	public IntPtr progressCallback;

	public int subdivideParts;

	public byte decimateBetweenParts;

	public int minFacesInPart;

	public MRDecimateParameters()
	{
		strategy = DecimateStrategy.MinimizeError;
		maxError = 0.001f;
		maxEdgeLen = float.MaxValue;
		maxBdShift = float.MaxValue;
		maxTriangleAspectRatio = 20f;
		criticalTriAspectRatio = float.MaxValue;
		tinyEdgeLength = -1f;
		stabilizer = 0.001f;
		optimizeVertexPos = 1;
		maxDeletedVertices = int.MaxValue;
		maxDeletedFaces = int.MaxValue;
		region = IntPtr.Zero;
		collapseNearNotFlippable = 1;
		touchNearBdEdges = 1;
		touchBdVerts = 1;
		maxAngleChange = -1f;
		packMesh = 0;
		progressCallback = IntPtr.Zero;
		subdivideParts = 1;
		decimateBetweenParts = 1;
		minFacesInPart = 0;
	}
}
