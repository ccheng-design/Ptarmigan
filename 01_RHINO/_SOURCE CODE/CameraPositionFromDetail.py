
#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

import sys


detailRef=rs.GetObject("Get Reference Detail to Copy From",filter=32768,preselect=True)
if detailRef is None:
    sys.exit()

detailChange=rs.GetObjects("Detail to Change", filter=32768, preselect=False)



detail = sc.doc.Objects.FindId(detailRef)

#CONVERT TO RHINOVIEWPORT CLASS
viewport = detail.Viewport

X = viewport.CameraX
Y = viewport.CameraY
Z = viewport.CameraZ
location = viewport.CameraLocation
direction = viewport.CameraDirection
target = viewport.CameraTarget
scale = detail.DetailGeometry.PageToModelRatio
parallel = viewport.IsParallelProjection


cam_plane = Rhino.Geometry.Plane(location, Z)

true_X = Rhino.Geometry.Vector3d(1,0,0)

rot_angle = Rhino.Geometry.Vector3d.VectorAngle(X,true_X,cam_plane)

print(360-(rot_angle*(180/math.pi)))

print("VectorX:", X)
print(Y)
print(Z)
print(direction)
print(parallel)
print("Target:", target)





for i in detailChange:
    detail = sc.doc.Objects.FindId(i)

    viewport = detail.Viewport

    if parallel is True:
        viewport.ChangeToParallelProjection(True)
    

    #viewport.SetCameraTarget(target, True)
    viewport.SetCameraLocations(target, location)
    

    

    detail.CommitChanges()
    detail.CommitViewportChanges()
    detail.CommitChanges()

sc.doc.Views.Redraw()

#SCALE FACTOR CHANGE
rs.DetailScale(i,1,scale)