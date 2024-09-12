import rhinoscriptsyntax as rs
import Rhino


#Not written by Clark Cheng

def main():
    """
    Creates text objects for layout names and page numbers in Rhino.
    """
    layout_info = get_layout_info_sorted()
    
    if not layout_info:
        print("No layouts found.")
        return
    
    # Prompt user to pick a point
    point = rs.GetPoint("Pick a point for the list")
    if not point:
        return  # User cancelled
    
    create_text_objects(layout_info, point)

def get_layout_info_sorted():
    """
    Retrieves the names and page numbers of all layouts in the document and sorts them based on page number.
    """
    doc = Rhino.RhinoDoc.ActiveDoc
    layouts = doc.Views.GetPageViews()
    layout_info = [(layout.PageName, layout.PageNumber + 1) for layout in layouts]  # Adjusted page number
    sorted_layout_info = sorted(layout_info, key=lambda x: x[1])  # Sort based on page number
    return sorted_layout_info

def create_text_objects(layout_info, base_point):
    """
    Creates text objects for each layout name and page number.
    """
    # Define text height
    text_height = .1
    
    # Define spacing between text objects
    spacing = text_height * 2.75
    
    # Loop through layout info and create text objects
    for index, (layout_name, page_number) in enumerate(layout_info, start=1):  # Starting page number at 1
        # Calculate position for the text
        text_position = base_point + Rhino.Geometry.Vector3d(0, -index * spacing, 0)
        
        # Add text objects for layout name and page number to the document
        layout_text = "{:<7} {}".format(page_number, layout_name)
        rs.AddText(layout_text, text_position, height=text_height)

if __name__ == '__main__':
    main()