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
using System.Linq;
using System.IO;

namespace _Ptarmigan
{
    public class MT_BooleanDifference : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MT_BooleanDifference class.
        /// </summary>
        public MT_BooleanDifference():
          base("MT_BooleanDifference", "MT_BD",
              "Boolean Difference like in Rhino, but its multi-threaded. List management is a must.",
              "Ptarmigan", "Solids")
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


            // Declare GH_Structure of GH_Brep (not DataTree<Brep>)
            GH_Structure<GH_Brep> S = new GH_Structure<GH_Brep>();
            GH_Structure<GH_Brep> D = new GH_Structure<GH_Brep>();

            // Get data as GH_Structure<GH_Brep>
            if (!DA.GetDataTree(0, out S) || !DA.GetDataTree(1, out D))
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input data is null or empty.");
                return;
            }

            // Sanity check: check if we have any branches
            if (S.IsEmpty || D.IsEmpty || S.PathCount == 0 || D.PathCount == 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, "Input trees are empty.");
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
            Print($"S tree has {S.PathCount} branches.");
            Print($"D tree has {D.PathCount} branches.");

            // Initialize dictionaries to store the results
            var mainBrepsMT = new ConcurrentDictionary<GH_Path, Brep>();
            var badBrepsMT = new ConcurrentDictionary<GH_Path, List<Brep>>();

            // Get maximum number of threads to run concurrently
            var totalMaxConcurrency = System.Environment.ProcessorCount - 1;
            //this.Component.Message = totalMaxConcurrency + " threads";

            // Loop through each path in S
            Parallel.ForEach(S.Paths, new ParallelOptions { MaxDegreeOfParallelism = totalMaxConcurrency }, pth =>
            {
                // Get the breps in each branch
                List<GH_Brep> branchS = S.get_Branch(pth).Cast<GH_Brep>().ToList();
                List<GH_Brep> branchD = D.get_Branch(pth).Cast<GH_Brep>().ToList();





                if (branchS == null || branchS.Count == 0)
                {
                    Print($"Branch {pth} in S is null or empty.");
                    return;
                }

                if (branchD == null || branchD.Count == 0)
                {
                    Print($"Branch {pth} in D is null or empty.");
                    return;
                }

                // Prepare to collect boolean difference results
                var badBrep = new List<Brep>();
                var mainBrep = branchS[0].Value; // Get the Brep value
                var diffBreps = branchD.Select(gb => gb.Value).ToList(); // Get list of Brep values

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
                        mainBrep = breps[0]; // Take the first result of the boolean difference
                    }
                }

                Print($"Processed branch {pth}. Main Brep: {mainBrep}, Bad Breps: {badBrep.Count}");

                // Store results in concurrent dictionaries
                mainBrepsMT[pth] = mainBrep;
                badBrepsMT[pth] = badBrep;
            });

            // Convert dictionaries to GH_Structure for output
            GH_Structure<GH_Brep> mainBreps = new GH_Structure<GH_Brep>();
            GH_Structure<GH_Brep> badBreps = new GH_Structure<GH_Brep>();

            foreach (KeyValuePair<GH_Path, Brep> p in mainBrepsMT)
            {
                mainBreps.Append(new GH_Brep(p.Value), p.Key);
            }

            foreach (KeyValuePair<GH_Path, List<Brep>> b in badBrepsMT)
            {
                foreach (var brep in b.Value)
                {
                    badBreps.Append(new GH_Brep(brep), b.Key);
                }
            }

            Print($"Main Breps tree has {mainBreps.PathCount} branches.");
            Print($"Bad Breps tree has {badBreps.PathCount} branches.");

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
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                var assembly = System.Reflection.Assembly.GetExecutingAssembly();
                var stream=assembly.GetManifestResourceStream("_Ptarmigan.Resources.MT_BooleanDifference-24px.png");
                return new System.Drawing.Bitmap(stream);
            }
        }
        

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("2684AC8A-16F7-4E62-9B07-1970E9D8C655"); }
        }
    }
}
