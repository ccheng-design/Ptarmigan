using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace _Ptarmigan.Resources
{
    internal static class NativeResolver
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        static NativeResolver()
        {
            try
            {
                string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                string meshLibDir = Path.Combine(assemblyDir, "lib", "MeshLib");

                if (Directory.Exists(meshLibDir))
                {
                    SetDllDirectory(meshLibDir);
                }
            }
            catch (Exception e)
            {
                // Optional: log in Rhino command line
                Rhino.RhinoApp.WriteLine($"[Ptarmigan] Failed to set DLL directory: {e.Message}");
            }
        }

        public static void Init() { } // call once to trigger static constructor
    }
}
