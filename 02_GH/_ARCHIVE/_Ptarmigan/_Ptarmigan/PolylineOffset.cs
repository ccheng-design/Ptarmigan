using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;



namespace _Ptarmigan
{
    public class PolylineOffset : GH_Component
    {
        public PolylineOffset()
          : base("PolylineOffset", "PolylineOffset",
              "Offset planar polylines",
              "Ptarmigan", "Curves")
        {
        }

        public override Guid ComponentGuid => new Guid("FD83AADB-CC21-49AE-92D7-34F68A2B3E84");
        protected override Bitmap Icon => null;

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polyline", "C", "Planar polyline to offset", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "D", "Offset distance(s)", GH_ParamAccess.list);
            pManager.AddPlaneParameter("Plane", "P", "Curve plane to use, optional", GH_ParamAccess.item);
            pManager.AddNumberParameter("Absolute tolerance", "T", "Absolute tolerance to use, optional", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Angle tolerance", "A", "Angle tolerance to use, optional", GH_ParamAccess.item, 0.0);
            pManager.AddBooleanParameter("Keep duplicates", "K", "Keep duplicate points", GH_ParamAccess.item, false);
            pManager.AddBooleanParameter("Avoid self intersections", "S", "Avoid self intersections by splitting the offsetted polyline into pieces", GH_ParamAccess.item, false);

            pManager[2].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter("Offsetted polyline", "O", "Offsetted polyline(s)", GH_ParamAccess.list);
            pManager.AddCurveParameter("Offsetted polyline reverse", "O", "Offsetted polyline(s) which switched direction due to offset", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve input = null;
            List<double> distances = new List<double>();
            Plane plane = Plane.WorldXY;
            double absTol = 0.0;
            double angleTol = 0.0;
            bool keepDuplicates = false;
            bool splitIntersections = false;

            if (!DA.GetData(0, ref input)) return;
            if (!DA.GetDataList(1, distances)) return;
            DA.GetData(2, ref plane);
            DA.GetData(3, ref absTol);
            DA.GetData(4, ref angleTol);
            DA.GetData(5, ref keepDuplicates);
            DA.GetData(6, ref splitIntersections);

            if (input == null || !input.IsPolyline())
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input must be a valid polyline.");
                return;
            }

            if (!input.TryGetPolyline(out Polyline poly))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to extract polyline.");
                return;
            }

            if (absTol == 0.0)
                absTol = RhinoDoc.ActiveDoc?.ModelAbsoluteTolerance ?? 0.01;

            if (angleTol == 0.0)
                angleTol = RhinoDoc.ActiveDoc?.ModelAngleToleranceRadians ?? (Math.PI / 180.0);

            if (plane == Plane.Unset)
            {
                if (!input.TryGetPlane(out plane))
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input polyline is not planar and no plane was supplied.");
                    return;
                }
            }

            // ---------- NEW: Build distance sets ----------
            // If the user supplied exactly one distance per vertex, treat it as per-vertex.
            // Otherwise, treat the list as multiple uniform distances (one output per value).
            List<List<double>> distanceSets = new List<List<double>>();

            if (distances.Count == poly.Count)
            {
                // Per-vertex mode
                distanceSets.Add(new List<double>(distances));
            }
            else
            {
                // Multiple uniform offsets mode
                if (distances.Count == 0)
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Provide at least one distance.");
                    return;
                }
                foreach (double d in distances)
                {
                    var set = Enumerable.Repeat(d, poly.Count).ToList();
                    distanceSets.Add(set);
                }
            }
            // ---------- /NEW ----------

            var allRegular = new List<Polyline>();
            var allReversed = new List<Polyline>();

            // Run the offset for each distance set
            foreach (var distSet in distanceSets)
            {
                // Clone working inputs for each run
                List<int> removed = new List<int>();
                Polyline working = new Polyline(poly);
                Polyline offset = null;

                // Ensure per-vertex list matches vertex count
                var perVertex = new List<double>(distSet);
                while (perVertex.Count < working.Count)
                    perVertex.Add(perVertex[perVertex.Count - 1]);
                if (perVertex.Count > working.Count)
                    perVertex = perVertex.GetRange(0, working.Count);

                // --- your existing iterative fix-up loop, unchanged except using perVertex ---
                while (true)
                {
                    offset = OffsetPolylineGeneralized(working, plane, perVertex, absTol, out removed);
                    if (offset == null)
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Offset operation failed.");
                        break;
                    }

                    int worst = -1;
                    double worstLength = 0.0;
                    for (int i = 0; i < working.Count - 1; i++)
                    {
                        Vector3d orig = working[i + 1] - working[i];
                        Vector3d off = offset[i + 1] - offset[i];
                        if (Vector3d.Multiply(orig, off) < 0.0)
                        {
                            double len = off.Length;
                            if (len > worstLength)
                            {
                                worst = i;
                                worstLength = len;
                            }
                        }
                    }

                    if (worst == -1 || (!working.IsClosed && (worst == 0 || worst == working.Count - 2)))
                        break;

                    int prev = (worst - 1 + working.Count - 1) % (working.Count - 1);
                    int next = (worst + 2) % (working.Count - 1);

                    Line l1 = new Line(working[prev], working[worst]);
                    Line l2 = new Line(working[worst + 1], working[next]);
                    if (!Intersection.LineLine(l1, l2, out double t1, out double t2, absTol, false))
                    {
                        AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Self-intersection resolution failed.");
                        break;
                    }

                    working.RemoveAt(worst);
                    perVertex.RemoveAt(worst);
                    removed.Add(worst);

                    Point3d mid = l1.PointAt(t1);
                    working[worst] = mid;

                    if (worst == working.Count - 1)
                        working[0] = mid;
                    else if (worst == 0)
                        working[working.Count - 1] = mid;
                }

                if (offset == null)
                    continue;

                if (!poly.IsClosed || !splitIntersections)
                {
                    if (!keepDuplicates)
                        RestoreRemovedPoints(offset, removed);

                    allRegular.Add(offset);
                    // Nothing to add to reversed list in this path
                }
                else
                {
                    List<Polyline> regularLoops, reversedLoops;
                    SplitSelfIntersectionsAndRebuildLoops(offset, working, plane, absTol, out regularLoops, out reversedLoops);

                    if (!keepDuplicates)
                    {
                        // Optional: if you want to restore removed points on loop pieces,
                        // you could try to map indices back; often unnecessary for split loops.
                    }

                    allRegular.AddRange(regularLoops);
                    allReversed.AddRange(reversedLoops);
                }
            }

            DA.SetDataList(0, allRegular);
            DA.SetDataList(1, allReversed);
        }


        private Polyline OffsetPolylineGeneralized(Polyline poly, Plane plane, List<double> distances, double tol, out List<int> removed)
        {
            removed = new List<int>();
            if (poly.Count < 2) return null;

            Polyline result = new Polyline();

            if (!poly.IsClosed)
            {
                Vector3d v = poly[1] - poly[0];
                if (!v.Unitize())
                {
                    poly.RemoveAt(0);
                    distances.RemoveAt(0);
                    removed.Add(0);
                    return OffsetPolylineGeneralized(poly, plane, distances, tol, out removed);
                }
                Vector3d perp = Vector3d.CrossProduct(plane.Normal, v);
                if (!perp.Unitize()) return null;
                result.Add(poly[0] + distances[0] * perp);
            }

            int last = poly.Count - 1;
            int start = poly.IsClosed ? 0 : 1;

            for (int i = start; i < last; i++)
            {
                int next = i + 1;
                int prev = (i == 0) ? last - 1 : i - 1;

                Point3d pt0 = poly[prev];
                Point3d pt1 = poly[i];
                Point3d pt2 = poly[next];

                Vector3d v0 = pt1 - pt0;
                Vector3d v1 = pt2 - pt1;
                if (!v0.Unitize())
                {
                    poly.RemoveAt(prev);
                    distances.RemoveAt(prev);
                    removed.Add(prev);
                    return OffsetPolylineGeneralized(poly, plane, distances, tol, out removed);
                }
                if (!v1.Unitize())
                {
                    poly.RemoveAt(i);
                    distances.RemoveAt(i);
                    removed.Add(i);
                    return OffsetPolylineGeneralized(poly, plane, distances, tol, out removed);
                }

                Vector3d avg = 0.5 * (v0 + v1);
                if (!avg.Unitize()) avg = Vector3d.Zero;

                double d0 = (i == 0) ? distances[last - 1] : distances[i - 1];
                double d1 = distances[i];

                if (d0 == d1 && avg.Length > 0)
                {
                    Vector3d perp = Vector3d.CrossProduct(plane.Normal, avg);
                    if (!perp.Unitize()) return null;
                    double scale = d0 / Math.Abs(Vector3d.Multiply(v0, avg));
                    result.Add(pt1 + scale * perp);
                }
                else
                {
                    Vector3d perp0 = Vector3d.CrossProduct(plane.Normal, v0);
                    Vector3d perp1 = Vector3d.CrossProduct(plane.Normal, v1);
                    Line l0 = new Line(pt0 + d0 * perp0, pt1 + d0 * perp0);
                    Line l1 = new Line(pt1 + d1 * perp1, pt2 + d1 * perp1);

                    if (!Intersection.LineLine(l0, l1, out double t0, out double t1, tol, false))
                        return null;

                    result.Add(l0.PointAt(t0));
                }
            }

            if (!poly.IsClosed)
            {
                Vector3d v = poly[last] - poly[last - 1];
                if (!v.Unitize())
                {
                    poly.RemoveAt(last - 1);
                    distances.RemoveAt(last - 1);
                    removed.Add(last - 1);
                    return OffsetPolylineGeneralized(poly, plane, distances, tol, out removed);
                }
                Vector3d perp = Vector3d.CrossProduct(plane.Normal, v);
                if (!perp.Unitize()) return null;
                result.Add(poly[last] + distances[last - 1] * perp);
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

        private void SplitSelfIntersectionsAndRebuildLoops(Polyline input, Polyline original, Plane plane, double tol, out List<Polyline> regular, out List<Polyline> reversed)
        {
            // Implement as previously discussed, matching the .gha self-intersection splitting strategy
            // Let me know if you want this part explicitly re-pasted too
            regular = new List<Polyline>(); // placeholder
            reversed = new List<Polyline>(); // placeholder
        }

        private class Point3dComparer : IEqualityComparer<Point3d>
        {
            private readonly double _tol;
            public Point3dComparer(double tol) { _tol = tol; }

            public bool Equals(Point3d a, Point3d b)
            {
                return a.DistanceToSquared(b) < _tol * _tol;
            }

            public int GetHashCode(Point3d pt)
            {
                int x = (int)(pt.X / _tol);
                int y = (int)(pt.Y / _tol);
                int z = (int)(pt.Z / _tol);
                return x ^ y ^ z;
            }
        }
    }
}
