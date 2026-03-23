using Grasshopper.Kernel;
using Rhino.Geometry;
using System;

namespace _Ptarmigan.Depreciated
{
    public class LocalXYToDD : GH_Component
    {
        public LocalXYToDD()
          : base(
              "Local XY to Decimal Degrees",
              "LocalXYToDD",
              "Convert a point from the custom local meter transform back to WGS84 longitude/latitude using the stored origin.",
              "Ptarmigan",
              "GIS")
        {
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddPointParameter(
                "XY Point",
                "xy",
                "Point in the custom local meter coordinate system.",
                GH_ParamAccess.item);

            pManager.AddNumberParameter(
                "Origin Longitude",
                "originLon",
                "Longitude of the local transform origin in decimal degrees.",
                GH_ParamAccess.item);

            pManager.AddNumberParameter(
                "Origin Latitude",
                "originLat",
                "Latitude of the local transform origin in decimal degrees.",
                GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            pManager.AddNumberParameter(
                "Latitude",
                "LAT",
                "Decimal degree latitude.",
                GH_ParamAccess.item);

            pManager.AddNumberParameter(
                "Longitude",
                "LON",
                "Decimal degree longitude.",
                GH_ParamAccess.item);

            pManager.AddPointParameter(
                "LonLat Point",
                "lonlat",
                "Point where X = longitude and Y = latitude in EPSG:4326.",
                GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Point3d xy = Point3d.Unset;
            double originLon = 0.0;
            double originLat = 0.0;

            if (!DA.GetData(0, ref xy)) return;
            if (!DA.GetData(1, ref originLon)) return;
            if (!DA.GetData(2, ref originLat)) return;

            Point3d lonLat = LocalProjection.LocalToLonLat(
                xy.X,
                xy.Y,
                originLon,
                originLat);

            Message = "EPSG:4326";

            DA.SetData(0, lonLat.Y);
            DA.SetData(1, lonLat.X);
            DA.SetData(2, lonLat);
        }

        protected override System.Drawing.Bitmap Icon => null;

        public override Guid ComponentGuid
        {
            get { return new Guid("A2B933CE-9D92-4625-B446-0D1FC474E205"); }
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