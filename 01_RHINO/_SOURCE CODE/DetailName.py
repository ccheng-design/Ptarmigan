

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



detailName=[]
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

    

    #%<ObjectName("166addf5-0772-4183-a517-73f9955bc1be")>%
    field='%<ObjectName("' + i.ToString() + '")>%'
    print(field)
    detailName.append(field)

    

#print(description)
for i,j in zip(description,detailName):
  
    print(j)
    rs.SetUserText(i, "TITLE", j)
    




