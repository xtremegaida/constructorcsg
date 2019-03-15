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
   public class CsgScaleWrapper : CsgGroupWrapper
   {
      public readonly CsgScale Object;

      public CsgScaleWrapper(CsgNodeWrapperRepository repo, CsgScale node)
         : base(repo, node)
      {
         Object = node;
      }

      public double X { get { return Object.Scale.X; } set { Object.Scale.X = value; OnPropertyChanged("X"); OnMeshChanged(); } }
      public double Y { get { return Object.Scale.Y; } set { Object.Scale.Y = value; OnPropertyChanged("Y"); OnMeshChanged(); } }
      public double Z { get { return Object.Scale.Z; } set { Object.Scale.Z = value; OnPropertyChanged("Z"); OnMeshChanged(); } }
   }
}
