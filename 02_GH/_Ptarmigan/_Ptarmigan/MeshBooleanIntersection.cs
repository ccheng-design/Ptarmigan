extern alias MRDotNet1;
using Grasshopper.Kernel;
using Rhino.Geometry;
using Rhino.Input;
using System;
using System.Collections.Generic;
using static MR;
using static MRDotNet1::MR.DotNet;


namespace _Ptarmigan
{
    public class MeshBooleanIntersect : GH_Component
    {
        /// <summary>
        /// Initializes a new instance of the MyComponent1 class.
        /// </summary>
        public MeshBooleanIntersect()
          : base("MeshBooleanIntersect", "MeshX",
              "Computes a Mesh Intersection",
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
            pManager.AddMeshParameter("Bool Mesh","bM","Booleaned Mesh",GH_ParamAccess.item);
        }

        /// <summary>
        /// This is the method that actually does the work.
        /// </summary>
        /// <param name="DA">The DA object is used to retrieve from inputs and store in outputs.</param>
        /// 

        public static class RhMeshMRMeshConvertors
        {
            public static MR.Const_Mesh RhinoMeshToConstMesh(Rhino.Geometry.Mesh rhMesh)
            {
                var mesh = RhinoMeshToMrMesh(rhMesh);
                return mesh == null ? null : new MR.Const_Mesh(mesh);
            }

            public static MR.Mesh RhinoMeshToMrMesh(Rhino.Geometry.Mesh rhMesh)
            {
                if (rhMesh == null) return null;

                var triMesh = rhMesh.DuplicateMesh();
                triMesh.Faces.ConvertQuadsToTriangles();
                triMesh.Vertices.CombineIdentical(true, true);
                triMesh.Vertices.CullUnused();

                // 1. Vertex coordinates
                var verts = new MR.VertCoords((ulong)triMesh.Vertices.Count);
                for (int i = 0; i < triMesh.Vertices.Count; i++)
                {
                    var v = triMesh.Vertices[i];
                    verts[new MR.VertId(i)] = new MR.Vector3f((float)v.X, (float)v.Y, (float)v.Z);
                }

                // 2. Triangle index container
                var tris = new MR.Triangulation((ulong)triMesh.Faces.Count);

                for (int i = 0; i < triMesh.Faces.Count; i++)
                {
                    var f = triMesh.Faces[i];
                    if (!f.IsTriangle)
                        throw new InvalidOperationException("Mesh must be fully triangulated.");

                    var tri = tris[new MR.FaceId(i)];
                    tri.elems._0 = new MR.VertId(f.A);
                    tri.elems._1 = new MR.VertId(f.B);
                    tri.elems._2 = new MR.VertId(f.C);
                    tris[new MR.FaceId(i)] = tri;
                }

                return MR.Mesh.fromTriangles(verts, tris);
            }
        }

        public static Rhino.Geometry.Mesh MrMeshToRhinoMesh(MR.Mesh mrMesh)
        {
            if (mrMesh == null)
                return null;

            var rhMesh = new Rhino.Geometry.Mesh();

            var verts = mrMesh.points;

            //vertices
            for (ulong i = 0; i < verts.size(); i++)
            {
                var v = verts[new MR.VertId(i)];
            }


            var tris = mrMesh.topology.getTriangulation();
            for (ulong i = 0; i < tris.size(); ++i)
            {
                var tri = tris[new MR.FaceId(i)];

                var va = tri.elems._0;
                var vb = tri.elems._1;
                var vc = tri.elems._2;

                rhMesh.Faces.AddFace(
                    va.id,
                    vb.id,
                    vc.id
                );
            }

            rhMesh.Vertices.CombineIdentical(true, true);
            rhMesh.Vertices.CullUnused();
            rhMesh.Normals.ComputeNormals();
            rhMesh.Compact();


            return rhMesh;
        }

        protected override void SolveInstance(IGH_DataAccess DA)
        {
            Rhino.Geometry.Mesh mA = null;
            Rhino.Geometry.Mesh mB = null;
            DA.GetData(0, ref mA);
            DA.GetData(1, ref mB);

            Rhino.Geometry.Mesh rhMeshA = mA as Rhino.Geometry.Mesh;
            Rhino.Geometry.Mesh rhMeshB = mB as Rhino.Geometry.Mesh;

            
            //convert rhinomesh to mrmesh
            var mrmeshA = RhMeshMRMeshConvertors.RhinoMeshToConstMesh(rhMeshA);
            var mrmeshB = RhMeshMRMeshConvertors.RhinoMeshToConstMesh(rhMeshB);

            MR.BooleanResult resultantBool = MR.boolean(mrmeshA,mrmeshB, MR.BooleanOperation.Intersection);

            //convert back to rhino mesh
            Rhino.Geometry.Mesh bI = MrMeshToRhinoMesh(resultantBool.mesh);


            DA.SetData(0, bI);
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
            get { return new Guid("3E387415-E047-4F55-AF56-C70267326CA2"); }
        }
    }
}