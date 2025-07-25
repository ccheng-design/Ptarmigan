using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshBoolean
{
	internal struct MRBooleanParameters
	{
		public IntPtr rigidB2A;

		public IntPtr mapper;

		public byte mergeAllNonIntersectingComponents;

		public IntPtr cb;
	}

	internal struct MRBooleanResult
	{
		public IntPtr mesh;

		public IntPtr errorString;

		public MRBooleanResult()
		{
			mesh = IntPtr.Zero;
			errorString = IntPtr.Zero;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRBooleanResult mrBoolean(IntPtr meshA, IntPtr meshB, BooleanOperation operation, ref MRBooleanParameters parameters);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrStringData(IntPtr str);

	public static BooleanResult Boolean(Mesh meshA, Mesh meshB, BooleanOperation op)
	{
		return Boolean(meshA, meshB, op, new BooleanParameters());
	}

	public unsafe static BooleanResult Boolean(Mesh meshA, Mesh meshB, BooleanOperation op, BooleanParameters parameters)
	{
		MRBooleanParameters parameters2 = default(MRBooleanParameters);
		parameters2.rigidB2A = ((parameters.rigidB2A == null) ? ((IntPtr)(void*)null) : parameters.rigidB2A.XfAddr());
		parameters2.mapper = ((parameters.mapper == null) ? ((IntPtr)(void*)null) : parameters.mapper.Mapper);
		parameters2.mergeAllNonIntersectingComponents = (parameters.mergeAllNonIntersectingComponents ? ((byte)1) : ((byte)0));
		parameters2.cb = IntPtr.Zero;
		MRBooleanResult mRBooleanResult = mrBoolean(meshA.mesh_, meshB.mesh_, op, ref parameters2);
		string text = string.Empty;
		if (mRBooleanResult.errorString != IntPtr.Zero)
		{
			text = Marshal.PtrToStringAnsi(mrStringData(mRBooleanResult.errorString));
		}
		if (!string.IsNullOrEmpty(text))
		{
			throw new SystemException(text);
		}
		return new BooleanResult
		{
			mesh = new Mesh(mRBooleanResult.mesh)
		};
	}
}
