
import rhinoscriptsyntax as rs
import random


#AutoName
#Creates name for each obj
#1. Establish naming convention
#2. Select objs in order
#3. Create layers with the manually entered naming convention
#4. Rename all objs
#5. Add text dot at obj


#Get string prefix
prefix=rs.GetString("Name/Abbreviation for Part")
#print(prefix)

#Get objs
parts=rs.GetObjects("Select in Order of Labels")
if not parts: print("None Selected")

else:
    #print(len(parts))


    values=[]
    
    start=rs.GetInteger("Start Value",1)
    step=rs.GetInteger("Step Value",1)
    
    count=len(parts)

    series=range(start, start + count * step, step)
    seq=[]


    for i in series:
        #Concat labels
        labels=str(prefix)+str(i)
        
        #Add to list
        seq.append(labels)
        color=random.randint(0,255),random.randint(0,255),random.randint(0,255)
        
        #adding color
        rs.AddLayer(labels,color=(color))

    #Zip is using the list in tandum
    for obj,seq in zip(parts,seq):
        rs.ObjectLayer(obj, seq)
        rs.ObjectName(obj,seq)
        
    

    

