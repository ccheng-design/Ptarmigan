using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshDecimate
{
	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern DecimateResult mrDecimateMesh(IntPtr mesh, ref MRDecimateParameters settings);

	public unsafe static DecimateResult Decimate(Mesh mesh, DecimateParameters settings)
	{
		MRDecimateParameters settings2 = new MRDecimateParameters();
		settings2.strategy = settings.strategy;
		settings2.maxError = settings.maxError;
		settings2.maxEdgeLen = settings.maxEdgeLen;
		settings2.maxBdShift = settings.maxBdShift;
		settings2.maxTriangleAspectRatio = settings.maxTriangleAspectRatio;
		settings2.criticalTriAspectRatio = settings.criticalTriAspectRatio;
		settings2.tinyEdgeLength = settings.tinyEdgeLength;
		settings2.stabilizer = settings.stabilizer;
		settings2.optimizeVertexPos = (settings.optimizeVertexPos ? ((byte)1) : ((byte)0));
		settings2.maxDeletedVertices = settings.maxDeletedVertices;
		settings2.maxDeletedFaces = settings.maxDeletedFaces;
		settings2.region = (((object)settings.region == null) ? ((IntPtr)(void*)null) : settings.region.bs_);
		settings2.collapseNearNotFlippable = (settings.collapseNearNotFlippable ? ((byte)1) : ((byte)0));
		settings2.touchNearBdEdges = (settings.touchNearBdEdges ? ((byte)1) : ((byte)0));
		settings2.touchBdVerts = (settings.touchBdVerts ? ((byte)1) : ((byte)0));
		settings2.maxAngleChange = settings.maxAngleChange;
		settings2.packMesh = (settings.packMesh ? ((byte)1) : ((byte)0));
		settings2.subdivideParts = settings.subdivideParts;
		settings2.decimateBetweenParts = (settings.decimateBetweenParts ? ((byte)1) : ((byte)0));
		settings2.progressCallback = IntPtr.Zero;
		settings2.minFacesInPart = settings.minFacesInPart;
		return mrDecimateMesh(mesh.mesh_, ref settings2);
	}
}
