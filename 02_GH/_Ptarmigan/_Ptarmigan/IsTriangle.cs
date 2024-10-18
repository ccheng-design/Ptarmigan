using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace _Ptarmigan
{
    public class IsTriangle : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public IsTriangle()
          : base("IsTriangle", "IsTri",
              "Gives pattern if polyline shape is a triangle",
              "Ptarmigan", "Curves")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("crv", "P", "Curves to test", GH_ParamAccess.item);

        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Pattern", "Pat", "Pattern determining if curve is triangular", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            //Sanity checks
            Curve crv = null;
            DA.GetData(0, ref crv);

            if (crv == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter crv failed to collect data");
                return;
            }

            Polyline polyline;
            if (!crv.TryGetPolyline(out polyline))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input Curve could not be converted to Polyline");
                return;
            }


            int segcount = polyline.SegmentCount;


            List<bool> IsTri = new List<bool> { };

            if (segcount == 3)
            {

                IsTri.Add(true);

            }
            else
            {

                IsTri.Add(false);
            }

            DataTree<bool> IsTri_tree = new DataTree<bool>();




            IsTri_tree.Flatten();


            IsTri_tree.AddRange(IsTri, new GH_Path(0)); // Adding data to the tree at path {0}

            // Output the DataTree
            DA.SetDataTree(0, IsTri_tree);

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
            get { return new Guid("8D441F14-B214-4881-B24A-E672814E555C"); }
        }
    }
}