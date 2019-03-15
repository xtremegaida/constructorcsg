using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConstructorEngine;

namespace Constructor
{
   public class CsgNodeWrapperRepository : INotifyPropertyChanged
   {
      public readonly Dictionary<CsgNode, CsgNodeWrapper> Wrappers = new Dictionary<CsgNode, CsgNodeWrapper>();
      public readonly Dictionary<string, CsgNodeWrapper> WrapperIds = new Dictionary<string, CsgNodeWrapper>();

      public event PropertyChangedEventHandler PropertyChanged;
      public event EventHandler ObjectRootsChanged;

      public IEnumerable<CsgNodeWrapper> ObjectRoots { get { return Wrappers.Values.Where(x => x.IsObjectRoot); } }

      public bool IsDirty { get { return isDirty; } }

      private bool isDirty;

      internal void OnObjectRootsChanged()
      {
         EventHandler handler = ObjectRootsChanged;
         if (handler != null) { handler(this, EventArgs.Empty); }
         OnPropertyChanged("ObjectRoots");
      }

      protected void OnPropertyChanged(string name)
      {
         PropertyChangedEventHandler handler = PropertyChanged;
         if (handler != null) { handler(this, new PropertyChangedEventArgs(name)); }
      }

      public void ResetDirty()
      {
         if (isDirty)
         {
            isDirty = false;
            OnPropertyChanged("IsDirty");
         }
      }

      private void SetDirty()
      {
         if (!isDirty)
         {
            isDirty = true;
            OnPropertyChanged("IsDirty");
         }
      }

      private void WrapperPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         SetDirty();
      }

      public void RemoveNode(CsgNodeWrapper wrapper)
      {
         wrapper.PropertyChanged -= WrapperPropertyChanged;
         Wrappers.Remove(wrapper.Node);
         WrapperIds.Remove(wrapper.Id);
         if (wrapper.IsObjectRoot) { OnObjectRootsChanged(); }
      }

      public CsgNodeWrapper GetWrapper(CsgNode node)
      {
         CsgNodeWrapper wrapper;
         if (!Wrappers.TryGetValue(node, out wrapper))
         {
            if (node is CsgCube) { wrapper = new CsgCubeWrapper(this, (CsgCube)node); }
            else if (node is CsgTranslate) { wrapper = new CsgTranslateWrapper(this, (CsgTranslate)node); }
            else if (node is CsgScale) { wrapper = new CsgScaleWrapper(this, (CsgScale)node); }
            else if (node is CsgRotate) { wrapper = new CsgRotateWrapper(this, (CsgRotate)node); }
            else if (node is CsgUnion) { wrapper = new CsgUnionWrapper(this, (CsgUnion)node); }
            else if (node is CsgSubtract) { wrapper = new CsgSubtractWrapper(this, (CsgSubtract)node); }
            else if (node is CsgIntersect) { wrapper = new CsgIntersectWrapper(this, (CsgIntersect)node); }
            else if (node is CsgGroup) { wrapper = new CsgGroupWrapper(this, (CsgGroup)node); }
            else { wrapper = new CsgNodeWrapper(this, node); }
            Wrappers[node] = wrapper;
            WrapperIds[wrapper.Id] = wrapper;
            wrapper.PropertyChanged += WrapperPropertyChanged;
            OnObjectRootsChanged();
            SetDirty();
         }
         return wrapper;
      }

      public CsgNodeWrapper GetWrapperById(string id)
      {
         CsgNodeWrapper wrapper;
         if (id == null) { return null; }
         WrapperIds.TryGetValue(id, out wrapper);
         return wrapper;
      }
   }
}
