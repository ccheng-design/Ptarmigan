import rhinoscriptsyntax as rs
import Rhino

#BOM
#Creates bill of materials based on the blocks being in the BOM layer (user defined)


items=("Use_BOM_Layer","No","Yes")
results=rs.GetBoolean("Use BOM Layer", items,(True))
#Definition on block list counting
def block_list_count():
        #Create list and dictionary to hold keys and values
        block_list=[]
        counts={}

        block_desc=[]
        desc_counts={}

        #for each object in the layer, test if its a block
        for obj in layer:
            if rs.IsBlockInstance(obj):
                block_names=rs.BlockInstanceName(obj)
                block_list.append(block_names)

        #get block description
        for names in block_list:      
                a=rs.BlockDescription(names)
                block_desc.append(a)
        

        
        #for each string in block_list list
        for string in block_list:
            if string in counts:
                counts[string]+=1
            else:
                counts[string]=1

        #for each string in block_desc list
        for string in block_desc:
            if string in desc_counts:
                desc_counts[string]+=1
            else:
                desc_counts[string]=1
        print(desc_counts)

        #create lists for 
        key_list=[]
        values_list=[]

        for string in counts:
            key_list.append(string)
            values_list.append(counts[string])

        #debugging
        #print(key_list)
        #print(values_list)

        key_list_length=[]
        for string in key_list:
            key_list_length.append(len(string))

        
        
        max_length=max(key_list_length)
        #debugging
        #print(max_length)

        pt=rs.GetPoint("Pick Point")

        #Text Height
                #text_height=0.1

        text_height=rs.GetReal("Text_Height",0.1)
        if text_height is None:
            return
        
        

        spacingy=text_height+0.15
        spacingx=max_length*text_height
        vector_y=Rhino.Geometry.Vector3d(0,-spacingy,0)
        vector_x=Rhino.Geometry.Vector3d(spacingx,0,0)

        #define the x direction
        number=pt+vector_x

        vector_x.Unitize() #unitize
        length=spacingx
        scaled_vector=vector_x*length


        
        #Debugging
        #print(pt+scaled_vector)

        #Add text
        for items in key_list:
            pt+=vector_y

            #debugging
            #rs.AddPoint(pt)
            

            #Top Line
            offset_y=Rhino.Geometry.Vector3d(0,(text_height/2)+0.01,0)
            rs.AddLine(pt+offset_y,(pt+scaled_vector+offset_y))

            #Bottom Line
            rs.AddLine(pt-offset_y,(pt+scaled_vector-offset_y))

            #Left Line
            rs.AddLine((pt+offset_y),(pt-offset_y))

            #Right Line
            rs.AddLine((pt+scaled_vector+offset_y),(pt+scaled_vector-offset_y))

            rs.AddText(items,pt,text_height,justification=131073)

        for i in values_list:
            number=number+vector_y

            #debugging
            #rs.AddPoint(number)

            rs.AddText(i,number,text_height,justification=131076)




if results==[True]:
    #Check if BOM layer exists in document
    if not rs.IsLayer("BOM"):
        print("Make BOM Layer")
    else:
        #Read from the Layer BOM
        layername=rs.LayerName("BOM")

        #Get objects by layer name
        layer=rs.ObjectsByLayer(layername,True)
        
        #run the function
        block_list_count()

else:
    
    #Get all blocks from Rhino Environment
    layer=rs.ObjectsByType(4096)
    #print(layer)
    
    block_list_count()