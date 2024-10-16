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
    public class MyComponent1 : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MyComponent1()
          : base("MyComponent1", "Nickname",
              "Description",
              "Category", "Subcategory")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddCurveParameter("Polygon", "P", "Curves to test.", GH_ParamAccess.item);
            
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBooleanParameter("Pattern", "Pat", "Pattern determining if curve is quadrilateral", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Polyline Polygon = null;

            DA.GetData(0, ref Polygon);

            //Sanity checks
            if (Polygon == null)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input parameter Polygon failed to collect data");
                return;
            }


            int segcount = Polygon.SegmentCount;


            List<bool> IsQuad = new List<bool> { };

            if (segcount == 4)
            {

                IsQuad.Add(true);

            }
            else
            {

                IsQuad.Add(false);
            }

            DataTree<bool> IsQuad_tree = new DataTree<bool>(IsQuad);
            IsQuad_tree.Flatten();
            bool a = IsQuad_tree.Branch(0)[0];
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
            get { return new Guid("794BEC0A-409F-4972-B593-B850F89FEB09"); }
        }
    }
}