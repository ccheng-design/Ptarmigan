using System;
using System.Collections.Generic;
using Grasshopper.Kernel;
using Rhino.Geometry;

//external DLLS
using Sharp3DBinPacking;
using Sharp3DBinPacking.Internal;
using Sharp3DBinPacking.Algorithms;

namespace _Ptarmigan
{
    public class ThreeDNestingBox : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public ThreeDNestingBox()
          : base("3DNestingBox", "3DNest",
              "Nests Objects in 3D Box; Works well with convex objects",
              "Ptarmigan", "Utilities")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to nest: meshes and/or breps. Each item is packed by its world axis-aligned bounding box.", GH_ParamAccess.list);
            pManager.AddBoxParameter("Box", "B", "Container box to nest into.", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spacing", "S", "Spacing: minimum gap between items and to the container walls, in model units.", GH_ParamAccess.item, 0.1);
            pManager.AddBooleanParameter("Run", "R", "Set to True to Run Iteration Counts; Default is False", GH_ParamAccess.item, false);
            pManager.AddNumberParameter("Iteration", "Iter", "Number of Iterations to Run; Set to 0 to Run constantly", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Seed", "S", "Randomize the iterations", GH_ParamAccess.item, 1);
            pManager.AddNumberParameter("Mutation", "M", "Mutation(0 - 100): % noise applied to the packing order each attempt. 0 = pure optimization.", GH_ParamAccess.item, 0);
            pManager.AddNumberParameter("Population", "P", "packing attempts per solve tick.", GH_ParamAccess.item, 5);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
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
            get { return new Guid("CC39C2B2-D328-4121-8F64-89215453311F"); }
        }
    }
}