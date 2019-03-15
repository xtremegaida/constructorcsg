using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace ConstructorEngine
{
   public class PhysicsSimulation
   {
      public readonly World World = new World(new CollisionSystemSAP());
      public float DeltaTime = 0.01f;

      public PhysicsSimulation()
      {
      }

      public void SetRootNode(CsgNode node)
      {
         float minX = 0, minY = 0, minZ = 0;
         float maxX = 0, maxY = 0, maxZ = 0;
         bool first = true;

         World.Clear();
         node.PopulateWorld(World);

         World.Gravity = new JVector(0, -1, 0);
         foreach (RigidBody body in World.RigidBodies)
         {
            JBBox box = body.BoundingBox;
            if (first)
            {
               first = false;
               minX = box.Min.X; minY = box.Min.Y; minZ = box.Min.Z;
               maxX = box.Max.X; maxY = box.Max.Y; maxZ = box.Max.Z;
            }
            else
            {
               if (minX > box.Min.X) { minX = box.Min.X; }
               if (maxX < box.Max.X) { maxX = box.Max.X; }
               if (minY > box.Min.Y) { minY = box.Min.Y; }
               if (maxY < box.Max.Y) { maxY = box.Max.Y; }
               if (minZ > box.Min.Z) { minZ = box.Min.Z; }
               if (maxZ < box.Max.Z) { maxZ = box.Max.Z; }
            }
         }
         RigidBody floor = new RigidBody(new BoxShape(new JVector((maxX - minX) * 3 + 1000000, 100, (maxZ - minZ) * 3 + 1000000)));
         floor.Position = new JVector((minX + maxX)  * 0.5f, minY - 51, (minZ + maxZ) * 0.5f);
         floor.IsStatic = true;
         World.AddBody(floor);
      }

      public void Run()
      {
         World.Step(DeltaTime, true);
         foreach (RigidBody body in World.RigidBodies)
         {
            Mesh3 mesh = body.Tag as Mesh3;
            if (mesh == null) { continue; }
            mesh.Position = body.Position.ToVector3();
            mesh.Transform = body.Orientation.ToMatrix3();
         }
      }

      public Mesh3[] GetMeshes()
      {
         List<Mesh3> meshes = new List<Mesh3>();
         foreach (RigidBody body in World.RigidBodies)
         {
            if (body.Tag is Mesh3) { meshes.Add((Mesh3)body.Tag); }
         }
         return meshes.ToArray();
      }
   }
}
