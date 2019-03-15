using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgScale : CsgGroup
   {
      public Vector3 Scale = new Vector3(1, 1, 1);

      public CsgScale() { Name = "Scale"; }

      public CsgScale(params CsgNode[] nodes) : base(nodes) { Name = "Scale"; }

      protected override Mesh3[] GenerateMeshes()
      {
         Mesh3[] meshes = base.GenerateMeshes();
         for (int i = 0; i < meshes.Length; i++)
         {
            Mesh3 mesh = meshes[i].Clone();
            mesh.Transform = Matrix3.Scale(Scale).Multiply(mesh.Transform);
            mesh.Position = mesh.Position.Scale(Scale);
            meshes[i] = mesh;
         }
         return meshes;
      }

      public override CsgNode Clone()
      {
         return new CsgScale(Children.Select(x => x.Clone()).ToArray())
         {
            Name = Name,
            Scale = Scale
         };
      }
   }
}
