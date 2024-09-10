
import rhinoscriptsyntax as rs
from System.Drawing import Color


#written by Clark Cheng
#email: ccheng@clarkcheng.design


#Surface Volume
#Gets the volume of each object and adds a text dot

#filter goemetry types
geo_types=rs.filter.polysurface | rs.filter.surface

volume=rs.GetObjects("Select PolySrf",preselect=True, filter=geo_types)
if not volume: print("None Selected")

else:

    #creating list of volumes and open_srfs
    volume_list=[]
    open_polysrf_list=[]

    for obj in volume:
        #measure volume
        a=rs.SurfaceVolume(obj)

        #mark objects red for all open polysrfs
        if a==None:
            bad=rs.ObjectColor(obj,Color.Red)
            open_polysrf_list.append(bad)
            
        
        else:
            #centroid as a guid
            centroid=rs.SurfaceVolumeCentroid(obj)

            #add to list for counting
            volume_list.append(a[0])


            #define string; can also use a f-string as well
            txt_dot_string="Volume: "+str(round(a[0],3))+" cubic "+str(rs.UnitSystemName(False,False,False))
            

            #string vol_info="Volume", round(a[0],3)
            rs.AddTextDot(txt_dot_string,centroid[0])


    #Output stats for what worked/didnt work
    print(len(volume_list), "objects were successfully calculated.", len(open_polysrf_list), "Are Marked in Red")
    print("Objects in Red are Not Closed Polysurfaces")
