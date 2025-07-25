using System;
using System.Collections.Generic;
using static MR.DotNet;

namespace MeshLibDemo
{
    public static class Program
    {
        static void Main(string[] args)
        {
            // Example input: user-defined vertices
            var vertices = new List<Vector3f>
            {
                new Vector3f(0f, 0f, 0f),
                new Vector3f(1f, 0f, 0f),
                new Vector3f(0f, 1f, 0f)
            };

            // Example input: user-defined face connectivity
            var triangles = new List<ThreeVertIds>
            {
                new ThreeVertIds(new VertId(0), new VertId(1), new VertId(2))
            };

            // Create mesh from user input
            Mesh mesh = CreateMesh(vertices, triangles);
            Console.WriteLine("Mesh created with " + mesh.Points.Count + " vertices.");
        }

        // Flexible mesh creation method
        public static Mesh CreateMesh(List<Vector3f> vertices, List<ThreeVertIds> triangles)
        {
            return Mesh.FromTriangles(vertices, triangles);
        }
    }
}
