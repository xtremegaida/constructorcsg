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
   public class CsgTranslateWrapper : CsgGroupWrapper
   {
      public readonly CsgTranslate Object;

      public CsgTranslateWrapper(CsgNodeWrapperRepository repo, CsgTranslate node)
         : base(repo, node)
      {
         Object = node;
      }

      public double X { get { return Object.Offset.X; } set { Object.Offset.X = value; OnPropertyChanged("X"); OnMeshChanged(); } }
      public double Y { get { return Object.Offset.Y; } set { Object.Offset.Y = value; OnPropertyChanged("Y"); OnMeshChanged(); } }
      public double Z { get { return Object.Offset.Z; } set { Object.Offset.Z = value; OnPropertyChanged("Z"); OnMeshChanged(); } }
   }
}
