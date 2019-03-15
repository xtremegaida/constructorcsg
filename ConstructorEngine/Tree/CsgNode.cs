using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jitter;
using Jitter.Dynamics;
using Jitter.LinearMath;

namespace ConstructorEngine
{
   public abstract class CsgNode
   {
      private readonly List<CsgNode> children = new List<CsgNode>();
      private readonly List<CsgNode> referencedBy = new List<CsgNode>();

      public string Id = Guid.NewGuid().ToString();
      public string Name;
      public bool IsObjectRoot;

      protected Mesh3[] cachedMesh;

      public CsgNode this[int index] { get { return children[index]; } }
      public IEnumerable<CsgNode> Children { get { return children; } }
      public int ChildrenCount { get { return children.Count; } }
      public IEnumerable<CsgNode> ReferencedBy { get { return referencedBy; } }
      public int ReferencedByCount { get { return referencedBy.Count; } }

      public bool AddChild(CsgNode node)
      {
         if (node == null || children.Contains(node)) { return false; }
         if (IsCyclic(node)) { return false; }
         children.Add(node);
         node.referencedBy.Add(this);
         return true;
      }

      public bool RemoveChild(CsgNode node)
      {
         if (node != null && children.Remove(node))
         {
            node.referencedBy.Remove(this);
            return true;
         }
         return false;
      }

      public bool MoveChild(CsgNode child, int toIndex)
      {
         if (children.Remove(child))
         {
            children.Insert(toIndex, child);
            return true;
         }
         return false;
      }

      public bool IsCyclic(CsgNode addedNode, HashSet<CsgNode> visited = null)
      {
         if (visited == null) { visited = new HashSet<CsgNode>(); }
         if (addedNode != null && !visited.Add(addedNode)) { return true; }
         if (!visited.Add(this)) { return true; }
         foreach (CsgNode parent in ReferencedBy) { if (parent.IsCyclic(null, visited)) { return true; } }
         return false;
      }

      public abstract CsgNode Clone();
      protected abstract Mesh3[] GenerateMeshes();

      public virtual Mesh3[] GetMeshes()
      {
         if (cachedMesh != null) { return cachedMesh; }
         return (cachedMesh = GenerateMeshes());
      }

      public virtual void InvalidateCache()
      {
         cachedMesh = null;
         for (int i = referencedBy.Count - 1; i >= 0; i--) { referencedBy[i].InvalidateCache(); }
      }

      public virtual void PopulateWorld(World world)
      {
         Mesh3[] meshes = GetMeshes();
         for (int i = 0; i < meshes.Length; i++) { world.AddBody(meshes[i].CreateRigidBody()); }
      }
   }
}
