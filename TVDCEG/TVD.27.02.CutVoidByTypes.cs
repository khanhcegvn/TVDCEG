#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    public class CutVoidByTypes
    {
        private TreeNode m_allelem = new TreeNode("Types Cut");
        public List<FamilyInstance> SelectedElement = new List<FamilyInstance>();
        Dictionary<string, Dictionary<string, List<FamilyInstance>>> dicmegr = new Dictionary<string, Dictionary<string, List<FamilyInstance>>>();
        Dictionary<string, Dictionary<string, List<FamilyInstance>>> dic = new Dictionary<string, Dictionary<string, List<FamilyInstance>>>();
        Dictionary<string, Dictionary<string, List<FamilyInstance>>> dicgenericmodel = new Dictionary<string, Dictionary<string, List<FamilyInstance>>>();
        Dictionary<string, Dictionary<string, List<FamilyInstance>>> dicinsulation = new Dictionary<string, Dictionary<string, List<FamilyInstance>>>();
        public TreeNode AllElementNames
        {
            get
            {
                return m_allelem;
            }
        }
        public CutVoidByTypes(Document doc)
        {
            Getallframing(doc);
            Getallgenericmodel(doc);
            Getinsulation(doc);
            dicmegr = dic.Union(dicgenericmodel).ToDictionary(pair => pair.Key, pair => pair.Value).Union(dicinsulation).ToDictionary(pair => pair.Key, pair => pair.Value);
        }
        public void Getallframing(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic2 = new Dictionary<string, List<FamilyInstance>>();
            var structural = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in structural)
            {
                if (dic2.ContainsKey(item.Name))
                {
                    dic2[item.Name].Add(item);
                }
                else
                {
                    dic2.Add(item.Name, new List<FamilyInstance> { item });
                }
            }
            foreach (KeyValuePair<string, List<FamilyInstance>> item in dic2)
            {
                string value = item.Value.First().Symbol.FamilyName;
                if (dic.ContainsKey(value))
                {
                    dic[value].Add(item.Key, item.Value);
                }
                else
                {
                    Dictionary<string, List<FamilyInstance>> dic3 = new Dictionary<string, List<FamilyInstance>>();
                    dic3.Add(item.Key, item.Value);
                    dic.Add(value, dic3);
                }
            }
            dic.OrderByDescending(x => x.Key).ToList();
            if (dic.Keys.Count != 0)
            {
                AssortElement("Structural Framming", dic);
            }
        }
        public void Getallgenericmodel(Document doc)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            Dictionary<string, List<FamilyInstance>> dic2 = new Dictionary<string, List<FamilyInstance>>();
            var genericmodel = new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in genericmodel)
            {
                ElementId eleid = item.GetTypeId();
                Element ele = doc.GetElement(eleid);
                Parameter pa = ele.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    if (!pa.AsString().Contains("VOID"))
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    if (item.Name.Contains("BRICK"))
                    {
                        list.Add(item);
                    }
                }
            }
            foreach (var item in list)
            {
                if (dic2.ContainsKey(item.Name))
                {
                    dic2[item.Name].Add(item);
                }
                else
                {
                    dic2.Add(item.Name, new List<FamilyInstance> { item });
                }
            }
            foreach (KeyValuePair<string, List<FamilyInstance>> item in dic2)
            {
                string value = item.Value.First().Symbol.FamilyName;
                if (dicgenericmodel.ContainsKey(value))
                {
                    dicgenericmodel[value].Add(item.Key, item.Value);
                }
                else
                {
                    Dictionary<string, List<FamilyInstance>> dic3 = new Dictionary<string, List<FamilyInstance>>();
                    dic3.Add(item.Key, item.Value);
                    dicgenericmodel.Add(value, dic3);
                }
            }
            dicgenericmodel.OrderByDescending(x => x.Key).ToList();
            if (dicgenericmodel.Keys.Count != 0)
            {
                AssortElement("Generic Model", dicgenericmodel);
            }
        }
        public void Getinsulation(Document doc)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            Dictionary<string, List<FamilyInstance>> dic2 = new Dictionary<string, List<FamilyInstance>>();
            var genericmodel = (from x in new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>() where x.SuperComponent != null select x).ToList();
            foreach (var item in genericmodel)
            {
                ElementId eleid = item.GetTypeId();
                Element ele = doc.GetElement(eleid);
                Parameter pa = ele.LookupParameter("CONTROL_MARK");
                if (pa != null)
                {
                    if (pa.AsString().Contains("BEADBOARD"))
                    {
                        list.Add(item);
                    }
                    if (item.Name.Contains("INSULATION"))
                    {
                        list.Add(item);
                    }
                }
                else
                {
                    if (item.Name.Contains("INSULATION"))
                    {
                        list.Add(item);
                    }
                }
            }
            foreach (var item in list)
            {
                if (dic2.ContainsKey(item.Name))
                {
                    dic2[item.Name].Add(item);
                }
                else
                {
                    dic2.Add(item.Name, new List<FamilyInstance> { item });
                }
            }
            foreach (KeyValuePair<string, List<FamilyInstance>> item in dic2)
            {
                string value = item.Value.First().Symbol.FamilyName;
                if (dicinsulation.ContainsKey(value))
                {
                    dicinsulation[value].Add(item.Key, item.Value);
                }
                else
                {
                    Dictionary<string, List<FamilyInstance>> dic3 = new Dictionary<string, List<FamilyInstance>>();
                    dic3.Add(item.Key, item.Value);
                    dicinsulation.Add(value, dic3);
                }
            }
            dicinsulation.OrderByDescending(x => x.Key).ToList();
            if (dicinsulation.Keys.Count != 0)
            {
                AssortElement("Special", dicinsulation);
            }
        }
        private void AssortElement(string type, Dictionary<string, Dictionary<string, List<FamilyInstance>>> dis)
        {
            TreeNode categoryNode = new TreeNode(type);
            categoryNode.ImageIndex = 0;
            categoryNode.Tag = type;
            foreach (KeyValuePair<string, Dictionary<string, List<FamilyInstance>>> item in dis)
            {
                TreeNode childNode = new TreeNode(item.Key);
                childNode.Expand();
                AddChildNode(childNode, item.Value.Keys.ToArray());
                categoryNode.Nodes.Add(childNode);
            }
            AllElementNames.Nodes.Add(categoryNode);
        }
        private void AddChildNode(TreeNode pParentNode, string[] pNodesList)
        {
            if (pNodesList == null)
                return;
            for (int i = 0; i < pNodesList.Length; i++)
            {
                TreeNode childNode = new TreeNode(pNodesList[i]);
                childNode.Expand();
                pParentNode.Nodes.Add(childNode);
            }
        }
        public void SelectElements()
        {
            ArrayList names = new ArrayList();
            foreach (TreeNode t in AllElementNames.Nodes)
            {
                foreach (TreeNode n in t.Nodes)
                {
                    foreach (TreeNode k in n.Nodes)
                    {
                        if (k.Checked && 0 == k.Nodes.Count)
                        {
                            names.Add(k.Text);
                        }
                    }
                }
            }
            foreach (var v in dicmegr)
            {
                foreach (var i in names)
                {
                    foreach (var p in v.Value)
                    {
                        if (i.Equals(p.Key))
                        {
                            foreach (var g in p.Value)
                            {
                                SelectedElement.Add(g);
                            }
                        }
                    }
                }
            }
        }
        public void Cutting(Document doc, List<FamilyInstance> voids, List<FamilyInstance> listcut)
        {
            Transaction tran = new Transaction(doc, "Invention: Cut void by type");
            tran.Start();
            foreach (var conn in voids)
            {
                foreach (var framming in listcut)
                {
                    try
                    {
                        if (InstanceVoidCutUtils.CanBeCutWithVoid(framming))
                        {
                            InstanceVoidCutUtils.AddInstanceVoidCut(doc, framming, conn);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            tran.Commit();
        }
        public void UnCut(Document doc, List<FamilyInstance> voids, List<FamilyInstance> listcut)
        {
            Transaction tran = new Transaction(doc, "Invention: Cut void by type");
            tran.Start();
            foreach (var conn in voids)
            {
                foreach (var framming in listcut)
                {
                    try
                    {
                        if (InstanceVoidCutUtils.CanBeCutWithVoid(framming))
                        {
                            InstanceVoidCutUtils.RemoveInstanceVoidCut(doc, framming, conn);
                        }
                    }
                    catch
                    {

                    }
                }
            }
            tran.Commit();
        }
    }
}
