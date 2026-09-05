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

def CopyViewFromDetail(sourceDetail, detailCopy):




    #Detail to a Guid
    detail = sc.doc.Objects.FindId(sourceDetail)

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

    """
    print(360-(rot_angle*(180/math.pi)))

    print("VectorX:", X)
    print(Y)
    print(Z)
    print(direction)
    print(parallel)
    print("Target:", target)

    """





    for i in detailCopy:
        detail = sc.doc.Objects.FindId(i)

        viewport = detail.Viewport

        if parallel is True:
            viewport.ChangeToParallelProjection(True)
        else:
            viewport.ChangeToPerspectiveProjection(True,lenslength)
        

        #viewport.SetCameraTarget(target, True)
        viewport.SetCameraLocations(target, location)
        

        

        detail.CommitChanges()
        detail.CommitViewportChanges()
        detail.CommitChanges()

    sc.doc.Views.Redraw()
    print("Detail View Successfully Synced.")

    #SCALE FACTOR CHANGE
    rs.DetailScale(i,1,scale)

detailGeo = rs.GetObject("Detail",filter=32768,preselect=True)
#Create Point
#refpt = rs.GetPoint("Select Point")
detail = sc.doc.Objects.FindId(detailGeo)
bb = detail.DetailGeometry.GetBoundingBox(True)

print(bb)



diagCorner = [
    bb.Min,
    bb.Max
]


rs.AddPoints(diagCorner)

#Make Line
start = rs.GetPoint("Start of Line")

gp = Rhino.Input.Custom.GetPoint()
gp.SetBasePoint(start, True)
endpt = gp.DrawLineFromPoint(start, True)
gp.Get()

breakline = rs.AddLine(start,gp.Point())

#Copy the Detail
page_view = sc.doc.Views.ActiveView

view = sc.doc.Views.ActiveView



#CopyViewFromDetail(detailGeo, test)


#Project Point
tMin = rs.CurveClosestPoint(breakline,bb.Min)
rs.AddPoint(rs.EvaluateCurve(breakline, tMin))


print(rs.Distance(bb.Min,rs.AddPoint(rs.EvaluateCurve(breakline, tMin))))

tMax = rs.CurveClosestPoint(breakline,bb.Max)
rs.AddPoint(rs.EvaluateCurve(breakline, tMax))

print(rs.Distance(bb.Max,rs.AddPoint(rs.EvaluateCurve(breakline, tMax))))

tCen = rs.CurveClosestPoint(breakline,bb.Center)
rs.AddPoint(rs.EvaluateCurve(breakline,tCen))


splitDetails = []
rightDetail = rs.AddDetail(view.MainViewport.Id, rs.EvaluateCurve(breakline, tMin), bb.Max)
leftDetail = rs.AddDetail(view.MainViewport.Id, rs.EvaluateCurve(breakline, tMax), bb.Min)

splitDetails.append(rightDetail)
splitDetails.append(leftDetail)

for i in splitDetails:
    detail = sc.doc.Objects.FindId(i)
    viewport = detail.Viewport

    location = viewport.CameraLocation
    target = viewport.CameraTarget

    newLocation = Rhino.Geometry.Point3d((location.X + 8), location.Y, location.Z)

    viewport.SetCameraLocation(newLocation, True)

    detail.CommitChanges()
    detail.CommitViewportChanges()


