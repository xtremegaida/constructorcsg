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
   public class CsgGroupTreeViewItem : CsgNodeTreeViewItem
   {
      private readonly CsgGroupWrapper group;
      private readonly Dictionary<CsgNodeWrapper, CsgNodeTreeViewItem> nodes = new Dictionary<CsgNodeWrapper, CsgNodeTreeViewItem>();

      protected override TreeViewItemImage HeaderImage { get { return TreeViewItemImage.Folder; } }

      public CsgGroupTreeViewItem(ObjectTreeView owner, CsgGroupWrapper group, bool expand = true)
         : base(owner, group)
      {
         this.group = group;
         RefreshChildren();
         group.PropertyChanged += NodePropertyChanged;
         IsExpanded = expand;
         AllowDrop = true;
      }

      public CsgGroupWrapper Group { get { return (group); } }

      public void RefreshChildren()
      {
         List<CsgNodeTreeViewItem> newNodes = new List<CsgNodeTreeViewItem>();
         foreach (CsgNodeWrapper wrapper in group.Children)
         {
            CsgNodeTreeViewItem view;
            if (!nodes.TryGetValue(wrapper, out view))
            {
               if (wrapper is CsgCubeWrapper) { view = new CsgCubeTreeViewItem(owner, (CsgCubeWrapper)wrapper); }
               else if (wrapper is CsgTranslateWrapper) { view = new CsgTranslateTreeViewItem(owner, (CsgTranslateWrapper)wrapper); }
               else if (wrapper is CsgScaleWrapper) { view = new CsgScaleTreeViewItem(owner, (CsgScaleWrapper)wrapper); }
               else if (wrapper is CsgRotateWrapper) { view = new CsgRotateTreeViewItem(owner, (CsgRotateWrapper)wrapper); }
               else if (wrapper is CsgUnionWrapper) { view = new CsgUnionTreeViewItem(owner, (CsgUnionWrapper)wrapper); }
               else if (wrapper is CsgSubtractWrapper) { view = new CsgSubtractTreeViewItem(owner, (CsgSubtractWrapper)wrapper); }
               else if (wrapper is CsgIntersectWrapper) { view = new CsgIntersectTreeViewItem(owner, (CsgIntersectWrapper)wrapper); }
               else if (wrapper is CsgGroupWrapper) { view = new CsgGroupTreeViewItem(owner, (CsgGroupWrapper)wrapper); }
            }
            if (view == null) { view = new CsgNodeTreeViewItem(owner, wrapper); }
            newNodes.Add(view);
         }
         nodes.Clear();
         Items.Clear();
         foreach (CsgNodeTreeViewItem node in newNodes)
         {
            Items.Add(node);
            nodes[node.Node] = node;
         }
      }

      private bool CanDrop(string idString)
      {
         string[] ids = (idString ?? string.Empty).Split('|');
         if (ids.Length == 2)
         {
            CsgNodeWrapper wrapper = Node.Repository.GetWrapperById(ids[0]);
            if (wrapper != null)
            {
               CsgNodeTreeViewItem item = this;
               while (item != null)
               {
                  if (item.Node == wrapper) { return false; }
                  item = item.Parent as CsgNodeTreeViewItem;
               }
               if (item == null) { return true; }
            }
         }
         return false;
      }

      protected override void OnDragEnter(DragEventArgs e)
      {
         if (!CanDrop(e.Data.GetData(DataFormats.Text) as string)) { e.Effects = DragDropEffects.None; }
         e.Handled = true;
         base.OnDragEnter(e);
      }

      protected override void OnDragOver(DragEventArgs e)
      {
         if (!CanDrop(e.Data.GetData(DataFormats.Text) as string)) { e.Effects = DragDropEffects.None; }
         e.Handled = true;
         base.OnDragOver(e);
      }

      protected override void OnDrop(DragEventArgs e)
      {
         string[] ids = (e.Data.GetData(DataFormats.Text) as string ?? string.Empty).Split('|');
         e.Handled = true;
         if (ids.Length == 2)
         {
            CsgNodeWrapper wrapper = Node.Repository.GetWrapperById(ids[0]);
            if (e.Effects == DragDropEffects.Move)
            {
               CsgGroupWrapper parent = Node.Repository.GetWrapperById(ids[1]) as CsgGroupWrapper;
               if (wrapper != null && parent != null)
               {
                  CsgNodeTreeViewItem item = this;
                  while (item != null)
                  {
                     if (item.Node == wrapper) { return; }
                     item = item.Parent as CsgNodeTreeViewItem;
                  }
                  if (parent.RemoveChild(wrapper))
                  {
                     if (!Group.AddChild(wrapper)) { parent.AddChild(wrapper); }
                  }
                  IsSelected = true;
               }
            }
            else if (e.Effects == DragDropEffects.Link)
            {
               if (wrapper != null)
               {
                  Group.AddChild(wrapper);
                  IsSelected = true;
               }
            }
         }
         base.OnDrop(e);
      }

      private void NodePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
      {
         if (e.PropertyName == "Children") { RefreshChildren(); }
      }
   }

   public static class TreeViewItemExtensions
   {
      public static void ExpandBubbleUp(this TreeViewItem item)
      {
         while ((item = item.Parent as TreeViewItem) != null) { item.IsExpanded = true; }
      }
   }
}
