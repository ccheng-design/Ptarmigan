import rhinoscriptsyntax as rs

#written by Clark Cheng
#email: ccheng@clarkcheng.design


def UP3():
    obj=rs.GetObjects("Select object to scale", preselect=True)
    if not obj:
        print("None selected")
        return
    if obj:
        origin=rs.GetPoint("Origin")
    if origin:
        
        #create toggle for scale
        copy=rs.GetBoolean("Copy",("Copy", "No", "Yes"),False)
        print(copy)
        
        #extract value from list
        copy=copy[0]
        
        #scale object
        rs.ScaleObject(obj,origin, (1.03,1.03,1.03),copy)
        #origin=rs.AddPoint(0,0,0)

UP3()