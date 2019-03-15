using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConstructorEngine
{
   public class CsgXmlSerializer
   {
      public readonly CsgNodeRepository Repository;
      private readonly Dictionary<string, CsgNode> nodeIds = new Dictionary<string, CsgNode>();
      private readonly List<Tuple<string, CsgGroup>> deferredLinks = new List<Tuple<string, CsgGroup>>();

      public CsgXmlSerializer() : this(new CsgNodeRepository()) { }
      public CsgXmlSerializer(CsgNodeRepository repository) { Repository = repository; }

      #region Load

      public void LoadXml(string xml)
      {
         XDocument doc = XDocument.Parse(xml);
         foreach (XElement root in doc.Elements("CsgNodeRepository")) { ReadElement(root, null); }
         for (int i = deferredLinks.Count - 1; i >= 0; i--)
         {
            CsgNode node;
            Tuple<string, CsgGroup> key = deferredLinks[i];
            if (nodeIds.TryGetValue(key.Item1, out node))
            {
               deferredLinks.RemoveAt(i);
               if (key.Item2 != null) { key.Item2.AddChild(node); }
               else { node.IsObjectRoot = true; Repository.RegisterNode(node); }
            }
         }
      }

      private void ReadElement(XElement root, CsgGroup owner)
      {
         foreach (XElement e in root.Elements())
         {
            CsgNode node = null;
            string id = e.Attribute("Id").GetValue();
            switch (e.Name.LocalName)
            {
               case "Link":
                  if (!nodeIds.TryGetValue(id, out node)) { deferredLinks.Add(new Tuple<string, CsgGroup>(id, owner)); }
                  break;
               case "CsgCube": node = ReadCube(e); break;
               case "CsgScale": node = ReadScale(e); break;
               case "CsgTranslate": node = ReadTranslate(e); break;
               case "CsgRotate": node = ReadRotate(e); break;
               default:
                  Type type = Type.GetType("ConstructorEngine." + e.Name.LocalName);
                  if (type != null) { node = Activator.CreateInstance(type) as CsgNode; }
                  break;
            }
            if (node != null)
            {
               node.Name = e.Attribute("Name").GetValue(node.Name);
               if (!string.IsNullOrEmpty(id)) { nodeIds[id] = node; }
               if (owner != null) { owner.AddChild(node); } else { node.IsObjectRoot = true; }
               Repository.RegisterNode(node);
               if (node is CsgGroup) { ReadElement(e, (CsgGroup)node); }
            }
         }
      }

      #region Geometry

      private CsgCube ReadCube(XElement e)
      {
         return new CsgCube()
         {
            Size = new Vector3()
            {
               X = e.Attribute("SizeX").GetValueDouble(1),
               Y = e.Attribute("SizeY").GetValueDouble(1),
               Z = e.Attribute("SizeZ").GetValueDouble(1),
            },
            Colour = (uint)e.Attribute("Colour").GetValueInt(0xffffff)
         };
      }

      #endregion

      #region Transform

      private CsgScale ReadScale(XElement e)
      {
         return new CsgScale()
         {
            Scale = new Vector3()
            {
               X = e.Attribute("X").GetValueDouble(1),
               Y = e.Attribute("Y").GetValueDouble(1),
               Z = e.Attribute("Z").GetValueDouble(1),
            }
         };
      }

      private CsgTranslate ReadTranslate(XElement e)
      {
         return new CsgTranslate()
         {
            Offset = new Vector3()
            {
               X = e.Attribute("X").GetValueDouble(1),
               Y = e.Attribute("Y").GetValueDouble(1),
               Z = e.Attribute("Z").GetValueDouble(1),
            }
         };
      }

      private CsgRotate ReadRotate(XElement e)
      {
         return new CsgRotate()
         {
            Angles = new Vector3()
            {
               X = e.Attribute("X").GetValueDouble(1),
               Y = e.Attribute("Y").GetValueDouble(1),
               Z = e.Attribute("Z").GetValueDouble(1),
            },
            Relative = e.Attribute("Relative").GetValueBoolean(true)
         };
      }

      #endregion

      #endregion

      #region Save

      public string SaveXml()
      {
         StringBuilder xml = new StringBuilder();
         HashSet<CsgNode> added = new HashSet<CsgNode>();
         xml.AppendLine("<CsgNodeRepository>");
         foreach (CsgNode node in Repository.Roots) { WriteNode(xml, added, node, "  "); }
         xml.AppendLine("</CsgNodeRepository>");
         return xml.ToString();
      }

      private void WriteNode(StringBuilder xml, HashSet<CsgNode> added, CsgNode node, string indent)
      {
         if (!added.Add(node))
         {
            xml.Append(indent);
            xml.Append("<Link Id=\"");
            xml.Append(node.Id);
            xml.AppendLine("\" />");
            return;
         }
         if (node is CsgCube) { WriteNode(xml, added, (CsgCube)node, indent); return; }
         if (node is CsgTranslate) { WriteNode(xml, added, (CsgTranslate)node, indent); return; }
         if (node is CsgRotate) { WriteNode(xml, added, (CsgRotate)node, indent); return; }
         if (node is CsgScale) { WriteNode(xml, added, (CsgScale)node, indent); return; }
         if (node is CsgGroup) { WriteNode(xml, added, (CsgGroup)node, indent); return; }
      }

      private void WriteNode(StringBuilder xml, HashSet<CsgNode> added, CsgGroup node, string indent)
      {
         string tagName = node.GetType().Name;
         xml.Append(indent);
         xml.Append("<");
         xml.Append(tagName);
         if (node.IsObjectRoot || node.ReferencedByCount != 1)
         {
            xml.Append(" Id=\"");
            xml.Append(node.Id);
            xml.Append("\"");
         }
         if (!string.IsNullOrEmpty(node.Name))
         {
            xml.Append(" Name=\"");
            xml.Append(node.Name);
            xml.Append("\"");
         }
         xml.AppendLine(">");
         foreach (CsgNode child in node.Children) { WriteNode(xml, added, child, indent + "  "); }
         xml.Append(indent);
         xml.Append("</");
         xml.Append(tagName);
         xml.AppendLine(">");
      }

      #region Geometry

      private void WriteNode(StringBuilder xml, HashSet<CsgNode> added, CsgCube node, string indent)
      {
         xml.Append(indent);
         xml.Append("<CsgCube");
         if (node.IsObjectRoot || node.ReferencedByCount != 1)
         {
            xml.Append(" Id=\"");
            xml.Append(node.Id);
            xml.Append("\"");
         }
         if (!string.IsNullOrEmpty(node.Name))
         {
            xml.Append(" Name=\"");
            xml.Append(node.Name);
            xml.Append("\"");
         }
         xml.Append(" SizeX=\"");
         xml.Append(node.Size.X);
         xml.Append("\" SizeY=\"");
         xml.Append(node.Size.Y);
         xml.Append("\" SizeZ=\"");
         xml.Append(node.Size.Z);
         xml.Append("\" Colour=\"0x");
         xml.Append(node.Colour.ToString("X"));
         xml.AppendLine("\" />");
      }

      #endregion

      #region Transform

      private void WriteNode(StringBuilder xml, HashSet<CsgNode> added, CsgTranslate node, string indent)
      {
         xml.Append(indent);
         xml.Append("<CsgTranslate");
         if (node.IsObjectRoot || node.ReferencedByCount != 1)
         {
            xml.Append(" Id=\"");
            xml.Append(node.Id);
            xml.Append("\"");
         }
         if (!string.IsNullOrEmpty(node.Name))
         {
            xml.Append(" Name=\"");
            xml.Append(node.Name);
            xml.Append("\"");
         }
         xml.Append(" X=\"");
         xml.Append(node.Offset.X);
         xml.Append("\" Y=\"");
         xml.Append(node.Offset.Y);
         xml.Append("\" Z=\"");
         xml.Append(node.Offset.Z);
         xml.AppendLine(">");
         foreach (CsgNode child in node.Children) { WriteNode(xml, added, child, indent + "  "); }
         xml.Append(indent);
         xml.AppendLine("</CsgTranslate>");
      }

      private void WriteNode(StringBuilder xml, HashSet<CsgNode> added, CsgRotate node, string indent)
      {
         xml.Append(indent);
         xml.Append("<CsgRotate");
         if (node.IsObjectRoot || node.ReferencedByCount != 1)
         {
            xml.Append(" Id=\"");
            xml.Append(node.Id);
            xml.Append("\"");
         }
         if (!string.IsNullOrEmpty(node.Name))
         {
            xml.Append(" Name=\"");
            xml.Append(node.Name);
            xml.Append("\"");
         }
         xml.Append(" X=\"");
         xml.Append(node.Angles.X);
         xml.Append("\" Y=\"");
         xml.Append(node.Angles.Y);
         xml.Append("\" Z=\"");
         xml.Append(node.Angles.Z);
         xml.Append("\" Relative=\"");
         xml.Append(node.Relative);
         xml.AppendLine(">");
         foreach (CsgNode child in node.Children) { WriteNode(xml, added, child, indent + "  "); }
         xml.Append(indent);
         xml.AppendLine("</CsgRotate>");
      }

      private void WriteNode(StringBuilder xml, HashSet<CsgNode> added, CsgScale node, string indent)
      {
         xml.Append(indent);
         xml.Append("<CsgScale");
         if (node.IsObjectRoot || node.ReferencedByCount != 1)
         {
            xml.Append(" Id=\"");
            xml.Append(node.Id);
            xml.Append("\"");
         }
         if (!string.IsNullOrEmpty(node.Name))
         {
            xml.Append(" Name=\"");
            xml.Append(node.Name);
            xml.Append("\"");
         }
         xml.Append(" X=\"");
         xml.Append(node.Scale.X);
         xml.Append("\" Y=\"");
         xml.Append(node.Scale.Y);
         xml.Append("\" Z=\"");
         xml.Append(node.Scale.Z);
         xml.AppendLine(">");
         foreach (CsgNode child in node.Children) { WriteNode(xml, added, child, indent + "  "); }
         xml.Append(indent);
         xml.AppendLine("</CsgScale>");
      }

      #endregion

      #endregion
   }

   static class XAttributeExtensions
   {
      public static string GetValue(this XAttribute a, string defaultValue = null)
      {
         if (a == null) { return defaultValue; }
         return a.Value ?? defaultValue;
      }

      public static double GetValueDouble(this XAttribute a, double defaultValue = 0)
      {
         double d = defaultValue;
         if (a == null) { return defaultValue; }
         if (a == null || a.Value == null || !double.TryParse(a.Value, out d)) { d = defaultValue; }
         return d;
      }

      public static int GetValueInt(this XAttribute a, int defaultValue = 0)
      {
         int i = defaultValue;
         if (a == null) { return defaultValue; }
         if (a == null || a.Value == null || !int.TryParse(a.Value, out i)) { i = defaultValue; }
         return i;
      }

      public static bool GetValueBoolean(this XAttribute a, bool defaultValue = false)
      {
         bool b = defaultValue;
         if (a == null) { return defaultValue; }
         if (a == null || a.Value == null || !bool.TryParse(a.Value, out b)) { b = defaultValue; }
         return b;
      }
   }
}
