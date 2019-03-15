using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace ConstructorEngine
{
   public static class PhysicsExtensions
   {
      public static TriangleMeshShape GetTriangleMeshShape(this Mesh3 mesh)
      {
         Octree octree = new Octree(
            mesh.Vertices.Select(x => new JVector((float)x.X, (float)x.Y, (float)x.Z)).ToList(),
            Enumerable.Range(0, mesh.TriangleCount).Select(x =>
               new TriangleVertexIndices(
                  mesh.TriangleIndices[x * 3 + 2],
                  mesh.TriangleIndices[x * 3 + 1],
                  mesh.TriangleIndices[x * 3])).ToList());
         octree.BuildOctree();
         return new TriangleMeshShape(octree);
      }

      public static RigidBody CreateRigidBody(this Mesh3 mesh)
      {
         return new RigidBody(mesh.GetTriangleMeshShape())
         {
            Position = mesh.Position.ToJVector(),
            Orientation = mesh.Transform.ToJMatrix(),
            Tag = mesh.Clone(),
            AffectedByGravity = true
         };
      }

      public static Matrix3 ToMatrix3(this JMatrix matrix)
      {
         return new Matrix3(
            matrix.M11, matrix.M12, matrix.M13,
            matrix.M21, matrix.M22, matrix.M23,
            matrix.M31, matrix.M32, matrix.M33);
      }

      public static Vector3 ToVector3(this JVector vector)
      {
         return new Vector3(vector.X, vector.Y, vector.Z);
      }

      public static JMatrix ToJMatrix(this Matrix3 matrix)
      {
         return new JMatrix(
            (float)matrix.Right.X, (float)matrix.Right.Y, (float)matrix.Right.Z,
            (float)matrix.Up.X, (float)matrix.Up.Y, (float)matrix.Up.Z,
            (float)matrix.Forward.X, (float)matrix.Forward.Y, (float)matrix.Forward.Z);
      }

      public static JVector ToJVector(this Vector3 vector)
      {
         return new JVector((float)vector.X, (float)vector.Y, (float)vector.Z);
      }
   }
}
