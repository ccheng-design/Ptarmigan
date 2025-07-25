using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;

namespace MR.DotNet;

public class ContinousContours : IDisposable
{
	internal struct MRContinuousContour
	{
		public IntPtr data;

		public ulong size;

		public IntPtr reserved;

		public MRContinuousContour()
		{
			data = IntPtr.Zero;
			size = 0uL;
			reserved = IntPtr.Zero;
		}
	}

	private bool disposed;

	private List<List<VariableEdgeTri>>? contours_;

	internal IntPtr mrContours_;

	public ReadOnlyCollection<List<VariableEdgeTri>> Contours
	{
		get
		{
			if (contours_ == null)
			{
				int num = (int)mrContinuousContoursSize(mrContours_);
				contours_ = new List<List<VariableEdgeTri>>();
				for (int i = 0; i < num; i++)
				{
					MRContinuousContour mRContinuousContour = mrContinuousContoursGet(mrContours_, (ulong)i);
					List<VariableEdgeTri> list = new List<VariableEdgeTri>();
					int num2 = Marshal.SizeOf(typeof(VariableEdgeTri));
					for (int j = 0; j < (int)mRContinuousContour.size; j++)
					{
						VariableEdgeTri item = Marshal.PtrToStructure<VariableEdgeTri>(IntPtr.Add(mRContinuousContour.data, j * num2));
						list.Add(item);
					}
					contours_.Add(list);
				}
			}
			return contours_.AsReadOnly();
		}
	}

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern MRContinuousContour mrContinuousContoursGet(IntPtr contours, ulong index);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern ulong mrContinuousContoursSize(IntPtr contours);

	[DllImport("MRMeshC.dll", CharSet = CharSet.Auto)]
	private static extern void mrContinuousContoursFree(IntPtr contours);

	internal ContinousContours(IntPtr mrContours)
	{
		mrContours_ = mrContours;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
	{
		if (!disposed)
		{
			if (mrContours_ != IntPtr.Zero)
			{
				mrContinuousContoursFree(mrContours_);
			}
			disposed = true;
		}
	}

	~ContinousContours()
	{
		Dispose(disposing: false);
	}
}
