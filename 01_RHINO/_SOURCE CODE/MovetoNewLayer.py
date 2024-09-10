
import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

#written by Clark Cheng
#email: ccheng@clarkcheng.design

#Select Rhino Objs and move to the newly made layer

name_layer_obj=rs.GetObjects(preselect=True)
if not name_layer_obj:
    print("None Selected")
    
else:
    layer = rs.StringBox("New layer name" )
    if layer: rs.AddLayer( layer )
    rs.ObjectLayer(name_layer_obj,layer)





