import scriptcontext as sc
import Rhino
import rhinoscriptsyntax as rs

#Written by Clark Cheng
#email: ccheng@clarkcheng.design


#Get Starting pts
pt=rs.GetPoint("Pick Point")

#hard coded spacing
spacing=3

#define vector
vector_y=Rhino.Geometry.Vector3d(0,-spacing,0)

#Get all layout views (page views) in the current Rhino document
#layouts = sc.doc.Views.GetPageViews()
layouts=Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews()
num_pages=len(layouts)
# Check if there are any layout views



if layouts:
    for l in layouts:
        page_name=l.PageName
        page_num=l.PageNumber

        #move points for names of pages
        pt=pt+vector_y
        rs.AddPoint(pt)

        #move points for page numbers

        text_name=rs.AddText(page_name,pt,justification=131072)
        text_num=rs.AddText(pagenum,)
        

        


        print(pagename)
    

else:
    print("No layouts found.")
