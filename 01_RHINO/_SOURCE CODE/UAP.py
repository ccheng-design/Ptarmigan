import Rhino
import scriptcontext as sc
import rhinoscriptsyntax as rs

def load_and_get_plugin_version(plugin_name):
    # Get the plugin ID by name
    plugin_id = Rhino.PlugIns.PlugIn.IdFromName(plugin_name)
    
    if not plugin_id:
        print(f"Plugin '{plugin_name}' not found.")
        return None
    
    # Check if the plugin is already loaded
    plugin = Rhino.PlugIns.PlugIn.Find(plugin_id)
    
    if not plugin:
        print(f"Plugin '{plugin_name}' is not loaded. Attempting to load...")
        
        # Attempt to load the plugin
        success = Rhino.PlugIns.PlugIn.LoadPlugIn(plugin_id)
        
        if not success:
            print(f"Failed to load plugin '{plugin_name}'.")
            return None
        
        # Try to find the plugin again after loading
        plugin = Rhino.PlugIns.PlugIn.Find(plugin_id)
        if not plugin:
            print(f"Plugin '{plugin_name}' could not be loaded or found after loading.")
            return None
    
    # Get the version (including build number) if plugin is loaded
    version = plugin.Version
    print(f"Plugin '{plugin_name}' version: {version}")
    return version

# Example usage for a .yak-installed plugin
plugin_name = "UAP"  # Replace with the actual .yak plugin name
load_and_get_plugin_version(plugin_name)
