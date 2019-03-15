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

namespace Constructor.Views
{
   /// <summary>
   /// Interaction logic for ObjectView.xaml
   /// </summary>
   public partial class ObjectView : UserControl
   {
      public ObjectView()
      {
         InitializeComponent();
         Nodes.Focus();
      }

      public LibraryView Owner { get { return (Nodes.Owner); } set { Nodes.Owner = value; } }

      public CsgGroupWrapper Node { get { return (Nodes.SelectedItem); } set { Nodes.SelectedItem = value; } }

      public Panel Toolbar { get { return (Nodes.Toolbar); } }

      private void RunExecuted(object sender, ExecutedRoutedEventArgs e)
      {
         if (Owner != null && Node != null) { new PhysicsDocument(Owner, Node); }
      }
   }
}
