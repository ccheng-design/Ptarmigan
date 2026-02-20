using System;
using System.Reflection;
using System.Resources;

using Rhino;
using Rhino.Commands;
using Rhino.PlugIns;

namespace RhinoCodePlatform.Rhino3D.Projects.Plugin
{
  public class ProjectPlugin : PlugIn
  {
    static readonly Guid s_projectId = new Guid("6fc9394a-6461-41c5-999b-6ee471610e8b");

    static bool s_initialized = false;
    static dynamic s_projectServer = default;
    static object s_project = default;

    public static void Initialize()
    {
      if (s_initialized)
        return;

      s_projectServer = ProjectInterop.GetProjectServer();
      if (s_projectServer is null)
      {
        RhinoApp.WriteLine($"Error loading plugin. Missing Rhino3D platform");
        return;
      }

      // get project
      dynamic dctx = ProjectInterop.CreateInvokeContext();
      dctx.Inputs["projectAssembly"] = typeof(ProjectPlugin).Assembly;
      dctx.Inputs["projectId"] = s_projectId;
      dctx.Inputs["projectData"] = GetProjectData();

      object project = default;
      if (s_projectServer.TryInvoke("plugins/v1/deserialize", dctx)
            && dctx.Outputs.TryGet("project", out project))
      {
        // server reports errors
        s_project = project;
      }

      s_initialized = true;
    }

    public static ProjectPlugin Instance { get; private set; }

    public static Rhino.Commands.Result RunCode(Command command, Guid id, RhinoDoc doc, RunMode mode)
    {
      if (s_project is null)
      {
        RhinoApp.WriteLine($"Error running command {id}. Project deserializiation failed.");
        return Rhino.Commands.Result.Failure;
      }

      dynamic rctx = ProjectInterop.CreateInvokeContext();
      rctx.Inputs["command"] = command;
      rctx.Inputs["project"] = s_project;
      rctx.Inputs["projectId"] = id;
      rctx.Inputs["doc"] = doc;
      rctx.Inputs["runMode"] = mode;

      if (s_projectServer.TryInvoke("plugins/v1/run", rctx))
      {
        if (rctx.Outputs.TryGet("commandResult", out Rhino.Commands.Result result))
        {
          return result;
        }

        return Rhino.Commands.Result.Success;
      }

      // server reports error
      else
        return Rhino.Commands.Result.Failure;
    }

    public ProjectPlugin() { Instance = this; }

    static string GetProjectData()
    {
      var rm = new ResourceManager("Plugin.Data", Assembly.GetExecutingAssembly());
      return rm.GetString("PROJECT-DATA");
    }
  }
}
