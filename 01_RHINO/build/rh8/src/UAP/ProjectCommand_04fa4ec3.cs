using System;

using Rhino;
using Rhino.Commands;

namespace RhinoCodePlatform.Rhino3D.Projects.Plugin
{
  [CommandStyle(Rhino.Commands.Style.ScriptRunner)]
  public class ProjectCommand_04fa4ec3 : Command
  {
    public Guid CommandId { get; } = new Guid("04fa4ec3-bd3d-4e25-a5a5-70ed98dcead4");

    public ProjectCommand_04fa4ec3() { Instance = this; }

    public static ProjectCommand_04fa4ec3 Instance { get; private set; }

    public override string EnglishName => "NametoLayers";

    protected override Rhino.Commands.Result RunCommand(RhinoDoc doc, RunMode mode)
    {
      // NOTE:
      // Initialize() attempts to loads the core rhinocode plugin
      // and prepare the scripting platform. This call can not be in any static
      // ctors of Command or Plugin classes since plugins can not be loaded while
      // rhino is loading this plugin. The call has an initialized check and is
      // very fast after the first run.
      ProjectPlugin.Initialize();

      return ProjectPlugin.RunCode(this, CommandId, doc, mode);
    }
  }
}
