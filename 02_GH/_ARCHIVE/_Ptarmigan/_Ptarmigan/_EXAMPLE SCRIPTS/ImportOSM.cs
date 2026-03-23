using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

using Rhino.Geometry;
using Rhino.Geometry.Intersect;

using OsmSharp;
using OsmSharp.Streams;
using OsmSharp.Tags;

namespace Ptarmigan
{
    public class ImportOSM : GH_Component
    {
        public ImportOSM()
          : base(
              "Import OSM",
              "ImportOSM",
              "Import vector OpenStreetMap data. Can stay in EPSG:4326 or transform to a reversible local meter space.",
              "Ptarmigan",
              "GIS")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter(
                "Boundary",
                "boundary",
                "Optional boundary curve for filtering. If Transform To Meters is false, boundary must also be in EPSG:4326 degree space. If true, boundary must be in the local meter space.",
                GH_ParamAccess.item);

            pManager.AddTextParameter(
                "OSM Data Location",
                "filePath",
                "Path to .osm or .pbf file.",
                GH_ParamAccess.item);

            pManager.AddBooleanParameter(
                "Transform To Meters",
                "meters",
                "True: convert lon/lat to reversible local meter space. False: keep raw EPSG:4326 lon/lat coordinates.",
                GH_ParamAccess.item,
                true);

            pManager.AddTextParameter(
                "Filter Fields",
                "filterFields",
                "Optional list of tag keys to keep, e.g. building, highway, landuse.",
                GH_ParamAccess.list);

            pManager.AddTextParameter(
                "Filter Field,Value",
                "filterFieldValue",
                "Optional list of key,value filters in the format key,value",
                GH_ParamAccess.list);

            pManager[0].Optional = true;
            pManager[3].Optional = true;
            pManager[4].Optional = true;
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddCurveParameter(
                "Extents",
                "extents",
                "Extents of imported OSM data in the chosen output space.",
                GH_ParamAccess.tree);

            pManager.AddTextParameter(
                "Fields",
                "fields",
                "Feature tag keys.",
                GH_ParamAccess.tree);

            pManager.AddTextParameter(
                "Values",
                "values",
                "Feature tag values.",
                GH_ParamAccess.tree);

            pManager.AddGeometryParameter(
                "Feature Geometry",
                "featureGeometry",
                "Imported OSM feature geometry.",
                GH_ParamAccess.tree);

            pManager.AddGeometryParameter(
                "Buildings",
                "buildings",
                "Extruded building geometry from building or building:part tags.",
                GH_ParamAccess.tree);

            pManager.AddNumberParameter(
                "Origin Longitude",
                "originLon",
                "Longitude of the local transform origin.",
                GH_ParamAccess.item);

            pManager.AddNumberParameter(
                "Origin Latitude",
                "originLat",
                "Latitude of the local transform origin.",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Curve boundary = null;
            string filePath = null;
            bool transformToMeters = true;
            List<string> filterFields = new List<string>();
            List<string> filterFieldValues = new List<string>();

            DA.GetData(0, ref boundary);

            if (!DA.GetData(1, ref filePath))
                return;

            if (!DA.GetData(2, ref transformToMeters))
                return;

            DA.GetDataList(3, filterFields);
            DA.GetDataList(4, filterFieldValues);

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Invalid OSM file path.");
                return;
            }

            try
            {
                Message = transformToMeters ? "Local meters" : "EPSG:4326";

                OsmImportResult result = OsmImporterCore.Import(
                    filePath,
                    boundary,
                    filterFields,
                    filterFieldValues,
                    transformToMeters);

                DA.SetDataTree(0, result.Extents);
                DA.SetDataTree(1, result.Fields);
                DA.SetDataTree(2, result.Values);
                DA.SetDataTree(3, result.FeatureGeometry);
                DA.SetDataTree(4, result.Buildings);
                DA.SetData(5, result.OriginLongitude);
                DA.SetData(6, result.OriginLatitude);
            }
            catch (Exception ex)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, ex.ToString());
            }
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid => new Guid("A8C52282-9C75-4E20-9956-F3D644A828D2");
    }

    public sealed class OsmImportResult
    {
        public GH_Structure<GH_Curve> Extents { get; } = new GH_Structure<GH_Curve>();
        public GH_Structure<GH_String> Fields { get; } = new GH_Structure<GH_String>();
        public GH_Structure<GH_String> Values { get; } = new GH_Structure<GH_String>();
        public GH_Structure<IGH_GeometricGoo> FeatureGeometry { get; } = new GH_Structure<IGH_GeometricGoo>();
        public GH_Structure<IGH_GeometricGoo> Buildings { get; } = new GH_Structure<IGH_GeometricGoo>();

        public double OriginLongitude { get; set; }
        public double OriginLatitude { get; set; }
    }

    internal static class OsmImporterCore
    {
        public static OsmImportResult Import(
            string filePath,
            Curve boundary,
            List<string> filterFields,
            List<string> filterFieldValues,
            bool transformToMeters)
        {
            var result = new OsmImportResult();

            List<OsmGeo> all = ReadAll(filePath);
            if (all.Count == 0)
                return result;

            var nodes = all
                .OfType<Node>()
                .Where(n => n.Id.HasValue && n.Longitude.HasValue && n.Latitude.HasValue)
                .ToDictionary(n => n.Id.Value, n => n);

            var ways = all
                .OfType<Way>()
                .Where(w => w.Id.HasValue)
                .ToDictionary(w => w.Id.Value, w => w);

            if (nodes.Count == 0)
                return result;

            double minLon = nodes.Values.Min(n => n.Longitude.Value);
            double maxLon = nodes.Values.Max(n => n.Longitude.Value);
            double minLat = nodes.Values.Min(n => n.Latitude.Value);
            double maxLat = nodes.Values.Max(n => n.Latitude.Value);

            double originLon = 0.5 * (minLon + maxLon);
            double originLat = 0.5 * (minLat + maxLat);

            result.OriginLongitude = originLon;
            result.OriginLatitude = originLat;

            Curve extentsCurve = BuildExtents(minLon, minLat, maxLon, maxLat, originLon, originLat, transformToMeters);
            if (extentsCurve != null)
            {
                result.Extents.Append(new GH_Curve(extentsCurve), new GH_Path(0));
            }

            int nodeBranch = 0;
            int wayBranch = 1000000;
            int relationBranch = 2000000;
            int buildingBranch = 0;

            foreach (var node in nodes.Values)
            {
                if (!PassesFilters(node.Tags, filterFields, filterFieldValues))
                    continue;

                Point3d pt = ToOutputPoint(
                    node.Longitude.Value,
                    node.Latitude.Value,
                    originLon,
                    originLat,
                    transformToMeters);

                if (!PassesBoundary(pt, boundary))
                    continue;

                GH_Path path = new GH_Path(nodeBranch++);
                result.FeatureGeometry.Append(new GH_Point(pt), path);
                AppendTags(node.Tags, result.Fields, result.Values, path);
            }

            foreach (var way in ways.Values)
            {
                if (way.Nodes == null || way.Nodes.Length < 2)
                    continue;

                if (!PassesFilters(way.Tags, filterFields, filterFieldValues))
                    continue;

                List<Point3d> pts = ResolveWayPoints(way, nodes, originLon, originLat, transformToMeters);
                if (pts.Count < 2)
                    continue;

                Curve curve = BuildCurveFromPoints(pts);
                if (curve == null)
                    continue;

                if (!PassesBoundary(curve, boundary))
                    continue;

                GH_Path path = new GH_Path(wayBranch++);
                result.FeatureGeometry.Append(new GH_Curve(curve), path);
                AppendTags(way.Tags, result.Fields, result.Values, path);

                if (IsBuilding(way.Tags))
                {
                    Brep b = TryCreateBuildingFromClosedCurve(curve, way.Tags);
                    if (b != null)
                    {
                        result.Buildings.Append(new GH_Brep(b), new GH_Path(buildingBranch++));
                    }
                }
            }

            foreach (var relation in all.OfType<Relation>())
            {
                if (!relation.Id.HasValue)
                    continue;

                if (!PassesFilters(relation.Tags, filterFields, filterFieldValues))
                    continue;

                List<Curve> relationCurves = ResolveRelationCurves(
                    relation,
                    ways,
                    nodes,
                    originLon,
                    originLat,
                    transformToMeters);

                GH_Path path = new GH_Path(relationBranch++);
                AppendTags(relation.Tags, result.Fields, result.Values, path);

                foreach (Curve crv in relationCurves)
                {
                    if (crv == null)
                        continue;

                    if (!PassesBoundary(crv, boundary))
                        continue;

                    result.FeatureGeometry.Append(new GH_Curve(crv), path);
                }

                if (IsBuilding(relation.Tags))
                {
                    List<Brep> buildingBreps = TryCreateBuildingFromRelationCurves(relationCurves, relation.Tags);
                    foreach (Brep b in buildingBreps)
                    {
                        if (b != null)
                        {
                            result.Buildings.Append(new GH_Brep(b), new GH_Path(buildingBranch++));
                        }
                    }
                }
            }

            return result;
        }

        private static List<OsmGeo> ReadAll(string filePath)
        {
            using (FileStream fs = File.OpenRead(filePath))
            {
                string ext = Path.GetExtension(filePath).ToLowerInvariant();

                if (ext == ".pbf")
                {
                    var src = new PBFOsmStreamSource(fs);
                    return src.Cast<OsmGeo>().ToList();
                }

                var xmlSrc = new XmlOsmStreamSource(fs);
                return xmlSrc.Cast<OsmGeo>().ToList();
            }
        }

        private static Curve BuildExtents(
            double minLon,
            double minLat,
            double maxLon,
            double maxLat,
            double originLon,
            double originLat,
            bool transformToMeters)
        {
            Point3d a = ToOutputPoint(minLon, minLat, originLon, originLat, transformToMeters);
            Point3d b = ToOutputPoint(maxLon, minLat, originLon, originLat, transformToMeters);
            Point3d c = ToOutputPoint(maxLon, maxLat, originLon, originLat, transformToMeters);
            Point3d d = ToOutputPoint(minLon, maxLat, originLon, originLat, transformToMeters);

            return new PolylineCurve(new[] { a, b, c, d, a });
        }

        private static Point3d ToOutputPoint(
            double lon,
            double lat,
            double originLon,
            double originLat,
            bool transformToMeters)
        {
            if (!transformToMeters)
                return new Point3d(lon, lat, 0.0);

            return LocalProjection.LonLatToLocal(lon, lat, originLon, originLat);
        }

        private static List<Point3d> ResolveWayPoints(
            Way way,
            Dictionary<long, Node> nodes,
            double originLon,
            double originLat,
            bool transformToMeters)
        {
            var pts = new List<Point3d>();

            foreach (long nodeId in way.Nodes)
            {
                if (!nodes.TryGetValue(nodeId, out Node node))
                    continue;

                if (!node.Longitude.HasValue || !node.Latitude.HasValue)
                    continue;

                Point3d p = ToOutputPoint(
                    node.Longitude.Value,
                    node.Latitude.Value,
                    originLon,
                    originLat,
                    transformToMeters);

                pts.Add(p);
            }

            var clean = new List<Point3d>();
            foreach (Point3d p in pts)
            {
                if (clean.Count == 0 || clean[clean.Count - 1].DistanceToSquared(p) > 1e-18)
                    clean.Add(p);
            }

            return clean;
        }

        private static Curve BuildCurveFromPoints(List<Point3d> pts)
        {
            if (pts == null || pts.Count < 2)
                return null;

            return new PolylineCurve(pts);
        }

        private static List<Curve> ResolveRelationCurves(
            Relation relation,
            Dictionary<long, Way> ways,
            Dictionary<long, Node> nodes,
            double originLon,
            double originLat,
            bool transformToMeters)
        {
            var curves = new List<Curve>();

            if (relation.Members == null || relation.Members.Length == 0)
                return curves;

            var outerCurves = new List<Curve>();
            var innerCurves = new List<Curve>();
            var untypedCurves = new List<Curve>();

            foreach (var member in relation.Members)
            {
                if (member.Type != OsmGeoType.Way)
                    continue;

                if (!ways.TryGetValue(member.Id, out Way way))
                    continue;

                List<Point3d> pts = ResolveWayPoints(way, nodes, originLon, originLat, transformToMeters);
                Curve c = BuildCurveFromPoints(pts);
                if (c == null)
                    continue;

                string role = (member.Role ?? string.Empty).Trim().ToLowerInvariant();
                if (role == "outer")
                    outerCurves.Add(c);
                else if (role == "inner")
                    innerCurves.Add(c);
                else
                    untypedCurves.Add(c);
            }

            curves.AddRange(JoinCurvesIfPossible(outerCurves));
            curves.AddRange(JoinCurvesIfPossible(innerCurves));
            curves.AddRange(JoinCurvesIfPossible(untypedCurves));

            return curves;
        }

        private static IEnumerable<Curve> JoinCurvesIfPossible(List<Curve> curves)
        {
            if (curves == null || curves.Count == 0)
                return Enumerable.Empty<Curve>();

            Curve[] joined = Curve.JoinCurves(curves, 1e-6);
            if (joined != null && joined.Length > 0)
                return joined;

            return curves;
        }

        private static bool PassesFilters(
            TagsCollectionBase tags,
            List<string> filterFields,
            List<string> filterFieldValues)
        {
            bool hasFieldFilter = filterFields != null && filterFields.Count > 0;
            bool hasFieldValueFilter = filterFieldValues != null && filterFieldValues.Count > 0;

            if (!hasFieldFilter && !hasFieldValueFilter)
                return true;

            if (tags == null)
                return false;

            bool fieldOk = !hasFieldFilter || filterFields.Any(f =>
                !string.IsNullOrWhiteSpace(f) &&
                tags.ContainsKey(f.Trim()));

            bool fieldValueOk = !hasFieldValueFilter || filterFieldValues.Any(fv =>
            {
                if (string.IsNullOrWhiteSpace(fv))
                    return false;

                string[] parts = fv.Split(new[] { ',' }, 2);
                if (parts.Length != 2)
                    return false;

                string key = parts[0].Trim();
                string value = parts[1].Trim();

                if (!tags.TryGetValue(key, out string actual))
                    return false;

                return string.Equals(actual ?? string.Empty, value, StringComparison.OrdinalIgnoreCase);
            });

            return fieldOk && fieldValueOk;
        }

        private static bool PassesBoundary(Point3d pt, Curve boundary)
        {
            if (boundary == null)
                return true;

            Plane plane;
            if (!boundary.TryGetPlane(out plane))
                plane = Plane.WorldXY;

            PointContainment containment = boundary.Contains(pt, plane, 1e-6);
            return containment != PointContainment.Outside;
        }

        private static bool PassesBoundary(Curve crv, Curve boundary)
        {
            if (boundary == null)
                return true;

            if (crv == null)
                return false;

            var ccx = Intersection.CurveCurve(crv, boundary, 1e-6, 1e-6);
            if (ccx != null && ccx.Count > 0)
                return true;

            if (crv.IsClosed)
            {
                BoundingBox bb = crv.GetBoundingBox(true);
                Point3d center = bb.Center;
                if (PassesBoundary(center, boundary))
                    return true;
            }

            double t;
            if (crv.LengthParameter(crv.GetLength() * 0.5, out t))
            {
                Point3d mid = crv.PointAt(t);
                return PassesBoundary(mid, boundary);
            }

            return false;
        }

        private static void AppendTags(
            TagsCollectionBase tags,
            GH_Structure<GH_String> fields,
            GH_Structure<GH_String> values,
            GH_Path path)
        {
            if (tags == null)
                return;

            foreach (var kv in tags)
            {
                fields.Append(new GH_String(kv.Key ?? string.Empty), path);
                values.Append(new GH_String(kv.Value ?? string.Empty), path);
            }
        }

        private static bool IsBuilding(TagsCollectionBase tags)
        {
            if (tags == null)
                return false;

            return tags.ContainsKey("building") || tags.ContainsKey("building:part");
        }

        private static Brep TryCreateBuildingFromClosedCurve(Curve curve, TagsCollectionBase tags)
        {
            if (curve == null || !curve.IsClosed)
                return null;

            Curve clean = curve.DuplicateCurve();
            clean.MakeClosed(1e-6);

            double height = Math.Abs(GetHeight(tags));
            if (height <= 0.0)
                height = 10.0;

            Extrusion extrusion = Extrusion.Create(clean, height, true);
            if (extrusion == null)
                return null;

            return extrusion.ToBrep();
        }

        private static List<Brep> TryCreateBuildingFromRelationCurves(List<Curve> curves, TagsCollectionBase tags)
        {
            var outBreps = new List<Brep>();

            if (curves == null || curves.Count == 0)
                return outBreps;

            double height = Math.Abs(GetHeight(tags));
            if (height <= 0.0)
                height = 10.0;

            foreach (Curve c in curves)
            {
                if (c == null || !c.IsClosed)
                    continue;

                Curve clean = c.DuplicateCurve();
                clean.MakeClosed(1e-6);

                Extrusion extrusion = Extrusion.Create(clean, height, true);
                if (extrusion != null)
                    outBreps.Add(extrusion.ToBrep());
            }

            return outBreps;
        }

        private static double GetHeight(TagsCollectionBase tags)
        {
            if (tags == null)
                return 10.0;

            if (tags.TryGetValue("height", out string h))
            {
                double parsedHeight;
                if (TryParseHeightMeters(h, out parsedHeight))
                    return Math.Max(parsedHeight, 0.1);
            }

            if (tags.TryGetValue("building:levels", out string levels))
            {
                if (double.TryParse(levels, NumberStyles.Float, CultureInfo.InvariantCulture, out double lv))
                    return Math.Max(lv * 3.0, 0.1);
            }

            if (tags.TryGetValue("levels", out string lv2))
            {
                if (double.TryParse(lv2, NumberStyles.Float, CultureInfo.InvariantCulture, out double lv))
                    return Math.Max(lv * 3.0, 0.1);
            }

            return 10.0;
        }

        private static bool TryParseHeightMeters(string raw, out double value)
        {
            value = 0.0;
            if (string.IsNullOrWhiteSpace(raw))
                return false;

            string s = raw.Trim().ToLowerInvariant();
            s = s.Replace("meters", "")
                 .Replace("meter", "")
                 .Replace("metres", "")
                 .Replace("metre", "")
                 .Replace("m", "")
                 .Trim();

            return double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
        }
    }

    internal static class LocalProjection
    {
        private const double EarthRadius = 6378137.0;

        public static Point3d LonLatToLocal(
            double lonDeg,
            double latDeg,
            double originLonDeg,
            double originLatDeg)
        {
            double lon = DegreesToRadians(lonDeg);
            double lat = DegreesToRadians(latDeg);
            double lon0 = DegreesToRadians(originLonDeg);
            double lat0 = DegreesToRadians(originLatDeg);

            double x = EarthRadius * (lon - lon0) * Math.Cos(lat0);
            double y = EarthRadius * (lat - lat0);

            return new Point3d(x, y, 0.0);
        }

        public static Point3d LocalToLonLat(
            double x,
            double y,
            double originLonDeg,
            double originLatDeg)
        {
            double lon0 = DegreesToRadians(originLonDeg);
            double lat0 = DegreesToRadians(originLatDeg);

            double lon = lon0 + x / (EarthRadius * Math.Cos(lat0));
            double lat = lat0 + y / EarthRadius;

            return new Point3d(
                RadiansToDegrees(lon),
                RadiansToDegrees(lat),
                0.0);
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        private static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }
    }
}