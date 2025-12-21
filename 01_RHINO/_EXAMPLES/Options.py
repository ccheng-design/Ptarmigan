"""
NOTE:

- Reference to RhinoCommmon.dll is added by default

- You can specify your script requirements like:

    # r: <package-specifier> [, <package-specifier>]
    # requirements: <package-specifier> [, <package-specifier>]

    For example this line will ask the runtime to install
    the listed packages before running the script:

    # requirements: pytoml, keras

    You can install specific versions of a package
    using pip-like package specifiers:

    # r: pytoml==0.10.2, keras>=2.6.0

- Use env directive to add an environment path to sys.path automatically
    # env: /path/to/your/site-packages/
"""
#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

OPTIONS = {
    "Diagonal":  "DIAG",
    "3Point":    "3PT",
    "Vertical":  "VERT",
    "Center":    "CENTER"
}

def choose_option():
    go = Rhino.Input.Custom.GetOption()
    go.SetCommandPrompt("Box mode")

    option_ids = {}
    for name in OPTIONS:
        option_ids[name] = go.AddOption(name)

    while True:
        r = go.Get()

        if r == Rhino.Input.GetResult.Option:
            for name, idx in option_ids.items():
                if go.OptionIndex() == idx:
                    return OPTIONS[name]

        if r == Rhino.Input.GetResult.Cancel:
            return None

mode = choose_option()

if mode == "DIAG":
    pass
elif mode == "3PT":
    pass
elif mode == "VERT":
    pass
elif mode == "CENTER":
    pass
