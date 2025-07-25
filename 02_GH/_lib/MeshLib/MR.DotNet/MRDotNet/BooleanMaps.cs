using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class BooleanMaps
{
	private IntPtr maps_;

	private List<FaceId>? cut2origin_;

	private List<FaceId>? cut2newFaces_;

	private List<VertId>? old2newVerts_;

	public ReadOnlyCollection<FaceId> Cut2Origin
	{
		get
		{
			if (cut2origin_ == null)
			{
				MRFaceMap mRFaceMap = mrBooleanResultMapperMapsCut2origin(maps_);
				cut2origin_ = new List<FaceId>((int)mRFaceMap.size);
				int num = Marshal.SizeOf(typeof(FaceId));
				for (int i = 0; i < (int)mRFaceMap.size; i++)
				{
					FaceId faceId = Marshal.PtrToStructure<FaceId>(IntPtr.Add(mRFaceMap.data, i * num));
					cut2origin_.Add(new FaceId(faceId.Id));
				}
			}
			return cut2origin_.AsReadOnly();
		}
	}

	public ReadOnlyCollection<FaceId> Cut2NewFaces
	{
		get
		{
			if (cut2newFaces_ == null)
			{
				MRFaceMap mRFaceMap = mrBooleanResultMapperMapsCut2newFaces(maps_);
				cut2newFaces_ = new List<FaceId>((int)mRFaceMap.size);
				int num = Marshal.SizeOf(typeof(FaceId));
				for (int i = 0; i < (int)mRFaceMap.size; i++)
				{
					FaceId faceId = Marshal.PtrToStructure<FaceId>(IntPtr.Add(mRFaceMap.data, i * num));
					cut2newFaces_.Add(new FaceId(faceId.Id));
				}
			}
			return cut2newFaces_.AsReadOnly();
		}
	}

	public ReadOnlyCollection<VertId> Old2NewVerts
	{
		get
		{
			if (old2newVerts_ == null)
			{
				MRVertMap mRVertMap = mrBooleanResultMapperMapsOld2NewVerts(maps_);
				old2newVerts_ = new List<VertId>((int)mRVertMap.size);
				int num = Marshal.SizeOf(typeof(VertId));
				for (int i = 0; i < (int)mRVertMap.size; i++)
				{
					VertId vertId = Marshal.PtrToStructure<VertId>(IntPtr.Add(mRVertMap.data, i * num));
					old2newVerts_.Add(new VertId(vertId.Id));
				}
			}
			return old2newVerts_.AsReadOnly();
		}
	}

	public bool Identity => mrBooleanResultMapperMapsIdentity(maps_);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRFaceMap mrBooleanResultMapperMapsCut2origin(IntPtr maps);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRFaceMap mrBooleanResultMapperMapsCut2newFaces(IntPtr maps);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern MRVertMap mrBooleanResultMapperMapsOld2NewVerts(IntPtr maps);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Ansi)]
	private static extern bool mrBooleanResultMapperMapsIdentity(IntPtr maps);

	internal BooleanMaps(IntPtr maps)
	{
		maps_ = maps;
	}
}
