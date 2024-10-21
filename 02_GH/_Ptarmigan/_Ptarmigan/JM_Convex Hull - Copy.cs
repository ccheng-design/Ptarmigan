using System;
using System.Collections.Generic;
using Eto.Drawing;
using Grasshopper.Kernel;
using Rhino.Geometry;



using System.Linq;
using System.Collections;
using System.Drawing;
using Rhino;

using Grasshopper;

using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;


namespace _Ptarmigan
{
    public class TestComponent : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the JM_Convex Hull class.
        /// </summary>
        public TestComponent():
           base("JM Convex Hull", "JM CH",
              "Computes the convex hull using the Jarvis march algorithm.",
              "Ptarmigan", "Curves")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddPointParameter("Points", "pts", "Collection of Points.", GH_ParamAccess.list);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddPointParameter("Sorted Points", "sorted pts", "Ordered Points", GH_ParamAccess.list);
            pManager.AddCurveParameter("Polyline","pline","Outline of algorithm",GH_ParamAccess.item);

        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Point3d> Jarvis_pts = new List<Point3d>();
            List<Vector3d> largest_v = new List<Vector3d>();

            


            //Logic for master point only START
            // Assuming Points is a list or array of Point3d objects
            List<Point3d> points = new List<Point3d>();

            DA.GetDataList(0, points);

            //Sanity checks
            if (points==null||points.Count()==0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter Points failed to collect data");
                return;
            }

            // Sort points by their X values
            points.Sort((p1, p2) => p1.X.CompareTo(p2.X));

            // Isolate the first point in the sorted list
            Point3d firstPoint = points[0];  // The first point

            // Vector representing the X-axis
            Vector3d xAxis = new Vector3d(1, 0, 0);

            // Variables to keep track of the largest angle and the corresponding point
            double largestAngle = 0;
            Point3d pointWithLargestAngle = firstPoint;

            // Calculate the angles with respect to the X-axis
            foreach (Point3d point in points)
            {
                // Create a vector from firstPoint to the current point
                Vector3d diff = new Vector3d(point.X - firstPoint.X, point.Y - firstPoint.Y, point.Z - firstPoint.Z);

                // Calculate the angle between the vector and the X-axis
                double angle = Vector3d.VectorAngle(diff, xAxis);

                // Check if this is the largest angle
                if (angle > largestAngle)
                {
                    largestAngle = angle;
                    pointWithLargestAngle = point;  // Store the point with the largest angle
                }
            }

            // Output the point with the largest angle
            Jarvis_pts.Add(firstPoint);
            Jarvis_pts.Add(pointWithLargestAngle);

            double f_x = firstPoint.X;
            double f_y = firstPoint.Y;
            double f_z = firstPoint.Z;

            double end_value = f_x + f_y + f_z;




            Vector3d initial_vector = new Vector3d(pointWithLargestAngle.X - firstPoint.X, pointWithLargestAngle.Y - firstPoint.Y, pointWithLargestAngle.Z - firstPoint.Z);

            largest_v.Add(initial_vector);


            //Logic for master point only END



            int iterationcount = 0;
            int maxiterations = 20;


            //Create new vectors to measure

            List<Vector3d> new_v = new List<Vector3d>();


            //Declare a list to store angle values
            List<double> measured_angles = new List<double>();

            //List of max angle values
            List<double> max_angles = new List<double>();


            bool going = true;





            //while loop
            while (iterationcount < maxiterations && going)
            {
                iterationcount++;

                //Index of list
                int index_reverse = largest_v.Count - 1;


                //isolate what needs to be reversed
                Vector3d to_be_reversed = largest_v[index_reverse];

                //reverse vector
                Vector3d reversed_v = to_be_reversed * -1;




                //Index for point list
                int index_pt = Jarvis_pts.Count - 1;


                //Isolate point for angle measurement
                Point3d angle_meas = Jarvis_pts[index_pt];





                foreach (Point3d point in points)
                {
                    Vector3d vector = point - angle_meas;
                    double angle = Vector3d.VectorAngle(reversed_v, vector);
                    measured_angles.Add(angle);
                    new_v.Add(vector);

                }

                //
                double maximum = measured_angles.Max();
                max_angles.Add(maximum);


                int pt_index = measured_angles.IndexOf(maximum);

                Vector3d new_max_angle = new_v[pt_index];
                largest_v.Add(new_max_angle);

                Point3d point_w_measured_angles = points[pt_index];

                //Add point to jarvis list
                Jarvis_pts.Add(point_w_measured_angles);

                //Check values
                //length of Jarvis list
                int Jarvis_length = Jarvis_pts.Count();


                Point3d newest_point = Jarvis_pts[Jarvis_length - 1];

                double n_x = newest_point.X;
                double n_y = newest_point.Y;
                double n_z = newest_point.Z;

                double check_value = n_x + n_y + n_z;

                if (check_value == end_value)
                {
                    going = false;
                }



                //clear lists
                measured_angles.Clear();
                new_v.Clear();
            }


            //Message box of how many points made it into the Wrapping
            int Jarvis_count = Jarvis_pts.Count();
            //this.Component.Message = Jarvis_count.ToString() + " Point(s)";
            this.Message = Jarvis_count.ToString() + " Point(s)";


            //Output

            //Hull_Points = Jarvis_pts;

            //Create polyline
            Polyline polyline = null;
            if(Jarvis_pts.Count()>1)
            {
                polyline = new Polyline(Jarvis_pts);
                
            }
            
            
            //Polyline = polyline;

            DA.SetDataList(0, Jarvis_pts);
            DA.SetData(1, polyline);
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
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("_Ptarmigan.Resources.convex_hull-24px.png");
                return new System.Drawing.Bitmap(stream);
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("CF279B1E-E00B-4B9A-BD7B-182233EA7A87"); }
        }
    }
}