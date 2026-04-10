

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

#options to use
OPTIONS = {
    "RoundUp":  "Up",
    "RoundDown":    "Dwn"
    
}

def choose_option():
    go = Rhino.Input.Custom.GetOption()
    go.SetCommandPrompt("Choose Rounding Type")

    option_ids = {}
    for name in OPTIONS:
        option_ids[name] = go.AddOption(name)

    while True:
        r = go.Get()

        if r == Rhino.Input.GetResult.Option:
            for name, idx in option_ids.items():
                if go.OptionIndex() == idx:
                    return OPTIONS[name]

        if r == Rhino.Input.GetResult.Cancel:
            return None


def ZoomExtentInDetail():

    detailGet=rs.GetObjects("Get Details",filter=32768,preselect=True)
    if detailGet is None:
        return

    items = ("ScaleFactor", "No", "Yes")
    zoomToFactor = rs.GetBoolean("ZoomSelectToArchScaleFactor",items,(True))
    #print(zoomToFactor)

    if zoomToFactor is None:
        return



    mode=None
    if zoomToFactor[0] == True:
        mode = choose_option()

    details=[]
    SF = [4,8,12,16,24,32,48,64,96,128,192,384]
    DS = []


    #RECIPROCAL OF THE SCALE FACTOR TO DRAWING SCALE FACTOR (FRACTIONAL)
    for x in SF:
        number = 1/x
        DS.append(number)
    print(DS)

    #DETAIL ADDING TO LIST
    for i in detailGet:
        rs.DetailLock(i,False)
        details.append(i)

    #ZOOM SELECTION
    currentView =rs.CurrentView()
    for i in details:

        test=rs.CurrentDetail(layout=currentView,detail=i,return_name=False)

        detailObj = sc.doc.Objects.FindId(i)



        rs.AllObjects(True)
        rs.ZoomSelected(test,True)
        rs.UnselectAllObjects()

        rs.CurrentDetail(layout=currentView,detail=i)
        
        #print(test)
        #print(scaleFactor)

    #ROUNDING DOWN
    if zoomToFactor[0] == True and mode == "Dwn":
        for k in details:
            ZSdetailScale = sc.doc.Objects.FindId(k)

            scaleFactor = ZSdetailScale.DetailGeometry.PageToModelRatio
            print("ROUND DOWN", scaleFactor)
            roundedSF = min(x for x in DS if x >= scaleFactor)
            print(roundedSF)

            rs.DetailScale(k,1,roundedSF)
            

    #ROUNDING  UP
    if zoomToFactor[0] is True and mode == "Up":
        for k in details:
            ZSdetailScale = sc.doc.Objects.FindId(k)

            scaleFactor = ZSdetailScale.DetailGeometry.PageToModelRatio
            print("ROUND UP", scaleFactor)
            roundedSF = max(x for x in DS if x <= scaleFactor)
            print(roundedSF)

            rs.DetailScale(k,1,roundedSF)


    #DEACTIVATES DETAIL
    activeview = sc.doc.Views.ActiveView

    activeview.SetPageAsActive()
    sc.doc.Views.Redraw()

if __name__=="__main__":
    ZoomExtentInDetail()