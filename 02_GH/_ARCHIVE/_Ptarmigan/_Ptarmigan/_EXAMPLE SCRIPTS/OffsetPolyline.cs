using System;
using System.Collections.Generic;
using System.Drawing;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace _Ptarmigan
{
    public class PolylineOffsetComponent : GH_Component
    {
        public PolylineOffsetComponent()
          : base("PolylineOffset", "OffsetPline",
              "Offset a planar polyline with optional per-segment distances.",
              "Curve", "Util")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "P", "Input planar polyline", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distances", "D", "Offset distances per segment", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "Pl", "Optional plane for offset direction", GH_ParamAccess.item, Plane.WorldXY);
            pManager.AddNumberParameter("Tolerance", "T", "Absolute tolerance", GH_ParamAccess.item, 0.01);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Offset Polyline", "O", "Resulting offset polyline", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve crv = null;
            List<double> distances = new List<double>();
            Plane plane = Plane.WorldXY;
            double tol = 0.01;

            if (!DA.GetData(0, ref crv)) return;
            if (!DA.GetDataList(1, distances)) return;
            DA.GetData(2, ref plane);
            DA.GetData(3, ref tol);

            if (!crv.IsPolyline())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input curve must be a polyline.");
                return;
            }

            if (!crv.TryGetPolyline(out Polyline poly))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to extract polyline.");
                return;
            }

            List<int> removed;
            Polyline offset = OffsetPolylineGeneralized(poly, plane, distances, tol, out removed);
            if (offset == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Offset operation failed.");
                return;
            }

            RestoreRemovedPoints(offset, removed);
            DA.SetData(0, offset);
        }

        public override Guid ComponentGuid => new Guid("103d7612-add8-4f29-b7b1-834077d1d4f7");

        protected override Bitmap Icon => null;

        // === HELPER METHODS ===

        private Polyline OffsetPolylineGeneralized(Polyline pinput, Plane offsetPlane, List<double> distances, double absTol, out List<int> removedIndices)
        {
            removedIndices = new List<int>();
            if (pinput.Count < 2) return null;

            Polyline result = new Polyline(pinput.Count);
            if (!pinput.IsClosed)
            {
                Vector3d v = pinput[1] - pinput[0];
                if (!v.Unitize())
                {
                    pinput.RemoveAt(0);
                    distances.RemoveAt(0);
                    removedIndices.Add(0);
                    return OffsetPolylineGeneralized(pinput, offsetPlane, distances, absTol, out removedIndices);
                }
                Vector3d perp = Vector3d.CrossProduct(offsetPlane.Normal, v);
                if (!perp.Unitize()) return null;
                result.Add(pinput[0] + distances[0] * perp);
            }

            int last = pinput.Count - 1;
            int start = pinput.IsClosed ? 0 : 1;

            for (int i = start; i < last; i++)
            {
                int next = i + 1;
                int prev = (i == 0) ? last - 1 : i - 1;

                Point3d pt0 = pinput[prev];
                Point3d pt1 = pinput[i];
                Point3d pt2 = pinput[next];

                Vector3d v0 = pt1 - pt0;
                Vector3d v1 = pt2 - pt1;
                if (!v0.Unitize())
                {
                    pinput.RemoveAt(prev);
                    distances.RemoveAt(prev);
                    removedIndices.Add(prev);
                    return OffsetPolylineGeneralized(pinput, offsetPlane, distances, absTol, out removedIndices);
                }
                if (!v1.Unitize())
                {
                    pinput.RemoveAt(i);
                    distances.RemoveAt(i);
                    removedIndices.Add(i);
                    return OffsetPolylineGeneralized(pinput, offsetPlane, distances, absTol, out removedIndices);
                }

                Vector3d avg = 0.5 * (v0 + v1);
                if (!avg.Unitize()) avg = Vector3d.Zero;

                double d0 = (i == 0) ? distances[last - 1] : distances[i - 1];
                double d1 = distances[i];

                if (d0 == d1 && avg.Length > 0)
                {
                    Vector3d perp = Vector3d.CrossProduct(offsetPlane.Normal, avg);
                    if (!perp.Unitize()) return null;
                    double scale = d0 / Math.Abs(Vector3d.Multiply(v0, avg));
                    result.Add(pt1 + scale * perp);
                }
                else
                {
                    Vector3d perp0 = Vector3d.CrossProduct(offsetPlane.Normal, v0);
                    Vector3d perp1 = Vector3d.CrossProduct(offsetPlane.Normal, v1);
                    Line l0 = new Line(pt0 + d0 * perp0, pt1 + d0 * perp0);
                    Line l1 = new Line(pt1 + d1 * perp1, pt2 + d1 * perp1);

                    if (!Intersection.LineLine(l0, l1, out double t0, out double t1, absTol, false))
                        return null;

                    result.Add(l0.PointAt(t0));
                }
            }

            if (!pinput.IsClosed)
            {
                Vector3d v = pinput[last] - pinput[last - 1];
                if (!v.Unitize())
                {
                    pinput.RemoveAt(last - 1);
                    distances.RemoveAt(last - 1);
                    removedIndices.Add(last - 1);
                    return OffsetPolylineGeneralized(pinput, offsetPlane, distances, absTol, out removedIndices);
                }
                Vector3d perp = Vector3d.CrossProduct(offsetPlane.Normal, v);
                if (!perp.Unitize()) return null;
                result.Add(pinput[last] + distances[last - 1] * perp);
            }
            else
            {
                result.Add(result[0]);
            }

            return result;
        }

        private void RestoreRemovedPoints(Polyline poly, List<int> removed)
        {
            for (int i = removed.Count - 1; i >= 0; i--)
            {
                int idx = removed[i];
                poly.Insert(idx, poly[idx]);
            }
        }
    }
}
