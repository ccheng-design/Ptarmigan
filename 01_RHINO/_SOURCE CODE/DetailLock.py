

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino


details=rs.GetObjects("Select Details",32768,preselect=True)

for i in details:
    rs.DetailLock(i,True)
    

