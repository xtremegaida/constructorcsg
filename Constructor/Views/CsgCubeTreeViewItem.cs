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
   public class CsgCubeTreeViewItem : CsgNodeTreeViewItem
   {
      private readonly CsgCubeWrapper obj;

      protected override TreeViewItemImage HeaderImage { get { return TreeViewItemImage.Input; } }

      public CsgCubeTreeViewItem(ObjectTreeView owner, CsgCubeWrapper obj)
         : base(owner, obj)
      {
         this.obj = obj;
      }

      public CsgCubeWrapper Object { get { return (obj); } }
   }
}
