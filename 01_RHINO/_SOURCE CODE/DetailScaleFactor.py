

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

rs.MessageBox("Select details and text in the same order!", buttons=0)


detailGet=rs.GetObjects("Get Details",filter=32768)
description=rs.GetObjects("Get Text",filter=4096)

drawingScaleFactor=[]
test=[]

for i in description:
    test.append(i)

detailGetCount=[]
for i in detailGet:
    detailGetCount.append(i)


print(len(test))
print(len(detailGetCount))

if len(test) != len(detailGetCount):
    sys.exit()

print(detailGet)
for i in detailGet:
    detailObj=sc.doc.Objects.FindId(i)

    #print(detailObj)

    viewport=detailObj.DetailGeometry.PageToModelRatio


    #print(viewport)

    x=Fraction(viewport).limit_denominator()
    fracScale=Fraction(12/x.denominator)

    drawingScale=str(fracScale) + '"' + " = " + "1'-0"+'"'
    #print(drawingScale)
    drawingScaleFactor.append(drawingScale)

    #print(str(fracScale) + '"' + " = " + "1'-0"+'"')

#print(description)
for i,j in zip(description,drawingScaleFactor):
  
    print(j)
    rs.SetUserText(i, "SCALE", j)




