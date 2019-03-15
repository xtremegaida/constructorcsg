using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgRotate : CsgGroup
   {
      public Vector3 Angles;
      public bool Relative = true;

      public CsgRotate() { Name = "Rotate"; }

      public CsgRotate(params CsgNode[] nodes) : base(nodes) { Name = "Rotate"; }

      protected override Mesh3[] GenerateMeshes()
      {
         Mesh3[] meshes = base.GenerateMeshes();
         Matrix3 matrix = Matrix3.Identity;
         if (Relative)
         {
            if (Angles.X != 0) { matrix = matrix.RotateRelX(Angles.X); }
            if (Angles.Y != 0) { matrix = matrix.RotateRelY(Angles.Y); }
            if (Angles.Z != 0) { matrix = matrix.RotateRelZ(Angles.Z); }
         }
         else
         {
            if (Angles.X != 0) { matrix = matrix.RotateAbsX(Angles.X); }
            if (Angles.Y != 0) { matrix = matrix.RotateAbsY(Angles.Y); }
            if (Angles.Z != 0) { matrix = matrix.RotateAbsZ(Angles.Z); }
         }
         for (int i = 0; i < meshes.Length; i++)
         {
            Mesh3 mesh = meshes[i].Clone();
            mesh.Transform = matrix.Multiply(mesh.Transform);
            mesh.Position = matrix.Transform(mesh.Position);
            meshes[i] = mesh;
         }
         return meshes;
      }

      public override CsgNode Clone()
      {
         return new CsgRotate(Children.Select(x => x.Clone()).ToArray())
         {
            Name = Name,
            Angles = Angles,
            Relative = Relative
         };
      }
   }
}
