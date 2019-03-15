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
   public class CsgIntersectWrapper : CsgGroupWrapper
   {
      public readonly CsgIntersect Object;

      public CsgIntersectWrapper(CsgNodeWrapperRepository repo, CsgIntersect node)
         : base(repo, node)
      {
         Object = node;
      }
   }
}
