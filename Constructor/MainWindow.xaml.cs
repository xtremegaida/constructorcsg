using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
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
using Microsoft.Win32;
using ConstructorEngine;
using Constructor.Views;

namespace Constructor
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      private CsgNodeWrapperRepository repo;
      private string fileName;

      public MainWindow()
      {
         InitializeComponent();
         libraryView.ObjectDocumentPane = Documents;
         NewClick(this, null);
      }

      private void SelectRepo(CsgNodeWrapperRepository repo)
      {
         this.repo = repo;
         BindingOperations.SetBinding(saveButton, Button.IsEnabledProperty,
            new Binding("IsDirty") { Source = repo, Mode = BindingMode.OneTime });
         libraryView.SelectedItem = repo;
      }

      private void NewClick(object sender, RoutedEventArgs e)
      {
         if (repo != null && repo.IsDirty)
         {
            var result = MessageBox.Show("Save changes to the current project?", "New", MessageBoxButton.YesNoCancel);
            if (result == MessageBoxResult.Yes) { SaveClick(this, e); }
            else if (result == MessageBoxResult.Cancel) { return; }
         }
         repo = new CsgNodeWrapperRepository();
         repo.GetWrapper(new CsgGroup(new CsgCube()) { Name = "New Object", IsObjectRoot = true });
         SelectRepo(repo);
      }

      private void LoadClick(object sender, RoutedEventArgs e)
      {
         OpenFileDialog open = new OpenFileDialog();
         open.Filter = "XML Files (*.xml)|*.xml";
         if (open.ShowDialog().GetValueOrDefault())
         {
            try
            {
               CsgXmlSerializer xml = new CsgXmlSerializer();
               xml.LoadXml(File.ReadAllText(fileName = open.FileName));
               CsgNodeWrapperRepository repo = new CsgNodeWrapperRepository();
               foreach (CsgNode node in xml.Repository.Roots) { repo.GetWrapper(node); }
               repo.ResetDirty();
               SelectRepo(repo);
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Error");
            }
         }
      }

      private void SaveClick(object sender, RoutedEventArgs e)
      {
         if (fileName == null) { SaveAsClick(sender, e); return; }
         try
         {
            CsgXmlSerializer xml = new CsgXmlSerializer();
            foreach (CsgNodeWrapper node in repo.ObjectRoots) { xml.Repository.RegisterNode(node.Node); }
            string text = xml.SaveXml();
            File.WriteAllText(fileName, text);
            repo.ResetDirty();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.Message, "Error");
         }
      }

      private void SaveAsClick(object sender, RoutedEventArgs e)
      {
         SaveFileDialog save = new SaveFileDialog();
         save.Filter = "XML Files (*.xml)|*.xml";
         if (save.ShowDialog().GetValueOrDefault())
         {
            try
            {
               CsgXmlSerializer xml = new CsgXmlSerializer();
               foreach (CsgNodeWrapper node in repo.ObjectRoots) { xml.Repository.RegisterNode(node.Node); }
               string text = xml.SaveXml();
               File.WriteAllText(fileName = save.FileName, text);
               repo.ResetDirty();
            }
            catch (Exception ex)
            {
               MessageBox.Show(ex.Message, "Error");
            }
         }
      }
   }
}
