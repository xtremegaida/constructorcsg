using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgIntersect : CsgGroup
   {
      public CsgIntersect() { Name = "Intersect"; }

      public CsgIntersect(params CsgNode[] nodes) : base(nodes) { Name = "Intersect"; }

      protected override Mesh3[] GenerateMeshes()
      {
         Mesh3 result = null;
         foreach (CsgNode node in Children)
         {
            Mesh3[] meshes = node.GetMeshes();
            if (result == null)
            {
               if (meshes == null || meshes.Length == 0) { break; }
               result = meshes[0];
               for (int i = 1; i < meshes.Length; i++)
               {
                  result = CarveSharp.CarveSharp.PerformCSG(result, meshes[i],
                     CarveSharp.CarveSharp.CSGOperations.Union);
               }
            }
            else
            {
               Mesh3 merged;
               if (meshes == null || meshes.Length == 0) { continue; }
               merged = meshes[0];
               for (int i = 1; i < meshes.Length; i++)
               {
                  merged = CarveSharp.CarveSharp.PerformCSG(merged, meshes[i],
                     CarveSharp.CarveSharp.CSGOperations.Union);
               }
               result = CarveSharp.CarveSharp.PerformCSG(result, merged,
                  CarveSharp.CarveSharp.CSGOperations.Intersection);
            }
         }
         if (result == null) { return new Mesh3[0]; }
         return new Mesh3[] { result };
      }

      public override CsgNode Clone()
      {
         return new CsgIntersect(Children.Select(x => x.Clone()).ToArray()) { Name = Name };
      }
   }
}
