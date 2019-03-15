using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstructorEngine;

namespace Constructor
{
   public class CsgNodeWrapper : INotifyPropertyChanged
   {
      public readonly CsgNodeWrapperRepository Repository;
      public readonly CsgNode Node;

      public event PropertyChangedEventHandler PropertyChanged;
      public event EventHandler MeshChanged;

      private bool isViewing;

      public CsgNodeWrapper(CsgNodeWrapperRepository repo, CsgNode node)
      {
         Repository = repo;
         Node = node;
         if (string.IsNullOrEmpty(node.Id)) { node.Id = Guid.NewGuid().ToString(); }
      }

      public string Id { get { return Node.Id; } }

      public string Name
      {
         get { return Node.Name; }
         set
         {
            if (Node.Name != value)
            {
               Node.Name = value;
               OnPropertyChanged("Name");
               OnDisplayNameChanged();
            }
         }
      }

      internal bool IsViewing
      {
         get { return isViewing; }
         set
         {
            if (isViewing != value)
            {
               isViewing = value;
               OnPropertyChanged("IsViewing");
               OnDisplayNameChanged();
            }
         }
      }

      internal bool IsObjectRoot
      {
         get { return Node.IsObjectRoot; }
         set
         {
            if (Node.IsObjectRoot != value)
            {
               Node.IsObjectRoot = value;
               OnPropertyChanged("IsObjectRoot");
               OnDisplayNameChanged();
               Repository.OnObjectRootsChanged();
            }
         }
      }

      public int ReferenceCount { get { return Node.ReferencedByCount; } }

      public string DisplayName
      {
         get
         {
            string name = Node.Name ?? string.Empty;
            if (Node.ReferencedByCount > 1) { name +=  " (Linked: " + Node.ReferencedByCount + ")"; }
            if (isViewing) { name += " (Viewing)"; }
            if (Node.IsObjectRoot) { name += " (Object)"; }
            return name;
         }
      }

      public void OnDisplayNameChanged()
      {
         OnPropertyChanged("DisplayName");
      }

      public CsgNodeWrapper Clone()
      {
         return Repository.GetWrapper(Node.Clone());
      }

      protected void OnPropertyChanged(string name)
      {
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null) { handler(this, new PropertyChangedEventArgs(name)); }
      }

      protected void OnMeshChanged()
      {
         Node.InvalidateCache();
         EventHandler handler = MeshChanged;
         if (handler != null) { handler(this, EventArgs.Empty); }
      }
   }
}
