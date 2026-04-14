
#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

def DetailDisplay():


    #Get detail from viewport
    ids=rs.GetObjects("Select Detail", rs.filter.detail,preselect=True)
    if ids is None:
        return

    #String box to type display name
    displayModeName = rs.StringBox("Display Mode Name","Shaded","Display Mode")
    if displayModeName is None:
        return



    for obj in ids:

        detail_id=ids[0]

        detail=sc.doc.Objects.FindId(detail_id)


        mode=Rhino.Display.DisplayModeDescription.FindByName(displayModeName)

        detail.Viewport.DisplayMode = mode

        #Commit Changes
        detail.CommitViewportChanges()

        page_view = detail.Viewport.ParentView

if __name__=="__main__":
    DetailDisplay()


