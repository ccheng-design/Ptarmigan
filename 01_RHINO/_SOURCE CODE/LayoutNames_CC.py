import scriptcontext as sc
import Rhino
import rhinoscriptsyntax as rs

#Written by Clark Cheng
#email: ccheng@clarkcheng.design


#Get Starting pts
pt=rs.GetPoint("Pick Point")

#hard coded spacing in the
spacing=3

#define vector for y movement
vector_y=Rhino.Geometry.Vector3d(0,-spacing,0)

#define vector for x movement
vector_x=Rhino.Geometry.Vector3d(20,0,0)


#Get all layout views (page views) in the current Rhino document
#layouts = sc.doc.Views.GetPageViews()
layouts=Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews()
num_pages=len(layouts)
# Check if there are any layout views


#move points for page numbers
start_pg_pt=pt+vector_x
print(start_pg_pt)

if layouts:
    for l in layouts:
        page_name=l.PageName

        #Convert int to string
        page_num=str(l.PageNumber)
        
        
        #move points for names of pages
        pt=pt+vector_y
        rs.AddPoint(pt)

        #move points for page numbers
        start_pg_pt=start_pg_pt+vector_y
        rs.AddPoint(start_pg_pt)


        text_name=rs.AddText(page_name,pt,justification=131072)

        text_num=rs.AddText(page_num, start_pg_pt, justification=1)
        

        


        print(page_name)
    

else:
    print("No layouts found.")
