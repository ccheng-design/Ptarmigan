using Rhino;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;
using System;
using System.Collections.Generic;


using System.Collections.Concurrent;
using System.Runtime.ExceptionServices;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using System.Collections;
using Grasshopper;
using Grasshopper.Kernel.Types;
using System.Threading.Tasks;
using System.Linq;
using System.IO;

namespace CurveOffsetTools
{
    public static class PolylineOffsetHelper
    {
        public static Polyline OffsetPolylineGeneralized(Polyline pinput, Plane offsetPlane, List<double> distances, double absTol, out List<int> removedIndices)
        {
            removedIndices = new List<int>();

            if (pinput.Count < 2)
                return null;

            Polyline result = new Polyline(pinput.Count);

            if (!pinput.IsClosed)
            {
                Vector3d startVec = pinput[1] - pinput[0];
                if (!startVec.Unitize())
                {
                    pinput.RemoveAt(0);
                    distances.RemoveAt(0);
                    removedIndices.Add(0);
                    return OffsetPolylineGeneralized(pinput, offsetPlane, distances, absTol, out removedIndices);
                }

                Vector3d perp = Vector3d.CrossProduct(offsetPlane.Normal, startVec);
                if (!perp.Unitize())
                {
                    RhinoApp.WriteLine("Invalid offset plane.");
                    return null;
                }

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

                double dist0 = (i == 0) ? GetDistanceForSegment(distances, last - 1) : GetDistanceForSegment(distances, i - 1);
                double dist1 = GetDistanceForSegment(distances, i);

                if (dist0 == dist1 && avg.Length > 0)
                {
                    Vector3d perp = Vector3d.CrossProduct(offsetPlane.Normal, avg);
                    if (!perp.Unitize())
                    {
                        RhinoApp.WriteLine("Invalid offset plane.");
                        return null;
                    }

                    double scale = dist0 / Math.Abs(Vector3d.Multiply(v0, avg));
                    result.Add(pt1 + scale * perp);
                }
                else
                {
                    Vector3d perp0 = Vector3d.CrossProduct(offsetPlane.Normal, v0);
                    Vector3d perp1 = Vector3d.CrossProduct(offsetPlane.Normal, v1);

                    Line line0 = new Line(pt0 + dist0 * perp0, pt1 + dist0 * perp0);
                    Line line1 = new Line(pt1 + dist1 * perp1, pt2 + dist1 * perp1);

                    if (!Intersection.LineLine(line0, line1, out double t0, out double t1, absTol, false))
                    {
                        RhinoApp.WriteLine("Line-line intersection failed.");
                        return null;
                    }

                    result.Add(line0.PointAt(t0));
                }
            }

            if (!pinput.IsClosed)
            {
                Vector3d endVec = pinput[last] - pinput[last - 1];
                if (!endVec.Unitize())
                {
                    pinput.RemoveAt(last - 1);
                    distances.RemoveAt(last - 1);
                    removedIndices.Add(last - 1);
                    return OffsetPolylineGeneralized(pinput, offsetPlane, distances, absTol, out removedIndices);
                }

                Vector3d perp = Vector3d.CrossProduct(offsetPlane.Normal, endVec);
                if (!perp.Unitize())
                {
                    RhinoApp.WriteLine("Invalid offset plane.");
                    return null;
                }

                result.Add(pinput[last] + distances[last - 1] * perp);
            }
            else
            {
                result.Add(result[0]); // Close the polyline
            }

            return result;
        }

        public static void RestoreRemovedPoints(Polyline polyline, List<int> removedIndices)
        {
            for (int i = removedIndices.Count - 1; i >= 0; i--)
            {
                int index = removedIndices[i];
                polyline.Insert(index, polyline[index]);
            }
        }

        private static double GetDistanceForSegment(List<double> distances, int index)
        {
            if (index >= distances.Count)
                return distances[distances.Count - 1];
            return distances[index];
        }
    }
}