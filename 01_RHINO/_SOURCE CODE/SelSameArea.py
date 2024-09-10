import rhinoscriptsyntax as rs
import math as m

#written by Clark Cheng
#email: ccheng@clarkcheng.design

#Selects objects with the duplicate areas. 
#This includes all objects that have multiple area
#You have the option of changing between specific area and overall pieces

#filter geo
geo_types=rs.filter.polysurface | rs.filter.surface | rs.filter.curve


#Get geometry
area_obj=rs.GetObjects("Select surfaces, polysurfaces, hatches, closed planar curves or polygon meshes for area calculation",preselect=True)
if not area_obj:
    print("None Selected")



else:
    #create list to hold geo
    measured_obj=[]

    #for loop in each obj
    for obj in area_obj:

        #for each obj measure the area
        area=rs.Area(obj)
        area=round(area,3)
        
        #debugging
        #print(area)

        #debugging
        measured_obj.append(area)
    print(measured_obj)


    #all objects
    all_obj_opt=("All_Objects", "False", "True")
    all_obj=rs.GetBoolean("Boolean options" ,all_obj_opt, (False))
    #print(all_obj)


    
    if all_obj==[True]:

        occur={}
        for i in measured_obj:
            occur[i]=occur.get(i,0)+1

        mapped=[occur[i]>1 for i in measured_obj]

        #cull pattern using list comprehension
        same_area = [d for d, p in zip(area_obj, mapped) if p]
        print("OBJ",same_area)

        rs.SelectObjects(same_area)

        print(len(same_area), "objects has the same area")
    else:
        #specific objects
        specific_obj=rs.GetObject()
        if not specific_obj:
            print("None Selected")
        else:
            specific_area=round(rs.Area(specific_obj),3)

            print(specific_area)

            pattern=[]
            same_geo=[]
            

            for i in measured_obj:
                if i == specific_area:
                    pattern.append(True)
                    #pattern.append(area_obj)

                else:
                    pattern.append(False)
            
            for obj, p in zip(area_obj,pattern):
                if p:
                    same_geo.append(obj)
            

            rs.SelectObjects(same_geo)
            print(len(same_geo),"objects have the same area of",specific_area, "square", rs.UnitSystemName(False,False,False))
            #rs.UnitSystemName(False)
            #print(pattern)
            #print(same_geo)
            #print(area)