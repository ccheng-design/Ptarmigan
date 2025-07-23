using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino;
using Rhino.Collections;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace CurveOffsetTools;

public class PolylineOffsetComponent : GH_Component
{
	private int _idx_inp_polyline;

	private int _idx_inp_distance;

	private int _idx_inp_plane;

	private int _idx_inp_absTol;

	private int _idx_inp_angleTol;

	private int _idx_inp_keepDuplicates;

	private int _idx_inp_splitAtSelfIntersections;

	private int _idx_outp_polyline;

	private int _idx_outp_polyline_reverse;

	private bool _angleTolUseDegrees;

	protected override Bitmap Icon => null;

	public override Guid ComponentGuid => new Guid("BCF55218-796B-421B-BCC7-5634C6C5B4E1");

	public PolylineOffsetComponent()
		: base("PolylineOffset", "PolylineOffset", "Offset planar polylines", "Curve", "Util")
	{
	}

	protected override void RegisterInputParams(GH_InputParamManager pManager)
	{
		_idx_inp_polyline = pManager.AddCurveParameter("Polyline", "C", "Planar polyline to offset", (GH_ParamAccess)0);
		_idx_inp_distance = pManager.AddNumberParameter("Distance", "D", "Offset distance(s)", (GH_ParamAccess)1);
		_idx_inp_plane = pManager.AddPlaneParameter("Plane", "P", "Curve plane to use, optional", (GH_ParamAccess)0);
		_idx_inp_absTol = pManager.AddNumberParameter("Absolute tolerance", "T", "Absolute tolerance to use, optional", (GH_ParamAccess)0, 0.0);
		_idx_inp_angleTol = pManager.AddAngleParameter("Angle tolerance", "A", "Angle tolerance to use, optional", (GH_ParamAccess)0, 0.0);
		_idx_inp_keepDuplicates = pManager.AddBooleanParameter("Keep duplicates", "K", "Keep duplicate points", (GH_ParamAccess)0, false);
		_idx_inp_splitAtSelfIntersections = pManager.AddBooleanParameter("Avoid self intersections", "S", "Avoid self intersections by splitting the offsetted polyline into pieces", (GH_ParamAccess)0, false);
		pManager[_idx_inp_plane].Optional = true;
		pManager[_idx_inp_absTol].Optional = true;
		pManager[_idx_inp_angleTol].Optional = true;
	}

	protected override void RegisterOutputParams(GH_OutputParamManager pManager)
	{
		_idx_outp_polyline = pManager.AddCurveParameter("Offsetted polyline", "O", "Offsetted polyline(s)", (GH_ParamAccess)1);
		_idx_outp_polyline_reverse = pManager.AddCurveParameter("Offsetted polyline reverse", "O", "Offsetted polyline(s) which switched direction due to offset", (GH_ParamAccess)1);
	}

	protected override void BeforeSolveInstance()
	{
		ref bool angleTolUseDegrees = ref _angleTolUseDegrees;
		IGH_Param obj = ((GH_Component)this).Params.Input[_idx_inp_angleTol];
		angleTolUseDegrees = ((Param_Number)((obj is Param_Number) ? obj : null)).UseDegrees;
	}

	protected override void SolveInstance(IGH_DataAccess DA)
	{
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011e: Unknown result type (might be due to invalid IL or missing references)
		//IL_01c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0228: Unknown result type (might be due to invalid IL or missing references)
		//IL_0231: Unknown result type (might be due to invalid IL or missing references)
		//IL_0236: Unknown result type (might be due to invalid IL or missing references)
		//IL_023b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0248: Unknown result type (might be due to invalid IL or missing references)
		//IL_0251: Unknown result type (might be due to invalid IL or missing references)
		//IL_0256: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_02fb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0303: Unknown result type (might be due to invalid IL or missing references)
		//IL_0308: Unknown result type (might be due to invalid IL or missing references)
		//IL_031a: Unknown result type (might be due to invalid IL or missing references)
		//IL_031f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0323: Unknown result type (might be due to invalid IL or missing references)
		//IL_0325: Unknown result type (might be due to invalid IL or missing references)
		//IL_032e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0330: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0344: Unknown result type (might be due to invalid IL or missing references)
		//IL_0384: Unknown result type (might be due to invalid IL or missing references)
		//IL_0419: Unknown result type (might be due to invalid IL or missing references)
		//IL_041e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0429: Unknown result type (might be due to invalid IL or missing references)
		//IL_042e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0433: Unknown result type (might be due to invalid IL or missing references)
		//IL_043a: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0475: Unknown result type (might be due to invalid IL or missing references)
		//IL_0480: Unknown result type (might be due to invalid IL or missing references)
		//IL_0485: Unknown result type (might be due to invalid IL or missing references)
		//IL_04bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_04c7: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d9: Unknown result type (might be due to invalid IL or missing references)
		//IL_04db: Unknown result type (might be due to invalid IL or missing references)
		//IL_051d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0526: Unknown result type (might be due to invalid IL or missing references)
		//IL_0536: Unknown result type (might be due to invalid IL or missing references)
		//IL_053d: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0556: Unknown result type (might be due to invalid IL or missing references)
		//IL_0580: Unknown result type (might be due to invalid IL or missing references)
		//IL_0589: Unknown result type (might be due to invalid IL or missing references)
		//IL_0599: Unknown result type (might be due to invalid IL or missing references)
		//IL_05a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_05ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_05b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_0666: Unknown result type (might be due to invalid IL or missing references)
		//IL_066b: Unknown result type (might be due to invalid IL or missing references)
		//IL_066f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0674: Unknown result type (might be due to invalid IL or missing references)
		//IL_0682: Unknown result type (might be due to invalid IL or missing references)
		//IL_0687: Unknown result type (might be due to invalid IL or missing references)
		//IL_068b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0690: Unknown result type (might be due to invalid IL or missing references)
		//IL_0692: Unknown result type (might be due to invalid IL or missing references)
		//IL_0697: Unknown result type (might be due to invalid IL or missing references)
		//IL_077f: Unknown result type (might be due to invalid IL or missing references)
		//IL_07ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_07bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_07c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_088e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0895: Expected O, but got Unknown
		//IL_081c: Unknown result type (might be due to invalid IL or missing references)
		//IL_08ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_08c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_08da: Unknown result type (might be due to invalid IL or missing references)
		//IL_08e1: Unknown result type (might be due to invalid IL or missing references)
		//IL_090c: Unknown result type (might be due to invalid IL or missing references)
		DA.SetData(_idx_outp_polyline, (object)null);
		DA.SetData(_idx_outp_polyline_reverse, (object)null);
		double num = 0.0;
		double num2 = 0.0;
		if (!DA.GetData<double>(_idx_inp_absTol, ref num) || num == 0.0)
		{
			num = RhinoDoc.ActiveDoc.ModelAbsoluteTolerance;
		}
		if (!DA.GetData<double>(_idx_inp_angleTol, ref num2) || num2 == 0.0)
		{
			num2 = RhinoDoc.ActiveDoc.ModelAngleToleranceRadians;
		}
		if (_angleTolUseDegrees)
		{
			num2 *= Math.PI / 180.0;
		}
		bool flag = false;
		if (!DA.GetData<bool>(_idx_inp_keepDuplicates, ref flag))
		{
			flag = false;
		}
		bool flag2 = false;
		if (!DA.GetData<bool>(_idx_inp_splitAtSelfIntersections, ref flag2))
		{
			flag2 = false;
		}
		Curve val = null;
		if (!DA.GetData<Curve>(_idx_inp_polyline, ref val))
		{
			return;
		}
		if (!flag)
		{
			Curve val2 = val.Simplify((CurveSimplifyOptions)63, num, num2);
			if (val2 != null)
			{
				val = val2;
			}
		}
		if (!val.IsPolyline())
		{
			((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Input curve is not a polyline.");
			return;
		}
		Polyline val3 = null;
		if (!val.TryGetPolyline(ref val3))
		{
			((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Input curve is not a polyline.");
			return;
		}
		Plane worldXY = Plane.WorldXY;
		if (!val.TryGetPlane(ref worldXY))
		{
			((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Input curve is not planar.");
			return;
		}
		List<double> list = new List<double>();
		if (!DA.GetDataList<double>(_idx_inp_distance, list))
		{
			return;
		}
		if (list.Count < 1)
		{
			((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "No offset distance(s) specified.");
			return;
		}
		if (list.Count > ((RhinoList<Point3d>)(object)val3).Count)
		{
			list = list.GetRange(0, ((RhinoList<Point3d>)(object)val3).Count);
		}
		else if (list.Count < ((RhinoList<Point3d>)(object)val3).Count)
		{
			list.AddRange(Enumerable.Repeat(list[list.Count - 1], ((RhinoList<Point3d>)(object)val3).Count - list.Count));
		}
		Plane offsetplane = Plane.WorldXY;
		if (!DA.GetData<Plane>(_idx_inp_plane, ref offsetplane))
		{
			offsetplane = worldXY;
		}
		List<int> list2 = new List<int>();
		Polyline val4 = null;
		Vector3d val6 = default(Vector3d);
		Line val11 = default(Line);
		Line val12 = default(Line);
		double num6 = default(double);
		while (true)
		{
			val4 = PolylineOffsetGeneralized(val3, offsetplane, list, num, list2);
			if (val4 == null)
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "PolylineOffsetGeneralized failed.");
				return;
			}
			int num3 = -1;
			double num4 = 0.0;
			for (int i = 0; i < ((RhinoList<Point3d>)(object)val3).Count - 1; i++)
			{
				Vector3d val5 = new Vector3d(((RhinoList<Point3d>)(object)val3)[i + 1] - ((RhinoList<Point3d>)(object)val3)[i]);
				((Vector3d)(ref val6))._002Ector(((RhinoList<Point3d>)(object)val4)[i + 1] - ((RhinoList<Point3d>)(object)val4)[i]);
				if (Vector3d.Multiply(val5, val6) < 0.0)
				{
					double length = ((Vector3d)(ref val6)).Length;
					if (num4 == 0.0 || length > num4)
					{
						num4 = length;
						num3 = i;
					}
				}
			}
			if (num3 == -1 || ((num3 == 0 || num3 == ((RhinoList<Point3d>)(object)val3).Count - 2) && !val3.IsClosed))
			{
				break;
			}
			Point3d val7 = ((RhinoList<Point3d>)(object)val3)[(num3 - 1 + (((RhinoList<Point3d>)(object)val3).Count - 1)) % (((RhinoList<Point3d>)(object)val3).Count - 1)];
			Point3d val8 = ((RhinoList<Point3d>)(object)val3)[num3];
			Point3d val9 = ((RhinoList<Point3d>)(object)val3)[num3 + 1];
			Point3d val10 = ((RhinoList<Point3d>)(object)val3)[(num3 + 2) % (((RhinoList<Point3d>)(object)val3).Count - 1)];
			((Line)(ref val11))._002Ector(val7, val8);
			((Line)(ref val12))._002Ector(val9, val10);
			double num5 = 0.0;
			if (!Intersection.LineLine(val11, val12, ref num6, ref num5, num, false))
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Line-line intersection failed.");
				return;
			}
			((RhinoList<Point3d>)(object)val3).RemoveAt(num3);
			list.RemoveAt(num3);
			list2.Add(num3);
			((RhinoList<Point3d>)(object)val3)[num3] = ((Line)(ref val11)).PointAt(num6);
			if (num3 == ((RhinoList<Point3d>)(object)val3).Count - 1)
			{
				((RhinoList<Point3d>)(object)val3)[0] = ((RhinoList<Point3d>)(object)val3)[((RhinoList<Point3d>)(object)val3).Count - 1];
			}
			else if (num3 == 0)
			{
				((RhinoList<Point3d>)(object)val3)[((RhinoList<Point3d>)(object)val3).Count - 1] = ((RhinoList<Point3d>)(object)val3)[0];
			}
		}
		if (!val3.IsClosed || !flag2)
		{
			PolylineRestoreDuplicates(val4, list2);
			DA.SetData(_idx_outp_polyline, (object)val4);
			return;
		}
		double num7 = 0.0;
		for (int j = 0; j < ((RhinoList<Point3d>)(object)val3).Count - 1; j++)
		{
			num7 += Vector3d.Multiply(Vector3d.CrossProduct(new Vector3d(((RhinoList<Point3d>)(object)val3)[j]), new Vector3d(((RhinoList<Point3d>)(object)val3)[j + 1])), ((Plane)(ref offsetplane)).Normal);
		}
		List<Line> list3 = new List<Line>(((RhinoList<Point3d>)(object)val3).Count - 1);
		for (int k = 0; k < ((RhinoList<Point3d>)(object)val3).Count - 1; k++)
		{
			list3.Add(new Line(((RhinoList<Point3d>)(object)val4)[k], ((RhinoList<Point3d>)(object)val4)[k + 1]));
		}
		bool flag3 = false;
		double num9 = default(double);
		Line item = default(Line);
		Line value = default(Line);
		Line item2 = default(Line);
		Line value2 = default(Line);
		for (int l = 0; l < list3.Count - 1; l++)
		{
			for (int m = l + 2; m < list3.Count; m++)
			{
				Line val13 = list3[l];
				Line val14 = list3[m];
				double num8 = 0.0;
				if (Intersection.LineLine(val13, val14, ref num9, ref num8, num, true))
				{
					double num10 = num / ((Line)(ref val13)).Length;
					double num11 = num / ((Line)(ref val14)).Length;
					if (num8 > num11 && num8 < 1.0 - num11)
					{
						((Line)(ref item))._002Ector(((Line)(ref val14)).From, ((Line)(ref val14)).PointAt(num8));
						((Line)(ref value))._002Ector(((Line)(ref val14)).PointAt(num8), ((Line)(ref val14)).To);
						list3[m] = value;
						list3.Insert(m, item);
						m++;
						flag3 = true;
					}
					if (num9 > num10 && num9 < 1.0 - num10)
					{
						((Line)(ref item2))._002Ector(((Line)(ref val13)).From, ((Line)(ref val13)).PointAt(num9));
						((Line)(ref value2))._002Ector(((Line)(ref val13)).PointAt(num9), ((Line)(ref val13)).To);
						list3[l] = value2;
						list3.Insert(l, item2);
						m++;
						flag3 = true;
					}
				}
			}
		}
		if (!flag3)
		{
			DA.SetData(_idx_outp_polyline, (object)val4);
			return;
		}
		List<int> list4 = new List<int>(list3.Count);
		for (int n = 0; n < list3.Count; n++)
		{
			list4.Add(-1);
		}
		int num12 = 0;
		Line val15;
		for (int num13 = 0; num13 < list3.Count; num13++)
		{
			int num14 = list4[num13];
			if (num14 < 0)
			{
				num14 = (list4[num13] = num12++);
			}
			val15 = list3[num13];
			Point3d val16 = ((Line)(ref val15)).From;
			for (int num16 = num13 + 2; num16 < list3.Count; num16++)
			{
				val15 = list3[num16];
				Vector3d val17 = ((Line)(ref val15)).From - val16;
				if (((Vector3d)(ref val17)).Length <= num)
				{
					list4[num16] = num14;
				}
			}
		}
		List<List<int>> list5 = new List<List<int>>();
		for (int num17 = 0; num17 < list4.Count; num17++)
		{
			int num18 = list4[num17];
			if (num18 >= list5.Count)
			{
				list5.Add(new List<int> { num17 });
			}
			else
			{
				list5[num18].Add(num17);
			}
		}
		List<bool> list6 = new List<bool>(list3.Count);
		for (int num19 = 0; num19 < list3.Count; num19++)
		{
			list6.Add(item: false);
		}
		List<List<Line>> list7 = new List<List<Line>>();
		for (int num20 = 0; num20 < list3.Count - 1; num20++)
		{
			if (list6[num20])
			{
				continue;
			}
			List<Line> list8 = new List<Line> { list3[num20] };
			list6[num20] = true;
			int num21 = num20 + 1;
			do
			{
				int index = list4[num21];
				List<int> list9 = list5[index];
				val15 = list8[list8.Count - 1];
				Vector3d direction = ((Line)(ref val15)).Direction;
				((Vector3d)(ref direction)).Reverse();
				int num22 = num21;
				foreach (int item3 in list9)
				{
					if (!list6[item3] && item3 > num22)
					{
						num22 = item3;
					}
				}
				list8.Add(list3[num22]);
				list6[num22] = true;
				num21 = (num22 + 1) % list3.Count;
			}
			while (!list6[num21]);
			list7.Add(list8);
		}
		List<Polyline> list10 = new List<Polyline>();
		List<Polyline> list11 = new List<Polyline>();
		foreach (List<Line> item4 in list7)
		{
			Polyline val18 = new Polyline();
			double num23 = 0.0;
			foreach (Line item5 in item4)
			{
				Line current3 = item5;
				((RhinoList<Point3d>)(object)val18).Add(((Line)(ref current3)).From);
				num23 += Vector3d.Multiply(Vector3d.CrossProduct(new Vector3d(((Line)(ref current3)).From), new Vector3d(((Line)(ref current3)).To)), ((Plane)(ref offsetplane)).Normal);
			}
			((RhinoList<Point3d>)(object)val18).Add(((RhinoList<Point3d>)(object)val18)[0]);
			if (Math.Sign(num7) == Math.Sign(num23))
			{
				list10.Add(val18);
			}
			else
			{
				list11.Add(val18);
			}
		}
		if (list10.Count > 0)
		{
			DA.SetDataList(_idx_outp_polyline, (IEnumerable)list10);
		}
		if (list11.Count > 0)
		{
			DA.SetDataList(_idx_outp_polyline_reverse, (IEnumerable)list11);
		}
	}

	private bool PolylineRestoreDuplicates(Polyline pinput, List<int> removedPointIndices)
	{
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		//IL_001d: Unknown result type (might be due to invalid IL or missing references)
		for (int num = removedPointIndices.Count - 1; num >= 0; num--)
		{
			int num2 = removedPointIndices[num];
			Point3d val = ((RhinoList<Point3d>)(object)pinput)[num2];
			((RhinoList<Point3d>)(object)pinput).Insert(num2, val);
		}
		return true;
	}

	private Polyline PolylineOffsetGeneralized(Polyline pinput, Plane offsetplane, List<double> arrDist, double absTol, List<int> removedPointIndices)
	{
		//IL_0019: Unknown result type (might be due to invalid IL or missing references)
		//IL_001f: Expected O, but got Unknown
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0033: Unknown result type (might be due to invalid IL or missing references)
		//IL_0038: Unknown result type (might be due to invalid IL or missing references)
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_006d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0072: Unknown result type (might be due to invalid IL or missing references)
		//IL_0073: Unknown result type (might be due to invalid IL or missing references)
		//IL_0078: Unknown result type (might be due to invalid IL or missing references)
		//IL_005f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0094: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0334: Unknown result type (might be due to invalid IL or missing references)
		//IL_033d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0342: Unknown result type (might be due to invalid IL or missing references)
		//IL_0347: Unknown result type (might be due to invalid IL or missing references)
		//IL_00fc: Unknown result type (might be due to invalid IL or missing references)
		//IL_0101: Unknown result type (might be due to invalid IL or missing references)
		//IL_0106: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_010d: Unknown result type (might be due to invalid IL or missing references)
		//IL_010f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0111: Unknown result type (might be due to invalid IL or missing references)
		//IL_0116: Unknown result type (might be due to invalid IL or missing references)
		//IL_037e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0383: Unknown result type (might be due to invalid IL or missing references)
		//IL_0385: Unknown result type (might be due to invalid IL or missing references)
		//IL_038a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0370: Unknown result type (might be due to invalid IL or missing references)
		//IL_0148: Unknown result type (might be due to invalid IL or missing references)
		//IL_014a: Unknown result type (might be due to invalid IL or missing references)
		//IL_014c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0151: Unknown result type (might be due to invalid IL or missing references)
		//IL_013c: Unknown result type (might be due to invalid IL or missing references)
		//IL_03a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03b8: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_018c: Unknown result type (might be due to invalid IL or missing references)
		//IL_018e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0195: Unknown result type (might be due to invalid IL or missing references)
		//IL_019a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0177: Unknown result type (might be due to invalid IL or missing references)
		//IL_0272: Unknown result type (might be due to invalid IL or missing references)
		//IL_0277: Unknown result type (might be due to invalid IL or missing references)
		//IL_0279: Unknown result type (might be due to invalid IL or missing references)
		//IL_027e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_0288: Unknown result type (might be due to invalid IL or missing references)
		//IL_028d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0292: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_0298: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ae: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_030f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_021b: Unknown result type (might be due to invalid IL or missing references)
		//IL_021d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0222: Unknown result type (might be due to invalid IL or missing references)
		//IL_023e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0240: Unknown result type (might be due to invalid IL or missing references)
		//IL_0253: Unknown result type (might be due to invalid IL or missing references)
		//IL_025a: Unknown result type (might be due to invalid IL or missing references)
		//IL_025c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0261: Unknown result type (might be due to invalid IL or missing references)
		if (pinput.Length < 2.0)
		{
			return null;
		}
		Polyline val = new Polyline(((RhinoList<Point3d>)(object)pinput).Count);
		if (!pinput.IsClosed)
		{
			Vector3d val2 = ((RhinoList<Point3d>)(object)pinput)[1] - ((RhinoList<Point3d>)(object)pinput)[0];
			if (!((Vector3d)(ref val2)).Unitize())
			{
				((RhinoList<Point3d>)(object)pinput).RemoveAt(0);
				arrDist.RemoveAt(0);
				removedPointIndices.Add(0);
				return PolylineOffsetGeneralized(pinput, offsetplane, arrDist, absTol, removedPointIndices);
			}
			val2 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val2);
			if (!((Vector3d)(ref val2)).Unitize())
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Specified curve plane is invalid.");
				return null;
			}
			((RhinoList<Point3d>)(object)val).Add(((RhinoList<Point3d>)(object)pinput)[0] + GetDistanceForSegment(arrDist, 0) * val2);
		}
		int num = ((RhinoList<Point3d>)(object)pinput).Count - 1;
		int num2 = 0;
		if (!pinput.IsClosed)
		{
			num2 = 1;
		}
		Line val11 = default(Line);
		Line val13 = default(Line);
		double num9 = default(double);
		for (int i = num2; i < num; i++)
		{
			int num3 = i + 1;
			int num4;
			Point3d val3;
			if (i == 0)
			{
				num4 = num - 1;
				val3 = ((RhinoList<Point3d>)(object)pinput)[num4];
			}
			else
			{
				num4 = i - 1;
				val3 = ((RhinoList<Point3d>)(object)pinput)[num4];
			}
			Point3d val4 = ((RhinoList<Point3d>)(object)pinput)[i];
			Point3d val5 = ((RhinoList<Point3d>)(object)pinput)[num3];
			Vector3d val6 = val4 - val3;
			if (!((Vector3d)(ref val6)).Unitize())
			{
				((RhinoList<Point3d>)(object)pinput).RemoveAt(num4);
				arrDist.RemoveAt(num4);
				removedPointIndices.Add(num4);
				return PolylineOffsetGeneralized(pinput, offsetplane, arrDist, absTol, removedPointIndices);
			}
			Vector3d val7 = val5 - val4;
			if (!((Vector3d)(ref val7)).Unitize())
			{
				((RhinoList<Point3d>)(object)pinput).RemoveAt(i);
				arrDist.RemoveAt(i);
				removedPointIndices.Add(i);
				return PolylineOffsetGeneralized(pinput, offsetplane, arrDist, absTol, removedPointIndices);
			}
			Vector3d val8 = 0.5 * (val6 + val7);
			if (!((Vector3d)(ref val8)).Unitize())
			{
				((Vector3d)(ref val8))._002Ector(0.0, 0.0, 0.0);
			}
			double num5 = 0.0;
			double num6 = ((i != 0) ? GetDistanceForSegment(arrDist, i - 1) : GetDistanceForSegment(arrDist, num - 1));
			num5 = GetDistanceForSegment(arrDist, i);
			if (num6 == num5 && ((Vector3d)(ref val8)).Length > 0.0)
			{
				Vector3d val9 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val8);
				if (!((Vector3d)(ref val9)).Unitize())
				{
					((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Specified curve plane is invalid.");
					return null;
				}
				double num7 = num6 / Math.Abs(Vector3d.Multiply(val6, val8));
				((RhinoList<Point3d>)(object)val).Add(((RhinoList<Point3d>)(object)pinput)[i] + num7 * val9);
				continue;
			}
			Vector3d val10 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val6);
			((Line)(ref val11))._002Ector(val3 + num6 * val10, val4 + num6 * val10);
			Vector3d val12 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val7);
			((Line)(ref val13))._002Ector(val4 + num5 * val12, val5 + num5 * val12);
			double num8 = 0.0;
			if (!Intersection.LineLine(val11, val13, ref num9, ref num8, absTol, false))
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Line-line intersection failed.");
				return null;
			}
			((RhinoList<Point3d>)(object)val).Add(((Line)(ref val11)).PointAt(num9));
		}
		if (!pinput.IsClosed)
		{
			Vector3d val14 = ((RhinoList<Point3d>)(object)pinput)[num] - ((RhinoList<Point3d>)(object)pinput)[num - 1];
			if (!((Vector3d)(ref val14)).Unitize())
			{
				((RhinoList<Point3d>)(object)pinput).RemoveAt(num - 1);
				arrDist.RemoveAt(num - 1);
				removedPointIndices.Add(num - 1);
				return PolylineOffsetGeneralized(pinput, offsetplane, arrDist, absTol, removedPointIndices);
			}
			val14 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val14);
			if (!((Vector3d)(ref val14)).Unitize())
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Specified curve plane is invalid.");
				return null;
			}
			((RhinoList<Point3d>)(object)val).Add(((RhinoList<Point3d>)(object)pinput)[num] + GetDistanceForSegment(arrDist, num - 1) * val14);
		}
		else
		{
			((RhinoList<Point3d>)(object)val).Add(((RhinoList<Point3d>)(object)val)[0]);
		}
		return val;
	}

	private double GetDistanceForSegment(List<double> arrDist, int idx)
	{
		if (idx >= arrDist.Count)
		{
			idx = arrDist.Count - 1;
		}
		return arrDist[idx];
	}
}
