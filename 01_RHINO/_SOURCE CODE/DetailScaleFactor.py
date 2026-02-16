

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino

import math
import sys


from fractions import Fraction
from decimal import Decimal


#Uses embedded text with attributes to define the scale factor
rs.MessageBox("Select details and text in the same order!", buttons=0, title="DetailScaleFactor")


detailGet=rs.GetObjects("Get Details",filter=32768)
description=rs.GetObjects("Get Text",filter=4096)

#Exit clause
if description or detailGet is None:
    print("Nothing Selected")



drawingScaleFactor=[]
descriptionCount=[]

#description count
for i in description:
    descriptionCount.append(i)

#detail count
detailGetCount=[]
for i in detailGet:
    detailGetCount.append(i)


print(len(descriptionCount))
print(len(detailGetCount))

if len(descriptionCount) != len(detailGetCount):
    sys.exit()

print(detailGet)
for i in detailGet:
    detailObj=sc.doc.Objects.FindId(i)

    #print(detailObj)

    viewport=detailObj.DetailGeometry.PageToModelRatio

    field='%<DetailScale("' + i.ToString() + '","'+ '#=1-0")>%'
    print(field)
    drawingScaleFactor.append(field)

    

#print(description)
for i,j in zip(description,drawingScaleFactor):
  
    print(j)
    rs.SetUserText(i, "SCALE", j)
    




