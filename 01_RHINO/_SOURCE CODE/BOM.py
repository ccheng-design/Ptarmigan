import rhinoscriptsyntax as rs




layername=rs.LayerName("BOM")
if not layername: print("Make BOM Layer")

#print(layername)  
layer=rs.ObjectsByLayer(layername,True)
#print(layer)

block_names = rs.BlockNames()
#print(block_names)

block_BOM=[]
counts={}

for obj in layer:
    if rs.IsBlockInstance(obj):
        block_names=rs.BlockInstanceName(obj)
        block_BOM.append(block_names)

for string in block_BOM:
    if string in counts:
        counts[string]+=1
    else:
        counts[string]=1

print(block_BOM)
print(counts)

txt=str(counts)

test=txt.split(":")
print(test)

