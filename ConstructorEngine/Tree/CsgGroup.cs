using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgGroup : CsgNode
   {
      public CsgGroup() { Name = "Group"; }

      public CsgGroup(params CsgNode[] nodes) : this()
      {
         if (nodes != null) { for (int i = 0; i < nodes.Length; i++) { AddChild(nodes[i]); } }
      }

      protected override Mesh3[] GenerateMeshes()
      {
         List<Mesh3> meshes = new List<Mesh3>();
         foreach (CsgNode node in Children) { meshes.AddRange(node.GetMeshes()); }
         return meshes.ToArray();
      }

      public override CsgNode Clone()
      {
         return new CsgGroup(Children.Select(x => x.Clone()).ToArray()) { Name = Name };
      }
   }
}
