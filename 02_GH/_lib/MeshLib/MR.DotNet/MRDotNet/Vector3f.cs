using System.Runtime.InteropServices;

namespace MR.DotNet;

public class Vector3f
{
	internal struct MRVector3f
	{
		public float x;

		public float y;

		public float z;

		public MRVector3f()
		{
			x = 0f;
			y = 0f;
			z = 0f;
		}
	}

	internal MRVector3f vec_;

	public float X
	{
		get
		{
			return vec_.x;
		}
		set
		{
			vec_.x = value;
		}
	}

	public float Y
	{
		get
		{
			return vec_.y;
		}
		set
		{
			vec_.y = value;
		}
	}

	public float Z
	{
		get
		{
			return vec_.z;
		}
		set
		{
			vec_.z = value;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVector3f mrVector3fDiagonal(float a);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVector3f mrVector3fPlusX();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVector3f mrVector3fPlusY();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVector3f mrVector3fPlusZ();

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVector3f mrVector3fAdd(ref MRVector3f a, ref MRVector3f b);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVector3f mrVector3fSub(ref MRVector3f a, ref MRVector3f b);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRVector3f mrVector3fMulScalar(ref MRVector3f a, float b);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern float mrVector3fLength(ref MRVector3f a);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern float mrVector3fLengthSq(ref MRVector3f a);

	public Vector3f()
	{
		vec_ = mrVector3fDiagonal(0f);
	}

	internal Vector3f(MRVector3f vec)
	{
		vec_ = vec;
	}

	public Vector3f(float x, float y, float z)
	{
		vec_.x = x;
		vec_.y = y;
		vec_.z = z;
	}

	public static Vector3f Diagonal(float a)
	{
		return new Vector3f(mrVector3fDiagonal(a));
	}

	public static Vector3f PlusX()
	{
		return new Vector3f(mrVector3fPlusX());
	}

	public static Vector3f PlusY()
	{
		return new Vector3f(mrVector3fPlusY());
	}

	public static Vector3f PlusZ()
	{
		return new Vector3f(mrVector3fPlusZ());
	}

	public static Vector3f operator +(Vector3f a, Vector3f b)
	{
		return new Vector3f(mrVector3fAdd(ref a.vec_, ref b.vec_));
	}

	public static Vector3f operator -(Vector3f a, Vector3f b)
	{
		return new Vector3f(mrVector3fSub(ref a.vec_, ref b.vec_));
	}

	public static Vector3f operator *(Vector3f a, float b)
	{
		return new Vector3f(mrVector3fMulScalar(ref a.vec_, b));
	}

	public static Vector3f operator *(float a, Vector3f b)
	{
		return new Vector3f(mrVector3fMulScalar(ref b.vec_, a));
	}

	public static bool operator ==(Vector3f a, Vector3f b)
	{
		if (a.vec_.x == b.vec_.x && a.vec_.y == b.vec_.y)
		{
			return a.vec_.z == b.vec_.z;
		}
		return false;
	}

	public static bool operator !=(Vector3f a, Vector3f b)
	{
		if (a.vec_.x == b.vec_.x && a.vec_.y == b.vec_.y)
		{
			return a.vec_.z != b.vec_.z;
		}
		return true;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is Vector3f))
		{
			return false;
		}
		return this == (Vector3f)obj;
	}

	public override int GetHashCode()
	{
		return vec_.x.GetHashCode() ^ vec_.y.GetHashCode() ^ vec_.z.GetHashCode();
	}

	public float Length()
	{
		return mrVector3fLength(ref vec_);
	}

	public float LengthSq()
	{
		return mrVector3fLengthSq(ref vec_);
	}
}
