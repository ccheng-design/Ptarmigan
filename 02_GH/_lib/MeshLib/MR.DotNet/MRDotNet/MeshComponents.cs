using System;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class MeshComponents
{
	internal struct MRFace2RegionMap
	{
		public IntPtr data;

		public ulong size;

		public IntPtr reserved;

		public MRFace2RegionMap()
		{
			data = IntPtr.Zero;
			size = 0uL;
			reserved = IntPtr.Zero;
		}
	}

	internal struct MRMeshComponentsMap
	{
		public unsafe MRFace2RegionMap* faceMap;

		public int numComponents;

		public unsafe MRMeshComponentsMap()
		{
			faceMap = null;
			numComponents = 0;
		}
	}

	internal struct MRMeshRegions
	{
		public IntPtr faces;

		public int numRegions;

		public MRMeshRegions()
		{
			faces = IntPtr.Zero;
			numRegions = 0;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrMeshComponentsGetComponent(ref MRMeshPart mp, FaceId id, FaceIncidence incidence, IntPtr cb);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern IntPtr mrMeshComponentsGetLargestComponent(ref MRMeshPart mp, FaceIncidence incidence, IntPtr cb, float minArea, int* numSmallerComponents);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrMeshComponentsGetLargeByAreaComponents(ref MRMeshPart mp, float minArea, IntPtr cb);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRMeshComponentsMap mrMeshComponentsGetAllComponentsMap(ref MRMeshPart mp, FaceIncidence incidence);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private unsafe static extern MRMeshRegions mrMeshComponentsGetLargeByAreaRegions(ref MRMeshPart mp, MRFace2RegionMap* face2RegionMap, int numRegions, float minArea);

	public static MeshComponentsMap GetAllComponentsMap(MeshPart mp, FaceIncidence incidence)
	{
		return new MeshComponentsMap(mrMeshComponentsGetAllComponentsMap(ref mp.mrMeshPart, incidence));
	}

	public unsafe static MeshRegions GetLargeByAreaRegions(MeshPart mp, MeshComponentsMap map, int numRegions, float minArea)
	{
		MRMeshRegions mRMeshRegions = mrMeshComponentsGetLargeByAreaRegions(ref mp.mrMeshPart, map.mrMap_.faceMap, numRegions, minArea);
		return new MeshRegions
		{
			faces = new BitSet(mRMeshRegions.faces),
			numRegions = mRMeshRegions.numRegions
		};
	}

	public static BitSet GetLargeByAreaComponents(MeshPart mp, float minArea)
	{
		return new BitSet(mrMeshComponentsGetLargeByAreaComponents(ref mp.mrMeshPart, minArea, IntPtr.Zero));
	}

	public unsafe static BitSet GetLargestComponent(MeshPart mp, FaceIncidence incidence, float minArea, out int numSmallerComponents)
	{
		fixed (int* numSmallerComponents2 = &numSmallerComponents)
		{
			return new BitSet(mrMeshComponentsGetLargestComponent(ref mp.mrMeshPart, incidence, IntPtr.Zero, minArea, numSmallerComponents2));
		}
	}

	public static BitSet GetComponent(MeshPart mp, FaceId id, FaceIncidence incidence)
	{
		return new BitSet(mrMeshComponentsGetComponent(ref mp.mrMeshPart, id, incidence, IntPtr.Zero));
	}
}
