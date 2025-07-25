using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class FixSelfIntersections
{
	public enum Method
	{
		Relax,
		CutAndFill
	}

	public struct Settings
	{
		public Method method;

		public int relaxIterations;

		public int maxExpand;

		public float subdivideEdgeLen;

		public Settings()
		{
			method = Method.Relax;
			relaxIterations = 5;
			maxExpand = 3;
			subdivideEdgeLen = 0f;
		}
	}

	internal struct MRFixSelfIntersectionsSettings
	{
		public Method method;

		public int relaxIterations;

		public int maxExpand;

		public float subdivideEdgeLen;

		public IntPtr cb;

		public MRFixSelfIntersectionsSettings()
		{
			method = Method.Relax;
			relaxIterations = 5;
			maxExpand = 3;
			subdivideEdgeLen = 0f;
			cb = IntPtr.Zero;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrStringData(IntPtr str);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrFixSelfIntersectionsGetFaces(IntPtr mesh, IntPtr cb, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern void mrFixSelfIntersectionsFix(IntPtr mesh, ref MRFixSelfIntersectionsSettings settings, ref IntPtr errorString);

	public static BitSet GetFaces(Mesh mesh)
	{
		IntPtr errorString = IntPtr.Zero;
		IntPtr bs = mrFixSelfIntersectionsGetFaces(mesh.mesh_, IntPtr.Zero, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new SystemException(Marshal.PtrToStringAnsi(mrStringData(errorString)));
		}
		return new BitSet(bs);
	}

	public static void Fix(ref Mesh mesh, Settings settings)
	{
		IntPtr errorString = IntPtr.Zero;
		MRFixSelfIntersectionsSettings settings2 = new MRFixSelfIntersectionsSettings();
		settings2.method = settings.method;
		settings2.relaxIterations = settings.relaxIterations;
		settings2.maxExpand = settings.maxExpand;
		settings2.subdivideEdgeLen = settings.subdivideEdgeLen;
		mrFixSelfIntersectionsFix(mesh.mesh_, ref settings2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new SystemException(Marshal.PtrToStringAnsi(mrStringData(errorString)));
		}
	}
}
