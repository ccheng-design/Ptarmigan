import rhinoscriptsyntax as rs
import Rhino

# BOM
# Creates bill of materials based on the blocks being in the BOM layer (user defined)

items = ("Use_BOM_Layer", "No", "Yes")
results = rs.GetBoolean("Use BOM Layer", items, (True))

# Definition on block list counting
def block_list_count():
    # Create list and dictionary to hold keys and values
    block_list = []
    block_desc = []
    counts = {}

    # for each object in the layer, test if it's a block
    for obj in layer:
        if rs.IsBlockInstance(obj):
            block_name = rs.BlockInstanceName(obj)
            block_description = rs.BlockDescription(block_name) or "None"
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

    spacingy = text_height + 0.15
    spacingx_name = max_length_names * text_height * 1.15
    spacingx_desc = (max_length_names * text_height) + (max_length_desc * text_height * 0.7)
    spacingx_amt = (max_length_names * text_height) + (max_length_desc * text_height) + 0.2

    vector_y = Rhino.Geometry.Vector3d(0, -spacingy, 0)
    vector_x_names = Rhino.Geometry.Vector3d(spacingx_name, 0, 0)
    vector_x_desc = Rhino.Geometry.Vector3d(spacingx_desc, 0, 0)
    vector_x_amt = Rhino.Geometry.Vector3d(spacingx_amt, 0, 0)

    # Define the x direction
    x_dir = pt + vector_x_names
    x_dir_desc = pt + vector_x_desc
    blk_desc = pt + vector_x_amt

    vector_x_names.Unitize()  # Unitize the vector
    length = spacingx_name
    scaled_vector = vector_x_names * length * 0.8

    left = []
    left_middle = []
    right_middle = []
    right = []

    # Add text for block names
    for items in key_list:
        pt += vector_y
        rs.AddPoint(pt)

        # Top Line
        offset_y = Rhino.Geometry.Vector3d(0, (spacingy / 2), 0)
        rs.AddLine(pt + offset_y, (pt + scaled_vector + offset_y))

        # Bottom Line
        rs.AddLine(pt - offset_y, (pt + scaled_vector - offset_y))

        # Left Line
        l = rs.AddLine((pt + offset_y), (pt - offset_y))
        left.append(l)

        # Add text
        rs.AddText(items, pt, text_height, justification=131073)

    # Add text for block descriptions
    for i in uniq_desc:
        x_dir_desc += vector_y
        rs.AddPoint(x_dir_desc)
        rs.AddText(i, x_dir_desc, text_height, justification=131076)

    # Add text for block counts
    for items in values_list:
        blk_desc += vector_y
        rs.AddPoint(blk_desc)
        rs.AddText(items, blk_desc, text_height, justification=131076)

if results == [True]:
    # Check if BOM layer exists in document
    if not rs.IsLayer("BOM"):
        print("Make BOM Layer")
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
