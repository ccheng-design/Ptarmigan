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



#Get detail from viewport
ids=rs.GetObjects("Select Detail", rs.filter.detail,preselect=True)

#String box to type display name
displayModeName = rs.StringBox("Display Mode Name","Shaded","Display Mode")

for obj in ids:

    detail_id=ids[0]

    detail=sc.doc.Objects.FindId(detail_id)

    vp=detail.Viewport



    mode=Rhino.Display.DisplayModeDescription.FindByName(displayModeName)

    detail.Viewport.DisplayMode = mode

    #Commit Changes
    detail.CommitViewportChanges()

    page_view = detail.Viewport.ParentView




