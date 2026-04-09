import Rhino
import rhinoscriptsyntax as rs
import scriptcontext as sc

#SCRIPT WRITTEN BY PASCAL GOLAY


def PlaneBasedBBox(geos,plane):
    xform = Rhino.Geometry.Transform.ChangeBasis(Rhino.Geometry.Plane.WorldXY, plane)
    bb = Rhino.Geometry.BoundingBox.Empty
    for geo in geos:
        objectbbox = geo.GetBoundingBox(xform)
        if objectbbox:
            bb = Rhino.Geometry.BoundingBox.Union(bb,objectbbox)
     
    plane_to_world = Rhino.Geometry.Transform.ChangeBasis(plane, Rhino.Geometry.Plane.WorldXY)
    bBox = Rhino.Geometry.Box(bb)
    bBox.Transform(plane_to_world)
    return bBox
    
def ScaleByIncrement():
    """

    Increment/Absolute option
    
    """
    id = rs.GetObject("Select object to scale", preselect=True)
    if not id:
        return
    geo = rs.coercegeometry(id)
    obj = rs.coercerhinoobject(id)
    pts = []
    
    while len(pts) < 2:
        
        mode = True
        if "SCALE_MODE" in sc.sticky:
            mode = sc.sticky["SCALE_MODE"]
        
        increment = 1
        if "SCALE_INCREMENT" in sc.sticky:
            increment = sc.sticky["SCALE_INCREMENT"]
        
        dimension = False
        if "SCALE_DIMENSION" in sc.sticky:
            dimension = sc.sticky["SCALE_DIMENSION"]
            
        length = 1
        if "SCALE_LENGTH" in sc.sticky:
            length = sc.sticky["SCALE_LENGTH"]
            
        gp = Rhino.Input.Custom.GetPoint()
        if len(pts) == 0:
            prompt = "Set base point for scale"
        else:
            prompt = "Set scale direction"
            gp.SetBasePoint(pts[0], True)
            gp.DrawLineFromPoint(pts[0], True)
            
        gp.SetCommandPrompt(prompt)
        
        opMode = Rhino.Input.Custom.OptionToggle(mode, "Absolute", "Increment")
        gp.AddOptionToggle("ScaleMode", opMode)
        
        if mode:
            opIncrement = Rhino.Input.Custom.OptionDouble(increment)
            gp.AddOptionDouble("Increment", opIncrement)
        else:
            opLength = Rhino.Input.Custom.OptionDouble(length)
            gp.AddOptionDouble("Length", opLength)
            
        opDimension = Rhino.Input.Custom.OptionToggle(dimension, "1D", "3D")
        gp.AddOptionToggle("ScaleDimension", opDimension)
        
        gp_rc = gp.Get()
        
        if( gp.CommandResult() != Rhino.Commands.Result.Success ):
            return
        if gp_rc == Rhino.Input.GetResult.Point:
            pts.append(gp.Point())

        if gp_rc == Rhino.Input.GetResult.Option:
            if mode:
                increment = opIncrement.CurrentValue
                sc.sticky["SCALE_INCREMENT"] = increment
            else:
                length = opLength.CurrentValue
                sc.sticky["SCALE_LENGTH"] = length
                
            dimension = opDimension.CurrentValue
            sc.sticky["SCALE_DIMENSION"] = dimension
            
            mode = opMode.CurrentValue
            sc.sticky["SCALE_MODE"] = mode
            
            continue
            
    vecDir = pts[0]-pts[1]
    myPlane = Rhino.Geometry.Plane(pts[0], vecDir)
    box = PlaneBasedBBox([geo],myPlane)
    corners = box.GetCorners()
    topPlane = box.Plane
    topPlane.Origin = corners[4]
    
    d = corners[0].DistanceTo(corners[4])
    
    if mode:
        factor = (d + increment)/d
    else:
        factor = length/d
        
    if dimension:
        xform = Rhino.Geometry.Transform.Scale(box.Plane,factor,factor,factor)
    else:
        xform = Rhino.Geometry.Transform.Scale(box.Plane,1,1,factor)
        
    sc.doc.Objects.Transform(id,xform, True)
    

    
        
if __name__ == '__main__':
    ScaleByIncrement()