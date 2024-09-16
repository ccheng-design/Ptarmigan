import rhinoscriptsyntax as rs




layername=rs.LayerName("BOM")
if not layername: print("Make BOM Layer")

#print(layername)  
layer=rs.ObjectsByLayer(layername,True)
#print(layer)

block_names = rs.BlockNames()
#print(block_names)

block_BOM=[]
test1=[]

for obj in layer:
    if rs.IsBlockInstance(obj):
        block_names=rs.BlockInstanceName(obj)
        block_BOM.append(block_names)



unique=set(block_BOM)
test1.append(unique)
        
test=block_BOM.count(test1[0])

print(block_BOM)
print(test1)
print(test)