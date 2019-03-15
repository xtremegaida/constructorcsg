using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ConstructorEngine;

namespace Constructor
{
   public class CsgCubeWrapper : CsgNodeWrapper
   {
      public readonly CsgCube Object;

      public CsgCubeWrapper(CsgNodeWrapperRepository repo, CsgCube node)
         : base(repo, node)
      {
         Object = node;
      }

      public double SizeX { get { return Object.Size.X; } set { Object.Size.X = value; OnPropertyChanged("SizeX"); OnMeshChanged(); } }
      public double SizeY { get { return Object.Size.Y; } set { Object.Size.Y = value; OnPropertyChanged("SizeY"); OnMeshChanged(); } }
      public double SizeZ { get { return Object.Size.Z; } set { Object.Size.Z = value; OnPropertyChanged("SizeZ"); OnMeshChanged(); } }

      public Color Colour
      {
         get
         {
            return Color.FromRgb((byte)((Object.Colour >> 16) & 0xff),
               (byte)((Object.Colour >> 8) & 0xff), (byte)(Object.Colour & 0xff));
         }
         set
         {
            Object.Colour = ((uint)value.R << 16) + ((uint)value.G << 8) + (uint)value.B;
            OnPropertyChanged("Colour");
            OnMeshChanged();
         }
      }
   }
}
