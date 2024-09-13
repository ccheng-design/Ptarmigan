import rhinoscriptsyntax as rs

#UnhideAll
#Shows all objects, including hidden layers and hidden objs

#Get layer names within document context
layers = rs.LayerNames()

layer_count=[]

#if any layers
if layers:
    #for each layer in layers
    for layer in layers:
        if rs.LayerVisible(layer)==False:
            rs.LayerVisible(layer,True)
            layer_count.append(rs.LayerVisible(layer))

objs = rs.AllObjects()
for obj in objs: rs.ShowObjects(obj)

print(len(layer_count),"Layers were hidden")