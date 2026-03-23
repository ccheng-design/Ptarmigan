using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using Rhino;
using Rhino.Geometry;

using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;
using System.IO;

using MR;
using static MR.DotNet;

namespace _Ptarmigan
{
    public class MeshBooleanSplit : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MeshBooleanSplit()
          : base("MeshBooleanSplit", "MeshBD",
              "Computes subraction/difference from 2 Meshes",
              "Ptarmigan", "Mesh")
        {
        }

        /// <summary>
        /// Registers all the input parameters for this component.
        /// </summary>
        protected override void RegisterInputParams(GH_Component.GH_InputParamManager pManager)
        {
            pManager.AddMeshParameter("MeshA", "mA", "Set A", GH_ParamAccess.item);
            pManager.AddMeshParameter("MeshB", "mB", "Set B", GH_ParamAccess.item);
        }

        /// <summary>
        /// Registers all the output parameters for this component.
        /// </summary>
        protected override void RegisterOutputParams(GH_Component.GH_OutputParamManager pManager)
        {
            pManager.AddMeshParameter("Bool Mesh", "bM", "Booleaned Mesh", GH_ParamAccess.item);
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
                // If Rhino 8 is available, you can also add:
                // triMesh.ConvertNonPlanarFacesToTriangles(1e-6);

                // Build point list
                var points = new List<MR.DotNet.Vector3f>(triMesh.Vertices.Count);
                foreach (var v in triMesh.Vertices)
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

            Rhino.Geometry.Mesh mA = null;
            Rhino.Geometry.Mesh mB = null;
            DA.GetData(0, ref mA);
            DA.GetData(1, ref mB);

            Rhino.Geometry.Mesh rhMesh = mA as Rhino.Geometry.Mesh;
            Rhino.Geometry.Mesh rhMesh2 = mB as Rhino.Geometry.Mesh;

            //convert rhino to mrdot mesh
            var mlMesh = RhMeshMRMeshConvertors.ToMeshLibMesh(rhMesh);
            //a = mlMesh;

            //convert rhino to mrdot mesh
            var mlMeshB = RhMeshMRMeshConvertors.ToMeshLibMesh(rhMesh2);

            var result = MR.DotNet.Boolean(mlMesh, mlMeshB, MR.DotNet.BooleanOperation.OutsideB);

            Rhino.Geometry.Mesh bM = RhMeshMRMeshConvertors.ToRhinoMesh(result.mesh);

            DA.SetData(0, bM);
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
            get { return new Guid("EA24A2AD-4540-44F7-8AF4-72044C245C69"); }
        }
    }
}