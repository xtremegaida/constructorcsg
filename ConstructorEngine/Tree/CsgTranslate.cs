using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgTranslate : CsgGroup
   {
      public Vector3 Offset;

      public CsgTranslate() { Name = "Translate"; }

      public CsgTranslate(params CsgNode[] nodes) : base(nodes) { Name = "Translate"; }

      protected override Mesh3[] GenerateMeshes()
      {
         Mesh3[] meshes = base.GenerateMeshes();
         for (int i = 0; i < meshes.Length; i++)
         {
            Mesh3 mesh = meshes[i].Clone();
            mesh.Position += Offset;
            meshes[i] = mesh;
         }
         return meshes;
      }

      public override CsgNode Clone()
      {
         return new CsgTranslate(Children.Select(x => x.Clone()).ToArray())
         {
            Name = Name,
            Offset = Offset
         };
      }
   }
}
