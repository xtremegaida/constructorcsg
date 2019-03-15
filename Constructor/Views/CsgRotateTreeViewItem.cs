using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ConstructorEngine;

namespace Constructor.Views
{
   public class CsgRotateTreeViewItem : CsgGroupTreeViewItem
   {
      private readonly CsgRotateWrapper obj;

      protected override TreeViewItemImage HeaderImage { get { return TreeViewItemImage.Graph; } }

      public CsgRotateTreeViewItem(ObjectTreeView owner, CsgRotateWrapper obj)
         : base(owner, obj)
      {
         this.obj = obj;
      }

      public CsgRotateWrapper Object { get { return (obj); } }
   }
}
