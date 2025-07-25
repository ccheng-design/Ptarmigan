using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class Offset
{
	internal struct MROffsetParameters
	{
		public float voxelSize;

		public IntPtr callBack;

		public SignDetectionMode signDetectionMode;

		public byte memoryEfficient;

		public MROffsetParameters()
		{
			voxelSize = 0f;
			callBack = IntPtr.Zero;
			signDetectionMode = SignDetectionMode.OpenVDB;
			memoryEfficient = 0;
		}
	}

	internal struct MRGeneralOffsetParameters
	{
		public float minNewVertDev;

		public float maxNewRank2VertDev;

		public float maxNewRank3VertDev;

		public float maxOldVertPosCorrection;

		public GeneralOffsetMode mode;

		public MRGeneralOffsetParameters()
		{
			minNewVertDev = 0.04f;
			maxNewRank2VertDev = 5f;
			maxNewRank3VertDev = 2f;
			maxOldVertPosCorrection = 0.5f;
			mode = GeneralOffsetMode.Standard;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern float mrSuggestVoxelSize(MRMeshPart mp, float approxNumVoxels);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrOffsetMesh(MRMeshPart mp, float offset, ref MROffsetParameters parameters, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrDoubleOffsetMesh(MRMeshPart mp, float offsetA, float offsetB, ref MROffsetParameters parameters, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMcOffsetMesh(MRMeshPart mp, float offset, ref MROffsetParameters parameters, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrMcShellMeshRegion(IntPtr mesh, IntPtr region, float offset, ref MROffsetParameters parameters, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrSharpOffsetMesh(MRMeshPart mp, float offset, ref MROffsetParameters parameters, ref MRGeneralOffsetParameters generalParams, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrGeneralOffsetMesh(MRMeshPart mp, float offset, ref MROffsetParameters parameters, ref MRGeneralOffsetParameters generalParams, ref IntPtr errorString);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern IntPtr mrThickenMesh(IntPtr mesh, float offset, ref MROffsetParameters parameters, ref MRGeneralOffsetParameters generalParams, ref IntPtr errorString);

	public static float SuggestVoxelSize(MeshPart mp, float approxNumVoxels)
	{
		return mrSuggestVoxelSize(mp.mrMeshPart, approxNumVoxels);
	}

	public static Mesh OffsetMesh(MeshPart mp, float offset, OffsetParameters parameters)
	{
		IntPtr errorString = IntPtr.Zero;
		MROffsetParameters parameters2 = new MROffsetParameters();
		parameters2.callBack = IntPtr.Zero;
		parameters2.memoryEfficient = (parameters.memoryEfficient ? ((byte)1) : ((byte)0));
		parameters2.signDetectionMode = parameters.signDetectionMode;
		parameters2.voxelSize = parameters.voxelSize;
		IntPtr mesh = mrOffsetMesh(mp.mrMeshPart, offset, ref parameters2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new Exception(Marshal.PtrToStringAnsi(errorString));
		}
		return new Mesh(mesh);
	}

	public static Mesh DoubleOffsetMesh(MeshPart mp, float offsetA, float offsetB, OffsetParameters parameters)
	{
		IntPtr errorString = IntPtr.Zero;
		MROffsetParameters parameters2 = new MROffsetParameters();
		parameters2.callBack = IntPtr.Zero;
		parameters2.memoryEfficient = (parameters.memoryEfficient ? ((byte)1) : ((byte)0));
		parameters2.signDetectionMode = parameters.signDetectionMode;
		parameters2.voxelSize = parameters.voxelSize;
		IntPtr mesh = mrDoubleOffsetMesh(mp.mrMeshPart, offsetA, offsetB, ref parameters2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new Exception(Marshal.PtrToStringAnsi(errorString));
		}
		return new Mesh(mesh);
	}

	public static Mesh McOffsetMesh(MeshPart mp, float offset, OffsetParameters parameters)
	{
		IntPtr errorString = IntPtr.Zero;
		MROffsetParameters parameters2 = new MROffsetParameters();
		parameters2.callBack = IntPtr.Zero;
		parameters2.memoryEfficient = (parameters.memoryEfficient ? ((byte)1) : ((byte)0));
		parameters2.signDetectionMode = parameters.signDetectionMode;
		parameters2.voxelSize = parameters.voxelSize;
		IntPtr mesh = mrMcOffsetMesh(mp.mrMeshPart, offset, ref parameters2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new Exception(Marshal.PtrToStringAnsi(errorString));
		}
		return new Mesh(mesh);
	}

	public static Mesh McShellMeshRegion(MeshPart mp, float offset, OffsetParameters parameters)
	{
		IntPtr errorString = IntPtr.Zero;
		MROffsetParameters parameters2 = new MROffsetParameters();
		parameters2.callBack = IntPtr.Zero;
		parameters2.memoryEfficient = (parameters.memoryEfficient ? ((byte)1) : ((byte)0));
		parameters2.signDetectionMode = parameters.signDetectionMode;
		parameters2.voxelSize = parameters.voxelSize;
		if ((object)mp.region == null)
		{
			throw new Exception("region is null");
		}
		IntPtr mesh = mrMcShellMeshRegion(mp.mesh.mesh_, mp.region.bs_, offset, ref parameters2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new Exception(Marshal.PtrToStringAnsi(errorString));
		}
		return new Mesh(mesh);
	}

	public static Mesh SharpOffsetMesh(MeshPart mp, float offset, OffsetParameters parameters, GeneralOffsetParameters generalParams)
	{
		IntPtr errorString = IntPtr.Zero;
		MROffsetParameters parameters2 = new MROffsetParameters();
		parameters2.callBack = IntPtr.Zero;
		parameters2.memoryEfficient = (parameters.memoryEfficient ? ((byte)1) : ((byte)0));
		parameters2.signDetectionMode = parameters.signDetectionMode;
		parameters2.voxelSize = parameters.voxelSize;
		MRGeneralOffsetParameters generalParams2 = new MRGeneralOffsetParameters();
		generalParams2.maxNewRank2VertDev = generalParams.maxNewRank2VertDev;
		generalParams2.maxNewRank3VertDev = generalParams.maxNewRank3VertDev;
		generalParams2.maxOldVertPosCorrection = generalParams.maxOldVertPosCorrection;
		generalParams2.minNewVertDev = generalParams.minNewVertDev;
		generalParams2.mode = generalParams.mode;
		IntPtr mesh = mrSharpOffsetMesh(mp.mrMeshPart, offset, ref parameters2, ref generalParams2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new Exception(Marshal.PtrToStringAnsi(errorString));
		}
		return new Mesh(mesh);
	}

	public static Mesh GeneralOffsetMesh(MeshPart mp, float offset, OffsetParameters parameters, GeneralOffsetParameters generalParams)
	{
		IntPtr errorString = IntPtr.Zero;
		MROffsetParameters parameters2 = new MROffsetParameters();
		parameters2.callBack = IntPtr.Zero;
		parameters2.memoryEfficient = (parameters.memoryEfficient ? ((byte)1) : ((byte)0));
		parameters2.signDetectionMode = parameters.signDetectionMode;
		parameters2.voxelSize = parameters.voxelSize;
		MRGeneralOffsetParameters generalParams2 = new MRGeneralOffsetParameters();
		generalParams2.maxNewRank2VertDev = generalParams.maxNewRank2VertDev;
		generalParams2.maxNewRank3VertDev = generalParams.maxNewRank3VertDev;
		generalParams2.maxOldVertPosCorrection = generalParams.maxOldVertPosCorrection;
		generalParams2.minNewVertDev = generalParams.minNewVertDev;
		generalParams2.mode = generalParams.mode;
		IntPtr mesh = mrGeneralOffsetMesh(mp.mrMeshPart, offset, ref parameters2, ref generalParams2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new Exception(Marshal.PtrToStringAnsi(errorString));
		}
		return new Mesh(mesh);
	}

	public static Mesh ThickenMesh(Mesh mesh, float offset, OffsetParameters parameters, GeneralOffsetParameters generalParams)
	{
		IntPtr errorString = IntPtr.Zero;
		MROffsetParameters parameters2 = new MROffsetParameters();
		parameters2.callBack = IntPtr.Zero;
		parameters2.memoryEfficient = (parameters.memoryEfficient ? ((byte)1) : ((byte)0));
		parameters2.signDetectionMode = parameters.signDetectionMode;
		parameters2.voxelSize = parameters.voxelSize;
		MRGeneralOffsetParameters generalParams2 = new MRGeneralOffsetParameters();
		generalParams2.maxNewRank2VertDev = generalParams.maxNewRank2VertDev;
		generalParams2.maxNewRank3VertDev = generalParams.maxNewRank3VertDev;
		generalParams2.maxOldVertPosCorrection = generalParams.maxOldVertPosCorrection;
		generalParams2.minNewVertDev = generalParams.minNewVertDev;
		generalParams2.mode = generalParams.mode;
		IntPtr mesh2 = mrThickenMesh(mesh.mesh_, offset, ref parameters2, ref generalParams2, ref errorString);
		if (errorString != IntPtr.Zero)
		{
			throw new Exception(Marshal.PtrToStringAnsi(errorString));
		}
		return new Mesh(mesh2);
	}
}
