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
            pManager.AddGeometryParameter("Geometry", "G", "Geometry to Nest", GH_ParamAccess.list);
            pManager.AddBoxParameter("Box", "B", "Box for Nesting", GH_ParamAccess.item);
            pManager.AddNumberParameter("Spacing", "S", "Spacing Between Parts", GH_ParamAccess.item, 0.1);
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