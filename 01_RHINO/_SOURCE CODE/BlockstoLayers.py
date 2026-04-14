import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino


def BlockstoLayers():

    obj=rs.GetObjects("Select",filter=4096)

    if obj is None:
        print("None Selected")

    else:

        #Get blocknames
        blocknames=rs.BlockNames(obj)

        count_names=[]

        for string in blocknames:
            a=rs.AddLayer(string)
            count_names.append(a)


        for geo,names in zip(obj,blocknames):
            rs.ObjectLayer(geo,names)

        print(len(count_names),"layers have been made")

if __name__=="__main__":
    BlockstoLayers()
