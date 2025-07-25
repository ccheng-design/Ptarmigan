using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class FillHoleMetric
{
	internal IntPtr mrMetric_;

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrFillHoleMetricFree(IntPtr metric);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern double mrCalcCombinedFillMetric(IntPtr mesh, IntPtr filledRegion, IntPtr metric);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrGetCircumscribedMetric(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrGetPlaneFillMetric(IntPtr mesh, EdgeId e);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrGetPlaneNormalizedFillMetric(IntPtr mesh, EdgeId e);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrGetComplexFillMetric(IntPtr mesh, EdgeId e);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrGetUniversalMetric(IntPtr mesh);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrGetMinAreaMetric(IntPtr mesh);

	private FillHoleMetric(IntPtr metric)
	{
		mrMetric_ = metric;
	}

	public FillHoleMetric()
	{
		mrMetric_ = IntPtr.Zero;
	}

	public static FillHoleMetric GetCircumscribedMetric(Mesh mesh)
	{
		return new FillHoleMetric(mrGetCircumscribedMetric(mesh.mesh_));
	}

	public static FillHoleMetric GetPlaneFillMetric(Mesh mesh, EdgeId e)
	{
		return new FillHoleMetric(mrGetPlaneFillMetric(mesh.mesh_, e));
	}

	public static FillHoleMetric GetPlaneNormalizedFillMetric(Mesh mesh, EdgeId e)
	{
		return new FillHoleMetric(mrGetPlaneNormalizedFillMetric(mesh.mesh_, e));
	}

	public static FillHoleMetric GetComplexFillMetric(Mesh mesh, EdgeId e)
	{
		return new FillHoleMetric(mrGetComplexFillMetric(mesh.mesh_, e));
	}

	public static FillHoleMetric GetUniversalMetric(Mesh mesh)
	{
		return new FillHoleMetric(mrGetUniversalMetric(mesh.mesh_));
	}

	public static FillHoleMetric GetMinAreaMetric(Mesh mesh)
	{
		return new FillHoleMetric(mrGetMinAreaMetric(mesh.mesh_));
	}

	~FillHoleMetric()
	{
		mrFillHoleMetricFree(mrMetric_);
	}

	public double CalcCombinedFillMetric(Mesh mesh, BitSet filledRegion)
	{
		return mrCalcCombinedFillMetric(mesh.mesh_, filledRegion.bs_, mrMetric_);
	}
}
