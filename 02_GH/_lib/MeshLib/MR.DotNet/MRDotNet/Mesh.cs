using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class Mesh : MeshOrPoints, IDisposable
{
	internal IntPtr mesh_;

	internal IntPtr meshTopology_;

	private bool needToDispose_ = true;

	private List<Vector3f>? points_;

	private BitSet? validPoints_;

	private BitSet? validFaces_;

	private List<ThreeVertIds>? triangulation_;

	private List<EdgeId>? holeRepresentiveEdges_;

	private Box3f? boundingBox_;

	public ReadOnlyCollection<Vector3f> Points
	{
		get
		{
			if (points_ == null)
			{
				int num = (int)mrMeshPointsNum(mesh_);
				points_ = new List<Vector3f>(num);
				int num2 = Marshal.SizeOf(typeof(Vector3f.MRVector3f));
				IntPtr pointer = mrMeshPoints(mesh_);
				for (int i = 0; i < num; i++)
				{
					Vector3f.MRVector3f vec = Marshal.PtrToStructure<Vector3f.MRVector3f>(IntPtr.Add(pointer, i * num2));
					points_.Add(new Vector3f(vec));
				}
			}
			return points_.AsReadOnly();
		}
	}

	public BitSetReadOnly ValidPoints
	{
		get
		{
			if ((object)validPoints_ == null)
			{
				validPoints_ = new BitSet(mrMeshTopologyGetValidVerts(meshTopology_));
			}
			return validPoints_;
		}
	}

	public unsafe Box3f BoundingBox
	{
		get
		{
			if (boundingBox_ == null)
			{
				boundingBox_ = new Box3f(mrMeshComputeBoundingBox(meshTopology_, (IntPtr)(void*)null));
			}
			return boundingBox_;
		}
	}

	public BitSetReadOnly ValidFaces
	{
		get
		{
			if ((object)validFaces_ == null)
			{
				validFaces_ = new BitSet(mrMeshTopologyGetValidFaces(meshTopology_));
			}
			return validFaces_;
		}
	}

	public ReadOnlyCollection<ThreeVertIds> Triangulation
	{
		get
		{
			if (triangulation_ == null)
			{
				MRTriangulation mRTriangulation = mrMeshTopologyGetTriangulation(meshTopology_);
				triangulation_ = new List<ThreeVertIds>((int)mRTriangulation.size);
				int num = Marshal.SizeOf(typeof(ThreeVertIds));
				IntPtr data = mRTriangulation.data;
				for (int i = 0; i < triangulation_.Capacity; i++)
				{
					IntPtr ptr = IntPtr.Add(data, i * num);
					triangulation_.Add(Marshal.PtrToStructure<ThreeVertIds>(ptr));
				}
			}
			return triangulation_.AsReadOnly();
		}
	}

	public ReadOnlyCollection<EdgeId> HoleRepresentiveEdges
	{
		get
		{
			if (holeRepresentiveEdges_ == null)
			{
				MREdgePath mREdgePath = mrMeshTopologyFindHoleRepresentiveEdges(meshTopology_);
				holeRepresentiveEdges_ = new List<EdgeId>((int)mREdgePath.size);
				int num = Marshal.SizeOf(typeof(EdgeId));
				IntPtr data = mREdgePath.data;
				for (int i = 0; i < (int)mREdgePath.size; i++)
				{
					IntPtr ptr = IntPtr.Add(data, i * num);
					holeRepresentiveEdges_.Add(Marshal.PtrToStructure<EdgeId>(ptr));
				}
			}
			return holeRepresentiveEdges_.AsReadOnly();
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshTopologyPack(IntPtr top);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshTopologyGetValidVerts(IntPtr top);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshTopologyGetValidFaces(IntPtr top);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ref MRTriangulation mrMeshTopologyGetTriangulation(IntPtr top);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ulong mrMeshTopologyFaceSize(IntPtr top);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ref MREdgePath mrMeshTopologyFindHoleRepresentiveEdges(IntPtr top);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshTopologyGetLeftTriVerts(IntPtr top, EdgeId a, ref VertId v0, ref VertId v1, ref VertId v2);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern int mrMeshTopologyFindNumHoles(IntPtr top, IntPtr holeRepresentativeEdges);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshFromTriangles(IntPtr vertexCoordinates, ulong vertexCoordinatesNum, IntPtr t, ulong tNum);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshFromTrianglesDuplicatingNonManifoldVertices(IntPtr vertexCoordinates, ulong vertexCoordinatesNum, IntPtr t, ulong tNum);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshPoints(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ulong mrMeshPointsNum(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMakeCube(ref Vector3f.MRVector3f size, ref Vector3f.MRVector3f baseCoords);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRMakeTorusParameters mrMakeTorusParametersNew();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMakeTorus(ref MRMakeTorusParameters parameters);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMakeTorusWithSelfIntersections(ref MRMakeTorusParameters parameters);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRSphereParams mrSphereParamsNew();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMakeSphere(ref MRSphereParams parameters);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern Box3f.MRBox3f mrMeshComputeBoundingBox(IntPtr mesh, IntPtr toWorld);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshTransform(IntPtr mesh, ref MRAffineXf3f xf, IntPtr region);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern double mrMeshVolume(IntPtr mesh, IntPtr region);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshPackOptimally(IntPtr mesh, bool preserveAABBTree);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshTopology(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMeshCopy(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshFree(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRFindProjectionParameters mrFindProjectionParametersNew();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRMeshProjectionResult mrFindProjection(ref Vector3f.MRVector3f pt, ref MRMeshPart mp, ref MRFindProjectionParameters parameters);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrStringData(IntPtr str);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ulong mrStringSize(IntPtr str);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrStringFree(IntPtr str);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern double mrMeshArea(IntPtr mesh, IntPtr region);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrMeshDeleteFaces(IntPtr mesh, IntPtr fs, IntPtr keepEdges);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern float mrMeshEdgeLength(IntPtr mesh, UndirectedEdgeId e);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern float mrMeshEdgeLengthSq(IntPtr mesh, UndirectedEdgeId e);

	internal Mesh(IntPtr mesh)
	{
		mesh_ = mesh;
		meshTopology_ = mrMeshTopology(mesh);
	}

	internal void SkipDisposingAtFinalize()
	{
		needToDispose_ = false;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!needToDispose_)
		{
			return;
		}
		if (disposing)
		{
			if ((object)validFaces_ != null)
			{
				validFaces_.Dispose();
				validFaces_ = null;
			}
			if ((object)validPoints_ != null)
			{
				validPoints_.Dispose();
				validPoints_ = null;
			}
		}
		if (mesh_ != IntPtr.Zero)
		{
			mrMeshFree(mesh_);
			mesh_ = IntPtr.Zero;
		}
		needToDispose_ = false;
	}

	~Mesh()
	{
		Dispose(disposing: false);
	}

	public VertId[] GetLeftTriVerts(EdgeId edgeId)
	{
		VertId[] array = new VertId[3];
		VertId v = default(VertId);
		VertId v2 = default(VertId);
		VertId v3 = default(VertId);
		mrMeshTopologyGetLeftTriVerts(a: new EdgeId
		{
			Id = edgeId.Id
		}, top: meshTopology_, v0: ref v, v1: ref v2, v2: ref v3);
		array[0].Id = v.Id;
		array[1].Id = v2.Id;
		array[2].Id = v3.Id;
		return array;
	}

	public unsafe void Transform(AffineXf3f xf)
	{
		mrMeshTransform(mesh_, ref xf.xf_, (IntPtr)(void*)null);
		clearManagedResources();
	}

	public void Transform(AffineXf3f xf, BitSet region)
	{
		mrMeshTransform(mesh_, ref xf.xf_, region.bs_);
		clearManagedResources();
	}

	public void PackOptimally()
	{
		mrMeshPackOptimally(mesh_, preserveAABBTree: true);
		clearManagedResources();
	}

	public unsafe double Volume()
	{
		return mrMeshVolume(mesh_, (IntPtr)(void*)null);
	}

	public double Volume(BitSet region)
	{
		return mrMeshVolume(mesh_, region.bs_);
	}

	public unsafe double Area(BitSet? region = null)
	{
		return mrMeshArea(mesh_, region?.bs_ ?? ((IntPtr)(void*)null));
	}

	public float EdgeLength(UndirectedEdgeId ue)
	{
		return mrMeshEdgeLength(e: new UndirectedEdgeId
		{
			Id = ue.Id
		}, mesh: mesh_);
	}

	public float EdgeLengthSq(UndirectedEdgeId ue)
	{
		return mrMeshEdgeLengthSq(e: new UndirectedEdgeId
		{
			Id = ue.Id
		}, mesh: mesh_);
	}

	public unsafe void DeleteFaces(BitSet faces, BitSet? edgesToKeep = null)
	{
		mrMeshDeleteFaces(mesh_, faces.bs_, edgesToKeep?.bs_ ?? ((IntPtr)(void*)null));
		clearManagedResources();
	}

	public Mesh Clone()
	{
		return new Mesh(mrMeshCopy(mesh_));
	}

	public static Mesh FromTriangles(List<Vector3f> points, List<ThreeVertIds> triangles)
	{
		int num = Marshal.SizeOf(typeof(Vector3f.MRVector3f));
		IntPtr intPtr = Marshal.AllocHGlobal(points.Count * num);
		int num2 = Marshal.SizeOf(typeof(ThreeVertIds));
		IntPtr intPtr2 = Marshal.AllocHGlobal(triangles.Count * num2);
		try
		{
			for (int i = 0; i < points.Count; i++)
			{
				Marshal.StructureToPtr(points[i].vec_, IntPtr.Add(intPtr, i * num), fDeleteOld: false);
			}
			for (int j = 0; j < triangles.Count; j++)
			{
				Marshal.StructureToPtr(triangles[j], IntPtr.Add(intPtr2, j * num2), fDeleteOld: false);
			}
			return new Mesh(mrMeshFromTriangles(intPtr, (ulong)points.Count, intPtr2, (ulong)triangles.Count));
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
		}
	}

	public static Mesh FromTrianglesDuplicatingNonManifoldVertices(List<Vector3f> points, List<ThreeVertIds> triangles)
	{
		int num = Marshal.SizeOf(typeof(Vector3f.MRVector3f));
		IntPtr intPtr = Marshal.AllocHGlobal(points.Count * num);
		int num2 = Marshal.SizeOf(typeof(ThreeVertIds));
		IntPtr intPtr2 = Marshal.AllocHGlobal(triangles.Count * num2);
		try
		{
			for (int i = 0; i < points.Count; i++)
			{
				Marshal.StructureToPtr(points[i], IntPtr.Add(intPtr, i * num), fDeleteOld: false);
			}
			for (int j = 0; j < triangles.Count; j++)
			{
				Marshal.StructureToPtr(triangles[j], IntPtr.Add(intPtr2, j * num2), fDeleteOld: false);
			}
			return new Mesh(mrMeshFromTrianglesDuplicatingNonManifoldVertices(intPtr, (ulong)points.Count, intPtr2, (ulong)triangles.Count));
		}
		finally
		{
			Marshal.FreeHGlobal(intPtr);
			Marshal.FreeHGlobal(intPtr2);
		}
	}

	public static Mesh MakeCube(Vector3f size, Vector3f baseCoords)
	{
		return new Mesh(mrMakeCube(ref size.vec_, ref baseCoords.vec_));
	}

	public static Mesh MakeSphere(float radius, int vertexCount)
	{
		MRSphereParams parameters = new MRSphereParams();
		parameters.radius = radius;
		parameters.numMeshVertices = vertexCount;
		return new Mesh(mrMakeSphere(ref parameters));
	}

	public static Mesh MakeTorus(float primaryRadius, float secondaryRadius, int primaryResolution, int secondaryResolution)
	{
		MRMakeTorusParameters parameters = new MRMakeTorusParameters();
		parameters.primaryRadius = primaryRadius;
		parameters.secondaryRadius = secondaryRadius;
		parameters.primaryResolution = primaryResolution;
		parameters.secondaryResolution = secondaryResolution;
		return new Mesh(mrMakeTorus(ref parameters));
	}

	internal static Mesh MakeTorusWithSelfIntersections(float primaryRadius, float secondaryRadius, int primaryResolution, int secondaryResolution)
	{
		MRMakeTorusParameters parameters = new MRMakeTorusParameters();
		parameters.primaryRadius = primaryRadius;
		parameters.secondaryRadius = secondaryRadius;
		parameters.primaryResolution = primaryResolution;
		parameters.secondaryResolution = secondaryResolution;
		return new Mesh(mrMakeTorusWithSelfIntersections(ref parameters));
	}

	public unsafe static MeshProjectionResult FindProjection(Vector3f point, MeshPart meshPart, float maxDistanceSquared = float.MaxValue, AffineXf3f? xf = null, float minDistanceSquared = 0f)
	{
		MRFindProjectionParameters parameters = new MRFindProjectionParameters();
		parameters.loDistLimitSq = minDistanceSquared;
		parameters.upDistLimitSq = maxDistanceSquared;
		parameters.xf = xf?.XfAddr() ?? ((IntPtr)(void*)null);
		MRMeshProjectionResult mRMeshProjectionResult = mrFindProjection(ref point.vec_, ref meshPart.mrMeshPart, ref parameters);
		MeshProjectionResult result = default(MeshProjectionResult);
		result.distanceSquared = mRMeshProjectionResult.distSq;
		result.pointOnFace = default(PointOnFace);
		result.pointOnFace.point = new Vector3f(mRMeshProjectionResult.proj.point);
		result.pointOnFace.faceId.Id = mRMeshProjectionResult.proj.face.Id;
		result.meshTriPoint = default(MeshTriPoint);
		result.meshTriPoint.e.Id = mRMeshProjectionResult.mtp.e.Id;
		result.meshTriPoint.bary.a = mRMeshProjectionResult.mtp.bary.a;
		result.meshTriPoint.bary.b = mRMeshProjectionResult.mtp.bary.b;
		return result;
	}

	private void clearManagedResources()
	{
		if ((object)validFaces_ != null)
		{
			validFaces_.Dispose();
			validFaces_ = null;
		}
		if ((object)validPoints_ != null)
		{
			validPoints_.Dispose();
			validPoints_ = null;
		}
		points_ = null;
		triangulation_ = null;
		holeRepresentiveEdges_ = null;
		boundingBox_ = null;
	}

	internal IntPtr varMesh()
	{
		clearManagedResources();
		return mesh_;
	}
}
