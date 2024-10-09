using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Ptarmigan
{
    public class PtarmiganInfo : GH_AssemblyInfo
    {
        public override string Name => "Ptarmigan";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("b3467c61-74d2-49e4-bfeb-ffdc786728f8");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";

        //Return a string representing the version.  This returns the same version as the assembly.
        public override string AssemblyVersion => GetType().Assembly.GetName().Version.ToString();
    }
}