
import rhinoscriptsyntax as rs
from System.Drawing import Color


#Surface Volume
#Gets the volume of each object and adds a text dot


volume=rs.GetObjects()
if not volume: print("None Selected")

volume_list=[]
open_polysrf_list=[]

for obj in volume:
    #measure volume
    a=rs.SurfaceVolume(obj)
    if a==None:
        bad=rs.ObjectColor(obj,Color.Red)
        open_polysrf_list.append(bad)
        
    else:
        #centroid as a guid
        centroid=rs.SurfaceVolumeCentroid(obj)


        #print(a[0])
        
        
        volume_list.append(a[0])

        txt_dot_string="Volume: "+str(round(a[0],3))+" cubic "+str(rs.UnitSystemName(False,False,False))
        print(txt_dot_string)

        #string vol_info="Volume", round(a[0],3)
        rs.AddTextDot(txt_dot_string,centroid[0])
print(len(volume_list), "objects were successfully calculated.", len(open_polysrf_list), "Are Marked in Red")
print("Objects in Red are Not Closed Polysurfaces")
#print(len(open_polysrf_list))
    







