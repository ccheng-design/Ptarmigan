// this is an extra line that isn't in the CS_Edit_Template and was added because line numbers in compiler messages are zero based and editor line numbers are 1 based. This extra line makes error line numbers correspond to editor line numbers.
#region using
using System;
using System.Collections;
using System.Collections.Generic;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
#endregion
using System.IO;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Runtime.InteropServices;


using System.Threading.Tasks;
using System.Collections.Concurrent;
using Rhino.DocObjects;
using Rhino.Collections;
using GH_IO;
using GH_IO.Serialization;
#region class comment
/// <summary>
/// This class will be instantiated on demand by the Script component.
/// </summary>
#endregion
public class Script_Instance : GH_ScriptInstance
{
#region Utility functions
  /// <summary>Print a String to the [Out] Parameter of the Script component.</summary>
  /// <param name="text">String to print.</param>
  private void Print(string text) { __out.Add(text); }
  /// <summary>Print a formatted String to the [Out] Parameter of the Script component.</summary>
  /// <param name="format">String format.</param>
  /// <param name="args">Formatting parameters.</param>
  private void Print(string format, params object[] args) { __out.Add(string.Format(format, args)); }
  /// <summary>Print useful information about an object instance to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj)); }
  /// <summary>Print the signatures of all the overloads of a specific method to the [Out] Parameter of the Script component. </summary>
  /// <param name="obj">Object instance to parse.</param>
  private void Reflect(object obj, string method_name) { __out.Add(GH_ScriptComponentUtilities.ReflectType_CS(obj, method_name)); }
#endregion

#region Members
  /// <summary>Gets the current Rhino document.</summary>
  private RhinoDoc RhinoDocument;
  /// <summary>Gets the Grasshopper document that owns this script.</summary>
  private GH_Document GrasshopperDocument;
  /// <summary>Gets the Grasshopper script component that owns this script.</summary>
  private IGH_Component Component; 
  /// <summary>
  /// Gets the current iteration count. The first call to RunScript() is associated with Iteration==0.
  /// Any subsequent call within the same solution will increment the Iteration count.
  /// </summary>
  private int Iteration;
#endregion

#region method comment
  /// <summary>
  /// This procedure contains the user code. Input parameters are provided as regular arguments, 
  /// Output parameters as ref arguments. You don't have to assign output parameters, 
  /// they will have a default value.
  /// </summary>
#endregion
  private void RunScript(DataTree<Brep> S, DataTree<Brep> D, ref object R, ref object BB)
  {
        /////////////////////////////
    //written by Brian Washburn//
    //washburnarchitecture.com //
    //2018.06.06               //
    /////////////////////////////
    // Code cleanup, RIL 2020-10-17 (removed loooooong namespaces)

    var tolerance = doc.ModelAbsoluteTolerance;

    // Declare dictionaries that work in parallel to hold the successful boolean results and
    // the unsuccessful boolean cutters

    var mainBrepsMT = new ConcurrentDictionary<GH_Path, Brep>();
    var badBrepsMT = new ConcurrentDictionary<GH_Path,List<Brep>>();

    // Need to reserve a processor for GUI, otherwise we get a "Server Busy..." warning.
    // The booleaning still works, you would just have to click through the warning

    var totalMaxConcurrancy = System.Environment.ProcessorCount - 1;

    // Display us how many threads we're using

    this.Component.Message = totalMaxConcurrancy + " threads";


    // Start of the parallel engine

    Parallel.ForEach(S.Paths, new ParallelOptions {MaxDegreeOfParallelism = totalMaxConcurrancy}, pth =>
      {

      var badBrep = new List<Brep>();
      var goodBrep = new List<Brep>();
      var mainBrep = S.Branch(pth)[0];
      var diffBreps = D.Branch(pth);

      // Difference one cutter brep at a time from the main brep in the branch
      // this allows the boolean operation to continue without failing
      // and bad cutter breps can be discarded to a list that can be used for troubleshooting
      // haven't noticed a hit big hit on performance

      foreach (Brep b in diffBreps)
      {
        var breps = new Brep[]{};
        breps = Brep.CreateBooleanDifference(mainBrep, b, tolerance);
        if ((breps == null) || (breps.Length < 1))
        {
          badBrep.Add(b);
        }
        else
        {
          mainBrep = breps[0];
        }
      }
      mainBrepsMT[pth] = mainBrep;
      badBrepsMT[pth] = badBrep;
      });
    // End of the parallel engine

    // Convert dictionaries to regular old data trees

    var mainBreps = new DataTree<Brep>();
    var badBreps = new DataTree<Brep>();

    foreach(KeyValuePair<GH_Path,Brep> p in mainBrepsMT)
    {
      mainBreps.Add(p.Value, p.Key);
    }

    foreach(KeyValuePair<GH_Path, List<Brep>> b in badBrepsMT)
    {
      badBreps.AddRange(b.Value, b.Key);
    }

    // OUTPUT

    R = mainBreps;
    BB = badBreps;


  }

  // <Custom additional code> 
  
  // </Custom additional code> 

  private List<string> __err = new List<string>(); //Do not modify this list directly.
  private List<string> __out = new List<string>(); //Do not modify this list directly.
  private RhinoDoc doc = Instances.ActiveRhinoDoc;       //Legacy field.
  private IGH_ActiveObject owner;                  //Legacy field.
  private int runCount;                            //Legacy field.
  
  public override void InvokeRunScript(IGH_Component owner, object rhinoDocument, int iteration, List<object> inputs, IGH_DataAccess DA)
  {
    //Prepare for a new run...
    //1. Reset lists
    this.__out.Clear();
    this.__err.Clear();

    this.Component = owner;
    this.Iteration = iteration;
    this.GrasshopperDocument = owner.OnPingDocument();
    this.RhinoDocument = rhinoDocument as Rhino.RhinoDoc;

    this.owner = this.Component;
    this.runCount = this.Iteration;
    this. doc = this.RhinoDocument;

    //2. Assign input parameters
        DataTree<Brep> S = null;
    if (inputs[0] != null)
    {
      S = GH_DirtyCaster.CastToTree<Brep>(inputs[0]);
    }

    DataTree<Brep> D = null;
    if (inputs[1] != null)
    {
      D = GH_DirtyCaster.CastToTree<Brep>(inputs[1]);
    }



    //3. Declare output parameters
      object R = null;
  object BB = null;


    //4. Invoke RunScript
    RunScript(S, D, ref R, ref BB);
      
    try
    {
      //5. Assign output parameters to component...
            if (R != null)
      {
        if (GH_Format.TreatAsCollection(R))
        {
          IEnumerable __enum_R = (IEnumerable)(R);
          DA.SetDataList(1, __enum_R);
        }
        else
        {
          if (R is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(1, (Grasshopper.Kernel.Data.IGH_DataTree)(R));
          }
          else
          {
            //assign direct
            DA.SetData(1, R);
          }
        }
      }
      else
      {
        DA.SetData(1, null);
      }
      if (BB != null)
      {
        if (GH_Format.TreatAsCollection(BB))
        {
          IEnumerable __enum_BB = (IEnumerable)(BB);
          DA.SetDataList(2, __enum_BB);
        }
        else
        {
          if (BB is Grasshopper.Kernel.Data.IGH_DataTree)
          {
            //merge tree
            DA.SetDataTree(2, (Grasshopper.Kernel.Data.IGH_DataTree)(BB));
          }
          else
          {
            //assign direct
            DA.SetData(2, BB);
          }
        }
      }
      else
      {
        DA.SetData(2, null);
      }

    }
    catch (Exception ex)
    {
      this.__err.Add(string.Format("Script exception: {0}", ex.Message));
    }
    finally
    {
      //Add errors and messages... 
      if (owner.Params.Output.Count > 0)
      {
        if (owner.Params.Output[0] is Grasshopper.Kernel.Parameters.Param_String)
        {
          List<string> __errors_plus_messages = new List<string>();
          if (this.__err != null) { __errors_plus_messages.AddRange(this.__err); }
          if (this.__out != null) { __errors_plus_messages.AddRange(this.__out); }
          if (__errors_plus_messages.Count > 0) 
            DA.SetDataList(0, __errors_plus_messages);
        }
      }
    }
  }
}
