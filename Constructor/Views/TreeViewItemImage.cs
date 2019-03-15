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

namespace Constructor
{
   public enum TreeViewItemImage
   {
      Folder, Graph, Node, Input, Output
   }

   public static class TreeViewItemImageSource
   {
      public static ImageSource GetImageSource(TreeViewItemImage image)
      {
         switch (image)
         {
            case TreeViewItemImage.Graph: return (Startup.Application.Resources["IconGraphSmall"] as ImageSource);
            case TreeViewItemImage.Node: return (Startup.Application.Resources["IconNodeSmall"] as ImageSource);
            case TreeViewItemImage.Input: return (Startup.Application.Resources["IconInputSmall"] as ImageSource);
            case TreeViewItemImage.Output: return (Startup.Application.Resources["IconOutputSmall"] as ImageSource);
            case TreeViewItemImage.Folder: return (Startup.Application.Resources["IconOpenSmall"] as ImageSource);
         }
         return (null);
      }
   }
}
