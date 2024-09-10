import rhinoscriptsyntax as rs


name_to_layer=rs.GetObjects()
if not name_to_layer: print("None Selected")

#create list
names=[]

for obj in name_to_layer:
    a=rs.ObjectName(obj)
    names.append(a)
print(names)








