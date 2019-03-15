using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgSubtract : CsgGroup
   {
      public CsgSubtract() { Name = "Subtract"; }

      public CsgSubtract(params CsgNode[] nodes) : base(nodes) { Name = "Subtract"; }

      protected override Mesh3[] GenerateMeshes()
      {
         Mesh3 result = null;
         foreach (CsgNode node in Children)
         {
            Mesh3[] meshes = node.GetMeshes();
            if (result == null)
            {
               if (meshes == null || meshes.Length == 0) { break; }
               if (meshes.Length == 1) { result = meshes[0]; continue; }
               result = meshes[0];
               for (int i = 1; i < meshes.Length; i++)
               {
                  result = CarveSharp.CarveSharp.PerformCSG(result, meshes[i],
                     CarveSharp.CarveSharp.CSGOperations.Union);
               }
            }
            else
            {
               for (int i = 0; i < meshes.Length; i++)
               {
                  result = CarveSharp.CarveSharp.PerformCSG(result, meshes[i],
                     CarveSharp.CarveSharp.CSGOperations.AMinusB);
               }
            }
         }
         if (result == null) { return new Mesh3[0]; }
         return new Mesh3[] { result };
      }

      public override CsgNode Clone()
      {
         return new CsgSubtract(Children.Select(x => x.Clone()).ToArray()) { Name = Name };
      }
   }
}
