using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Rhino;
using Rhino.Geometry;
using System.Collections;
using Grasshopper;
using Grasshopper.Kernel.Types;
using System.Threading.Tasks;

namespace _Ptarmigan
{
    public class MT_BooleanDifference : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MT_BooleanDifference class.
        /// </summary>
        public MT_BooleanDifference():
          base("MT_BooleanDifference", "MT_BD",
              "Description",
              "Category", "Subcategory")
        {
        }

        #region Utility functions
        private void Print(string text) { __out.Add(text); }
        private void Print(string format, params object[] args) { __out.Add(string.Format(format, args)); }
        private void Reflect(object obj) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj)); }
        private void Reflect(object obj, string method_name) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name)); }
        #endregion

        #region Members
        private RhinoDoc RhinoDocument;
        private GH_Document GrasshopperDocument;
        private IGH_Component Component;
        private int Iteration;
        #endregion

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddBrepParameter("Solid", "S", "First set of solids", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Difference", "D", "Second set of solids", GH_ParamAccess.tree);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddBrepParameter("Result", "B", "Resulting Boolean", GH_ParamAccess.tree);
            pManager.AddBrepParameter("Remainder", "Re", "Booleans that didn't work", GH_ParamAccess.tree);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            DataTree<Brep> S = new DataTree<Brep>();
            DataTree<Brep> D = new DataTree<Brep>();

            DA.GetData(0, ref S);
            DA.GetData(1, ref D);

            if (S == null || D == null || S.BranchCount == 0 || D.BranchCount == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input data is null or empty.");
                return;
            }

            if (RhinoDocument == null)
            {
                Print("Rhino document is not initialized. Using a default tolerance value.");
                RhinoDocument = Rhino.RhinoDoc.ActiveDoc;
                if (RhinoDocument == null)
                {
                    Print("No active Rhino document found. Using default tolerance of 0.01.");
                }
            }

            double tolerance = RhinoDocument != null ? RhinoDocument.ModelAbsoluteTolerance : 0.01;

            Print($"Using tolerance: {tolerance}");
            Print($"S tree has {S.BranchCount} branches.");
            Print($"D tree has {D.BranchCount} branches.");

            var mainBrepsMT = new ConcurrentDictionary<GH_Path, Brep>();
            var badBrepsMT = new ConcurrentDictionary<GH_Path, List<Brep>>();

            var totalMaxConcurrency = System.Environment.ProcessorCount - 1;
            this.Component.Message = totalMaxConcurrency + " threads";

            Parallel.ForEach(S.Paths, new ParallelOptions { MaxDegreeOfParallelism = totalMaxConcurrency }, pth =>
            {
                if (S.Branch(pth) == null || S.Branch(pth).Count == 0)
                {
                    Print($"Branch {pth} in S is null or empty.");
                    return;
                }

                if (D.Branch(pth) == null || D.Branch(pth).Count == 0)
                {
                    Print($"Branch {pth} in D is null or empty.");
                    return;
                }

                var badBrep = new List<Brep>();
                var mainBrep = S.Branch(pth)[0]; // Main brep for this branch
                var diffBreps = D.Branch(pth);   // Cutters for this branch

                foreach (Brep b in diffBreps)
                {
                    if (b == null) continue; // Skip null breps
                    var breps = Brep.CreateBooleanDifference(mainBrep, b, tolerance);
                    if (breps == null || breps.Length < 1)
                    {
                        badBrep.Add(b);
                    }
                    else
                    {
                        mainBrep = breps[0];
                    }
                }

                Print($"Processed branch {pth}. Main Brep: {mainBrep}, Bad Breps: {badBrep.Count}");

                mainBrepsMT[pth] = mainBrep;
                badBrepsMT[pth] = badBrep;
            });

            var mainBreps = new DataTree<Brep>();
            var badBreps = new DataTree<Brep>();

            foreach (KeyValuePair<GH_Path, Brep> p in mainBrepsMT)
            {
                mainBreps.Add(p.Value, p.Key);
            }

            foreach (KeyValuePair<GH_Path, List<Brep>> b in badBrepsMT)
            {
                badBreps.AddRange(b.Value, b.Key);
            }

            Print($"Main Breps tree has {mainBreps.BranchCount} branches.");
            Print($"Bad Breps tree has {badBreps.BranchCount} branches.");

            // Assign the output
            DA.SetDataTree(0, mainBreps);
            DA.SetDataTree(1, badBreps);
        }

        private List<string> __err = new List<string>();
        private List<string> __out = new List<string>();
        private RhinoDoc doc = Instances.ActiveRhinoDoc;
        private IGH_ActiveObject owner;
        private int runCount;

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon => null;
        

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2684AC8A-16F7-4E62-9B07-1970E9D8C655"); }
        }
    }
}
