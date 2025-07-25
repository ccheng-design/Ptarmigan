using System;

namespace MR.DotNet;

internal struct MRFindProjectionParameters
{
	public float upDistLimitSq;

	public IntPtr xf;

	public float loDistLimitSq;

	public MRFindProjectionParameters()
	{
		upDistLimitSq = float.MaxValue;
		xf = IntPtr.Zero;
		loDistLimitSq = 0f;
	}
}
