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

doc = Rhino.RhinoDoc.ActiveDoc
#rs.ZoomExtents(all=True)

pageViews = sc.doc.Views.GetPageViews()

for page in pageViews:
    doc.Views.ActiveView=page

    #origin
    origin = Rhino.Geometry.Point3d(0,0,0)
    #Vector
    x = Rhino.Geometry.Vector3d(1,0,0)
    y = Rhino.Geometry.Vector3d(0,1,0)


    #Creates rectangle
    plane = Rhino.Geometry.Plane(origin,x,y)

    Rhino.Geometry.Rectangle3d(plane,17,11)

    page.Redraw
