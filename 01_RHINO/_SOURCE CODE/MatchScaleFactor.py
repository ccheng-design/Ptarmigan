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


details=rs.GetObject("Get Obj")

instance=sc.doc.Objects.FindId(details)

subobj=instance.GetSubObjects()

valueText=[]

for o in subobj:
    geo = o.Geometry
    if isinstance(geo, Rhino.Geometry.TextEntity):
        valueText.append(geo.PlainText)
        print(o.Attributes.Name, geo.PlainText)

scaleFactor=valueText[2]

print(scaleFactor)


