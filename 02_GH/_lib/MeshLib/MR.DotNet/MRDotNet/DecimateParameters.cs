namespace MR.DotNet;

public struct DecimateParameters
{
	public DecimateStrategy strategy;

	public float maxError;

	public float maxEdgeLen;

	public float maxBdShift;

	public float maxTriangleAspectRatio;

	public float criticalTriAspectRatio;

	public float tinyEdgeLength;

	public float stabilizer;

	public bool optimizeVertexPos;

	public int maxDeletedVertices;

	public int maxDeletedFaces;

	public BitSet? region;

	public bool collapseNearNotFlippable;

	public bool touchNearBdEdges;

	public bool touchBdVerts;

	public float maxAngleChange;

	public bool packMesh;

	public int subdivideParts;

	public bool decimateBetweenParts;

	public int minFacesInPart;

	public DecimateParameters()
	{
		strategy = DecimateStrategy.MinimizeError;
		maxError = 0.001f;
		maxEdgeLen = float.MaxValue;
		maxBdShift = float.MaxValue;
		maxTriangleAspectRatio = 20f;
		criticalTriAspectRatio = float.MaxValue;
		tinyEdgeLength = -1f;
		stabilizer = 0.001f;
		optimizeVertexPos = true;
		maxDeletedVertices = int.MaxValue;
		maxDeletedFaces = int.MaxValue;
		region = null;
		collapseNearNotFlippable = false;
		touchNearBdEdges = true;
		touchBdVerts = true;
		maxAngleChange = -1f;
		packMesh = false;
		subdivideParts = 1;
		decimateBetweenParts = true;
		minFacesInPart = 0;
	}
}
