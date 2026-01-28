

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino
import sys


details=rs.GetObjects("Select Details",32768,preselect=True)

if details is None:
    sys.exit
else:

    for i in details:
        rs.DetailLock(i,True)
    

