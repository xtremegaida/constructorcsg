using System;
using System.Collections.Generic;
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
   /// <summary>
   /// Interaction logic for ObjectTreeView.xaml
   /// </summary>
   public partial class ObjectTreeView : UserControl
   {
      public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register
         ("SelectedItem", typeof(CsgGroupWrapper), typeof(ObjectTreeView), new PropertyMetadata(PropertyChanged));
      public static readonly DependencyProperty SelectedNodeValueProperty = DependencyProperty.Register
         ("SelectedNodeValue", typeof(object), typeof(ObjectTreeView));

      public CsgGroupWrapper SelectedItem
      {
         get { return ((CsgGroupWrapper)GetValue(SelectedItemProperty)); }
         set { SetValue(SelectedItemProperty, value); }
      }

      public object SelectedNodeValue
      {
         get { return (GetValue(SelectedNodeValueProperty)); }
         set { SetValue(SelectedNodeValueProperty, value); }
      }

      public Panel Toolbar { get { return ButtonPanel; } }

      public LibraryView Owner { get { return (owner); } set { owner = value; } }

      private CsgNodeTreeViewItem root;
      private ContextMenu addMenu;
      private ContextMenu generalMenu;
      private LibraryView owner;

      public ObjectTreeView()
      {
         InitializeComponent();
         SetBinding(SelectedNodeValueProperty,
            new Binding("SelectedValue") { Source = nodeView, Mode = BindingMode.OneWay });

         addMenu = new ContextMenu();
         BuildAddMenu(addMenu);

         generalMenu = new ContextMenu();
         BuildGeneralMenu(generalMenu);
      }

      private void BuildAddMenu(ItemsControl parent)
      {
         MenuItem item = new MenuItem() { Header = "Group" };
         item.Click += (s, e) => { AddNode(new CsgGroup()); };
         parent.Items.Add(item);
         item = new MenuItem() { Header = "Geometry" };
         parent.Items.Add(item);
         MenuItem subItem = new MenuItem() { Header = "Cube" };
         subItem.Click += (s, e) => { AddNode(new CsgCube()); };
         item.Items.Add(subItem);
         item = new MenuItem() { Header = "Transform" };
         parent.Items.Add(item);
         subItem = new MenuItem() { Header = "Translate" };
         subItem.Click += (s, e) => { AddNode(new CsgTranslate()); };
         item.Items.Add(subItem);
         subItem = new MenuItem() { Header = "Scale" };
         subItem.Click += (s, e) => { AddNode(new CsgScale()); };
         item.Items.Add(subItem);
         subItem = new MenuItem() { Header = "Rotate" };
         subItem.Click += (s, e) => { AddNode(new CsgRotate()); };
         item.Items.Add(subItem);
         item = new MenuItem() { Header = "CSG" };
         parent.Items.Add(item);
         subItem = new MenuItem() { Header = "Union" };
         subItem.Click += (s, e) => { AddNode(new CsgUnion()); };
         item.Items.Add(subItem);
         subItem = new MenuItem() { Header = "Difference" };
         subItem.Click += (s, e) => { AddNode(new CsgSubtract()); };
         item.Items.Add(subItem);
         subItem = new MenuItem() { Header = "Intersect" };
         subItem.Click += (s, e) => { AddNode(new CsgIntersect()); };
         item.Items.Add(subItem);
      }

      private void BuildGeneralMenu(ItemsControl parent)
      {
         MenuItem item = new MenuItem() { Header = "Add" };
         BuildAddMenu(item);
         parent.Items.Add(item);
         parent.Items.Add(new Separator());
         item = new MenuItem() { Header = "View Physics" };
         item.Click += (s, e) => { ViewNodePhysics(); };
         parent.Items.Add(item);
         parent.Items.Add(new Separator());
         item = new MenuItem() { Header = "Duplicate" };
         item.Click += (s, e) => { DuplicateNode(); };
         parent.Items.Add(item);
         item = new MenuItem() { Header = "Make Object" };
         item.Click += (s, e) => { MakeObjectNode(); };
         parent.Items.Add(item);
         parent.Items.Add(new Separator());
         item = new MenuItem() { Header = "Move First" };
         item.Click += (s, e) => { MoveNodeFirst(); };
         parent.Items.Add(item);
         item = new MenuItem() { Header = "Move Up" };
         item.Click += (s, e) => { MoveNodeUp(); };
         parent.Items.Add(item);
         item = new MenuItem() { Header = "Move Down" };
         item.Click += (s, e) => { MoveNodeDown(); };
         parent.Items.Add(item);
         item = new MenuItem() { Header = "Move Last" };
         item.Click += (s, e) => { MoveNodeLast(); };
         parent.Items.Add(item);
         parent.Items.Add(new Separator());
         item = new MenuItem() { Header = "Delete" };
         item.Click += (s, e) => { DeleteNode(); };
         parent.Items.Add(item);
      }

      private void AddNode(CsgNode node)
      {
         if (root == null) { return; }
         CsgNodeWrapper wrapper = root.Node.Repository.GetWrapper(node);
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         CsgGroupWrapper selected = null;
         while (item != null)
         {
            selected = item.Node as CsgGroupWrapper;
            if (selected != null) { break; }
            item = item.Parent as CsgNodeTreeViewItem;
         }
         if (selected == null) { selected = root.Node as CsgGroupWrapper; }
         if (selected != null) { selected.AddChild(wrapper); }
      }

      private void DuplicateNode()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         int newIndex; for (newIndex = parent.Group.ChildrenCount - 1; newIndex >= 0; newIndex--)
         {
            if (parent.Group.Group[newIndex] == item.Node.Node) { break; }
         }
         if (newIndex < 0) { newIndex = parent.Group.ChildrenCount; } else { newIndex++; }
         CsgNodeWrapper clone = item.Node.Clone();
         parent.Group.AddChild(clone);
         if (newIndex != (parent.Group.ChildrenCount - 1)) { parent.Group.MoveChild(clone, newIndex); }
         parent.Items.Cast<CsgNodeTreeViewItem>().First(x => x.Node == clone).IsSelected = true;
      }

      private void MakeObjectNode()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         if (item.Node is CsgGroupWrapper) { item.Node.IsObjectRoot = true; }
      }

      private void ViewNodePhysics()
      {
         if (owner == null) { return; }
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { if (SelectedItem != null) { new PhysicsDocument(owner, SelectedItem); } return; }
         if (item.Node is CsgGroupWrapper) { new PhysicsDocument(owner, (CsgGroupWrapper)item.Node); }
      }

      private void MoveNodeFirst()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         parent.Group.MoveChild(item.Node, 0);
      }

      private void MoveNodeUp()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         int newIndex; for (newIndex = parent.Group.ChildrenCount - 1; newIndex >= 0; newIndex--)
         {
            if (parent.Group.Group[newIndex] == item.Node.Node) { break; }
         }
         newIndex--;
         if (newIndex < 0) { newIndex = 0; }
         parent.Group.MoveChild(item.Node, newIndex);
         item.IsSelected = true;
      }

      private void MoveNodeDown()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         int newIndex; for (newIndex = parent.Group.ChildrenCount - 1; newIndex >= 0; newIndex--)
         {
            if (parent.Group.Group[newIndex] == item.Node.Node) { break; }
         }
         newIndex++;
         if (newIndex < 0) { newIndex = 0; }
         parent.Group.MoveChild(item.Node, newIndex);
         item.IsSelected = true;
      }

      private void MoveNodeLast()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         parent.Group.MoveChild(item.Node, parent.Group.ChildrenCount - 1);
      }

      private void MoveNodeLeft()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         CsgGroupTreeViewItem superParent = parent.Parent as CsgGroupTreeViewItem;
         if (superParent == null) { return; }
         int newIndex; for (newIndex = superParent.Group.ChildrenCount - 1; newIndex >= 0; newIndex--)
         {
            if (superParent.Group.Group[newIndex] == parent.Node.Node) { break; }
         }
         if (newIndex <= 0) { newIndex = superParent.Group.ChildrenCount - 1; }
         parent.Group.RemoveChild(item.Node);
         superParent.Group.AddChild(item.Node);
         newIndex++; if (newIndex >= superParent.Group.ChildrenCount) { newIndex = superParent.Group.ChildrenCount - 1; }
         superParent.Group.MoveChild(item.Node, newIndex);
         parent.IsExpanded = false;
         superParent.Items.Cast<CsgNodeTreeViewItem>().First(x => x.Node == item.Node).IsSelected = true;
      }

      private void MoveNodeRight()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         int newIndex; for (newIndex = parent.Group.ChildrenCount - 1; newIndex >= 0; newIndex--)
         {
            if (parent.Group.Group[newIndex] == item.Node.Node) { break; }
         }
         if (newIndex <= 0) { return; }
         CsgGroupTreeViewItem newParent = null;
         if (newIndex > 0) { newParent = parent.Items[newIndex - 1] as CsgGroupTreeViewItem; }
         if (newParent == null && (newIndex + 1) < parent.Items.Count) { newParent = parent.Items[newIndex + 1] as CsgGroupTreeViewItem; }
         if (newParent != null)
         {
            parent.Group.RemoveChild(item.Node);
            newParent.Group.AddChild(item.Node);
            parent.IsExpanded = true;
            newParent.IsExpanded = true;
            item = newParent.Items.Cast<CsgNodeTreeViewItem>().First(x => x.Node == item.Node);
            item.IsSelected = true;
            item.Focus();
         }
      }

      private void DeleteNode()
      {
         CsgNodeTreeViewItem item = nodeView.SelectedItem as CsgNodeTreeViewItem;
         if (item == null) { return; }
         CsgGroupTreeViewItem parent = item.Parent as CsgGroupTreeViewItem;
         if (parent == null) { return; }
         parent.Group.RemoveChild(item.Node);
         RecursiveDelete(item.Node as CsgGroupWrapper);
      }

      private void RecursiveDelete(CsgGroupWrapper group)
      {
         if (group == null) { return; }
         if (group.ReferenceCount > 0) { return; }
         if (group.IsObjectRoot) { return; }
         group.Repository.RemoveNode(group);
         foreach (var item in group.Children.ToArray())
         {
            group.RemoveChild(item);
            RecursiveDelete(item as CsgGroupWrapper);
         }
      }

      private void RunClick(object sender, RoutedEventArgs e)
      {
         CsgGroupWrapper node = SelectedItem;
         if (node != null) { new PhysicsDocument(owner, node); }
      }

      private void AddClick(object sender, RoutedEventArgs e)
      {
         addMenu.PlacementTarget = sender as UIElement;
         addMenu.IsOpen = true;
         e.Handled = true;
      }

      private void RemoveClick(object sender, RoutedEventArgs e)
      {
         DeleteNode();
      }

      private void CopyClick(object sender, RoutedEventArgs e)
      {
         DuplicateNode();
      }

      public void ShowContextMenu(UIElement element)
      {
         generalMenu.PlacementTarget = element;
         generalMenu.IsOpen = true;
      }

      private void LoadObject(CsgGroupWrapper node)
      {
         nodeView.Items.Clear();
         this.root = null;
         if (node != null)
         {
            nodeView.Items.Add((this.root = new CsgGroupTreeViewItem(this, node)));
            this.root.IsSelected = true;
         }
      }

      protected override void OnPreviewKeyDown(KeyEventArgs e)
      {
         if (e.Key == Key.Delete) { DeleteNode(); e.Handled = true; }
         if (e.Key == Key.Up || e.Key == Key.Down || e.Key == Key.Left || e.Key == Key.Right)
         {
            if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) ||
                e.KeyboardDevice.IsKeyDown(Key.RightShift))
            {
               e.Handled = true;
               switch (e.Key)
               {
                  case Key.Up: MoveNodeUp(); break;
                  case Key.Down: MoveNodeDown(); break;
                  case Key.Left: MoveNodeLeft(); break;
                  case Key.Right: MoveNodeRight(); break;
                  default: break;
               }
            }
         }
         base.OnPreviewKeyDown(e);
      }

      private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
      {
         if (args.Property == SelectedItemProperty)
         {
            ((ObjectTreeView)sender).LoadObject(args.NewValue as CsgGroupWrapper);
         }
      }
   }
}
