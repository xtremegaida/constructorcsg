using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class Mesh3
   {
      private static Vector3[] emptyArray = new Vector3[0];

      public readonly Vector3[] Vertices;
      public readonly uint[] VertexColours;
      public readonly int[] TriangleIndices;
      public readonly int TriangleCount;
      public Vector3 Position;
      public Matrix3 Transform = Matrix3.Identity;

      private Vector3[] vertexNormals;

      public Vector3[] VertexNormals { get { return (vertexNormals ?? emptyArray); } }
      public int VertexCount { get { return (Vertices.Length); } }
      public bool HasNormals { get { return (vertexNormals != null && vertexNormals.Length > 0); } }
      public bool HasColours { get { return (VertexColours != null && VertexColours.Length > 0); } }

      public Mesh3(Vector3[] vertices, int[] triangleIndices)
      {
         Vertices = vertices ?? new Vector3[0];
         TriangleIndices = triangleIndices ?? new int[0];
         TriangleCount = (int)(triangleIndices.Length / 3);
      }

      public Mesh3(Vector3[] vertices, int[] triangleIndices, uint[] colours)
         : this(vertices, triangleIndices)
      {
         VertexColours = colours;
      }

      public Mesh3(Vector3[] vertices, int[] triangleIndices, Vector3[] normals)
         : this(vertices, triangleIndices)
      {
         vertexNormals = normals;
      }

      public Mesh3(Vector3[] vertices, int[] triangleIndices, uint[] colours, Vector3[] normals)
         : this(vertices, triangleIndices, colours)
      {
         vertexNormals = normals;
      }

      public Mesh3 Clone(bool deep = false)
      {
         if (deep)
         {
            return new Mesh3(Vertices.ToArray(), TriangleIndices.ToArray(),
               VertexColours != null ? VertexColours.ToArray() : null,
               vertexNormals != null ? vertexNormals.ToArray() : null)
            {
               Position = Position,
               Transform = Transform
            };
         }
         else
         {
            return new Mesh3(Vertices, TriangleIndices, VertexColours, vertexNormals)
            {
               Position = Position,
               Transform = Transform
            };
         }
      }

      public void CalculateNormals()
      {
         vertexNormals = new Vector3[Vertices.Length];
         for (int i = TriangleCount - 1; i >= 0; i--)
         {
            int o = i * 3;
            int i0 = TriangleIndices[o];
            int i1 = TriangleIndices[o + 1];
            int i2 = TriangleIndices[o + 2];
            Vector3 a = Vertices[i0];
            Vector3 b = Vertices[i1];
            Vector3 c = Vertices[i2];
            Vector3 normal = (b - a).CrossProduct(c - a).Normalise();
            vertexNormals[i0] += normal;
            vertexNormals[i1] += normal;
            vertexNormals[i2] += normal;
         }
         Parallel.For(0, vertexNormals.Length,
            new Action<int>(i => { vertexNormals[i] = vertexNormals[i].Normalise(); }));
      }

      public Vector3[] GetTransformedVertices()
      {
         Vector3[] transformed = new Vector3[Vertices.Length];
         if (Transform == Matrix3.Identity)
         {
            Parallel.For(0, Vertices.Length,
               new Action<int>(i => { transformed[i] = Vertices[i] + Position; }));
         }
         else
         {
            Parallel.For(0, Vertices.Length,
               new Action<int>(i => { transformed[i] = Vertices[i] * Transform + Position; }));
         }
         return transformed;
      }

      public Vector3[] GetTransformedNormals()
      {
         Vector3[] transformed = vertexNormals;
         if (vertexNormals != null && Transform != Matrix3.Identity)
         {
            transformed = new Vector3[vertexNormals.Length];
            Parallel.For(0, vertexNormals.Length,
               new Action<int>(i => { transformed[i] = vertexNormals[i] * Transform; }));
         }
         return transformed ?? emptyArray;
      }
   }
}
