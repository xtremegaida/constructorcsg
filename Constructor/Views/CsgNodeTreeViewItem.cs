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
   public class CsgNodeTreeViewItem : TreeViewItem
   {
      protected readonly ObjectTreeView owner;
      private readonly CsgNodeWrapper node;

      protected virtual TreeViewItemImage HeaderImage { get { return TreeViewItemImage.Graph; } }

      public CsgNodeTreeViewItem(ObjectTreeView owner, CsgNodeWrapper node)
      {
         this.owner = owner;
         this.node = node;
         Header = new TreeViewItemHeader(node, "DisplayName", HeaderImage);
         AllowDrop = false;
      }

      public CsgNodeWrapper Node { get { return (node); } }
      public object Value { get { return (node); } }

      protected override void OnMouseUp(MouseButtonEventArgs e)
      {
         if (e.ChangedButton == MouseButton.Right)
         {
            owner.ShowContextMenu(this);
            IsSelected = true;
            e.Handled = true;
         }
         base.OnMouseUp(e);
      }

      protected override void OnMouseMove(MouseEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Pressed)
         {
            CsgGroupTreeViewItem parent = Parent as CsgGroupTreeViewItem;
            if (parent != null)
            {
               DragDrop.DoDragDrop(this, Node.Id + "|" + parent.Node.Id, DragDropEffects.Move);
               e.Handled = true;
            }
         }
         else if (e.RightButton == MouseButtonState.Pressed)
         {
            CsgGroupTreeViewItem parent = Parent as CsgGroupTreeViewItem;
            if (parent != null)
            {
               DragDrop.DoDragDrop(this, Node.Id + "|" + parent.Node.Id, DragDropEffects.Link);
               e.Handled = true;
            }
         }
         base.OnMouseMove(e);
      }
   }
}
