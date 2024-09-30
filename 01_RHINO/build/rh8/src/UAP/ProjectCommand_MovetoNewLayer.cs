using System;

using Rhino;
using Rhino.Commands;

namespace RhinoCodePlatform.Rhino3D.Projects.Plugin
{
  [CommandStyle(Rhino.Commands.Style.ScriptRunner)]
  public class ProjectCommand_MovetoNewLayer : Command
  {
    public Guid CommandId { get; } = new Guid("74fab7fb-bb5e-4282-9ecf-082febb2ead2");

    public ProjectCommand_MovetoNewLayer() { Instance = this; }

    public static ProjectCommand_MovetoNewLayer Instance { get; private set; }

    public override string EnglishName => "MovetoNewLayer";

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
