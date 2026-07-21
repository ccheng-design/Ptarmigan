using System;
using System.Collections.Generic;

using Grasshopper.Kernel;
using Rhino.Geometry;

namespace _Ptarmigan
{
    public class HoudiniExportObj : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public HoudiniExportObj()
          : base("Houdini Export: Obj Ref", "Nickname",
              "Exports Mesh to Houdini; It will export a .obj, then add a File SOP to reference it.",
              "Ptarmigan", "Export")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh to Export", GH_ParamAccess.list);
            pManager.AddTextParameter("FilePath", "F", "Filepath to export to; Include the name of the .hip file in your filepath as well", GH_ParamAccess.item);
            pManager.AddTextParameter("GeometryName", "GN", "Name for the Mesh/Meshes", GH_ParamAccess.item);
            pManager.AddBooleanParameter("Y-Up", "yUp", "Houdini is Y-Up by default; It will export as Y-Up by default", GH_ParamAccess.item, true);
            pManager.AddBooleanParameter("Export", "E", "Toggle to Perform Export; False by default", GH_ParamAccess.item, false);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddTextParameter("Report", "R", "Report on the Export", GH_ParamAccess.item);
            pManager.AddTextParameter("objFilePath", "objPath", "Filepath to .obj file", GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            List<Mesh> meshes = new List<Mesh>();
            //List<Mesh> meshes = null;
            DA.GetDataList(0, meshes);

            string FilePath = null;
            DA.GetData(1,ref FilePath);

            string GeometryName = null;
            DA.GetData(2,ref GeometryName);

            bool yUp = true;
            DA.GetData(3, ref  yUp);

            bool export = false;
            DA.GetData(4, ref  export);            


            string report = "";
            string objPath = "";
            Main();

            // ---- everything below is local functions (no access modifiers in script mode) ----

            void Main()
            {
                if (!export) { report = "Set export = true to run."; return; }

                // ---- validate / coerce -------------------------------------------------
                // Accepts raw Mesh, GH goo (GH_Mesh etc.), single item or list — no type
                // hint required on the input. Anything that isn't a mesh is skipped.
                var meshList = new List<Mesh>();
                var src = meshes as System.Collections.IEnumerable;
                if (src != null && !(meshes is Mesh))
                {
                    foreach (object o in src)
                    { var mm = CoerceMesh(o); if (mm != null) meshList.Add(mm); }
                }
                else
                { var mm = CoerceMesh(meshes); if (mm != null) meshList.Add(mm); }
                if (meshList.Count == 0)
                { report = "No meshes supplied. This component only exports meshes."; return; }
                if (string.IsNullOrWhiteSpace(FilePath))
                { report = "FilePath is empty."; return; }
                if (string.IsNullOrWhiteSpace(GeometryName)) GeometryName = "geo1";
                GeometryName = SanitizeNodeName(GeometryName);

                FilePath = System.IO.Path.GetFullPath(FilePath);
                if (!FilePath.EndsWith(".hip", StringComparison.OrdinalIgnoreCase))
                    FilePath += ".hip";
                string dir = System.IO.Path.GetDirectoryName(FilePath);
                string hipName = System.IO.Path.GetFileNameWithoutExtension(FilePath);
                System.IO.Directory.CreateDirectory(dir);

                // ---- join + orient meshes ---------------------------------------------
                var joined = new Mesh();
                foreach (var m in meshList)
                {
                    var c = m.DuplicateMesh();
                    if (c.Faces.Count > 0) joined.Append(c);
                }
                if (joined.Vertices.Count == 0)
                { report = "Meshes contained no faces/vertices."; return; }

                // ---- write OBJ ---------------------------------------------------------
                string objFile = hipName + "_" + GeometryName + ".obj";
                string objFullPath = System.IO.Path.Combine(dir, objFile);
                WriteObj(joined, objFullPath, yUp);

                // ---- write HIP ---------------------------------------------------------
                // File SOP references the obj via $HIP so the hip+obj pair is relocatable.
                string sopFileParm = "$HIP/" + objFile;
                WriteHip(FilePath, GeometryName, "file1", sopFileParm);

                objPath = objFullPath;
                report = string.Format("OK\nhip: {0}\nobj: {1}\nverts: {2}  faces: {3}",
                                        FilePath, objFullPath,
                                        joined.Vertices.Count, joined.Faces.Count);
            }

            // Unwraps a Mesh from raw geometry or Grasshopper goo (GH_Mesh has a .Value
            // property holding the Rhino mesh). Returns null for anything that isn't a mesh.
            Mesh CoerceMesh(object o)
            {
                if (o == null) return null;
                var direct = o as Mesh;
                if (direct != null) return direct;
                var p = o.GetType().GetProperty("Value");
                if (p != null) return p.GetValue(o, null) as Mesh;
                return null;
            }

            // ===========================================================================
            // OBJ writer — positions + faces (tri/quad), 1-based indices, invariant culture
            // ===========================================================================
            void WriteObj(Mesh m, string path, bool yUp)
            {
                var inv = System.Globalization.CultureInfo.InvariantCulture;
                var sb = new System.Text.StringBuilder(m.Vertices.Count * 32);
                sb.Append("# exported from Grasshopper\n");

                for (int i = 0; i < m.Vertices.Count; i++)
                {
                    var v = m.Vertices[i];
                    double x = v.X, y = v.Y, z = v.Z;
                    if (yUp) { double t = y; y = z; z = -t; }   // Rhino Z-up → Houdini Y-up
                    sb.Append("v ")
                      .Append(x.ToString("R", inv)).Append(' ')
                      .Append(y.ToString("R", inv)).Append(' ')
                      .Append(z.ToString("R", inv)).Append('\n');
                }
                for (int i = 0; i < m.Faces.Count; i++)
                {
                    var f = m.Faces[i];
                    if (f.IsQuad)
                        sb.Append("f ").Append(f.A + 1).Append(' ').Append(f.B + 1)
                          .Append(' ').Append(f.C + 1).Append(' ').Append(f.D + 1).Append('\n');
                    else
                        sb.Append("f ").Append(f.A + 1).Append(' ').Append(f.B + 1)
                          .Append(' ').Append(f.C + 1).Append('\n');
                }
                System.IO.File.WriteAllText(path, sb.ToString(), new System.Text.UTF8Encoding(false));
            }

            // ===========================================================================
            // HIP writer — odc CPIO archive of hscript sections
            // ===========================================================================
            void WriteHip(string FilePath, string geo, string sop, string fileParm)
            {
                long now = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
                const string ver = "21.0.440";   // written into node userdata; harmless on other versions

                var sections = new List<KeyValuePair<string, byte[]>>();
                Action<string, string> add = (name, text) =>
                    sections.Add(new KeyValuePair<string, byte[]>(name, new System.Text.UTF8Encoding(false).GetBytes(text)));
                Action<string, byte[]> addB = (name, bytes) =>
                    sections.Add(new KeyValuePair<string, byte[]>(name, bytes));

                add(".start",
                    "fplayback -i on -r off -f 1 -e on -h on -t on -a on -k on -s 1\n" +
                    "tcur 0\nfps 24\ntset 0 10\nfrange 1 240\nunitlength 1\nunitmass 1\n" +
                    "prompt '`strcat(oppwf(), \" -> \")`'\n");
                add(".variables",
                    "set -g ACTIVETAKE = 'Main'\n" +
                    "set -g _HIP_SAVEVERSION = '" + ver + "'\n" +
                    "set -g status = '0'\n");
                add(".takeconfig", "takename -g take\n");

                // /obj/<geo>  (Geometry object)
                add("obj/" + geo + ".init", "type = geo\nmatchesdef = 1\n");
                add("obj/" + geo + ".def", DefBlock(true, now, /*renderFlag*/ false));
                add("obj/" + geo + ".parm", "{\nversion 0.8\n}\n");
                addB("obj/" + geo + ".userdata", UserData(ver));

                // /obj/<geo>/<sop>  (File SOP)
                add("obj/" + geo + "/" + sop + ".init", "type = file\nmatchesdef = 1\n");
                add("obj/" + geo + "/" + sop + ".def", DefBlock(false, now, /*renderFlag*/ true));
                add("obj/" + geo + "/" + sop + ".parm",
                    "{\nversion 0.8\nfile\t[ 0\tlocks=0 ]\t(\t\"" + fileParm + "\"\t)\n}\n");
                addB("obj/" + geo + "/" + sop + ".userdata", UserData(ver));

                add("obj/" + geo + ".net", "1\n");
                add("obj.net", "1\n");
                add(".cwd", "opcf /\ntakeset Main\n");

                using (var fs = System.IO.File.Create(FilePath))
                {
                    foreach (var kv in sections) WriteOdcEntry(fs, kv.Key, kv.Value, now);
                    WriteOdcEntry(fs, "TRAILER!!!", new byte[0], now);
                }
            }

            // Node definition block shared by the object and the SOP.
            string DefBlock(bool isObj, long now, bool renderFlag)
            {
                var sb = new System.Text.StringBuilder();
                if (isObj)
                {
                    sb.Append("objflags objflags =  origin off\n");
                    sb.Append("pretransform UT_DMatrix4 1 0 0 0 0 1 0 0 0 0 1 0 0 0 0 1\n");
                }
                else
                    sb.Append("sopflags sopflags = \n");

                sb.Append("comment \"\"\n");
                sb.Append("position 0 0\n");
                sb.Append("connectornextid 0\n");
                sb.Append("flags =  lock off model off template off footprint off xray off bypass off display on render ")
                  .Append(renderFlag ? "on" : "off")
                  .Append(" highlight off unload off savedata off compress on colordefault on exposed on")
                  .Append(isObj ? " selectable on" : "")
                  .Append('\n');
                sb.Append("outputsNamed3\n{\n}\ninputsNamed3\n{\n}\ninputs\n{\n}\n");
                sb.Append("stat\n{\n  create ").Append(now).Append("\n  modify ").Append(now)
                  .Append("\n  author grasshopper@export\n  access 0777\n}\n");
                sb.Append("color UT_Color RGB 0.8 0.8 0.8 \n");
                sb.Append("delscript \"\"\n");
                sb.Append("exprlanguage hscript\nend\n");
                return sb.ToString();
            }

            // Node userdata blob: big-endian [int32 count][int16 len]"___Version___"[int32 3][int16 len]"<ver>"
            byte[] UserData(string version)
            {
                var ms = new System.IO.MemoryStream();
                Action<int> i32 = v => { ms.WriteByte((byte)(v >> 24)); ms.WriteByte((byte)(v >> 16)); ms.WriteByte((byte)(v >> 8)); ms.WriteByte((byte)v); };
                Action<short> i16 = v => { ms.WriteByte((byte)(v >> 8)); ms.WriteByte((byte)v); };
                var key = System.Text.Encoding.ASCII.GetBytes("___Version___");
                var val = System.Text.Encoding.ASCII.GetBytes(version);
                i32(1);
                i16((short)key.Length); ms.Write(key, 0, key.Length);
                i32(3);
                i16((short)val.Length); ms.Write(val, 0, val.Length);
                return ms.ToArray();
            }

            // odc cpio header: all-ASCII octal fields, 76 bytes, then name+NUL, then body. No padding.
            void WriteOdcEntry(System.IO.Stream s, string name, byte[] body, long mtime)
            {
                var nameBytes = System.Text.Encoding.ASCII.GetBytes(name);
                string hdr =
                    "070707" +               // magic
                    "000000" +               // dev
                    "000000" +               // ino
                    "100644" +               // mode
                    "000000" +               // uid
                    "000000" +               // gid
                    "000001" +               // nlink
                    "000000" +               // rdev
                    Oct(mtime, 11) +         // mtime
                    Oct(nameBytes.Length + 1, 6) +   // namesize (incl. NUL)
                    Oct(body.Length, 11);            // filesize
                var h = System.Text.Encoding.ASCII.GetBytes(hdr);
                s.Write(h, 0, h.Length);
                s.Write(nameBytes, 0, nameBytes.Length);
                s.WriteByte(0);
                s.Write(body, 0, body.Length);
            }

            string Oct(long v, int width)
            {
                return Convert.ToString(v, 8).PadLeft(width, '0');
            }

            string SanitizeNodeName(string n)
            {
                var sb = new System.Text.StringBuilder();
                foreach (char c in n)
                    sb.Append(char.IsLetterOrDigit(c) || c == '_' ? c : '_');
                if (sb.Length == 0 || char.IsDigit(sb[0])) sb.Insert(0, '_');
                return sb.ToString();
            }

            DA.SetData(0, report);
            DA.SetData(1, objPath);
        }

        /// <summary>
        /// Provides an Icon for the component.
        /// </summary>
        protected override System.Drawing.Bitmap Icon
        {
            get
            {
                //You can add image files to your project resources and access them like this:
                // return Resources.IconForThisComponent;
                return null;
            }
        }

        /// <summary>
        /// Gets the unique ID for this component. Do not change this ID after release.
        /// </summary>
        public override Guid ComponentGuid
        {
            get { return new Guid("BAA2FCBB-5A0C-434E-A9FB-6485E984512C"); }
        }
    }
}