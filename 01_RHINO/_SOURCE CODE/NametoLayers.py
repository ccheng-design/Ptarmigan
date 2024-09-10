import rhinoscriptsyntax as rs
import random
from System.Drawing import Color


#written by Clark Cheng
#email: ccheng@clarkcheng.design

#Select Rhino Objs and move to the newly made layer based on the obj names
#Random colors


name_to_layer=rs.GetObjects()
if not name_to_layer: print("None Selected")

#create list
names=[]

for obj in name_to_layer:
    a=rs.ObjectName(obj)

    if a==None:
        #Creates red layer to store no named objs
        rs.AddLayer("None", color=Color.Red)
        rs.ObjectLayer(obj, "None")
    else:



        #random color for layers
        randcolor=random.randint(0,255),random.randint(0,255),random.randint(0,255)
        #print(randcolor)

        #add layering names
        rs.AddLayer(a,color=(randcolor))

        rs.ObjectLayer(obj,a)
        names.append(a)
print(names)
