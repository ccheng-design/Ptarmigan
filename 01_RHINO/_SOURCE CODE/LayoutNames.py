import Rhino
import rhinoscriptsyntax as rs
import time
import sys

#Written by Clark Cheng
#email: ccheng@clarkcheng.design


#Get Starting pts
pt=rs.GetPoint("Pick Point")
if not pt:print("None Selected")



else:
     

    #hard coded spacing in the
    spacing=0.25

    #define vector for y movement
    vector_y=Rhino.Geometry.Vector3d(0,-spacing,0)

    #define vector for x movement
    vector_x=Rhino.Geometry.Vector3d(-0.264,0,0)

    #Get all layout views (page views) in the current Rhino document
    #layouts = sc.doc.Views.GetPageViews()
    layouts=Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews()
    num_pages=len(layouts)
    

    unsorted_name=[]
    unsorted_number=[]
    for string in layouts:
            #rhino common page number
            page_name=string.PageName
            unsorted_name.append(page_name)

            #Convert int to string
            page_num=((string.PageNumber)+1)
            unsorted_number.append(page_num)

    #zip list together
    zipped_list=zip(unsorted_number,unsorted_name)
    sorted_zipped=sorted(zipped_list)
    new_number,new_name=zip(*sorted_zipped)

    sorted_name=list(new_name)
    sorted_number=list(new_number)
    
    



    #move points for page numbers
    start_pg_pt=pt+vector_x
    #print(start_pg_pt)

    #Total count of page numbers
    total_pg_count=[]
    title=[]
    page_numbers=[]

    all_text=rs.AddGroup("titles")

    items=("Group","No","Yes")
    group=rs.GetBoolean("Group Text?",items, True)
    if group is None:
        raise ValueError("No group")
    
    
    
    print(group[0])
    start_time = time.time() 

    if layouts:
        for name, number in zip(sorted_name,sorted_number):
            #rhino common page number
            #page_name=l.PageName

            #Convert int to string
            #page_num=str((l.PageNumber)+1)
            
            
            #move points for names of pages
            pt=pt+vector_y
            #debugging
            #rs.AddPoint(pt)

            #move points for page numbers a=a+1
            start_pg_pt=start_pg_pt+vector_y
            #debugging
            #rs.AddPoint(start_pg_pt)

            #Page names
            text_name=rs.AddText(name.upper(),pt,height=0.1,justification=131072)
            title.append(text_name)

            #Page numbers justification number added 2=center 131072=middle
            text_num=rs.AddText(number, start_pg_pt,height=0.1, justification=131074)
            page_numbers.append(text_num)

            if group[0]==True:
                #Add to group
                rs.AddObjectsToGroup(text_name,"titles")
                rs.AddObjectsToGroup(text_num,"titles")
            
            
            
            
            #Add page count to list for addition
            total_pg_count.append(page_num)

            #print(page_name)
        
    else:
        print("No layouts found.")


#Info on runtime
end_time = time.time()
execution_time = end_time-start_time

#End of command
print("It took",round(execution_time,3),"Seconds to Create",len(total_pg_count),"Drawing Titles &",len(total_pg_count), "Page Numbers")