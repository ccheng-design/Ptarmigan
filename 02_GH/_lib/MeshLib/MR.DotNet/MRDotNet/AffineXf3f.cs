using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class AffineXf3f
{
	internal MRAffineXf3f xf_;

	private Matrix3f A_;

	private Vector3f b_;

	public Matrix3f A
	{
		get
		{
			return A_;
		}
		set
		{
			A_ = value;
			xf_.A = value.mat_;
		}
	}

	public Vector3f B
	{
		get
		{
			return b_;
		}
		set
		{
			b_ = value;
			xf_.b = value.vec_;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRAffineXf3f mrAffineXf3fMul(ref MRAffineXf3f a, ref MRAffineXf3f b);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern Vector3f.MRVector3f mrAffineXf3fApply(ref MRAffineXf3f xf, ref Vector3f.MRVector3f v);

	internal unsafe IntPtr XfAddr()
	{
		fixed (MRAffineXf3f* value = &xf_)
		{
			return new IntPtr(value);
		}
	}

	internal AffineXf3f(MRAffineXf3f xf)
	{
		xf_ = xf;
		A_ = new Matrix3f(xf.A);
		b_ = new Vector3f(xf.b);
	}

	public AffineXf3f()
		: this(new Matrix3f(), new Vector3f())
	{
	}

	public AffineXf3f(Matrix3f A)
		: this(A, new Vector3f())
	{
	}

	public AffineXf3f(Vector3f b)
		: this(new Matrix3f(), b)
	{
	}

	public AffineXf3f(Matrix3f A, Vector3f b)
	{
		A_ = A;
		b_ = b;
		xf_.A = A.mat_;
		xf_.b = b.vec_;
	}

	public Vector3f Apply(Vector3f v)
	{
		return new Vector3f(mrAffineXf3fApply(ref xf_, ref v.vec_));
	}

	public static AffineXf3f operator *(AffineXf3f a, AffineXf3f b)
	{
		return new AffineXf3f(mrAffineXf3fMul(ref a.xf_, ref b.xf_));
	}
}
