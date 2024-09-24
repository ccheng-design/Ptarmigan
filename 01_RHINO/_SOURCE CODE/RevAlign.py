import rhinoscriptsyntax as rs
import scriptcontext as sc
import Rhino

   
msg="Select a surface or Brep face to offset"
srf_filter=Rhino.DocObjects.ObjectType.Surface 
rc,objref=Rhino.Input.RhinoGet.GetOneObject(msg,False,srf_filter)
if rc==Rhino.Commands.Result.Success:
    #offset=rs.GetReal("Offset value")
    
    face=objref.Face()
    subSrf=face.ToNurbsSurface()

    oSrf=subSrf.Offset(0,sc.doc.ModelAbsoluteTolerance)
    a=sc.doc.Objects.AddSurface(oSrf)
    sc.doc.Views.Redraw()

    #print(a)

    centroid=rs.SurfaceAreaCentroid(a)
    print(centroid[0])
else:
    print("None Selected")
