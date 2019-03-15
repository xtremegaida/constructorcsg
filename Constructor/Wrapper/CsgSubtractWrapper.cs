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
   public class CsgSubtractWrapper : CsgGroupWrapper
   {
      public readonly CsgSubtract Object;

      public CsgSubtractWrapper(CsgNodeWrapperRepository repo, CsgSubtract node)
         : base(repo, node)
      {
         Object = node;
      }
   }
}
