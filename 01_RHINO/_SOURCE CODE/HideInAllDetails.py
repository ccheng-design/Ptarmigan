
#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

def HideInAllDetails():

    objs=rs.GetObject("Select Object to Hide")
    rh_obj = sc.doc.Objects.FindId(objs)
    details=rs.ObjectsByType(32768)

    print(details)

    for detail in details:
        
        
        #active viewport
        #i = sc.doc.Views.ActiveView.ActiveViewportID
        attributes = rh_obj.Attributes

        attributes.AddHideInDetailOverride(detail)
        rh_obj.CommitChanges()

    sc.doc.Views.Redraw()

if __name__ == "__main__":
    HideInAllDetails()

