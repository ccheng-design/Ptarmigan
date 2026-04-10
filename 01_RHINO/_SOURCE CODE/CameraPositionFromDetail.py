
#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

import sys


def CameraPositionFromDetail():


    detailRef=rs.GetObject("Get Reference Detail to Copy From",filter=32768,preselect=True)
    if detailRef is None:
        return

    detailChange=rs.GetObjects("Detail to Change", filter=32768, preselect=False)
    if detailChange is None:
        return


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

    """
    print(360-(rot_angle*(180/math.pi)))

    print("VectorX:", X)
    print(Y)
    print(Z)
    print(direction)
    print(parallel)
    print("Target:", target)

    """





    for i in detailChange:
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

if __name__=="__main__":
    CameraPositionFromDetail()