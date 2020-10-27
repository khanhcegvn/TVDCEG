#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using TVDCEG.Ultis;
using System.Collections;
using System.Windows.Forms;
#endregion

namespace TVDCEG
{
    public class CheckClashProduct
    {
        private static CheckClashProduct _instance;
        private CheckClashProduct()
        {

        }
        public static CheckClashProduct Instance => _instance ?? (_instance = new CheckClashProduct());
        public Dictionary<string, List<FamilyInstance>> GetAllProduct(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            List<FamilyInstance> Col1 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in Col1)
            {
                if (dic.ContainsKey(item.Symbol.FamilyName))
                {
                    dic[item.Symbol.FamilyName].Add(item);
                }
                else
                {
                    dic.Add(item.Symbol.FamilyName, new List<FamilyInstance> { item });
                }
            }
            return dic;
        }
        public List<FamilyInstance> Checkintersect(Document doc, Solid solid, List<ElementId> ColelementIds, FamilyInstance instance)
        {
            List<FamilyInstance> listfam = new List<FamilyInstance>();
            if (ColelementIds.Count == 0)
            {
                return listfam;
            }
            ICollection<ElementId> col = new FilteredElementCollector(doc, ColelementIds).OfClass(typeof(FamilyInstance)).WherePasses(new ElementIntersectsSolidFilter(solid)).ToElementIds();
            var newlist = (from x in col.ToList() where x.IntegerValue != instance.Id.IntegerValue select x).ToList();
            foreach (var i in newlist)
            {
                Element element = doc.GetElement(i);
                FamilyInstance familyInstance = element as FamilyInstance;
                if (familyInstance != null && familyInstance.Name != "CONNECTOR_COMPONENT")
                {
                    var val = GetSupFamilyInstance(familyInstance);
                    Element ele = instance.SuperComponent;
                    if (ele != null)
                    {
                        if (val.Id != ele.Id)
                        {
                            listfam.Add(val);
                        }
                    }
                    else
                    {
                        listfam.Add(val);
                    }
                }
            }
            listfam.OrderByDescending(x => x.Id);
            return Removeduplicatevalue(listfam);
        }
        public List<FamilyInstance> Removeduplicatevalue(List<FamilyInstance> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    FamilyInstance instance1 = list[i];
                    FamilyInstance instance2 = list[j];
                    if (instance1.Id == instance2.Id)
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
            return list;
        }
        public Dictionary<string, string> Excute(Document doc, List<ElementId> elementIds)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> dic2 = new Dictionary<string, List<string>>();
            Dictionary<string, string> dic3 = new Dictionary<string, string>();
            List<ElementId> listids = new List<ElementId>();
            foreach (var item in elementIds)
            {
                FamilyInstance familyInstance = (FamilyInstance)doc.GetElement(item);
                var warped = GetWarped(doc, familyInstance);
                listids.Add(warped.Id);
            }
            ProgressBarform progressBarform = new ProgressBarform(listids.Count, "Loading...");
            progressBarform.Show();
            for (int i = 0; i < listids.Count - 1; i++)
            {
                var item = (FamilyInstance)doc.GetElement(listids[i]);
                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                var WARPED = GetWarped(doc, item);
                List<Solid> solids = AllSolids(WARPED);
                if (solids != null)
                {
                    foreach (Solid it in solids)
                    {
                        if (it != null && it.Faces.Size < 50)
                        {
                            IList<FamilyInstance> a1 = Checkintersect(doc, it, listids, item);
                            var sup = GetSuperInstances(new List<FamilyInstance> { item });
                            List<FamilyInstance> checksup = GetSuperInstances(a1.ToList());
                            foreach (var i1 in checksup)
                            {
                                if (dic.ContainsKey(sup.First().Name))
                                {
                                    dic[sup.First().Name].Add(Unionstring(sup.First(), i1));
                                }
                                else
                                {
                                    dic.Add(sup.First().Name, new List<string> { Unionstring(sup.First(), i1) });
                                }
                            }
                        }
                    }
                }
                listids.RemoveAt(i);
                i--;
            }
            foreach (var item in dic.Keys)
            {
                dic2.Add(item, RemoveContankey(dic[item]));
            }
            foreach (var item2 in dic2.Keys)
            {
                var fg = SpilitS(dic2[item2]);
                foreach (var item in fg.Keys)
                {
                    dic3.Add(item, fg[item]);
                }
            }
            progressBarform.Close();
            return dic3;
        }
        public FamilyInstance GetWarped(Document doc, FamilyInstance familyInstance)
        {
            FamilyInstance flat = null;
            foreach (var i in familyInstance.GetSubComponentIds())
            {
                Element ele = doc.GetElement(i);
                if (ele.Name.Contains("WARPED"))
                {
                    flat = ele as FamilyInstance;
                }
            }
            if (flat != null)
            {
                return flat;
            }
            else
            {
                return familyInstance;
            }
        }
        public Dictionary<string, string> SpilitS(List<string> liststring)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> dic2 = new Dictionary<string, List<string>>();
            Dictionary<string, string> dic1 = new Dictionary<string, string>();
            foreach (var item in liststring)
            {
                var stringnote = item.Split(' ').ToList();
                var nstring = string.Concat(new object[] { stringnote[0], " ", stringnote[1], " ", stringnote[2] });
                if (dic2.ContainsKey(nstring))
                {
                    dic2[nstring].Add(item);
                }
                else
                {
                    dic2.Add(nstring, new List<string> { item });
                }
            }
            foreach (var item2 in dic2.Keys)
            {
                foreach (var item in dic2[item2])
                {
                    var g = item.Split(' ').ToList();
                    var newstring = item2 + " " + "intersect with" + " " + string.Concat(new object[] { g[4] });
                    if (dic.ContainsKey(newstring))
                    {
                        dic[newstring].Add(item);
                    }
                    else
                    {
                        dic.Add(newstring, new List<string> { item });
                    }
                }
            }
            foreach (var item in dic.Keys)
            {
                dic1.Add(item, Unionstringid(dic[item]));
            }
            return dic1;
        }
        public string Unionstringid(List<string> liststring)
        {
            string hh = liststring[0].Split(':').ToList()[1].Split(' ').ToList()[1];
            string newstring = hh + ";" + liststring[0].Split(':').Last();
            for (int i = 1; i < liststring.Count; i++)
            {
                var item = liststring[i];
                var ar = item.Split(':').Last();
                ar.Replace(" ", "");
                newstring = newstring + ";" + ar;
            }
            return newstring;
        }
        string Unionstring(FamilyInstance ele1, FamilyInstance ele2)
        {
            string item = string.Concat(new object[] { ele1.Name, " ", ":", " ", ele1.Id, " ", "intersect", " ", ele2.Name, " ", ":", " ", ele2.Id });
            return item;
        }
        public void Removeifcontain(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var key = list[i].Split(':').ToList();
                var s1 = key[1].ToString().Split(' ').ToList()[1];
                var s2 = key.Last();
                if (Convert.ToInt32(s1) == Convert.ToInt32(s2))
                {
                    list.Remove(list[i]);
                    i--;
                }
            }
        }
        public int Spilitstring(string name)
        {
            var key = name.Split(':').ToList();
            var s1 = key[1].ToString().Split(' ').ToList()[1];
            var s2 = key.Last();
            int sum = Convert.ToInt32(s1) + Convert.ToInt32(s2);
            return sum;
        }
        List<string> RemoveContankey(List<string> list)
        {
            list.Sort();
            Removeifcontain(list);
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = i - 1; j < i; j++)
                {
                    int str1 = Spilitstring(list[i]);
                    int str2 = Spilitstring(list[j]);
                    if (str1 == str2)
                    {
                        list.RemoveAt(j);
                        i--;
                    }
                }
            }
            return list;
        }
        public List<FamilyInstance> GetSuperInstances(List<FamilyInstance> familyInstances)
        {
            var superInstances = new List<FamilyInstance>();
            foreach (var instance in familyInstances)
            {
                var f1 = instance.SuperComponent as FamilyInstance;
                if (instance.SuperComponent != null)
                {
                    superInstances.Add(f1);
                }
                if (instance.SuperComponent == null)
                {
                    superInstances.Add(instance);
                }

            }
            return superInstances;
        }
        public FamilyInstance GetSupFamilyInstance(FamilyInstance familyInstance)
        {
            var f1 = familyInstance;
            var f2 = familyInstance.SuperComponent as FamilyInstance;
            if (familyInstance.SuperComponent != null)
            {
                familyInstance = f2;
                return familyInstance;
            }
            else
            {
                return familyInstance;
            }
        }
        public Dictionary<string, List<FamilyInstance>> GetAllVoid(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            List<FamilyInstance> Col2 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in Col2)
            {
                if (dic.ContainsKey(item.Symbol.FamilyName))
                {
                    dic[item.Symbol.FamilyName].Add(item);
                }
                else
                {
                    dic.Add(item.Symbol.FamilyName, new List<FamilyInstance> { item });
                }
            }
            return dic;
        }
        public void GetSolidsFromSymbol(FamilyInstance familyInstance, ref List<Solid> solids)
        {
            Options option = new Options();
            option.ComputeReferences = true;
            option.IncludeNonVisibleObjects = true;
            option.DetailLevel = ViewDetailLevel.Undefined;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    GeometryInstance instance = geoObject as Autodesk.Revit.DB.GeometryInstance;
                    if (null != instance)
                    {
                        Transform transform = familyInstance.GetTransform();
                        GeometryElement instanTVDCEGeometryElement = instance.GetSymbolGeometry(transform);
                        foreach (GeometryObject instObj in instanTVDCEGeometryElement)
                        {
                            // Try to find solid
                            Solid solid = instObj as Solid;
                            if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;
                            if (!solids.Contains(solid)) solids.Add(solid);
                        }
                    }
                }
            }
        }
        public void GetSolids(FamilyInstance familyInstance, ref List<Solid> solids)
        {
            Options option = new Options();
            option.ComputeReferences = true;
            option.IncludeNonVisibleObjects = true;
            option.DetailLevel = ViewDetailLevel.Undefined;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    // Try to find solid
                    Solid solid = geoObject as Solid;
                    if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;
                    if (!solids.Contains(solid)) solids.Add(solid);
                }
            }
        }
        public void GetSolidsFromNested(FamilyInstance familyInstance, ref List<Solid> solids)
        {
            Document doc = familyInstance.Document;
            foreach (ElementId id in familyInstance.GetSubComponentIds())
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                if (instance == null) continue;
                GetSolids(instance, ref solids);
                GetSolidsFromSymbol(instance, ref solids);
            }
        }
        public List<Solid> AllSolids(FamilyInstance familyInstance)
        {
            List<Solid> allSolids = new List<Solid>();
            GetSolids(familyInstance, ref allSolids);
            GetSolidsFromSymbol(familyInstance, ref allSolids);
            GetSolidsFromNested(familyInstance, ref allSolids);
            return allSolids;
        }
        public void SelectElement(Document doc, UIDocument uidoc, List<ElementId> elementIds)
        {
            View3D view = Get3Dview(doc).First();
            List<XYZ> pointsmax = new List<XYZ>();
            List<XYZ> pointsmin = new List<XYZ>();
            foreach (var i in elementIds)
            {
                Element element = doc.GetElement(i);
                BoundingBoxXYZ boxXYZ = element.get_BoundingBox(view);
                XYZ max = boxXYZ.Max;
                XYZ min = boxXYZ.Min;
                pointsmax.Add(max);
                pointsmin.Add(min);
            }
            var Bpoint = new Maxpoint(pointsmax);
            var Vpoint = new Minpoint(pointsmin);
            XYZ Maxpoint = new XYZ(Bpoint.Xmax, Bpoint.Ymax, Bpoint.Zmax);
            XYZ Minpoint = new XYZ(Vpoint.Xmin, Vpoint.Ymin, Vpoint.Zmin);
            BoundingBoxXYZ viewSectionBox = new BoundingBoxXYZ();
            viewSectionBox.Max = Maxpoint;
            viewSectionBox.Min = Minpoint;
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Move And Resize Section Box");
                view.SetSectionBox(viewSectionBox);
                tx.Commit();
            }
            uidoc.ActiveView = view;
            uidoc.Selection.SetElementIds(elementIds);
            uidoc.RefreshActiveView();
            uidoc.ShowElements(elementIds);
        }
        public List<ElementId> Stringtoelementid(Dictionary<string, string> dic, List<string> listname)
        {
            List<ElementId> listid = new List<ElementId>();
            foreach (var name in listname)
            {
                var key = dic[name].Split(';').ToList();
                foreach (var item in key)
                {
                    var s1 = Convert.ToInt32(item);
                    var ele = new ElementId(s1);
                    listid.Add(ele);
                }
            }
            return listid;
        }
        public List<View3D> Get3Dview(Document doc)
        {
            var col = (from View3D x in new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>() where !x.IsAssemblyView select x).ToList();
            var col2 = (from View3D y in col where !y.IsTemplate select y).ToList();
            return col2;
        }
    }
    public class ElementMerge
    {
        private TreeNode m_allelem = new TreeNode("Elements (All)");
        public List<FamilyInstance> SelectedElement = new List<FamilyInstance>();
        ICollection<ElementId> listspecial = new List<ElementId>();
        Dictionary<string, List<FamilyInstance>> dicmrg = new Dictionary<string, List<FamilyInstance>>();
        Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
        Dictionary<string, List<FamilyInstance>> dic2 = new Dictionary<string, List<FamilyInstance>>();
        Dictionary<string, List<FamilyInstance>> dic3 = new Dictionary<string, List<FamilyInstance>>();
        Dictionary<string, List<FamilyInstance>> dic4 = new Dictionary<string, List<FamilyInstance>>();
        Dictionary<string, List<FamilyInstance>> dic5 = new Dictionary<string, List<FamilyInstance>>();
        public TreeNode AllElementNames
        {
            get
            {
                return m_allelem;
            }
        }
        public ElementMerge(Document doc)
        {
            //GetAllElement(doc);
            Getallvoid(doc);
            GetallStructuralFramming(doc);
            double X = 0;
            foreach (var item in dic3.Keys.ToList())
            {
                X = X + dic3[item].Count;
            }
            dicmrg = (dic2.Union(dic3).ToDictionary(pair => pair.Key, pair => pair.Value));
        }
        private void Getallvoid(Document doc)
        {
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Autodesk.Revit.DB.Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var value = pa.AsString();
                    if (value.Contains("VOID"))
                    {
                        if (dic2.ContainsKey("(" + i.Symbol.FamilyName + ")" + " " + i.Name))
                        {
                            var ele = i.SuperComponent;
                            if (ele == null)
                            {
                                dic2["(" + i.Symbol.FamilyName + ")" + " " + i.Name].Add(i);
                            }
                        }
                        else
                        {
                            var ele = i.SuperComponent;
                            if (ele == null)
                            {
                                dic2.Add("(" + i.Symbol.FamilyName + ")" + " " + i.Name, new List<FamilyInstance> { i });
                            }
                        }
                    }
                }

            }
            dic2.OrderByDescending(x => x.Key).ToList();
            foreach (var y in dic2)
            {
                AssortElement(y.Key, "VOID");
            }
        }
        private void Getallgenericmodel(Document doc)
        {
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Autodesk.Revit.DB.Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var value = pa.AsString();
                    if (!value.Contains("VOID"))
                    {
                        if (dic4.ContainsKey("(" + i.Symbol.FamilyName + ")" + " " + i.Name))
                        {
                            var ele = i.SuperComponent;
                            if (ele == null)
                            {
                                dic4["(" + i.Symbol.FamilyName + ")" + " " + i.Name].Add(i);
                            }
                        }
                        else
                        {
                            var ele = i.SuperComponent;
                            if (ele == null)
                            {
                                dic4.Add("(" + i.Symbol.FamilyName + ")" + " " + i.Name, new List<FamilyInstance> { i });
                            }
                        }
                    }
                }
                else
                {
                    if (dic4.ContainsKey("(" + i.Symbol.FamilyName + ")" + " " + i.Name))
                    {
                        var ele = i.SuperComponent;
                        if (ele == null)
                        {
                            dic4["(" + i.Symbol.FamilyName + ")" + " " + i.Name].Add(i);
                        }
                    }
                    else
                    {
                        var ele = i.SuperComponent;
                        if (ele == null)
                        {
                            dic4.Add("(" + i.Symbol.FamilyName + ")" + " " + i.Name, new List<FamilyInstance> { i });
                        }
                    }
                }
            }
            dic4.OrderByDescending(x => x.Key).ToList();
            foreach (var y in dic4)
            {
                AssortElement(y.Key, "Generic Model");
            }
        }
        private void GetallStructuralFramming(Document doc)
        {
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in Conn)
            {
                var ele = item.SuperComponent;
                if (ele == null)
                {
                    if (item.Symbol.FamilyName.Contains("DOUBLE_TEE") || item.Symbol.FamilyName.Contains("SINGLE_TEE"))
                    {
                        if (dic3.ContainsKey("DOUBLE_TEE"))
                        {
                            dic3["DOUBLE_TEE"].Add(item);
                        }
                        else
                        {
                            dic3.Add("DOUBLE_TEE", new List<FamilyInstance> { item });
                        }
                    }
                    if (item.Symbol.FamilyName.Contains("GIRDER") || item.Symbol.FamilyName.Contains("RBEAM"))
                    {
                        if (dic3.ContainsKey("BEAM"))
                        {
                            dic3["BEAM"].Add(item);
                        }
                        else
                        {
                            dic3.Add("BEAM", new List<FamilyInstance> { item });
                        }
                    }
                    if (item.Symbol.FamilyName.Contains("SPANDREL"))
                    {
                        if (dic3.ContainsKey("SPANDREL"))
                        {
                            dic3["SPANDREL"].Add(item);
                        }
                        else
                        {
                            dic3.Add("SPANDREL", new List<FamilyInstance> { item });
                        }
                    }
                    if (item.Symbol.FamilyName.Contains("WALL"))
                    {
                        if (dic3.ContainsKey("WALL"))
                        {
                            dic3["WALL"].Add(item);
                        }
                        else
                        {
                            dic3.Add("WALL", new List<FamilyInstance> { item });
                        }
                    }
                    if (item.Symbol.FamilyName.Contains("COLUMN"))
                    {
                        if (dic3.ContainsKey("COLUMN"))
                        {
                            dic3["COLUMN"].Add(item);
                        }
                        else
                        {
                            dic3.Add("COLUMN", new List<FamilyInstance> { item });
                        }
                    }
                }
            }
            //foreach (var i in Conn)
            //{
            //    if (dic3.ContainsKey("(" + i.Symbol.FamilyName + ")" + " " + i.Name))
            //    {
            //        var ele = i.SuperComponent;
            //        if (ele == null)
            //        {
            //            dic3["(" + i.Symbol.FamilyName + ")" + " " + i.Name].Add(i);
            //        }
            //    }
            //    else
            //    {
            //        var ele = i.SuperComponent;
            //        if (ele == null)
            //        {
            //            dic3.Add("(" + i.Symbol.FamilyName + ")" + " " + i.Name, new List<FamilyInstance> { i });
            //        }
            //    }
            //}
            dic3.OrderByDescending(x => x.Key).ToList();
            foreach (var y in dic3)
            {
                AssortElement(y.Key, "Structural Framming");
            }
        }
        private void AssortElement(string m_Element, string type)
        {
            foreach (TreeNode t in AllElementNames.Nodes)
            {
                if (t.Tag.Equals(type))
                {
                    t.Nodes.Add(new TreeNode(m_Element));
                    return;
                }
            }
            TreeNode categoryNode = new TreeNode(type);
            categoryNode.Tag = type;
            categoryNode.Nodes.Add(new TreeNode(m_Element));
            AllElementNames.Nodes.Add(categoryNode);
        }
        public void SelectElements()
        {
            ArrayList names = new ArrayList();
            foreach (TreeNode t in AllElementNames.Nodes)
            {
                foreach (TreeNode n in t.Nodes)
                {
                    if (n.Checked && 0 == n.Nodes.Count)
                    {
                        names.Add(n.Text);
                    }
                }
            }
            foreach (var v in dicmrg)
            {
                foreach (var i in names)
                {
                    if (i.Equals(v.Key))
                    {
                        foreach (var g in v.Value)
                        {
                            SelectedElement.Add(g);
                        }
                    }
                }
            }
        }
    }

    public class SettingCheckClashProduct
    {
        private static SettingCheckClashProduct _instance;
        private SettingCheckClashProduct()
        {

        }
        public static SettingCheckClashProduct Instance => _instance ?? (_instance = new SettingCheckClashProduct());
        public string DimensionType { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.23.SettingCheckClashProduct";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingCheckClashProduct.json";
        }

        public string GetFullFileName()
        {
            return GetFolderPath() + "\\" + GetFileName();
        }

        public void SaveSetting()
        {
            var gh = GetFullFileName();
            SettingExtension.SaveSetting(this, GetFullFileName());
        }

        public SettingCheckClashProduct GetSetting()
        {
            SettingCheckClashProduct setting = SettingExtension.GetSetting<SettingCheckClashProduct>(GetFullFileName());
            if (setting == null) setting = new SettingCheckClashProduct();
            return setting;
        }
    }

}
