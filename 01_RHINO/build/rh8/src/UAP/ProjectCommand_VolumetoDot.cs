using System;

using Rhino;
using Rhino.Commands;

namespace RhinoCodePlatform.Rhino3D.Projects.Plugin
{
  [CommandStyle(Rhino.Commands.Style.ScriptRunner)]
  public class ProjectCommand_VolumetoDot : Command
  {
    public Guid CommandId { get; } = new Guid("2e8c81ae-dc9d-49a6-8bf9-263e3cb0912b");

    public ProjectCommand_VolumetoDot() { Instance = this; }

    public static ProjectCommand_VolumetoDot Instance { get; private set; }

    public override string EnglishName => "VolumetoDot";

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
