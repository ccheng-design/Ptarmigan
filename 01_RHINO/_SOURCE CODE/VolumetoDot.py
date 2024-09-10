
import rhinoscriptsyntax as rs

#Surface Volume
#Gets the volume of each object and adds a text dot


volume=rs.GetObjects()
if not volume: print("None Selected")

volume_list=[]
point_list=[]

for obj in volume:
    #measure volume
    a=rs.SurfaceVolume(obj)
    b=rs.SurfaceVolumeCentroid(obj)

    
    
    
    volume_list.append(a[0])
    point_list.append(b)
    
    
pts=rs.PointCoordinates(point_list)

#rs.AddTextDot(volume_list,point_list)
print(volume_list)
print(point_list)




