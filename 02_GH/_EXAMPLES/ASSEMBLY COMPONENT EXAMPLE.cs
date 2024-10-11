using Grasshopper;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyGrasshopperAssembly1
{
    public class MyGrasshopperAssemblyComponent2 : GH_Component
    {
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public MyGrasshopperAssemblyComponent2()
          : base("Convex Hull", "CH",
            "test convex",
            "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            //USE FOR INPUT WIRES
            pManager.AddPointParameter("Points", "Pts", "Points to calc", GH_ParamAccess.list);
            pManager.AddPointParameter('Points', "Pts","Points to calc", GH_ParamAccess.item,0);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            //USE FOR OUTPUT WIRES
            pManager.AddPointParameter("Hull Points", "Hull Pts", "All outer points", GH_ParamAccess.item);
            pManager.AddCurveParameter("Polyline", "PLine", "Polyline Drawn", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object can be used to retrieve data from input parameters and 
        /// to store data in output parameters.</param>
        protected override async void SolveInstance(IGH_DataAccess DA)
        {
            //USE FOR INPUT VALUE
            List<Point3d> Points= new List<Point3d>();
            DA.GetDataList(0, Points);
            DA.GetDataList(0, ref Points);

            //Sanity checks
            if (points==null||points.Count()==0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter Points failed to collect data");
                return;
}
            

            //USE FOR OUTPUT VALUE
            DA.SetData(0, Hull_Points);
            DA.SetData(1, polyline);

        }

        /// <summary>
        /// Provides an Icon for every component that will be visible in the User Interface.
        /// Icons need to be 24x24 pixels.
        /// You can add image files to your project resources and access them like this:
        /// return Resources.IconForThisComponent;
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("_Ptarmigan.Resources.convex_hull-24px.png");
                return new System.Drawing.Bitmap(stream);
            }
        }

        /// <summary>
        /// Each component must have a unique Guid to identify it. 
        /// It is vital this Guid doesn't change otherwise old ghx files 
        /// that use the old ID will partially fail during loading.
        /// </summary>
        public override Guid ComponentGuid => new Guid("92f50079-8f41-4988-ae8a-d360a9ff6d28");
    }
}