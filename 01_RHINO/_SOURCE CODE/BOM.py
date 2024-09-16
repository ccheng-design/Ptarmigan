import rhinoscriptsyntax as rs



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


print(key_list)
print(values_list)

for items in key_list:
    rs.AddText(items,(0,0,0))

for i in values_list:
    rs.AddText(i,(0,0,0))
