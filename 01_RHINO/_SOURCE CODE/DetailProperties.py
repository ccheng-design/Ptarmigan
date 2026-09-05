

#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino




detailRef=rs.GetObject("Get Reference Detail to Copy From",filter=32768,preselect=True)






#Detail to a Guid
detail = sc.doc.Objects.FindId(detailRef)

#CONVERT TO RHINOVIEWPORT CLASS
viewport = detail.Viewport

#Getting Viewport Properties
X = viewport.CameraX
Y = viewport.CameraY
Z = viewport.CameraZ
location = viewport.CameraLocation
direction = viewport.CameraDirection
target = viewport.CameraTarget
scale = detail.DetailGeometry.PageToModelRatio
parallel = viewport.IsParallelProjection
lenslength = viewport.Camera35mmLensLength


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

#rs.AddPoint(target)

bb = detail.DetailGeometry.GetBoundingBox(True)

diagCorners = [
    bb.Max,
    bb.Min
]

#rs.AddPoints(diagCorners)
print(rs.Distance(diagCorners[0], diagCorners[1]))

print(bb.Max)
print(bb.Max.X)
print(bb.Min)
print(bb.Min.X)

print(bb.GetCorners())

width = bb.Max.X-bb.Min.X
length = bb.Max.Y-bb.Min.Y

print(width)
print(length)


center = bb.Center
rs.AddPoint(center)

print(center.X)
print(center.Y)

if (width>length):
    sc.doc.Objects.AddPoint(Rhino.Geometry.Point3d((center.X + 1),center.Y,0))
else:
    
    sc.doc.Objects.AddPoint(Rhino.Geometry.Point3d(center.X,(center.Y + 1),0))
