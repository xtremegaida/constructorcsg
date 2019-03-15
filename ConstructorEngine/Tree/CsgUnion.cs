using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgUnion : CsgGroup
   {
      public CsgUnion() { Name = "Union"; }

      public CsgUnion(params CsgNode[] nodes) : base(nodes) { Name = "Union"; }

      protected override Mesh3[] GenerateMeshes()
      {
         Mesh3[] meshes = base.GenerateMeshes();
         if (meshes.Length <= 1) { return meshes; }
         Mesh3 result = meshes[0];
         for (int i = 1; i < meshes.Length; i++)
         {
            result = CarveSharp.CarveSharp.PerformCSG(result, meshes[i],
               CarveSharp.CarveSharp.CSGOperations.Union);
         }
         return new Mesh3[] { result };
      }

      public override CsgNode Clone()
      {
         return new CsgUnion(Children.Select(x => x.Clone()).ToArray()) { Name = Name };
      }
   }
}
