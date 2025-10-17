using Grasshopper.Kernel;
using Rhino.Geometry;
using System;
using System.Collections.Generic;

using Grasshopper;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using static MR.DotNet;

using System.Linq;
using System.Collections;
using System.Drawing;

using Rhino;

namespace _Ptarmigan
{
    public class Shell : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public Shell()
          : base("Shell", "Shell",
              "Creates a shell from the Base Mesh. If Mesh is Open, it will attempt to bridge the open edge loop together.",
              "Ptarmigan", "Mesh")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("Mesh", "M", "Mesh to Shell", GH_ParamAccess.item);
            pManager.AddNumberParameter("Distance", "Dist", "Distance to Offset; Input only Positive Numbers", GH_ParamAccess.item, 0.0);
            pManager.AddNumberParameter("Voxel Size", "VS", "Voxel Size is Based on Real World Units", GH_ParamAccess.item, 0.5);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Fused Mesh", "FM", "Both Shell and Original Mesh Fused Together", GH_ParamAccess.item);
            pManager.AddMeshParameter("Disjointed Mesh", "DM", "Mesh Shells Separated", GH_ParamAccess.list);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        /// 
        public static class RhMeshMRMeshConvertors
        {
            /// <summary>
            /// Convert a Rhino mesh into a MeshLib (MR.DotNet) mesh.
            /// </summary>
            public static MR.DotNet.Mesh ToMeshLibMesh(Rhino.Geometry.Mesh rhMesh)
            {
                if (rhMesh == null) return null;

                // Duplicate and triangulate quads
                var triMesh = rhMesh.DuplicateMesh();
                triMesh.Faces.ConvertQuadsToTriangles();

                // Build point list
                var points = new List<MR.DotNet.Vector3f>(triMesh.Vertices.Count);
                foreach (Point3f v in triMesh.Vertices)
                    points.Add(new MR.DotNet.Vector3f((float)v.X, (float)v.Y, (float)v.Z));

                // Build triangle list
                var tris = new List<MR.DotNet.ThreeVertIds>(triMesh.Faces.Count);
                foreach (var f in triMesh.Faces)
                {
                    if (f.IsTriangle)
                    {
                        var tri = new MR.DotNet.ThreeVertIds();
                        tri.v0.Id = f.A;
                        tri.v1.Id = f.B;
                        tri.v2.Id = f.C;
                        tris.Add(tri);
                    }
                }

                // Construct MeshLib mesh
                return MR.DotNet.Mesh.FromTriangles(points, tris);
            }

            public static Rhino.Geometry.Mesh ToRhinoMesh(MR.DotNet.Mesh mlMesh)
            {
                if (mlMesh == null) return null;

                var rhMesh = new Rhino.Geometry.Mesh();

                // Vertices
                foreach (var p in mlMesh.Points)
                    rhMesh.Vertices.Add(p.X, p.Y, p.Z);

                // Faces
                foreach (var tri in mlMesh.Triangulation)
                    rhMesh.Faces.AddFace(tri.v0.Id, tri.v1.Id, tri.v2.Id);

                // Cleanup
                rhMesh.Vertices.CombineIdentical(true, true);
                rhMesh.Vertices.CullUnused();
                rhMesh.Faces.CullDegenerateFaces();
                rhMesh.UnifyNormals();
                rhMesh.Normals.ComputeNormals();
                rhMesh.Compact();

                return rhMesh;
            }
        }
        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.Geometry.Mesh M = null;
            Double Dist = 0.0;
            Double VS = 0.0;

            DA.GetData(0, ref M);
            DA.GetData(1, ref Dist);
            DA.GetData(2, ref VS);

            //Sanity Check
            if (Dist <0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Value Cannot be Below Zero");
                return;
            }

            //Sanity Check
            if (VS <= 0)
            {
                this.AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Value Cannot be Zero");
                return;
            }

            //convert to MRDot Mesh
            var mlMesh = RhMeshMRMeshConvertors.ToMeshLibMesh(M);

            //Convert to voxels
            MR.DotNet.MeshPart VMesh = new MR.DotNet.MeshPart(mlMesh);

            float VSize = (float)VS;

            var offsetParams = new OffsetParameters();
            offsetParams.voxelSize = VSize;
            offsetParams.memoryEfficient = true;
            offsetParams.signDetectionMode = SignDetectionMode.OpenVDB;

            var generalParams = new GeneralOffsetParameters();

            float offsetDist = (float)Dist;

            MR.DotNet.Mesh offsetMesh = Offset.ThickenMesh(mlMesh, offsetDist, offsetParams, generalParams);

            //a = RhMeshMRMeshConvertors.ToRhinoMesh(offsetMesh);

            Rhino.Geometry.Mesh RHOffsetMesh = RhMeshMRMeshConvertors.ToRhinoMesh(offsetMesh);

            //Normal Output
            DA.SetData(0, RHOffsetMesh);

            //SplitDisjointMesh
            Rhino.Geometry.Mesh[] sides = RHOffsetMesh.SplitDisjointPieces();



            //SplitDisjoint Output
            DA.SetDataList(1, sides);
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
            get { return new Guid("68CAF3E1-C553-40F2-A138-4059B4AF9C24"); }
        }
    }
}