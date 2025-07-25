using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshFillHole
{
	internal struct MRFillHoleParams
	{
		public IntPtr metric;

		public IntPtr outNewFaces;

		public MultipleEdgesResolveMode multipleEdgesResolveMode;

		public byte makeDegenerateBand;

		public int maxPolygonSubdivisions;

		public IntPtr stopBeforeBadTriangulation;

		public MRFillHoleParams()
		{
			metric = IntPtr.Zero;
			outNewFaces = IntPtr.Zero;
			multipleEdgesResolveMode = MultipleEdgesResolveMode.Simple;
			makeDegenerateBand = 0;
			maxPolygonSubdivisions = 20;
			stopBeforeBadTriangulation = IntPtr.Zero;
		}
	}

	internal struct MRFillHoleNicelyParams
	{
		public MRFillHoleParams triangulationParams;

		public byte triangulateOnly;

		public float maxEdgeLen;

		public int maxEdgeSplits;

		public float maxAngleChangeAfterFlip;

		public byte smoothCurvature;

		public byte naturalSmooth;

		public EdgeWeights edgeWeights;

		public MRFillHoleNicelyParams()
		{
			edgeWeights = EdgeWeights.Unit;
			triangulationParams = new MRFillHoleParams();
			triangulateOnly = 0;
			maxEdgeLen = 0f;
			maxEdgeSplits = 1000;
			maxAngleChangeAfterFlip = MathF.PI / 6f;
			smoothCurvature = 1;
			naturalSmooth = 0;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrFillHole(IntPtr mesh, EdgeId a, ref MRFillHoleParams parameters);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrFillHoles(IntPtr mesh, IntPtr pAs, ulong asNum, ref MRFillHoleParams parameters);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrFillHoleNicely(IntPtr mesh, EdgeId holeEdge, ref MRFillHoleNicelyParams parameters);

	public unsafe static void FillHole(ref Mesh mesh, EdgeId a, FillHoleParams parameters)
	{
		MRFillHoleParams parameters2 = default(MRFillHoleParams);
		parameters2.metric = parameters.Metric.mrMetric_;
		parameters2.outNewFaces = parameters.OutNewFaces?.bs_ ?? IntPtr.Zero;
		parameters2.multipleEdgesResolveMode = parameters.MultipleEdgesResolveMode;
		parameters2.makeDegenerateBand = (parameters.MakeDegenerateBand ? ((byte)1) : ((byte)0));
		parameters2.maxPolygonSubdivisions = parameters.MaxPolygonSubdivisions;
		byte b = 0;
		parameters2.stopBeforeBadTriangulation = (parameters.StopBeforeBadTriangulation.HasValue ? new IntPtr(&b) : IntPtr.Zero);
		mrFillHole(mesh.varMesh(), a, ref parameters2);
		if (parameters.StopBeforeBadTriangulation.HasValue)
		{
			parameters.StopBeforeBadTriangulation = b > 0;
			if (parameters.StopBeforeBadTriangulation.Value)
			{
				throw new Exception("Bad triangulation");
			}
		}
	}

	public unsafe static BitSet FillHoleNicely(ref Mesh mesh, EdgeId holeEdge, FillHoleNicelyParams parameters)
	{
		MRFillHoleNicelyParams parameters2 = default(MRFillHoleNicelyParams);
		parameters2.triangulationParams.metric = parameters.triangulationParams.Metric.mrMetric_;
		parameters2.triangulationParams.outNewFaces = parameters.triangulationParams.OutNewFaces?.bs_ ?? IntPtr.Zero;
		parameters2.triangulationParams.multipleEdgesResolveMode = parameters.triangulationParams.MultipleEdgesResolveMode;
		parameters2.triangulationParams.makeDegenerateBand = (parameters.triangulationParams.MakeDegenerateBand ? ((byte)1) : ((byte)0));
		parameters2.triangulationParams.maxPolygonSubdivisions = parameters.triangulationParams.MaxPolygonSubdivisions;
		byte b = 0;
		parameters2.triangulationParams.stopBeforeBadTriangulation = (parameters.triangulationParams.StopBeforeBadTriangulation.HasValue ? new IntPtr(&b) : IntPtr.Zero);
		parameters2.triangulateOnly = (parameters.TriangulateOnly ? ((byte)1) : ((byte)0));
		parameters2.maxEdgeLen = parameters.MaxEdgeLen;
		parameters2.maxEdgeSplits = parameters.MaxEdgeSplits;
		parameters2.maxAngleChangeAfterFlip = parameters.MaxAngleChangeAfterFlip;
		parameters2.smoothCurvature = (parameters.SmoothCurvature ? ((byte)1) : ((byte)0));
		parameters2.naturalSmooth = (parameters.NaturalSmooth ? ((byte)1) : ((byte)0));
		parameters2.edgeWeights = parameters.EdgeWeights;
		BitSet result = new BitSet(mrFillHoleNicely(mesh.varMesh(), holeEdge, ref parameters2));
		if (parameters.triangulationParams.StopBeforeBadTriangulation.HasValue)
		{
			parameters.triangulationParams.StopBeforeBadTriangulation = b > 0;
			if (parameters.triangulationParams.StopBeforeBadTriangulation.Value)
			{
				throw new Exception("Bad triangulation");
			}
		}
		return result;
	}

	public unsafe static void FillHoles(ref Mesh mesh, List<EdgeId> edges, FillHoleParams parameters)
	{
		MRFillHoleParams parameters2 = default(MRFillHoleParams);
		parameters2.metric = parameters.Metric.mrMetric_;
		parameters2.outNewFaces = parameters.OutNewFaces?.bs_ ?? IntPtr.Zero;
		parameters2.multipleEdgesResolveMode = parameters.MultipleEdgesResolveMode;
		parameters2.makeDegenerateBand = (parameters.MakeDegenerateBand ? ((byte)1) : ((byte)0));
		parameters2.maxPolygonSubdivisions = parameters.MaxPolygonSubdivisions;
		byte b = 0;
		parameters2.stopBeforeBadTriangulation = (parameters.StopBeforeBadTriangulation.HasValue ? new IntPtr(&b) : IntPtr.Zero);
		int num = Marshal.SizeOf(typeof(EdgeId));
		IntPtr intPtr = Marshal.AllocHGlobal(edges.Count * num);
		try
		{
			for (int i = 0; i < edges.Count; i++)
			{
				Marshal.StructureToPtr(edges[i], IntPtr.Add(intPtr, i * num), fDeleteOld: false);
			}
			mrFillHoles(mesh.varMesh(), intPtr, (ulong)edges.Count, ref parameters2);
			if (parameters.StopBeforeBadTriangulation.HasValue)
			{
				parameters.StopBeforeBadTriangulation = b > 0;
				if (parameters.StopBeforeBadTriangulation.Value)
				{
					throw new Exception("Bad triangulation");
				}
			}
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
		}
	}
}
