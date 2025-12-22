
import rhinoscriptsyntax as rs
import scriptcontext as sc
import math

import System
import System.Collections.Generic
import Rhino


#options to use
OPTIONS = {
    "DrawingScale":  "DS",
    "ScaleFactor":    "SF",
    "ViewportScale":  "VS"
}

def choose_option():
    go = Rhino.Input.Custom.GetOption()
    go.SetCommandPrompt("Choose Scale Type")

    option_ids = {}
    for name in OPTIONS:
        option_ids[name] = go.AddOption(name)

    while True:
        r = go.Get()

        if r == Rhino.Input.GetResult.Option:
            for name, idx in option_ids.items():
                if go.OptionIndex() == idx:
                    return OPTIONS[name]

        if r == Rhino.Input.GetResult.Cancel:
            return None

detail=rs.GetObjects("Select Detail", rs.filter.detail,preselect=True)

#layout scale factor
layoutSfactor=rs.GetReal("Layout Scale Factor",1,0)
if layoutSfactor is None:

    print("Cancelled")
    raise SystemExit()

mode = choose_option()
if mode is None:
    print("No Value Given")
    raise SystemExit()

#drawing scale
if mode == "DS":
    
    modelSfactor=(1/rs.GetReal("Drawing Scale Factor; Fractional or Decimal is Allowed",1,0))*12
    if modelSfactor == 0:
        print("No Value Given")
        raise SystemExit()

#scale factor
elif mode == "SF":
    
    modelSfactor=rs.GetReal("Scale Factor",1,0)
    if modelSfactor == 0:
        print("No Value Given")
        raise SystemExit()

#viewport scale
elif mode == "VS":
   
    modelSfactor=1/rs.GetReal("Viewport Scale Factor",1,0)
    if modelSfactor ==0:
        print("No Value Given")
        raise SystemExit()



#foreach detail selected
for obj in detail:

    rs.DetailScale(obj,modelSfactor,layoutSfactor)