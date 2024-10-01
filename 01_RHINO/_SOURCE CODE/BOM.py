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

        desc_counts=[]
        block_desc=[]

        #for each object in the layer, test if its a block
        for obj in layer:
            if rs.IsBlockInstance(obj):
                block_names=rs.BlockInstanceName(obj)
                block_list.append(block_names)

        #get block description
        for names in block_list:      
                a=rs.BlockDescription(names)
                block_desc.append(a)
        

        
        #for each string in block_list list; counting the amount of unique blocks
        for string in block_list:
            if string in counts:
                counts[string]+=1
            else:
                counts[string]=1
        

        #for each string in block_desc list
###################################################################################

        #create list of unique parts
        unique_blk_names=set(block_list)

        mapped_index={}
        index=0

        for string in unique_blk_names:
            mapped_index[string]=index
            index=index+1

        mapped_indices=[]
        for i in unique_blk_names:
            mapped_indices.append(mapped_index[i])
            #outputs the index

        for i in mapped_indices:
            desc_counts.append(block_desc[i])

        print(desc_counts)
        
##########################################################################
        #create lists for 
        key_list=[]
        values_list=[]
        uniq_desc=[]
        

        #Appending the count into a list
        for string in counts:
            key_list.append(string)
            values_list.append(counts[string])

        #Appending description
        for string in desc_counts:
            
            if string is None: uniq_desc.append("None")
            uniq_desc.append(string)

        #Getting maximum length for the strings of block names
        key_list_length=[]
        for string in key_list:
            key_list_length.append(len(string))
        
        
        block_desc_length=[]
        for string in uniq_desc:
            if string is None: block_desc_length.append(0)
            else:block_desc_length.append(len(string))

        if not block_desc_length: return
        max_length_desc=max(block_desc_length)
        

        max_length_names=max(key_list_length)
        

        pt=rs.GetPoint("Pick Point")

        #Text Height
                #text_height=0.1

        text_height=rs.GetReal("Text_Height",0.1)
        if text_height is None:
            return
        
        

        spacingy=text_height+0.15
        spacingx_name=max_length_names*text_height*1.15
        spacingx_desc=(max_length_names*text_height)+(max_length_desc*text_height)
        spacingx_amt=(max_length_names*text_height)+(max_length_desc*text_height)+0.2

        vector_y=Rhino.Geometry.Vector3d(0,-spacingy,0)
        vector_x_names=Rhino.Geometry.Vector3d(spacingx_name,0,0)
        vector_x_desc=Rhino.Geometry.Vector3d(spacingx_desc,0,0)
        vector_x_amt=Rhino.Geometry.Vector3d(spacingx_amt,0,0)
        pg_num_vector=Rhino.Geometry.Vector3d(spacingx_name+(text_height*3),0,0)

        #define the x direction
        x_dir=pt+vector_x_names

        x_dir_desc=pt+vector_x_desc
        blk_desc=pt+pg_num_vector

        vector_x_names.Unitize() #unitize
        length=spacingx_name
        scaled_vector=vector_x_names*length


        
        #Debugging
        #print(pt+scaled_vector)

        middle=[]
        left=[]

        #Add text
        for items in key_list:
            pt+=vector_y

            #rs.AddPoint(pt)
            

            #Top Line
            offset_y=Rhino.Geometry.Vector3d(0,(spacingy/2),0)
            rs.AddLine(pt+offset_y,(pt+scaled_vector+offset_y))
            

            
            #rs.AddPoint(pt+blk_desc)


            #Bottom Line
            rs.AddLine(pt-offset_y,(pt+scaled_vector-offset_y))

            #Left Line
            l=rs.AddLine((pt+offset_y),(pt-offset_y))

            #middle Line
            m=rs.AddLine((pt+scaled_vector+offset_y),(pt+scaled_vector-offset_y))


            middle.append(m)
            left.append(l)

            rs.AddText(items,pt,text_height,justification=131073)

        for i in uniq_desc: #block description
            x_dir_desc+=vector_y

            #debugging
            rs.AddPoint(x_dir_desc)
            
            
            rs.AddText(i,x_dir_desc,text_height,justification=131076)

        for items in values_list: #count of each block listed
            blk_desc+=vector_y

            #Right Line
            rs.AddLine((blk_desc-(vector_y/2)),(blk_desc+(vector_y/2)))
            
            pt=vector_y

            rs.AddPoint(pt)
            rs.AddPoint(blk_desc)

            rs.AddText(items,blk_desc,text_height,justification=131076)
            #

        #Join middle side of curves and simplify
        if len(middle)!=1:
            a=rs.JoinCurves(middle,True)
            rs.SimplifyCurve(a)

        #Join left side of curves and simplify
        if len(left)!=1:
            a=rs.JoinCurves(left,True)
            rs.SimplifyCurve(a)


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