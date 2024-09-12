import scriptcontext as sc
import Rhino
import rhinoscriptsyntax as rs

#Written by Clark Cheng
#email: ccheng@clarkcheng.design


#Get Starting pts
pt=rs.GetPoint("Pick Point")
#print(pt[1])

#Get all layout views (page views) in the current Rhino document
#layouts = sc.doc.Views.GetPageViews()
layouts=Rhino.RhinoDoc.ActiveDoc.Views.GetPageViews()

# Check if there are any layout views



if layouts:

    basepts=[]
    y_move=[]
    results=[]
    pagenum=len(layouts)
    transform=range(0,pagenum*5,pagenum)

    for i in transform:
        y_move.append(i)
    print(y_move)
    
    spacing=5

    for l,layouts in enumerate(layouts):
        pagename=layouts.PageName

        text=rs.AddText(pagename,pt)

        text_pos=pt+Rhino.Geometry.Vector3d(0, -2 * spacing, 0)

        


        print(pagename)
    

else:
    print("No layouts found.")
