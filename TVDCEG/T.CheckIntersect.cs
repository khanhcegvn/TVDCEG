#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CheckIntersectcmd : IExternalCommand
    {
        public IList<Data> ListAssembly = new List<Data>();
        public List<ElementId> listshow = new List<ElementId>();
        public List<FamilyInstance> list1 = new List<FamilyInstance>();
        public ICollection<ElementId> Memberid = new List<ElementId>();
        public List<ElementId> ids = new List<ElementId>();
        private ExternalEvent _exEvent;
        public IList<FamilyInstance> listcheck = new List<FamilyInstance>();
        public Dictionary<string, string> dic = new Dictionary<string, string>();
        public double[] parameterEmbed = new double[]
               {
                100,
                102,101,103,104,200,201,202,300,301,302
               };
        public double[] parameterRebar = new double[]
               {
                401,402,403,404,405,410
               };
        public double[] parameterStrand = new double[]
             {
                120,121,122,123,124,125,126,127
             };
        public double[] parameterLifting = new double[]
             {
                130,131,132
             };
        public Document doc;
        public UIDocument uidoc;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            Get_info v1 = new Get_info(doc, this);
            var form = new FrmCheck(this, doc);
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                if (form.r2 != null)
                {
                    Filterlist(doc, form.r2, ref list1, out Memberid);
                    var diclist = Filterelementbylist(doc, Memberid);
                    if (form.checkboxRebar)
                    {
                        if (diclist.Keys.ToList().Contains("REBAR"))
                        {
                            diclist["REBAR"].ToList().ForEach(x => ids.Add(x));
                        }
                    }
                    if (form.checkboxEmbed)
                    {
                        if (diclist.Keys.ToList().Contains("EMBED"))
                        {
                            diclist["EMBED"].ToList().ForEach(x => ids.Add(x));
                        }
                    }
                    if (form.checkboxStrand)
                    {
                        if (diclist.Keys.ToList().Contains("STRAND"))
                        {
                            diclist["STRAND"].ToList().ForEach(x => ids.Add(x));
                        }
                    }
                    if (form.checkboxLifting)
                    {
                        if (diclist.Keys.ToList().Contains("LIFTING"))
                        {
                            diclist["LIFTING"].ToList().ForEach(x => ids.Add(x));
                        }
                    }
                    if(form.checkboxEmbed)
                    {
                        Caculatetor(ids);
                    }
                    Showform(doc, ids, ref dic, form);
                }
            }
            return Result.Succeeded;
        }
        public void Filterlist(Document doc, AssemblyInstance assemblyInstance, ref List<FamilyInstance> list1, out ICollection<ElementId> Memberid)
        {
            Memberid = assemblyInstance.GetMemberIds();
            List<FamilyInstance> listinstance = new List<FamilyInstance>();
            listinstance = (from x in Memberid where !doc.GetElement(x).Category.Name.Contains("Structural Framing") select (FamilyInstance)doc.GetElement(x)).ToList();
            foreach (var item in listinstance)
            {
                ElementId faid = item.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("IDENTITY_DESCRIPTION");
                if (pa != null)
                {
                    string pa1 = pa.AsString();
                    list1.Add(item);
                }
            }
        }
        public Dictionary<string, List<ElementId>> Filterelementbylist(Document doc, ICollection<ElementId> elementIds)
        {
            Dictionary<string, List<ElementId>> dic = new Dictionary<string, List<ElementId>>();
            foreach (ElementId item in elementIds)
            {
                FamilyInstance ele = (FamilyInstance)doc.GetElement(item);
                ElementId faid = ele.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("SORTING_ORDER");
                if (pa != null)
                {
                    var pa1 = pa.AsInteger();
                    double value = Convert.ToDouble(pa1);
                    if (CheckElementParameter(parameterRebar, value))
                    {
                        if (dic.ContainsKey("REBAR"))
                        {
                            dic["REBAR"].Add(item);
                        }
                        else
                        {
                            dic.Add("REBAR", new List<ElementId> { item });
                        }
                    }
                    if (CheckElementParameter(parameterEmbed, value))
                    {
                        if (dic.ContainsKey("EMBED"))
                        {
                            dic["EMBED"].Add(item);
                        }
                        else
                        {
                            dic.Add("EMBED", new List<ElementId> { item });
                        }
                    }
                    if (CheckElementParameter(parameterStrand, value))
                    {
                        if (dic.ContainsKey("STRAND"))
                        {
                            dic["STRAND"].Add(item);
                        }
                        else
                        {
                            dic.Add("STRAND", new List<ElementId> { item });
                        }
                    }
                    if (CheckElementParameter(parameterLifting, value))
                    {
                        if (dic.ContainsKey("LIFTING"))
                        {
                            dic["LIFTING"].Add(item);
                        }
                        else
                        {
                            dic.Add("LIFTING", new List<ElementId> { item });
                        }
                    }
                }
            }
            return dic;
        }
        public bool CheckElementParameter(double[] parameters, double a)
        {
            var result = (from x in parameters.ToList() where a == x select x).ToList();
            if (result.Count != 0) return true;
            else return false;
        }
        public void Showform(Document doc, List<ElementId> elementIds, ref Dictionary<string, string> dic, FrmCheck frmCheck)
        {
            dic = Doing(doc, elementIds);
            var form2 = new Interference_Report(this, doc, frmCheck);
            this._exEvent = ExternalEvent.Create((IExternalEventHandler)new Intersect_event(this, doc, form2, frmCheck));
            form2.Show();
            form2.ExEvent = this._exEvent;
        }
        public Dictionary<string, string> Doing(Document doc, List<ElementId> elementIds)
        {
            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> dic2 = new Dictionary<string, List<string>>();
            Dictionary<string, string> dic3 = new Dictionary<string, string>();
            List<ElementId> listids = elementIds.ToList();
            ProgressBarform progressBarform = new ProgressBarform(listids.Count, "Loading...");
            progressBarform.Show();
            for (int i = 0; i < listids.Count; i++)
            {
                var item = (FamilyInstance)doc.GetElement(listids[i]);
                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                ElementId faid = item.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("IDENTITY_DESCRIPTION");
                Transform transform = item.GetTransform();
                if (pa != null)
                {
                    string pa1 = pa.AsString();
                    if (pa1.Contains("STRAND"))
                    {
                        IList<Solid> solids1 = lbr_.CreateSolid(doc, item, transform);
                        if (solids1 != null)
                        {
                            foreach (Solid it in solids1)
                            {
                                IList<FamilyInstance> a1 = Checkstrand(doc, it, listids);
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
                    else
                    {
                        List<Solid> solids = AllSolids(item);
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
        public void Caculatetor(List<ElementId> elementIds)
        {
            for (int i = 0; i < elementIds.Count; i++)
            {
                for (int j = 0; j < elementIds.Count; j++)
                {
                    FamilyInstance ele = (FamilyInstance)doc.GetElement(elementIds[i]);
                    ElementId faid = ele.GetTypeId();
                    Element elemtype = doc.GetElement(faid);
                    Parameter pa = elemtype.LookupParameter("SORTING_ORDER");
                    var pa1 = pa.AsInteger();
                    double value = Convert.ToDouble(pa1);
                    if (CheckElementParameter(parameterEmbed,value))
                    {
                        var temp = elementIds[i];
                        elementIds[i] = elementIds[j];
                        elementIds[j] = temp;
                    }
                }
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
        public List<ElementId> Stringtoelementid(List<string> listname)
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
        string Unionstring(FamilyInstance ele1, FamilyInstance ele2)
        {
            string item = string.Concat(new object[] { ele1.Name, " ", ":", " ", ele1.Id, " ", "intersect", " ", ele2.Name, " ", ":", " ", ele2.Id });
            return item;
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
        public List<FamilyInstance> Checkintersect(Document doc, Solid solid, List<ElementId> ColelementIds, FamilyInstance instance)
        {
            List<FamilyInstance> listfam = new List<FamilyInstance>();
            if (ColelementIds.Count == 0)
            {
                return listfam;
            }
            FilteredElementCollector filtercol = new FilteredElementCollector(doc, ColelementIds);
            ICollection<ElementId> col = filtercol.OfClass(typeof(FamilyInstance)).WherePasses(new ElementIntersectsSolidFilter(solid)).ToElementIds();
            foreach (var i in col)
            {
                Element element = doc.GetElement(i);
                FamilyInstance familyInstance = element as FamilyInstance;
                if (familyInstance != null && familyInstance.Name != "CONNECTOR_COMPONENT")
                {
                    if (!familyInstance.Symbol.Category.Name.Contains("Structural Framing"))
                    {
                        var val = GetSupFamilyInstance(familyInstance);
                        Element ele = instance.SuperComponent;
                        if (ele != null)
                        {
                            if (val.Id.IntegerValue != ele.Id.IntegerValue)
                            {
                                //listfam.Add(val);
                                Solid solid1 = AllSolids(familyInstance).First();
                                if (CheckSolid(solid, solid1))
                                {
                                    listfam.Add(val);
                                }
                            }
                        }
                        else
                        {
                            //listfam.Add(val);
                            if (val.Id.IntegerValue != instance.Id.IntegerValue)
                            {
                                Solid solid1 = AllSolids(familyInstance).First();
                                if (CheckSolid(solid, solid1))
                                {
                                    listfam.Add(val);
                                }
                            }
                        }
                    }
                }
            }
            return listfam;
        }
        public bool CheckSolid(Solid solid1,Solid solid2)
        {
            bool flag = false;
            try
            {
                Solid solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
                if (solid.Volume > 0.000000001)
                {
                    flag = true;
                }
                return flag;
            }
            catch
            {
                return flag;
            }
        }
        public IList<FamilyInstance> Checkstrand(Document doc, Solid solid, List<ElementId> ColelementIds)
        {
            IList<FamilyInstance> listfam = new List<FamilyInstance>();
            if (ColelementIds.Count == 0)
            {
                return listfam;
            }
            FilteredElementCollector filtercol = new FilteredElementCollector(doc, ColelementIds);
            ICollection<ElementId> col = filtercol.OfClass(typeof(FamilyInstance)).WherePasses(new ElementIntersectsSolidFilter(solid)).ToElementIds();
            foreach (var i in col)
            {
                Element element = doc.GetElement(i);
                FamilyInstance familyInstance = element as FamilyInstance;
                if (familyInstance != null && familyInstance.Symbol.Category.Name != "Structural Framing")
                {
                    listfam.Add(familyInstance);
                }
            }
            return listfam;
        }
        public void SelectElement(Document doc, List<ElementId> elementIds)
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
        public List<View3D> Get3Dview(Document doc)
        {
            var col = (from View3D x in new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>() where !x.IsAssemblyView select x).ToList();
            var col2 = (from View3D y in col where !y.IsTemplate select y).ToList();
            return col2;
        }
        public void GetFaces(FamilyInstance familyInstance, ref List<Face> faces)
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
                    Face face = geoObject as Face;
                    if (null == face || 0 == face.Area) continue;
                    if (!faces.Contains(face)) faces.Add(face);
                }
            }
        }
        public void GetFacesFromSymbol(FamilyInstance familyInstance, ref List<Face> faces)
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
                            Face face = instObj as Face;
                            if (null == face || 0 == face.Area) continue;
                            if (!faces.Contains(face)) faces.Add(face);
                        }
                    }
                }
            }
        }
        public void GetFacesFromNested(FamilyInstance familyInstance, ref List<Face> faces)
        {
            Document doc = familyInstance.Document;
            foreach (ElementId id in familyInstance.GetSubComponentIds())
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                if (instance == null) continue;
                GetFaces(instance, ref faces);
                GetFacesFromSymbol(instance, ref faces);
            }
        }
        public List<Face> AllFaces(FamilyInstance familyInstance)
        {
            List<Face> allFaces = new List<Face>();
            GetFaces(familyInstance, ref allFaces);
            GetFacesFromSymbol(familyInstance, ref allFaces);
            GetFacesFromNested(familyInstance, ref allFaces);
            return allFaces;
        }
    }
    //lay tat ca assembly trong du an 
    public class Get_info
    {
        void Getallassembly(Document doc, CheckIntersectcmd command)
        {
            FilteredElementCollector col
             = new FilteredElementCollector(doc)
               .WhereElementIsNotElementType()
               .OfCategory(BuiltInCategory.OST_Assemblies);
            foreach (var i in col)
            {
                command.ListAssembly.Add(new Data(i.Name, i.Id));
            }
        }
        public Get_info(Document doc, CheckIntersectcmd command)
        {
            Getallassembly(doc, command);
        }
    }
    public class Data
    {
        public string Assemblyname { get; set; }
        public ElementId Assemblyid { get; set; }
        public Data(string a, ElementId b)
        {
            Assemblyname = a;
            Assemblyid = b;
        }
    }
}
