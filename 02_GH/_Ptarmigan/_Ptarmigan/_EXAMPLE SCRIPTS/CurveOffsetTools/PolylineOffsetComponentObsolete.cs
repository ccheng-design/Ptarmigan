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

[Obsolete("Use PolylineOffsetComponent instead.")]
public class PolylineOffsetComponentObsolete : GH_Component
{
	private int _idx_inp_polyline;

	private int _idx_inp_distance;

	private int _idx_inp_plane;

	private int _idx_inp_absTol;

	private int _idx_inp_angleTol;

	private int _idx_outp_polyline;

	private int _idx_outp_polyline_reverse;

	private bool _angleTolUseDegrees;

	public override GH_Exposure Exposure => (GH_Exposure)(-1);

	protected override Bitmap Icon => null;

	public override Guid ComponentGuid => new Guid("2bc639ed-3104-4d4d-81a8-8c60bae087fe");

	public PolylineOffsetComponentObsolete()
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
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_0193: Unknown result type (might be due to invalid IL or missing references)
		//IL_0198: Unknown result type (might be due to invalid IL or missing references)
		//IL_01aa: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ac: Unknown result type (might be due to invalid IL or missing references)
		//IL_01b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_01de: Unknown result type (might be due to invalid IL or missing references)
		//IL_01e7: Unknown result type (might be due to invalid IL or missing references)
		//IL_01ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_01f1: Unknown result type (might be due to invalid IL or missing references)
		//IL_01fe: Unknown result type (might be due to invalid IL or missing references)
		//IL_0207: Unknown result type (might be due to invalid IL or missing references)
		//IL_020c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0216: Unknown result type (might be due to invalid IL or missing references)
		//IL_029d: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cc: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d5: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_02e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02f6: Unknown result type (might be due to invalid IL or missing references)
		//IL_03cb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03db: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e0: Unknown result type (might be due to invalid IL or missing references)
		//IL_03e5: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ec: Unknown result type (might be due to invalid IL or missing references)
		//IL_032d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0352: Unknown result type (might be due to invalid IL or missing references)
		//IL_0427: Unknown result type (might be due to invalid IL or missing references)
		//IL_0432: Unknown result type (might be due to invalid IL or missing references)
		//IL_0437: Unknown result type (might be due to invalid IL or missing references)
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_046e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0473: Unknown result type (might be due to invalid IL or missing references)
		//IL_0479: Unknown result type (might be due to invalid IL or missing references)
		//IL_047e: Unknown result type (might be due to invalid IL or missing references)
		//IL_048b: Unknown result type (might be due to invalid IL or missing references)
		//IL_048d: Unknown result type (might be due to invalid IL or missing references)
		//IL_04cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_04d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04e8: Unknown result type (might be due to invalid IL or missing references)
		//IL_04ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_04fd: Unknown result type (might be due to invalid IL or missing references)
		//IL_0508: Unknown result type (might be due to invalid IL or missing references)
		//IL_0532: Unknown result type (might be due to invalid IL or missing references)
		//IL_053b: Unknown result type (might be due to invalid IL or missing references)
		//IL_054b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0552: Unknown result type (might be due to invalid IL or missing references)
		//IL_0560: Unknown result type (might be due to invalid IL or missing references)
		//IL_056b: Unknown result type (might be due to invalid IL or missing references)
		//IL_062c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0631: Unknown result type (might be due to invalid IL or missing references)
		//IL_0635: Unknown result type (might be due to invalid IL or missing references)
		//IL_063a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0648: Unknown result type (might be due to invalid IL or missing references)
		//IL_064d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0651: Unknown result type (might be due to invalid IL or missing references)
		//IL_0656: Unknown result type (might be due to invalid IL or missing references)
		//IL_0658: Unknown result type (might be due to invalid IL or missing references)
		//IL_065d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0745: Unknown result type (might be due to invalid IL or missing references)
		//IL_0780: Unknown result type (might be due to invalid IL or missing references)
		//IL_0785: Unknown result type (might be due to invalid IL or missing references)
		//IL_0789: Unknown result type (might be due to invalid IL or missing references)
		//IL_078e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0854: Unknown result type (might be due to invalid IL or missing references)
		//IL_085b: Expected O, but got Unknown
		//IL_07e2: Unknown result type (might be due to invalid IL or missing references)
		//IL_0871: Unknown result type (might be due to invalid IL or missing references)
		//IL_0876: Unknown result type (might be due to invalid IL or missing references)
		//IL_087c: Unknown result type (might be due to invalid IL or missing references)
		//IL_088a: Unknown result type (might be due to invalid IL or missing references)
		//IL_088f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0896: Unknown result type (might be due to invalid IL or missing references)
		//IL_089b: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a0: Unknown result type (might be due to invalid IL or missing references)
		//IL_08a7: Unknown result type (might be due to invalid IL or missing references)
		//IL_08d2: Unknown result type (might be due to invalid IL or missing references)
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
		Curve val = null;
		if (!DA.GetData<Curve>(_idx_inp_polyline, ref val))
		{
			return;
		}
		Curve val2 = val.Simplify((CurveSimplifyOptions)63, num, num2);
		if (val2 != null)
		{
			val = val2;
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
		Polyline val4 = null;
		Vector3d val6 = default(Vector3d);
		Line val11 = default(Line);
		Line val12 = default(Line);
		double num6 = default(double);
		while (true)
		{
			val4 = PolylineOffsetGeneralized(val3, offsetplane, list, num);
			if (val4 == null)
			{
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
			if (num3 == -1)
			{
				break;
			}
			if ((num3 == 0 || num3 == ((RhinoList<Point3d>)(object)val3).Count - 2) && !val3.IsClosed)
			{
				return;
			}
			Point3d val7 = ((RhinoList<Point3d>)(object)val3)[(num3 - 2 + ((RhinoList<Point3d>)(object)val3).Count) % (((RhinoList<Point3d>)(object)val3).Count - 1)];
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
		bool flag = false;
		if (!val3.IsClosed)
		{
			if (flag)
			{
				DA.SetData(_idx_outp_polyline_reverse, (object)val4);
			}
			else
			{
				DA.SetData(_idx_outp_polyline, (object)val4);
			}
			return;
		}
		double num7 = 0.0;
		for (int j = 0; j < ((RhinoList<Point3d>)(object)val3).Count - 1; j++)
		{
			num7 += Vector3d.Multiply(Vector3d.CrossProduct(new Vector3d(((RhinoList<Point3d>)(object)val3)[j]), new Vector3d(((RhinoList<Point3d>)(object)val3)[j + 1])), ((Plane)(ref offsetplane)).Normal);
		}
		List<Line> list2 = new List<Line>(((RhinoList<Point3d>)(object)val3).Count - 1);
		for (int k = 0; k < ((RhinoList<Point3d>)(object)val3).Count - 1; k++)
		{
			list2.Add(new Line(((RhinoList<Point3d>)(object)val4)[k], ((RhinoList<Point3d>)(object)val4)[k + 1]));
		}
		bool flag2 = false;
		double num9 = default(double);
		Line item = default(Line);
		Line value = default(Line);
		Line item2 = default(Line);
		Line value2 = default(Line);
		for (int l = 0; l < list2.Count - 1; l++)
		{
			for (int m = l + 2; m < list2.Count; m++)
			{
				Line val13 = list2[l];
				Line val14 = list2[m];
				double num8 = 0.0;
				if (Intersection.LineLine(val13, val14, ref num9, ref num8, num, true))
				{
					double num10 = num / ((Line)(ref val13)).Length;
					double num11 = num / ((Line)(ref val14)).Length;
					if (num8 > num11 && num8 < 1.0 - num11)
					{
						((Line)(ref item))._002Ector(((Line)(ref val14)).From, ((Line)(ref val14)).PointAt(num8));
						((Line)(ref value))._002Ector(((Line)(ref val14)).PointAt(num8), ((Line)(ref val14)).To);
						list2[m] = value;
						list2.Insert(m, item);
						m++;
						flag2 = true;
					}
					if (num9 > num10 && num9 < 1.0 - num10)
					{
						((Line)(ref item2))._002Ector(((Line)(ref val13)).From, ((Line)(ref val13)).PointAt(num9));
						((Line)(ref value2))._002Ector(((Line)(ref val13)).PointAt(num9), ((Line)(ref val13)).To);
						list2[l] = value2;
						list2.Insert(l, item2);
						m++;
						flag2 = true;
					}
				}
			}
		}
		if (!flag2)
		{
			if (flag)
			{
				DA.SetData(_idx_outp_polyline_reverse, (object)val4);
			}
			else
			{
				DA.SetData(_idx_outp_polyline, (object)val4);
			}
			return;
		}
		List<int> list3 = new List<int>(list2.Count);
		for (int n = 0; n < list2.Count; n++)
		{
			list3.Add(-1);
		}
		int num12 = 0;
		Line val15;
		for (int num13 = 0; num13 < list2.Count; num13++)
		{
			int num14 = list3[num13];
			if (num14 < 0)
			{
				num14 = (list3[num13] = num12++);
			}
			val15 = list2[num13];
			Point3d val16 = ((Line)(ref val15)).From;
			for (int num16 = num13 + 2; num16 < list2.Count; num16++)
			{
				val15 = list2[num16];
				Vector3d val17 = ((Line)(ref val15)).From - val16;
				if (((Vector3d)(ref val17)).Length <= num)
				{
					list3[num16] = num14;
				}
			}
		}
		List<List<int>> list4 = new List<List<int>>();
		for (int num17 = 0; num17 < list3.Count; num17++)
		{
			int num18 = list3[num17];
			if (num18 >= list4.Count)
			{
				list4.Add(new List<int> { num17 });
			}
			else
			{
				list4[num18].Add(num17);
			}
		}
		List<bool> list5 = new List<bool>(list2.Count);
		for (int num19 = 0; num19 < list2.Count; num19++)
		{
			list5.Add(item: false);
		}
		List<List<Line>> list6 = new List<List<Line>>();
		for (int num20 = 0; num20 < list2.Count - 1; num20++)
		{
			if (list5[num20])
			{
				continue;
			}
			List<Line> list7 = new List<Line> { list2[num20] };
			list5[num20] = true;
			int num21 = num20 + 1;
			do
			{
				int index = list3[num21];
				List<int> list8 = list4[index];
				val15 = list7[list7.Count - 1];
				Vector3d direction = ((Line)(ref val15)).Direction;
				((Vector3d)(ref direction)).Reverse();
				int num22 = num21;
				foreach (int item3 in list8)
				{
					if (!list5[item3] && item3 > num22)
					{
						num22 = item3;
					}
				}
				list7.Add(list2[num22]);
				list5[num22] = true;
				num21 = (num22 + 1) % list2.Count;
			}
			while (!list5[num21]);
			list6.Add(list7);
		}
		List<Polyline> list9 = new List<Polyline>();
		List<Polyline> list10 = new List<Polyline>();
		foreach (List<Line> item4 in list6)
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
				list9.Add(val18);
			}
			else
			{
				list10.Add(val18);
			}
		}
		if (list9.Count > 0)
		{
			DA.SetDataList(_idx_outp_polyline, (IEnumerable)list9);
		}
		if (list10.Count > 0)
		{
			DA.SetDataList(_idx_outp_polyline_reverse, (IEnumerable)list10);
		}
	}

	private Polyline PolylineOffsetGeneralized(Polyline p, Plane offsetplane, List<double> arrDist, double absTol)
	{
		//IL_002e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0034: Expected O, but got Unknown
		//IL_0041: Unknown result type (might be due to invalid IL or missing references)
		//IL_0048: Unknown result type (might be due to invalid IL or missing references)
		//IL_004d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0052: Unknown result type (might be due to invalid IL or missing references)
		//IL_0088: Unknown result type (might be due to invalid IL or missing references)
		//IL_008d: Unknown result type (might be due to invalid IL or missing references)
		//IL_008e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Unknown result type (might be due to invalid IL or missing references)
		//IL_005d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0064: Expected O, but got Unknown
		//IL_007c: Unknown result type (might be due to invalid IL or missing references)
		//IL_00d7: Unknown result type (might be due to invalid IL or missing references)
		//IL_00dc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f4: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f9: Unknown result type (might be due to invalid IL or missing references)
		//IL_00e6: Unknown result type (might be due to invalid IL or missing references)
		//IL_00eb: Unknown result type (might be due to invalid IL or missing references)
		//IL_03d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_0338: Unknown result type (might be due to invalid IL or missing references)
		//IL_0341: Unknown result type (might be due to invalid IL or missing references)
		//IL_0346: Unknown result type (might be due to invalid IL or missing references)
		//IL_034b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0100: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Unknown result type (might be due to invalid IL or missing references)
		//IL_0107: Unknown result type (might be due to invalid IL or missing references)
		//IL_0109: Unknown result type (might be due to invalid IL or missing references)
		//IL_010b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0110: Unknown result type (might be due to invalid IL or missing references)
		//IL_0382: Unknown result type (might be due to invalid IL or missing references)
		//IL_0387: Unknown result type (might be due to invalid IL or missing references)
		//IL_0389: Unknown result type (might be due to invalid IL or missing references)
		//IL_038e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0357: Unknown result type (might be due to invalid IL or missing references)
		//IL_035e: Expected O, but got Unknown
		//IL_0376: Unknown result type (might be due to invalid IL or missing references)
		//IL_0147: Unknown result type (might be due to invalid IL or missing references)
		//IL_0149: Unknown result type (might be due to invalid IL or missing references)
		//IL_014b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0150: Unknown result type (might be due to invalid IL or missing references)
		//IL_011c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0123: Expected O, but got Unknown
		//IL_013d: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ab: Unknown result type (might be due to invalid IL or missing references)
		//IL_03ba: Unknown result type (might be due to invalid IL or missing references)
		//IL_03bc: Unknown result type (might be due to invalid IL or missing references)
		//IL_03c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_0190: Unknown result type (might be due to invalid IL or missing references)
		//IL_0192: Unknown result type (might be due to invalid IL or missing references)
		//IL_0194: Unknown result type (might be due to invalid IL or missing references)
		//IL_0199: Unknown result type (might be due to invalid IL or missing references)
		//IL_019e: Unknown result type (might be due to invalid IL or missing references)
		//IL_015c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0163: Expected O, but got Unknown
		//IL_017d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0276: Unknown result type (might be due to invalid IL or missing references)
		//IL_027b: Unknown result type (might be due to invalid IL or missing references)
		//IL_027d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0282: Unknown result type (might be due to invalid IL or missing references)
		//IL_0286: Unknown result type (might be due to invalid IL or missing references)
		//IL_028a: Unknown result type (might be due to invalid IL or missing references)
		//IL_028c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0291: Unknown result type (might be due to invalid IL or missing references)
		//IL_0296: Unknown result type (might be due to invalid IL or missing references)
		//IL_029a: Unknown result type (might be due to invalid IL or missing references)
		//IL_029c: Unknown result type (might be due to invalid IL or missing references)
		//IL_02a1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ad: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b2: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b4: Unknown result type (might be due to invalid IL or missing references)
		//IL_02b9: Unknown result type (might be due to invalid IL or missing references)
		//IL_02bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02c8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02cd: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d1: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_02d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ed: Unknown result type (might be due to invalid IL or missing references)
		//IL_02ef: Unknown result type (might be due to invalid IL or missing references)
		//IL_0313: Unknown result type (might be due to invalid IL or missing references)
		//IL_021a: Unknown result type (might be due to invalid IL or missing references)
		//IL_021f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0221: Unknown result type (might be due to invalid IL or missing references)
		//IL_0226: Unknown result type (might be due to invalid IL or missing references)
		//IL_0242: Unknown result type (might be due to invalid IL or missing references)
		//IL_0244: Unknown result type (might be due to invalid IL or missing references)
		//IL_0257: Unknown result type (might be due to invalid IL or missing references)
		//IL_025e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0260: Unknown result type (might be due to invalid IL or missing references)
		//IL_0265: Unknown result type (might be due to invalid IL or missing references)
		if (p.Length < 2.0)
		{
			return null;
		}
		int num = ((RhinoList<Point3d>)(object)p).Count - 1;
		int num2 = 0;
		if (!p.IsClosed)
		{
			num2 = 1;
		}
		Polyline val = new Polyline(((RhinoList<Point3d>)(object)p).Count);
		if (!p.IsClosed)
		{
			Vector3d val2 = ((RhinoList<Point3d>)(object)p)[1] - ((RhinoList<Point3d>)(object)p)[0];
			if (!((Vector3d)(ref val2)).Unitize())
			{
				p = new Polyline((IEnumerable<Point3d>)p);
				((RhinoList<Point3d>)(object)p).RemoveAt(0);
				arrDist = new List<double>(arrDist);
				arrDist.RemoveAt(0);
				return PolylineOffsetGeneralized(p, offsetplane, arrDist, absTol);
			}
			val2 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val2);
			if (!((Vector3d)(ref val2)).Unitize())
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Specified curve plane is invalid.");
				return null;
			}
			((RhinoList<Point3d>)(object)val).Add(((RhinoList<Point3d>)(object)p)[0] + GetDistanceForSegment(arrDist, 0) * val2);
		}
		Line val11 = default(Line);
		Line val13 = default(Line);
		double num7 = default(double);
		for (int i = num2; i < num; i++)
		{
			Point3d val3 = ((RhinoList<Point3d>)(object)p)[i];
			Point3d val4 = ((i != 0) ? ((RhinoList<Point3d>)(object)p)[i - 1] : ((RhinoList<Point3d>)(object)p)[num - 1]);
			Point3d val5 = ((RhinoList<Point3d>)(object)p)[i + 1];
			Vector3d val6 = val3 - val4;
			if (!((Vector3d)(ref val6)).Unitize())
			{
				p = new Polyline((IEnumerable<Point3d>)p);
				((RhinoList<Point3d>)(object)p).RemoveAt(i);
				arrDist = new List<double>(arrDist);
				arrDist.RemoveAt(i);
				return PolylineOffsetGeneralized(p, offsetplane, arrDist, absTol);
			}
			Vector3d val7 = val5 - val3;
			if (!((Vector3d)(ref val7)).Unitize())
			{
				p = new Polyline((IEnumerable<Point3d>)p);
				((RhinoList<Point3d>)(object)p).RemoveAt(i);
				arrDist = new List<double>(arrDist);
				arrDist.RemoveAt(i);
				return PolylineOffsetGeneralized(p, offsetplane, arrDist, absTol);
			}
			Vector3d val8 = 0.5 * (val6 + val7);
			if (!((Vector3d)(ref val8)).Unitize())
			{
				((Vector3d)(ref val8))._002Ector(0.0, 0.0, 0.0);
			}
			double num3 = 0.0;
			double num4 = ((i != 0) ? GetDistanceForSegment(arrDist, i - 1) : GetDistanceForSegment(arrDist, num - 1));
			num3 = GetDistanceForSegment(arrDist, i);
			if (num4 == num3 && ((Vector3d)(ref val8)).Length > 0.0)
			{
				Vector3d val9 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val8);
				if (!((Vector3d)(ref val9)).Unitize())
				{
					((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Specified curve plane is invalid.");
					return null;
				}
				double num5 = num4 / Math.Abs(Vector3d.Multiply(val6, val8));
				((RhinoList<Point3d>)(object)val).Add(((RhinoList<Point3d>)(object)p)[i] + num5 * val9);
				continue;
			}
			Vector3d val10 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val6);
			((Line)(ref val11))._002Ector(val4 + num4 * val10, val3 + num4 * val10);
			Vector3d val12 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val7);
			((Line)(ref val13))._002Ector(val3 + num3 * val12, val5 + num3 * val12);
			double num6 = 0.0;
			if (!Intersection.LineLine(val11, val13, ref num7, ref num6, absTol, false))
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Line-line intersection failed.");
				return null;
			}
			((RhinoList<Point3d>)(object)val).Add(((Line)(ref val11)).PointAt(num7));
		}
		if (!p.IsClosed)
		{
			Vector3d val14 = ((RhinoList<Point3d>)(object)p)[num] - ((RhinoList<Point3d>)(object)p)[num - 1];
			if (!((Vector3d)(ref val14)).Unitize())
			{
				p = new Polyline((IEnumerable<Point3d>)p);
				((RhinoList<Point3d>)(object)p).RemoveAt(num);
				arrDist = new List<double>(arrDist);
				arrDist.RemoveAt(num);
				return PolylineOffsetGeneralized(p, offsetplane, arrDist, absTol);
			}
			val14 = Vector3d.CrossProduct(((Plane)(ref offsetplane)).Normal, val14);
			if (!((Vector3d)(ref val14)).Unitize())
			{
				((GH_ActiveObject)this).AddRuntimeMessage((GH_RuntimeMessageLevel)20, "Specified curve plane is invalid.");
				return null;
			}
			((RhinoList<Point3d>)(object)val).Add(((RhinoList<Point3d>)(object)p)[num] + GetDistanceForSegment(arrDist, num - 1) * val14);
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
