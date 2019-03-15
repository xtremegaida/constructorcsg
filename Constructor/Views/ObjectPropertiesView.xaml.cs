using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Constructor.Views
{
   /// <summary>
   /// Interaction logic for ObjectPropertiesView.xaml
   /// </summary>
   public partial class ObjectPropertiesView : UserControl
   {
      public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register
         ("SelectedItem", typeof(CsgNodeWrapper), typeof(ObjectPropertiesView), new PropertyMetadata(PropertyChanged));

      public CsgNodeWrapper SelectedItem
      {
         get { return (CsgNodeWrapper)GetValue(SelectedItemProperty); }
         set { SetValue(SelectedItemProperty, value); }
      }

      public ObjectPropertiesView()
      {
         InitializeComponent();
      }

      private void ClearBindings(DependencyObject obj)
      {
         if (obj == null) { return; }
         BindingOperations.ClearAllBindings(obj);
         if (obj is Panel) { foreach (UIElement element in ((Panel)obj).Children) { ClearBindings(element); } }
         else if (obj is ItemsControl) { foreach (UIElement element in ((ItemsControl)obj).Items) { ClearBindings(element); } }
         else if (obj is ContentControl) { ClearBindings(((ContentControl)obj).Content as DependencyObject); }
      }

      private void UpdateProperties(CsgNodeWrapper node)
      {
         foreach (UIElement element in PropertiesPanel.Children) { ClearBindings(element); }
         PropertiesPanel.Children.Clear();
         if (node == null) { return; }
         foreach (PropertyInfo property in node.GetType().GetProperties().OrderBy(x => x.Name))
         {
            if (!property.CanWrite || !property.CanRead) { continue; }
            DockPanel panel = new DockPanel();
            panel.Margin = new Thickness(5, 5, 5, 0);
            Label label = new Label();
            label.Content = property.Name;
            label.SetValue(DockPanel.DockProperty, Dock.Left);
            label.Margin = new Thickness(0, 0, 10, 0);
            label.HorizontalContentAlignment = HorizontalAlignment.Right;
            label.VerticalContentAlignment = VerticalAlignment.Center;
            label.Width = 100;
            panel.Children.Add(label);
            UIElement control = CreateControl(node, property);
            panel.Children.Add(control);
            PropertiesPanel.Children.Add(panel);
         }
      }

      private UIElement CreateControl(CsgNodeWrapper wrapper, PropertyInfo property)
      {
         if (property.PropertyType == typeof(bool))
         {
            CheckBox check = new CheckBox();
            check.SetBinding(CheckBox.IsCheckedProperty,
               new Binding(property.Name) { Source = wrapper, Mode = BindingMode.TwoWay });
            return (check);
         }
         TextBox text = new TextBox();
         text.SetBinding(TextBox.TextProperty,
            new Binding(property.Name) { Source = wrapper, Mode = BindingMode.TwoWay });
         return (text);
      }

      private static void PropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
      {
         if (args.Property == SelectedItemProperty)
         {
            ((ObjectPropertiesView)sender).UpdateProperties(args.NewValue as CsgNodeWrapper);
         }
      }
   }
}
