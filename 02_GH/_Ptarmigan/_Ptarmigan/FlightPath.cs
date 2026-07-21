using Grasshopper.Kernel;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Input.Custom;
using Rhino.Render.ChangeQueue;
using System;
using System.Collections.Generic;

namespace _Ptarmigan
{
    public class FlightPath : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public FlightPath()
          : base("FlightPath", "FlightP",
              "Creates flight path based on points and parameters",
              "Ptarmigan", "Drone")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddTextParameter("MissionConfig", "MC", "Mission Configuration text; Use the Mission Configuration Component!", GH_ParamAccess.item);
            pManager.AddTextParameter("ExecuteHeightMode", "EHM", "Determines the Height Mode", GH_ParamAccess.item, "relativeToStartPoint");
            pManager.AddCurveParameter("FlightPath","FP","Flight Path as a Curve",GH_ParamAccess.item);
            pManager.AddNumberParameter("Height", "H", "Height of the flight path", GH_ParamAccess.item, 40);
            pManager.AddNumberParameter("GimbalPitchRotateAngle", "GPRA", "Pitch rotation angle of the gimball", GH_ParamAccess.item, -20);
            pManager.AddPointParameter("PointofInterest", "POI", "Optional Point of Interest", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter(".wpml Flight Path", ".wpml", ".wpml file", GH_ParamAccess.item);
            pManager.AddVectorParameter("Flight Tangent Vectors", "V", "Flight Tangent Vectors", GH_ParamAccess.item);
            pManager.AddPointParameter("Fight Points", "P", "Points from the Flight Path", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            String missionconfig = null;
            DA.GetData(0, ref missionconfig);

            String ehm = null;
            DA.GetData(1, ref ehm);

            Curve flightpath = null;
            DA.GetData(2,ref flightpath);

            double height = 40;
            DA.GetData(3, ref height);

            double gpra = -20;
            DA.GetData(4, ref gpra);

            Point3d poi = Point3d.Unset;
            DA.GetData(5, ref poi);

            //Folder Start
            string folderStart = missionconfig + @"
        <Folder>
            <wpml:templateId>0</wpml:templateId>
            <wpml:executeHeightMode>relativeToStartPoint</wpml:executeHeightMode>
            <wpml:waylineId>0</wpml:waylineId>
            <wpml:distance>0</wpml:distance>
            <wpml:duration>0</wpml:duration>
            <wpml:autoFlightSpeed>2.5</wpml:autoFlightSpeed>";

            Point3d[] pts;
            double[] waypoints = flightpath.DivideByCount(10, true, out pts);

            List<double> X = new List<double>();
            List<double> Y = new List<double>();

            Point3d[] c = pts;
            DA.SetData(2, c);

            //Point Coordinates
            foreach (Point3d p in pts)
            {
                double XCoord = p.X;
                double YCoord = p.Y;

                X.Add(XCoord);
                Y.Add(YCoord);

            }

            //Tangent Vector
            List<double> angle = new List<double>();
            List<Plane> DroneOri = new List<Plane>();
            List<Vector3d> tangent = new List<Vector3d>();

            foreach (double i in waypoints)
            {
                Vector3d vector = flightpath.TangentAt(i);
                double flightAngle = Vector3d.VectorAngle(vector, Vector3d.YAxis, Plane.WorldXY);
                double flightAngleDegrees = Rhino.RhinoMath.ToDegrees(flightAngle);
                angle.Add(flightAngleDegrees);
                tangent.Add(vector);

                //Cross Product
                //Drone Orientation
                Vector3d perpV = Vector3d.CrossProduct(vector, Vector3d.ZAxis);
                Plane ori = new Plane(flightpath.PointAt(i), perpV, vector);
                DroneOri.Add(ori);
            }



            List<Vector3d> b = tangent;
            DA.SetData(1, b);

            //Point of Interest
            if (poi == Point3d.Unset)
            {
                poi = new Point3d(0.000000, 0.000000, 0.000000);
            }
            else
            {
                double poiX = poi.X;
                double poiY = poi.Y;
                double poiZ = poi.Z;

                Rhino.Geometry.Point3d POIpt = new Point3d(poiX, poiY, poiZ);

            }




            List<string> placeholder = new List<string>();
            for (int i = 0; i < X.Count; i++)
            {
                double x = X[i];
                double y = Y[i];
                double an = angle[i];


                string coordinates = $@"
            <Placemark>
                <Point>
                    <coordinates>
                        {x},{y}
                    </coordinates>
                </Point>
                <wpml:index>0</wpml:index>
                <wpml:executeHeight>{height}</wpml:executeHeight>
                <wpml:waypointSpeed>2.5</wpml:waypointSpeed>
                <wpml:waypointHeadingParam>
                    <wpml:waypointHeadingMode>smoothTransition</wpml:waypointHeadingMode>
                    <wpml:waypointHeadingAngle>{an}</wpml:waypointHeadingAngle>
                    <wpml:waypointPoiPoint>{poi}</wpml:waypointPoiPoint>
                    <wpml:waypointHeadingAngleEnable>1</wpml:waypointHeadingAngleEnable>
                    <wpml:waypointHeadingPathMode>followBadArc</wpml:waypointHeadingPathMode>
                    <wpml:waypointHeadingPoiIndex>0</wpml:waypointHeadingPoiIndex>
                </wpml:waypointHeadingParam>
                <wpml:waypointTurnParam>
                    <wpml:waypointTurnMode>toPointAndStopWithContinuityCurvature</wpml:waypointTurnMode>
                    <wpml:waypointTurnDampingDist>0</wpml:waypointTurnDampingDist>
                </wpml:waypointTurnParam>
                <wpml:useStraightLine>0</wpml:useStraightLine>
                <wpml:actionGroup>
                    <wpml:actionGroupId>1</wpml:actionGroupId>
                    <wpml:actionGroupStartIndex>0</wpml:actionGroupStartIndex>
                    <wpml:actionGroupEndIndex>0</wpml:actionGroupEndIndex>
                    <wpml:actionGroupMode>parallel</wpml:actionGroupMode>
                    <wpml:actionTrigger>
                        <wpml:actionTriggerType>reachPoint</wpml:actionTriggerType>
                    </wpml:actionTrigger>
                    <wpml:action>
                        <wpml:actionId>1</wpml:actionId>
                        <wpml:actionActuatorFunc>gimbalRotate</wpml:actionActuatorFunc>
                        <wpml:actionActuatorFuncParam>
                            <wpml:gimbalHeadingYawBase>aircraft</wpml:gimbalHeadingYawBase>
                            <wpml:gimbalRotateMode>absoluteAngle</wpml:gimbalRotateMode>
                            <wpml:gimbalPitchRotateEnable>1</wpml:gimbalPitchRotateEnable>
                            <wpml:gimbalPitchRotateAngle>{gpra}</wpml:gimbalPitchRotateAngle>
                            <wpml:gimbalRollRotateEnable>1</wpml:gimbalRollRotateEnable>
                            <wpml:gimbalRollRotateAngle>0</wpml:gimbalRollRotateAngle>
                            <wpml:gimbalYawRotateEnable>0</wpml:gimbalYawRotateEnable>
                            <wpml:gimbalYawRotateAngle>0</wpml:gimbalYawRotateAngle>
                            <wpml:gimbalRotateTimeEnable>0</wpml:gimbalRotateTimeEnable>
                            <wpml:gimbalRotateTime>0</wpml:gimbalRotateTime>
                            <wpml:payloadPositionIndex>0</wpml:payloadPositionIndex>
                        </wpml:actionActuatorFuncParam>
                    </wpml:action>
                    </wpml:actionGroup>
                <wpml:actionGroup>
                    <wpml:actionGroupId>2</wpml:actionGroupId>
                    <wpml:actionGroupStartIndex>0</wpml:actionGroupStartIndex>
                    <wpml:actionGroupEndIndex>1</wpml:actionGroupEndIndex>
                    <wpml:actionGroupMode>parallel</wpml:actionGroupMode>
                    <wpml:actionTrigger>
                        <wpml:actionTriggerType>reachPoint</wpml:actionTriggerType>
                    </wpml:actionTrigger>
                    <wpml:action>
                        <wpml:actionId>2</wpml:actionId>
                        <wpml:actionActuatorFunc>gimbalEvenlyRotate</wpml:actionActuatorFunc>
                        <wpml:actionActuatorFuncParam>
                        <wpml:gimbalPitchRotateAngle>-18.5</wpml:gimbalPitchRotateAngle>
                        <wpml:gimbalRollRotateAngle>0</wpml:gimbalRollRotateAngle>
                        <wpml:payloadPositionIndex>0</wpml:payloadPositionIndex>
                        </wpml:actionActuatorFuncParam>
                    </wpml:action>
                    </wpml:actionGroup>
                <wpml:waypointGimbalHeadingParam>
                    <wpml:waypointGimbalPitchAngle>0</wpml:waypointGimbalPitchAngle>
                    <wpml:waypointGimbalYawAngle>0</wpml:waypointGimbalYawAngle>
                </wpml:waypointGimbalHeadingParam>
            </Placemark>";

                placeholder.Add(coordinates);

            }

            //Folder End
            string folderEnd = @"
        </Folder>
    </Document>
</kml>";



            string a = folderStart + string.Join("", placeholder) + folderEnd;
            DA.SetData(0, a);

        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("BBEEECE9-BB92-4562-B219-997771495C02"); }
        }
    }
}