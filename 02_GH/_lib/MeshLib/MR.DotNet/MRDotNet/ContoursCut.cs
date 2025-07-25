using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class ContoursCut
{
	internal struct MROneMeshIntersection
	{
		public int primitiveId;

		public byte primitiveIdIndex;

		public Vector3f.MRVector3f coordinate;

		public MROneMeshIntersection()
		{
			coordinate = default(Vector3f.MRVector3f);
			primitiveId = 0;
			primitiveIdIndex = 0;
		}
	}

	internal struct MRVectorOneMeshIntersection
	{
		public IntPtr data;

		public ulong size;

		public IntPtr reserved;

		public MRVectorOneMeshIntersection()
		{
			data = IntPtr.Zero;
			size = 0uL;
			reserved = IntPtr.Zero;
		}
	}

	internal struct MROneMeshContour
	{
		public MRVectorOneMeshIntersection intersections;

		public byte closed;

		public MROneMeshContour()
		{
			intersections = default(MRVectorOneMeshIntersection);
			closed = 0;
		}
	}

	internal struct MRVariableEdgeTri
	{
		public EdgeId edge;

		public FaceId tri;

		public bool isEdgeATriB;

		public MRVariableEdgeTri()
		{
			edge = default(EdgeId);
			tri = default(FaceId);
			isEdgeATriB = false;
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MROneMeshContour mrOneMeshContoursGet(IntPtr contours, ulong index);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrOneMeshContoursSize(IntPtr contours);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrOneMeshContoursFree(IntPtr contours);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern IntPtr mrGetOneMeshIntersectionContours(IntPtr meshA, IntPtr meshB, IntPtr continousContours, bool getMeshAIntersections, ref CoordinateConverters.MRCoordinateConverters converters, IntPtr rigidB2A);

	public static List<OneMeshContour> GetOneMeshIntersectionContours(Mesh meshA, Mesh meshB, ContinousContours contours, bool getMeshAIntersections, CoordinateConverters converters, AffineXf3f? rigidB2A = null)
	{
		IntPtr contours2 = mrGetOneMeshIntersectionContours(meshA.mesh_, meshB.mesh_, contours.mrContours_, getMeshAIntersections, ref converters.conv_, rigidB2A?.XfAddr() ?? IntPtr.Zero);
		int num = (int)mrOneMeshContoursSize(contours2);
		List<OneMeshContour> list = new List<OneMeshContour>(num);
		for (int i = 0; i < num; i++)
		{
			MROneMeshContour mROneMeshContour = mrOneMeshContoursGet(contours2, (ulong)i);
			OneMeshContour item = new OneMeshContour
			{
				intersections = new List<OneMeshIntersection>((int)mROneMeshContour.intersections.size),
				closed = (mROneMeshContour.closed > 0)
			};
			for (int j = 0; j < (int)mROneMeshContour.intersections.size; j++)
			{
				IntPtr data = mROneMeshContour.intersections.data;
				int num2 = Marshal.SizeOf(typeof(MROneMeshIntersection));
				MROneMeshIntersection mROneMeshIntersection = (MROneMeshIntersection)Marshal.PtrToStructure(IntPtr.Add(data, j * num2), typeof(MROneMeshIntersection));
				OneMeshIntersection oneMeshIntersection = new OneMeshIntersection
				{
					variantIndex = (VariantIndex)mROneMeshIntersection.primitiveIdIndex,
					index = mROneMeshIntersection.primitiveId,
					coordinate = new Vector3f(mROneMeshIntersection.coordinate)
				};
				item.intersections.Add(new OneMeshIntersection
				{
					variantIndex = (VariantIndex)mROneMeshIntersection.primitiveIdIndex,
					index = mROneMeshIntersection.primitiveId,
					coordinate = new Vector3f(mROneMeshIntersection.coordinate)
				});
			}
			list.Add(item);
		}
		return list;
	}
}
