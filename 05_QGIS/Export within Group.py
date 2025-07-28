import os
import re
from qgis.core import (
    QgsProject,
    QgsVectorFileWriter,
    QgsMapLayer,
    QgsLayerTreeGroup,
    QgsCoordinateReferenceSystem
)

# === USER SETTINGS ===
group_name = "Point Ends"  # <- Change this to your actual group name
export_folder = "D:/Side Projects/Turkiye Journey GIS Map/Google Maps"
export_format = "KML"
target_crs = QgsCoordinateReferenceSystem("EPSG:4326")  # KML requires WGS84

# Create the export folder if it doesn't exist
if not os.path.exists(export_folder):
    os.makedirs(export_folder)

# Allow Turkish characters by only removing forbidden characters
def sanitize_filename(name):
    return re.sub(r'[\\/*?:"<>|]', "_", name)

# Recursive export of all vector layers in a group
def export_layers_in_group(group):
    for child in group.children():
        if isinstance(child, QgsLayerTreeGroup):
            export_layers_in_group(child)  # recurse into subgroups
        elif hasattr(child, 'layer') and isinstance(child.layer(), QgsMapLayer):
            layer = child.layer()
            if layer.type() == QgsMapLayer.VectorLayer:
                safe_name = sanitize_filename(layer.name())
                output_path = os.path.join(export_folder, f"{safe_name}.kml")

                error = QgsVectorFileWriter.writeAsVectorFormat(
                    layer,
                    output_path,
                    "UTF-8",  # encoding for Turkish characters
                    target_crs,
                    export_format
                )

                if error == QgsVectorFileWriter.NoError:
                    print(f"✅ Exported: {layer.name()} → {output_path}")
                else:
                    print(f"❌ Failed to export: {layer.name()}")
            else:
                print(f"⚠️ Skipped (not a vector layer): {child.name()}")

# Locate the group
root = QgsProject.instance().layerTreeRoot()
group = root.findGroup(group_name)

if not group:
    raise Exception(f"Group '{group_name}' not found in the layer tree.")

# Start export
export_layers_in_group(group)
print("✅ All vector layers exported to KML.")
