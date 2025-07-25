using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class PointCloud : MeshOrPoints, IDisposable
{
	private bool disposed;

	internal IntPtr pc_;

	private List<Vector3f>? points_;

	private List<Vector3f>? normals_;

	private BitSet? validPoints_;

	private Box3f? boundingBox_;

	public ReadOnlyCollection<Vector3f> Points
	{
		get
		{
			if (points_ == null)
			{
				int num = (int)mrPointCloudPointsNum(pc_);
				points_ = new List<Vector3f>(num);
				int num2 = Marshal.SizeOf(typeof(Vector3f.MRVector3f));
				IntPtr pointer = mrPointCloudPoints(pc_);
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
				validPoints_ = new BitSet(mrPointCloudValidPoints(pc_));
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
				boundingBox_ = new Box3f(mrPointCloudComputeBoundingBox(pc_, (IntPtr)(void*)null));
			}
			return boundingBox_;
		}
	}

	public ReadOnlyCollection<Vector3f> Normals
	{
		get
		{
			if (normals_ == null)
			{
				int num = (int)mrPointCloudNormalsNum(pc_);
				normals_ = new List<Vector3f>(num);
				int num2 = Marshal.SizeOf(typeof(Vector3f.MRVector3f));
				IntPtr pointer = mrPointCloudNormals(pc_);
				for (int i = 0; i < num; i++)
				{
					Vector3f.MRVector3f vec = Marshal.PtrToStructure<Vector3f.MRVector3f>(IntPtr.Add(pointer, i * num2));
					normals_.Add(new Vector3f(vec));
				}
			}
			return normals_.AsReadOnly();
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrPointCloudNew();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrPointCloudFromPoints(IntPtr points, ulong pointsNum);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrPointCloudPoints(IntPtr pc);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrPointCloudPointsRef(IntPtr pc);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ulong mrPointCloudPointsNum(IntPtr pc);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrPointCloudNormals(IntPtr pc);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern ulong mrPointCloudNormalsNum(IntPtr pc);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrPointCloudValidPoints(IntPtr pc);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern Box3f.MRBox3f mrPointCloudComputeBoundingBox(IntPtr pc, IntPtr toWorld);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern VertId mrPointCloudAddPoint(IntPtr pc, ref Vector3f.MRVector3f point);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern VertId mrPointCloudAddPointWithNormal(IntPtr pc, ref Vector3f.MRVector3f point, ref Vector3f.MRVector3f normal);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrPointCloudFree(IntPtr pc);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private unsafe static extern IntPtr mrPointsLoadFromAnySupportedFormat(string filename, IntPtr* errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private unsafe static extern void mrPointsSaveToAnySupportedFormat(IntPtr pc, string file, IntPtr* errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrLoadIOExtras();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrStringData(IntPtr str);

	public PointCloud()
	{
		pc_ = mrPointCloudNew();
	}

	internal PointCloud(IntPtr pc)
	{
		pc_ = pc;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (disposing && (object)validPoints_ != null)
		{
			validPoints_.Dispose();
			validPoints_ = null;
		}
		if (!disposed)
		{
			if (pc_ != IntPtr.Zero)
			{
				mrPointCloudFree(pc_);
			}
			disposed = true;
		}
	}

	~PointCloud()
	{
		mrPointCloudFree(pc_);
	}

	public unsafe static PointCloud FromAnySupportedFormat(string path)
	{
		mrLoadIOExtras();
		IntPtr intPtr = default(IntPtr);
		IntPtr pc = mrPointsLoadFromAnySupportedFormat(path, &intPtr);
		if (intPtr != IntPtr.Zero)
		{
			throw new SystemException(Marshal.PtrToStringAnsi(mrStringData(intPtr)));
		}
		return new PointCloud(pc);
	}

	public unsafe static void ToAnySupportedFormat(PointCloud pc, string path)
	{
		mrLoadIOExtras();
		IntPtr intPtr = default(IntPtr);
		mrPointsSaveToAnySupportedFormat(pc.pc_, path, &intPtr);
		if (intPtr != IntPtr.Zero)
		{
			throw new SystemException(Marshal.PtrToStringAnsi(mrStringData(intPtr)));
		}
	}

	public void AddPoint(Vector3f point)
	{
		if (mrPointCloudNormalsNum(pc_) != 0)
		{
			throw new InvalidOperationException("Normals must be empty");
		}
		mrPointCloudAddPoint(pc_, ref point.vec_);
	}

	public void AddPoint(Vector3f point, Vector3f normal)
	{
		if (mrPointCloudNormalsNum(pc_) != mrPointCloudPointsNum(pc_))
		{
			throw new InvalidOperationException("Points and normals must have the same size");
		}
		mrPointCloudAddPointWithNormal(pc_, ref point.vec_, ref normal.vec_);
	}
}
