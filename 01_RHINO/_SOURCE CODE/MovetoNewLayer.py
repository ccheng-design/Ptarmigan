
import rhinoscriptsyntax as rs

#written by Clark Cheng
#email: ccheng@clarkcheng.design

#Select Rhino Objs and move to the newly made layer or an existing layer
#layer01::layer02

name_layer_obj=rs.GetObjects(preselect=True)
if not name_layer_obj:
    print("None Selected")
    
else:
    layer = rs.StringBox(message="New layer name. For child layers, format like layer01::layer02", default_value=None,title="Layer Name")

    #Add Layer
    if layer: rs.AddLayer( layer )

    #Move obj to newly named layer
    if layer:
        rs.ObjectLayer(name_layer_obj,layer)
        print("Moved",len(name_layer_obj), "objects to", layer)
    else:
        print("No Layer Name Given")
    