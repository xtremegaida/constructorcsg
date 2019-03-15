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
   public class ObjectDocument : LayoutDocument
   {
      private readonly ObjectView view;

      public ObjectDocument(LibraryView owner, CsgGroupWrapper node)
      {
         Content = view = new ObjectView() { Node = node, Owner = owner };
         BindingOperations.SetBinding(this, TitleProperty,
            new Binding("Name") { Source = node, Mode = BindingMode.OneWay });
         if (owner != null) { owner.RegisterOpenDocument(this); }
      }

      public CsgGroupWrapper Node { get { return (view.Node); } }

      public Panel Toolbar { get { return (view.Toolbar); } }
   }
}
