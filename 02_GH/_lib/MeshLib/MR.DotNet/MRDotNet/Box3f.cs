using System.Runtime.InteropServices;

namespace MR.DotNet;

public class Box3f
{
	internal struct MRBox3f
	{
		public Vector3f.MRVector3f min;

		public Vector3f.MRVector3f max;
	}

	private MRBox3f box_;

	private Vector3f min_;

	private Vector3f max_;

	public Vector3f Min
	{
		get
		{
			return min_;
		}
		set
		{
			min_ = value;
			box_.min = value.vec_;
		}
	}

	public Vector3f Max
	{
		get
		{
			return max_;
		}
		set
		{
			max_ = value;
			box_.max = value.vec_;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRBox3f mrBox3fNew();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	[return: MarshalAs(UnmanagedType.I1)]
	private static extern bool mrBox3fValid(ref MRBox3f box);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern Vector3f.MRVector3f mrBox3fSize(ref MRBox3f box);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern float mrBox3fDiagonal(ref MRBox3f box);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern float mrBox3fVolume(ref MRBox3f box);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern Vector3f.MRVector3f mrBox3fCenter(ref MRBox3f box);

	internal Box3f(MRBox3f box)
	{
		box_ = box;
		min_ = new Vector3f(box_.min);
		max_ = new Vector3f(box_.max);
	}

	public Box3f()
	{
		box_ = mrBox3fNew();
		min_ = new Vector3f(box_.min);
		max_ = new Vector3f(box_.max);
	}

	public Box3f(Vector3f min, Vector3f max)
	{
		box_.min = min.vec_;
		box_.max = max.vec_;
		min_ = min;
		max_ = max;
	}

	public bool Valid()
	{
		return mrBox3fValid(ref box_);
	}

	public Vector3f Size()
	{
		return new Vector3f(mrBox3fSize(ref box_));
	}

	public float Diagonal()
	{
		return mrBox3fDiagonal(ref box_);
	}

	public float Volume()
	{
		return mrBox3fVolume(ref box_);
	}

	public Vector3f Center()
	{
		return new Vector3f(mrBox3fCenter(ref box_));
	}
}
