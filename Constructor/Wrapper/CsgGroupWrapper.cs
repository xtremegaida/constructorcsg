using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstructorEngine;

namespace Constructor
{
   public class CsgGroupWrapper : CsgNodeWrapper
   {
      private List<CsgNodeWrapper> children = new List<CsgNodeWrapper>();

      public readonly CsgGroup Group;

      public CsgGroupWrapper(CsgNodeWrapperRepository repo, CsgGroup node)
         : base(repo, node)
      {
         Group = node;
         UpdateChildren();
      }

      public IEnumerable<CsgNodeWrapper> Children { get { return children; } }
      public int ChildrenCount { get { return children.Count; } }

      public bool AddChild(CsgNodeWrapper child)
      {
         bool success = Group.AddChild(child.Node);
         child.OnDisplayNameChanged();
         UpdateChildren();
         return success;
      }

      public bool RemoveChild(CsgNodeWrapper child)
      {
         bool success = Group.RemoveChild(child.Node);
         child.OnDisplayNameChanged();
         UpdateChildren();
         return success;
      }

      public bool MoveChild(CsgNodeWrapper child, int toIndex)
      {
         bool success = Group.MoveChild(child.Node, toIndex);
         UpdateChildren();
         return success;
      }

      private void UpdateChildren()
      {
         bool changed = false;
         int oldCount = ChildrenCount;
         int newCount = Group.ChildrenCount;
         if (oldCount != newCount) { changed = true; }
         else
         {
            for (int i = oldCount - 1; i >= 0; i--)
            {
               if (children[i].Node != Group[i]) { changed = true; break; }
            }
         }
         if (changed)
         {
            for (int i = 0; i < children.Count; i++)
            {
               children[i].MeshChanged -= ChildMeshChanged;
            }
            children = new List<CsgNodeWrapper>();
            for (int i = 0; i < newCount; i++)
            {
               CsgNodeWrapper wrapper = Repository.GetWrapper(Group[i]);
               wrapper.MeshChanged += ChildMeshChanged;
               children.Add(wrapper);
            }
            if (oldCount != newCount) { OnPropertyChanged("ChildrenCount"); }
            OnPropertyChanged("Children");
            OnMeshChanged();
         }
      }

      private void ChildMeshChanged(object sender, EventArgs e)
      {
         OnMeshChanged();
      }
   }
}
