import scriptcontext as sc
import Rhino
import rhinoscriptsyntax as rs
import time

#Written by Clark Cheng
#email: ccheng@clarkcheng.design


#Get Starting pts
pt=rs.GetPoint("Pick Point")
if not pt:print("None Selected")



else:
    start_time = time.time()  

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
    # Check if there are any layout views


    #move points for page numbers
    start_pg_pt=pt+vector_x
    #print(start_pg_pt)

    #Total count of page numbers
    total_pg_count=[]

    if layouts:
        for l in layouts:
            #rhino common page number
            page_name=l.PageName

            #Convert int to string
            page_num=str(l.PageNumber)
            
            
            #move points for names of pages
            pt=pt+vector_y
            rs.AddPoint(pt)

            #move points for page numbers
            start_pg_pt=start_pg_pt+vector_y
            rs.AddPoint(start_pg_pt)

            #Page names
            text_name=rs.AddText(page_name,pt,height=0.1,justification=131072)

            #Page numbers justification number added 2=center 131072=middle
            text_num=rs.AddText(page_num, start_pg_pt,height=0.1, justification=131074)
            
            total_pg_count.append(page_num)

            


            #print(page_name)
        

    else:
        print("No layouts found.")

#Info on runtime
end_time = time.time()
execution_time = end_time-start_time

#End of command
print("It took",round(execution_time,3),"Seconds to Create",len(total_pg_count),"Drawing Titles &",len(total_pg_count), "Page Numbers")