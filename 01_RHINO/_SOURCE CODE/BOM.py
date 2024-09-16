import rhinoscriptsyntax as rs
import Rhino

#BOM
#Creates bill of materials based on the blocks being in the BOM layer (user defined)


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

pt=rs.GetPoint("Pick Point")
#pt=Rhino.Geometry.Point3d(0,0,0)

spacing=1
vector_y=Rhino.Geometry.Vector3d(0,-spacing,0)
vector_x=Rhino.Geometry.Vector3d(1,0,0)

#define the x direction
number=pt+vector_x

vector_x.Unitize() #unitize
length=10
scaled_vector=vector_x*length

print(pt+scaled_vector)

#Add text
for items in key_list:
    pt+=vector_y
    rs.AddPoint(pt)
    #pt+scaled_vector
    
    rs.AddLine(pt,(pt+scaled_vector))

    rs.AddText(items,pt,height=0.1)

for i in values_list:
    number=number+vector_y

    rs.AddText(i,number,height=0.1)
