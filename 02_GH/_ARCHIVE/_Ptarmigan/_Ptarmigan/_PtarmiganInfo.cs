using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace _Ptarmigan
{
    public class _PtarmiganInfo : GH_AssemblyInfo
    {
        public override string Name => "_Ptarmigan";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "Components for fabrication and 3D modeling";

        public override Guid Id => new Guid("052bd0ca-6c62-441e-8d16-546a148ebcec");

        //Return a string identifying you or your company.
        public override string AuthorName => "Clark Cheng";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "cchengg@clarkcheng.design";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
    public static class NativeLoader
    {
        [DllImport("kernel32", SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        static NativeLoader()
        {
            string assemblyDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string meshLibDir = Path.Combine(assemblyDir, "lib", "MeshLib");
            if (Directory.Exists(meshLibDir))
            {
                SetDllDirectory(meshLibDir);
            }
        }

        public static void Init() { }
    }
}