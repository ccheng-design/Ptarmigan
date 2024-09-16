import rhinoscriptsyntax as rs
import Rhino


#Read from the Layer BOM
layername=rs.LayerName("BOM")
if not layername: print("Make BOM Layer")

#Get objects by layer name
layer=rs.ObjectsByLayer(layername,True)

#Create list and dictionary to hold keys and values
block_BOM=[]
counts={}

#for each object in the layer, test if its a block
for obj in layer:
    if rs.IsBlockInstance(obj):
        block_names=rs.BlockInstanceName(obj)
        block_BOM.append(block_names)

#for each string in block_BOM list
for string in block_BOM:
    if string in counts:
        counts[string]+=1
    else:
        counts[string]=1

#create lists for 
key_list=[]
values_list=[]

for string in counts:
    key_list.append(string)
    values_list.append(counts[string])

#debugging
print(key_list)
print(values_list)

pt=Rhino.Geometry.Point3d(0,0,0)

spacing=20
vector_y=Rhino.Geometry.Vector3d(0,-spacing,0)
vector_x=Rhino.Geometry.Vector3d(100,0,0)

number=pt+vector_x

#Add text
for items in key_list:
    pt=pt+vector_y

    rs.AddText(items,pt)

for i in values_list:
    number=number+vector_y

    rs.AddText(i,number)
