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
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;
using ConstructorEngine;

namespace Constructor.Views
{
   public class PhysicsDocument : LayoutDocument
   {
      private readonly PhysicsView3D view;

      public PhysicsDocument(LibraryView owner, CsgGroupWrapper node)
      {
         Content = view = new PhysicsView3D() { SelectedItem = node };
         BindingOperations.SetBinding(this, TitleProperty,
            new Binding("Name") { Source = node, Mode = BindingMode.OneWay });
         if (owner != null) { owner.RegisterOpenDocument(this); }
         Closed += (s, e) => { view.Stop(); };
      }

      public CsgGroupWrapper Node { get { return (view.SelectedItem as CsgGroupWrapper); } }
   }
}
