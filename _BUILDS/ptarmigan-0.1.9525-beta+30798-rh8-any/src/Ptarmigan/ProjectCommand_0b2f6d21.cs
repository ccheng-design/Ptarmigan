using System;

using Rhino;
using Rhino.Commands;

namespace RhinoCodePlatform.Rhino3D.Projects.Plugin
{
  [CommandStyle(Rhino.Commands.Style.ScriptRunner)]
  public class ProjectCommand_0b2f6d21 : Command
  {
    public Guid CommandId { get; } = new Guid("0b2f6d21-58e2-4fe0-bc6f-e2625cadf5a6");

    public ProjectCommand_0b2f6d21() { Instance = this; }

    public static ProjectCommand_0b2f6d21 Instance { get; private set; }

    public override string EnglishName => "Ptarmigan";

    protected override string CommandContextHelpUrl => "";

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
