import rhinoscriptsyntax as rs
import scriptcontext as sc
import Rhino
import System.Drawing.Color as Col

def main():
    """
    Creates a part list for all blocks in a document. Numbers will correspond with 
    balloons, but balloons don't need to be present for the table to be generated
    
    version 1.3
    www.studiogijs.nl
    
    version 1.1 adds table heading
    version 1.2 option for choosing between all or only top level blocks
    version 1.3 adds block description. Use change_block_description.py
    or change in block manager
   
    """
    t = sc.sticky['top_level_only'] if sc.sticky.has_key('top_level_only') else 0 #0 = top level only, 1= all blocks
    if t==None:
        t=0
    top_level_only = rs.GetBoolean("annotate top level blocks only?", ["top_level_only", "yes", "no"],t)
    if not top_level_only:
        return
    sc.sticky['top_level_only'] = top_level_only[0]
    
    
    previous_layer = rs.CurrentLayer()
    #check if layer 'annotation' exist, else create it
    if not rs.IsLayer("annotation"): rs.AddLayer("annotation")
    rs.LayerColor("annotation",Col.Black)
    
    rs.CurrentLayer("annotation")
    
    groups = sc.doc.ActiveDoc.Groups
    partlist = []
    
    # Gather block names from layout objects
    blocknames = get_block_names()
    if not blocknames:
        print "This file does not contain block items (titleblock will be ignored)"
        return
    
    # Sort block names alphabetically
    blocknames.sort()
    
    # Add headings
    texts = ["ITEM", "PART NO", "PART DESCR", "QTY"]
    partlist.append(texts)
    
    texts = []
    for block_nr, blockname in enumerate(blocknames, 1):
        texts.append(str(block_nr))
        texts.append(blockname)
        description = rs.BlockDescription(blockname)
        if description is None:
            description = ""
        texts.append(description)
        blockcount = get_block_count(blockname)
        texts.append(str(blockcount))
        partlist.append(texts)
        texts = []
    
    create_table(partlist)
    
    # Change back to previous layer
    rs.CurrentLayer(previous_layer)

def get_block_count(blockname):
    # Count instances of the block
    block_instances = rs.BlockInstances(blockname)
    if block_instances:
        return len(block_instances)
    return 0

def get_block_names():
    block_instances = rs.ObjectsByType(4096)  # Object type 4096 represents block instances
    blocknames = []
    for block_instance in block_instances:
        blockname = rs.BlockInstanceName(block_instance)
        if blockname and blockname != "" and "titleblock" not in blockname.lower():
            if rs.IsBlockInUse(blockname, where_to_look=sc.sticky['top_level_only']):
                if blockname not in blocknames:
                    blocknames.append(blockname)
    
    if len(blocknames) > 0:
        return blocknames
    return False
    
def create_table(partlist):
    # Check if "partlistgroup" exists, else create it
    g = rs.GroupNames()
    if not g or not "partlistgroup" in g:
        rs.AddGroup("partlistgroup")
    
    # Clean the group
    group= sc.doc.Groups.FindName("partlistgroup")
    objs = sc.doc.ActiveDoc.Groups.GroupMembers(group.Index)
    rs.DeleteObjects(objs)
    
    # Base table width on largest name
    blocknames = [texts[1] for texts in partlist[1:]]  # Extract block names
    twidth = 3 + max(blocknames, key=len).Length*2.1 
    desc = 3 + max([texts[2] for texts in partlist[1:]], key=len).Length*2.1
    if desc < 3 + 5*2.1:
        desc = 3 + 5*2.1
    
    def addTexts(texts, y):
        # Function to add texts to the table
        for i, text in enumerate(texts):
            if i == 0:
                a = 10
                just = Rhino.Geometry.TextJustification.BottomRight
                
            elif i == 1:
                a = 13.5
                just = Rhino.Geometry.TextJustification.BottomLeft
            elif i == 2:
                a = 13.5 + twidth
                just = Rhino.Geometry.TextJustification.BottomLeft
            else:
                a = 20 + twidth + desc
                just = Rhino.Geometry.TextJustification.BottomRight
            plane = Rhino.Geometry.Plane.WorldXY
            plane.Origin = Rhino.Geometry.Point3d(a, y-4, 0)
            
            textobject = sc.doc.Objects.AddText(text, plane, 2, 'Roboto Light', False, False, just)
            rs.AddObjectToGroup(textobject, "partlistgroup")

    def add_borders(i, y):
        # Function to add borders to the table
        start = Rhino.Geometry.Point3d(0, y-6, 0)
        end = Rhino.Geometry.Point3d(22+ twidth + desc, y-6, 0)
        line = sc.doc.Objects.AddLine(start, end)  # Bottom border
        rs.AddObjectToGroup(line, "partlistgroup")
        if i == 0:
            # Add top border
            trans = Rhino.Geometry.Transform.Translation(0, 6, 0)
            h_line = Rhino.Geometry.Line(start, end)
            h_line.Transform(trans)
            line = sc.doc.Objects.AddLine(h_line)
            rs.AddObjectToGroup(line, "partlistgroup")
        # Add vertical lines
        v_start = Rhino.Geometry.Point3d(0, y, 0)
        v_end = Rhino.Geometry.Point3d(0, y-6, 0)
        
        v_line = Rhino.Geometry.Line(v_start, v_end)
        line = sc.doc.Objects.AddLine(v_line)
        rs.AddObjectToGroup(line, "partlistgroup")
        
        #
        trans = Rhino.Geometry.Transform.Translation(12, 0, 0)
        v_line.Transform(trans)
        line = sc.doc.Objects.AddLine(v_line)
        rs.AddObjectToGroup(line, "partlistgroup")
        
        trans = Rhino.Geometry.Transform.Translation(twidth, 0, 0)
        v_line.Transform(trans)
        line = sc.doc.Objects.AddLine(v_line)
        rs.AddObjectToGroup(line, "partlistgroup")
        
        trans = Rhino.Geometry.Transform.Translation(desc, 0, 0)
        v_line.Transform(trans)
        line = sc.doc.Objects.AddLine(v_line)
        rs.AddObjectToGroup(line, "partlistgroup")
        
        trans = Rhino.Geometry.Transform.Translation(10, 0, 0)
        v_line.Transform(trans)
        line = sc.doc.Objects.AddLine(v_line)
        rs.AddObjectToGroup(line, "partlistgroup")

    y = len(partlist)*6
    # Get insertion point
    target = Rhino.Geometry.Point3d(0, 0, 0)
    
    for i, texts in enumerate(partlist):
        addTexts(texts, y)
        add_borders(i, y)
        y -= 6
    
    group= sc.doc.Groups.FindName("partlistgroup")
    objs = sc.doc.ActiveDoc.Groups.GroupMembers(group.Index)
    rs.MoveObjects(objs, target)

if __name__ == '__main__':
    main()
