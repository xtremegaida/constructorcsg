using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Constructor
{
   /// <summary>
   /// Interaction logic for App.xaml
   /// </summary>
   public partial class App : Application
   {
   }

   public static class Startup
   {
      public static App Application;

      [STAThread]
      public static void Main(string[] args)
      {
         /*AppDomain.CurrentDomain.AssemblyResolve += (sender, e) =>
         {
            string resourceName = "Generator.Dependencies." + new AssemblyName(e.Name).Name + ".dll";
            var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            if (stream == null) { return (null); }
            using (stream)
            {
               byte[] assemblyData = new byte[stream.Length];
               stream.Read(assemblyData, 0, assemblyData.Length);
               return (Assembly.Load(assemblyData));
            }
         };*/
         try
         {
            Application = new App();
            Application.InitializeComponent();
            Application.Run();
         }
         catch (Exception ex)
         {
            MessageBox.Show(ex.ToString(), "Fatal Error");
         }
      }
   }
}
