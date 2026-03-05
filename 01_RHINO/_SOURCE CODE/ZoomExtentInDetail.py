

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

detailGet=rs.GetObjects("Get Details",filter=32768,preselect=True)

details=[]

for i in detailGet:
    rs.DetailLock(i,False)
    details.append(i)

currentView =rs.CurrentView()
for i in detailGet:

    test=rs.CurrentDetail(layout=currentView,detail=i,return_name=False)

    rs.AllObjects(True)
    rs.ZoomSelected(test,True)
    rs.UnselectAllObjects()

    rs.CurrentDetail(layout=currentView,detail=None)
    
    print(test)


