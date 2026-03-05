
#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino


detailRef=rs.GetObjects("Get Details",filter=32768,preselect=True)

#detailChange=rs.GetObjects("Detail to Change", filter=32768, preselect=False)
for i in detailRef:

    detail = sc.doc.Objects.FindId(i)

    #CONVERT TO RHINOVIEWPORT CLASS
    viewport = detail.Viewport

    X = viewport.CameraX
    Y = viewport.CameraY
    Z = viewport.CameraZ
    location = viewport.CameraLocation
    direction = viewport.CameraDirection

    cam_plane = Rhino.Geometry.Plane(location, Z)

    true_X = Rhino.Geometry.Vector3d(1,0,0)

    rot_angle = Rhino.Geometry.Vector3d.VectorAngle(X,true_X,cam_plane)

    print(360-(rot_angle*(180/math.pi)))

    print(X)
    print(Y)
    print(Z)
    print(direction)