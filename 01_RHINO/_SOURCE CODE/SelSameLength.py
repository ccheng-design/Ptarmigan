"""
NOTE:

- Reference to RhinoCommmon.dll is added by default

- You can specify your script requirements like:

    # r: <package-specifier> [, <package-specifier>]
    # requirements: <package-specifier> [, <package-specifier>]

    For example this line will ask the runtime to install
    the listed packages before running the script:

    # requirements: pytoml, keras

    You can install specific versions of a package
    using pip-like package specifiers:

    # r: pytoml==0.10.2, keras>=2.6.0

- Use env directive to add an environment path to sys.path automatically
    # env: /path/to/your/site-packages/
"""
#! python3

import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino


#Get geometry
length_obj=rs.GetObjects("Select Curves for Length Calculation",4,preselect=True)
if not length_obj:
    print("None Selected")



else:
    #create list to hold geo
    measured_length=[]

    #for loop in each obj
    for obj in length_obj:

        #for each obj measure the area
        area=rs.CurveLength(obj)
        area=round(area,3)
        
        #debugging
        #print(area)

        #debugging
        measured_length.append(area)
    print(measured_length)


    #all objects
    all_obj_opt=("All_Objects", "False", "True")
    all_obj=rs.GetBoolean("Boolean options" ,all_obj_opt, (False))
    #print(all_obj)


    
    if all_obj==[True]:

        occur={}
        for i in measured_length:
            occur[i]=occur.get(i,0)+1

        mapped=[occur[i]>1 for i in measured_length]

        #cull pattern using list comprehension
        same_length = [d for d, p in zip(length_obj, mapped) if p]
        print("OBJ",same_length)

        rs.SelectObjects(same_length)

        print(len(same_length), "objects has the same area")
    else:
        #specific objects
        specific_obj=rs.GetObject()
        if not specific_obj:
            print("None Selected")
        else:
            specific_length=round(rs.Area(specific_obj),3)

            print(specific_length)

            pattern=[]
            same_geo=[]
            

            for i in measured_length:
                if i == specific_length:
                    pattern.append(True)
                    #pattern.append(length_obj)

                else:
                    pattern.append(False)
            
            for obj, p in zip(length_obj,pattern):
                if p:
                    same_geo.append(obj)
            

            rs.SelectObjects(same_geo)
            print(len(same_geo),"objects have the same area of",specific_length, "square", rs.UnitSystemName(False,False,False))
            #rs.UnitSystemName(False)
            #print(pattern)
            #print(same_geo)
            #print(area)

    


