import rhinoscriptsyntax as rs
import math as m


#Get geometry
area_obj=rs.GetObjects("Select Geo",preselect=True)
if not area_obj:
    print("None Selected")

else:
    #create list to hold geo
    measured_obj=[]
    
    for obj in area_obj:

        #for each obj measure the area
        area=rs.Area(obj)
        area=round(area,3)
        
        #debugging
        #print(area)

        #debugging
        measured_obj.append(area)
    print(measured_obj)
    
    #unique=set(measured_obj)
    #print(unique)
    
    occur={}
    for i in measured_obj:
        occur[i]=occur.get(i,0)+1
    
    mapped=[occur[i]>1 for i in measured_obj]
    
    result=[]

    #convert true and false into 1 and 0
    #for i in mapped:
        #p=int(i)
        #result.append(p)
    #print(result)
    
    obj_select=[]
    uniquepts=[]

    #cull pattern using list comprehension
    same_area = [d for d, p in zip(area_obj, mapped) if p]
    print("OBJ",same_area)

    rs.SelectObjects(same_area)
    
    print(len(same_area), "objects has the same area")



