using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructorEngine
{
   public class CsgNodeRepository
   {
      public readonly Dictionary<string, CsgNode> Nodes = new Dictionary<string, CsgNode>();

      public IEnumerable<CsgNode> Roots
      {
         get { return Nodes.Values.Where(x => x.IsObjectRoot || x.ReferencedByCount == 0); }
      }

      public void RegisterNode(CsgNode node)
      {
         if (Nodes.ContainsKey(node.Id)) { return; }
         Nodes[node.Id] = node;
         foreach (CsgNode child in node.Children) { RegisterNode(child); }
      }

      public CsgNode GetById(string id)
      {
         CsgNode node;
         Nodes.TryGetValue(id, out node);
         return node;
      }
   }
}
