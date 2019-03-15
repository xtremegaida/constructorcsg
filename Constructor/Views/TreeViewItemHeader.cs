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
   public class TreeViewItemHeader : StackPanel
   {
      private readonly Image imageView;
      private readonly TextBlock textView;
      private readonly TreeViewItemImage imageType;

      public TreeViewItemHeader(string title, TreeViewItemImage image)
      {
         imageView = new Image();
         textView = new TextBlock();
         imageType = image;

         imageView.Margin = new Thickness(0, 0, 4, 0);
         imageView.Width = 20;
         imageView.Height = 20;
         imageView.Source = TreeViewItemImageSource.GetImageSource(image);

         textView.VerticalAlignment = VerticalAlignment.Center;
         textView.TextWrapping = TextWrapping.Wrap;
         textView.Foreground = Brushes.White;
         textView.Margin = new Thickness(4);
         textView.Text = title;

         HorizontalAlignment = HorizontalAlignment.Stretch;
         VerticalAlignment = VerticalAlignment.Center;
         Orientation = Orientation.Horizontal;
         Children.Add(imageView);
         Children.Add(textView);
      }

      public TreeViewItemHeader(object obj, string path, TreeViewItemImage image)
         : this(null, image)
      {
         textView.SetBinding(TextBlock.TextProperty,
            new Binding(path) { Source = obj, Mode = BindingMode.OneWay });
      }

      public string Text { get { return (textView.Text); } }

      public TreeViewItemImage ImageType { get { return (imageType); } }

      public override string ToString()
      {
         return (textView.Text);
      }
   }
}
