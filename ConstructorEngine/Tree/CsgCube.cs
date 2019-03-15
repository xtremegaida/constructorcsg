using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;

namespace ConstructorEngine
{
   public class CsgCube : CsgNode
   {
      public Vector3 Size = new Vector3(1, 1, 1);
      public uint Colour = 0xffffffff;

      public CsgCube() { Name = "Cube"; }

      protected override Mesh3[] GenerateMeshes()
      {
         return new Mesh3[] { new Mesh3(
            new Vector3[] {
               new Vector3(0, 0, 0),
               new Vector3(Size.X, 0, 0),
               new Vector3(Size.X, Size.Y, 0),
               new Vector3(0, Size.Y, 0),
               new Vector3(0, 0, Size.Z),
               new Vector3(Size.X, 0, Size.Z),
               new Vector3(Size.X, Size.Y, Size.Z),
               new Vector3(0, Size.Y, Size.Z),
            },
            new int[] {
               0, 3, 2, 2, 1, 0, // -Z
               4, 5, 6, 6, 7, 4, // +Z
               7, 3, 0, 0, 4, 7, // -X
               6, 5, 1, 1, 2, 6, // +X
               0, 1, 5, 5, 4, 0, // -Y
               2, 3, 7, 7, 6, 2  // +Y
            },
            new uint[] {
               Colour, Colour, Colour, Colour,
               Colour, Colour, Colour, Colour
            }
         ) };
      }

      public override CsgNode Clone()
      {
         return new CsgCube()
         {
            Name = Name,
            Colour = Colour,
            Size = Size
         };
      }
   }
}
