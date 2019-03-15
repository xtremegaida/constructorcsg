﻿using System;
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
using ConstructorEngine;

namespace Constructor.Views
{
   public class CsgTranslateTreeViewItem : CsgGroupTreeViewItem
   {
      private readonly CsgTranslateWrapper obj;

      protected override TreeViewItemImage HeaderImage { get { return TreeViewItemImage.Graph; } }

      public CsgTranslateTreeViewItem(ObjectTreeView owner, CsgTranslateWrapper obj)
         : base(owner, obj)
      {
         this.obj = obj;
      }

      public CsgTranslateWrapper Object { get { return (obj); } }
   }
}
