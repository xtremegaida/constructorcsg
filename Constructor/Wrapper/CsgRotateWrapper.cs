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
   public class CsgRotateWrapper : CsgGroupWrapper
   {
      public readonly CsgRotate Object;

      public CsgRotateWrapper(CsgNodeWrapperRepository repo, CsgRotate node)
         : base(repo, node)
      {
         Object = node;
      }

      public double X
      {
         get { return Math.Floor(Object.Angles.X * 18000 / Math.PI) / 100.0; }
         set { Object.Angles.X = value * Math.PI / 180.0; OnPropertyChanged("X"); OnMeshChanged(); }
      }

      public double Y
      {
         get { return Math.Floor(Object.Angles.Y * 18000 / Math.PI) / 100.0; }
         set { Object.Angles.Y = value * Math.PI / 180.0; OnPropertyChanged("Y"); OnMeshChanged(); }
      }

      public double Z
      {
         get { return Math.Floor(Object.Angles.Z * 18000 / Math.PI) / 100.0; }
         set { Object.Angles.Z = value * Math.PI / 180.0; OnPropertyChanged("Z"); OnMeshChanged(); }
      }

      public bool Relative
      {
         get { return Object.Relative; }
         set { Object.Relative = value; OnPropertyChanged("Relative"); OnMeshChanged(); }
      }
   }
}
