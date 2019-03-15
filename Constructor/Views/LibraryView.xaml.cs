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
using Xceed.Wpf.AvalonDock.Layout;

namespace Constructor.Views
{
   /// <summary>
   /// Interaction logic for LibraryView.xaml
   /// </summary>
   public partial class LibraryView : UserControl
   {
      public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register
         ("SelectedItem", typeof(CsgNodeWrapperRepository), typeof(LibraryView), new PropertyMetadata(PropertyChanged));

      public CsgNodeWrapperRepository SelectedItem
      {
         get { return ((CsgNodeWrapperRepository)GetValue(SelectedItemProperty)); }
         set { SetValue(SelectedItemProperty, value); }
      }

      public LayoutDocumentPane ObjectDocumentPane;

      private HashSet<LayoutDocument> openDocuments = new HashSet<LayoutDocument>();
      private CsgNodeWrapperRepository library;

      public LibraryView()
      {
         InitializeComponent();
         objectList.MouseDoubleClick += ShowItem;
         objectList.MouseMove += StartDrag;
      }

      private void LoadLibrary(CsgNodeWrapperRepository library)
      {
         foreach (ObjectDocument doc in openDocuments.ToArray()) { doc.Close(); }
         openDocuments.Clear();
         this.library = library;
         objectList.DisplayMemberPath = "Name";
         BindingOperations.ClearBinding(objectList, ListBox.ItemsSourceProperty);
         BindingOperations.SetBinding(objectList, ListBox.ItemsSourceProperty,
            new Binding("ObjectRoots") { Source = library, Mode = BindingMode.OneWay });
         if (objectList.Items.Count > 0)
         {
            objectList.SelectedIndex = 0;
            ShowItem(objectList, null);
         }
      }

      private void RunExecuted(object sender, ExecutedRoutedEventArgs e)
      {
         CsgGroupWrapper node = objectList.SelectedItem as CsgGroupWrapper;
         if (node != null) { new PhysicsDocument(this, node); }
      }

      private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
      {
         if (args.Property == SelectedItemProperty)
         {
            ((LibraryView)sender).LoadLibrary(args.NewValue as CsgNodeWrapperRepository);
         }
      }

      private void StartDrag(object sender, MouseEventArgs e)
      {
         if (e.LeftButton == MouseButtonState.Pressed)
         {
            CsgNodeWrapper selected = objectList.SelectedItem as CsgNodeWrapper;
            if (selected != null)
            {
               DragDrop.DoDragDrop(objectList, selected.Id + "|0", DragDropEffects.Link);
               e.Handled = true;
            }
         }
      }

      private void ShowItem(object sender, RoutedEventArgs e)
      {
         CsgGroupWrapper selected = objectList.SelectedItem as CsgGroupWrapper;
         if (selected != null)
         {
            ObjectDocument doc;
            foreach (LayoutDocument o in openDocuments)
            {
               doc = o as ObjectDocument;
               if (doc == null) { continue; }
               if (doc.Node == selected)
               {
                  if (!doc.IsFloating && !ObjectDocumentPane.Children.Contains(o))
                  {
                     ObjectDocumentPane.Children.Add(o);
                  }
                  doc.IsActive = true;
                  if (doc.Content is UIElement) { ((UIElement)doc.Content).Focus(); }
                  return;
               }
            }
            doc = new ObjectDocument(this, selected);
            doc.IsActive = true;
            if (doc.Content is UIElement) { ((UIElement)doc.Content).Focus(); }
         }
      }

      private void DeleteClick(object sender, RoutedEventArgs e)
      {
         CsgNodeWrapper selected = objectList.SelectedItem as CsgNodeWrapper;
         if (selected != null) { selected.IsObjectRoot = false; }
      }

      public void RegisterOpenDocument(LayoutDocument doc)
      {
         openDocuments.Add(doc);
         doc.Closed += (se, ev) => { openDocuments.Remove(doc); };
         ObjectDocumentPane.Children.Add(doc);
         doc.IsActive = true;
      }
   }
}
