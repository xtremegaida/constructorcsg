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
   public class CsgUnionWrapper : CsgGroupWrapper
   {
      public readonly CsgUnion Object;

      public CsgUnionWrapper(CsgNodeWrapperRepository repo, CsgUnion node)
         : base(repo, node)
      {
         Object = node;
      }
   }
}
