
import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

def DetailUnlock():


    details=rs.GetObjects("select Details",32768,preselect=True)

    for i in details:
        rs.DetailLock(i,False)

if __name__ =="__main__":
    DetailUnlock()