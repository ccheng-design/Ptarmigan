
import rhinoscriptsyntax as rs


#AutoName
#Creates name for each obj
#1. Establish naming convention
#2. Select objs in order
#3. Create layers with the manually entered naming convention


#Get string prefix
prefix=rs.GetString("Name/Abbreviation for Part")
print(prefix)

#Get objs
parts=rs.GetObjects()
if not parts: print("None Selected")

else:
    print(len(parts))



