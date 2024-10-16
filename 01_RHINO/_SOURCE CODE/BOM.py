import rhinoscriptsyntax as rs
import Rhino
import scriptcontext as sc
import sys

# BOM
# Creates bill of materials based on the blocks being in the BOM layer (user defined)

items = ("Use_BOM_Layer", "No", "Yes")
results = rs.GetBoolean("Use BOM Layer", items, (True))

if results is None:
    print("None Selected")
    sys.exit()

# Definition on block list counting
def block_list_count():
    # Create list and dictionary to hold keys and values
    block_list = []
    block_desc = []
    counts = {}
    font="Roboto Light"
    font_header="Roboto Light"

    # for each object in the layer, check if it is a block
    for obj in layer:
        if rs.IsBlockInstance(obj):
            block_name = rs.BlockInstanceName(obj)
            block_description = rs.BlockDescription(block_name) or "გამა"
            block_list.append(block_name)
            block_desc.append(block_description)

    # Sort both block names and block descriptions together to maintain alignment
    block_list_desc = sorted(zip(block_list, block_desc))

    # Unzip back into two lists (sorted and aligned)
    block_list, block_desc = zip(*block_list_desc)

    # Count the occurrences of each block
    for block_name in block_list:
        counts[block_name] = counts.get(block_name, 0) + 1

    # Filter the descriptions by making sure there are no duplicates
    unique_block_desc = []
    for name, description in zip(block_list, block_desc):
        if (name, description) not in unique_block_desc:
            unique_block_desc.append((name, description))

    # Separate the names and descriptions after filtering
    filtered_block_list, filtered_block_desc = zip(*unique_block_desc)

    # Now display the filtered names, descriptions, and counts
    key_list = list(counts.keys())
    values_list = list(counts.values())
    uniq_desc = list(filtered_block_desc)

    # Get maximum lengths for string display
    key_list_length = [len(string) for string in key_list]
    block_desc_length = [len(string) for string in uniq_desc]

    max_length_desc = max(block_desc_length)
    max_length_names = max(key_list_length)

    pt = rs.GetPoint("Pick Point")
    text_height = rs.GetReal("Text_Height", 0.1)
    if text_height is None:
        return

    # Spacings
    spacingy = text_height *2
    spacingx_name = max_length_names * text_height * 1.7
    spacingx_desc = (max_length_names * text_height) + (max_length_desc * text_height * 1.4)
    spacingx_amt = spacingx_desc *1.2

    vector_y = Rhino.Geometry.Vector3d(0, -spacingy, 0)
    vector_x_names = Rhino.Geometry.Vector3d(spacingx_name, 0, 0)
    vector_x_desc = Rhino.Geometry.Vector3d(spacingx_desc, 0, 0)
    vector_x_amt = Rhino.Geometry.Vector3d(spacingx_amt, 0, 0)
    pg_num_vector = Rhino.Geometry.Vector3d(spacingx_name + (text_height * 3), 0, 0)

    # Define the x direction
    x_dir = pt + vector_x_names
    x_dir_desc = pt + vector_x_desc
    blk_amt = pt + vector_x_amt

    vector_x_names.Unitize()  # Unitize the vector
    length = spacingx_name
    scaled_vector = vector_x_names * length * 0.8

    left = []
    left_middle = []
    right_middle = []
    right = []

    #Title block
    
    global_text_offset=Rhino.Geometry.Vector3d(text_height*1.02,0,0)



    #ITEM
    item_offset=-0.05
    item_pt=pt+Rhino.Geometry.Vector3d(item_offset,0,0)-(vector_y/2)
    #rs.AddPoint(item_pt)

    rs.AddText("ITEM",item_pt,text_height,font_header,justification=131076)


    #PART NO
    
    title_block_pts=(pt-(vector_y/2))+Rhino.Geometry.Vector3d(text_height*1.02,0,0)
    #rs.AddPoint(title_block_pts)

    item_name=rs.AddText("PART NO",title_block_pts,text_height,font_header,justification=131073)

    #PART DESCR
    part_desc_pt=(x_dir_desc-(vector_y/2))-global_text_offset
    #rs.AddPoint(part_desc_pt)

    part_descr_name=rs.AddText("PART DESCR",part_desc_pt,text_height,font_header,justification=131076)

    #QTY
    quantity_pt=blk_amt-(vector_y/2)-global_text_offset
    #rs.AddPoint(quantity_pt)

    quantity_name=rs.AddText("QTY",quantity_pt,text_height,font_header,justification=131076)


    #Lines for title blocks
    #Top lines
    item_ln_offset=(-4*text_height)*((item_offset*-1)+1)

    pt_1=pt-vector_y
    #rs.AddPoint(pt_1)
    pt_2=blk_amt-vector_y
    #rs.AddPoint(pt_2)

    #rs.AddPoint(pt_1+Rhino.Geometry.Vector3d(item_ln_offset,0,0))
    
    #top horizontial line
    rs.AddLine(pt_1+Rhino.Geometry.Vector3d(item_ln_offset,0,0),pt_2)

    #vertial lines
    
    rs.AddLine(pt_1,pt+(vector_y/2))

    rs.AddLine(x_dir_desc+(vector_y/2),x_dir_desc-(vector_y))

    rs.AddLine(blk_amt+(vector_y/2),blk_amt-vector_y)
    
    #rs.AddLine()

    
    

    items_number=[]
    Left_horiz_ln=[]
    # Add text for block names
    for items in key_list:
        pt += vector_y
        
        new_pt=pt+Rhino.Geometry.Vector3d(item_ln_offset,0,0)+(vector_y/2)
        Left_horiz_ln.append((pt+Rhino.Geometry.Vector3d(item_ln_offset,0,0)+(vector_y/2)))

        

        items_number.append(pt)

        


        

        # Horizontial Line Bottom
        offset_y = Rhino.Geometry.Vector3d(0, (spacingy / 2), 0)
        
        bottom_crv=rs.AddLine(new_pt,pt+vector_x_amt-offset_y)

        # Horizontial Line Top
        transform=Rhino.Geometry.Transform.Translation(offset_y*2)
        
        top_crv=rs.coercecurve(bottom_crv)
        top_crv.Transform(transform)

        sc.doc.Objects.AddCurve(top_crv)
        # Left Line
        l = rs.AddLine((pt + offset_y), (pt - offset_y))
        left.append(l)

        # Left Middle Line
        
        lm = rs.AddLine((pt + scaled_vector + offset_y), (pt + scaled_vector - offset_y))
        
        left_middle.append(lm)

        # Add text
        offset_pt=pt+Rhino.Geometry.Vector3d(text_height*1.02,0,0)
        rs.AddText(items.upper(), offset_pt, text_height,font, justification=131073)
    rs.AddPolyline(Left_horiz_ln)
    rs.AddLine(pt_1+Rhino.Geometry.Vector3d(item_ln_offset,0,0),Left_horiz_ln[0])

    total=len(items_number)
    total_numbers=[]
    total_pts=[]

    for i in range(1,total+1):
        total_numbers.append(i)

    
    print(total_numbers)
    
    for item in items_number:
        offset_x=Rhino.Geometry.Vector3d(item_offset,0,0)
        
        pt=item+offset_x
        #rs.AddPoint(pt)
        total_pts.append(pt)
    
    for point,num in zip(total_pts,total_numbers):
        rs.AddText(num,point,text_height,font,justification=131076)

     

    # Add text for block descriptions
    for i in uniq_desc:
        x_dir_desc += vector_y
        #rs.AddPoint(x_dir_desc)
        

        # Right Middle Line
        rm = rs.AddLine((x_dir_desc + offset_y), (x_dir_desc - offset_y))
        right_middle.append(rm)

        offset_desc=x_dir_desc-global_text_offset
        rs.AddText(i.upper(), offset_desc, text_height,font, justification=131076)

    # Add text for block counts
    for items in values_list:
        blk_amt += vector_y

        # Right Line
        r = rs.AddLine((blk_amt - (vector_y / 2)), (blk_amt + (vector_y / 2)))
        right.append(r)

        #rs.AddPoint(blk_amt)

        offset_count=blk_amt-global_text_offset
        rs.AddText(items, offset_count, text_height, font, justification=131076)

    # Join left middle side of curves and simplify
    if len(left_middle) != 1:
        a = rs.JoinCurves(left_middle, True)
        rs.SimplifyCurve(a)
        c=rs.coercecurve(a)
        start_pt=c.PointAtStart
        x=Rhino.Geometry.Line(start_pt,vector_y,spacingy*-1.5)
        

        sc.doc.Objects.AddCurve(Rhino.Geometry.LineCurve(x))
        

    # Join left side of curves and simplify
    if len(left) != 1:
        a = rs.JoinCurves(left, True)
        rs.SimplifyCurve(a)
        

    # Join right middle side of curves and simplify
    if len(right_middle) != 1:
        a = rs.JoinCurves(right_middle, True)
        rs.SimplifyCurve(a)
        

    

    # Join right side of curves and simplify
    if len(right) != 1:
        a = rs.JoinCurves(right, True)
        rs.SimplifyCurve(a)
        
        
        
    


if results == [True]:
    # Check if BOM layer exists in document
    if not rs.IsLayer("BOM"):
        print("Make BOM Layer")
    else:
    
        if rs.IsLayerEmpty("BOM")==True:
            print("Layer is empty")
            sys.exit()
        else:
            # Read from the Layer BOM
            layername = rs.LayerName("BOM")

            # Get objects by layer name
            layer = rs.ObjectsByLayer(layername, True)

            # Run the function
            block_list_count()
    


else:
    # Get all blocks from Rhino Environment
    layer = rs.ObjectsByType(4096)
    block_list_count()
