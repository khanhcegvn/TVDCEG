#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CheckWorkPlanecmd : IExternalCommand
    {
        public List<ReferencePlane> listskp = new List<ReferencePlane>();
        public FamilyInstance instance = null;
        public List<FamilyInstance> listinstance = new List<FamilyInstance>();
        public List<GroupInstanceInfo> listsource = new List<GroupInstanceInfo>();
        public List<FamilyInstance> source = new List<FamilyInstance>();
        public ICollection<ElementId> list = new List<ElementId>();
        public FamilyInstance newfamily = null;
        public ElementMgr elementMgr = null;
        private ExternalEvent _exEvent;
        public UIDocument uidoc;
        public Selection sel;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            sel = uidoc.Selection;
            elementMgr = new ElementMgr(doc);
            var form = new FrmShowRefernceplane(this, doc);
            if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            {
                if (form.list.Count != 0)
                {
                    source = form.list;
                    Doding(doc, form.list,form.checklevelgrid);
                }
            }
            return Result.Succeeded;
        }
        private void Doding(Document doc, List<FamilyInstance> listinstance,bool checklevelandgrid)
        {
            ProgressBarform progressBarform = new ProgressBarform(listinstance.Count, "Loading...");
            progressBarform.Show();
            foreach (var instance in listinstance)
            {
                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                var Workplane = instance.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
                if (Workplane != null)
                {
                    var value = Workplane.AsString();
                    if(checklevelandgrid)
                    {
                        if (!value.StartsWith("Reference Plane"))
                        {
                            listsource.Add(new GroupInstanceInfo(instance));
                        }
                    }
                    else
                    {
                        if (!value.StartsWith("Grid") && !value.StartsWith("Reference Plane") && !value.StartsWith("Level"))
                        {
                            listsource.Add(new GroupInstanceInfo(instance));
                        }
                    }
                }
            }
            progressBarform.Close();
            this._exEvent = ExternalEvent.Create((IExternalEventHandler)new CheckWorkPlaneEvent(this, doc));
            var form = new FrmCheckReference(this, doc);
            form.Show();
            form.ExEvent = this._exEvent;
        }
        public void Updateinfo(List<FamilyInstance> list,bool check)
        {
            foreach (var instance in list)
            {
                var Workplane = instance.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
                if (Workplane != null)
                {
                    var value = Workplane.AsString();
                    if(check)
                    {
                        if (!value.StartsWith("Reference Plane"))
                        {
                            listsource.Add(new GroupInstanceInfo(instance));
                        }
                    }
                    else
                    {
                        if (!value.StartsWith("Grid") && !value.StartsWith("Reference Plane") && !value.StartsWith("Level"))
                        {
                            listsource.Add(new GroupInstanceInfo(instance));
                        }
                    }
                }
            }
        }
        public void Export(List<GroupInstanceInfo> familyinstances)
        {
            #region xuat bang EPPlus
            //using (SaveFileDialog saveDialog = new SaveFileDialog())
            //{
            //    saveDialog.Filter = "Excel((.xlsx)|*.xlsx";
            //    if (saveDialog.ShowDialog() != DialogResult.Cancel)
            //    {
            //        string exportFilePath = saveDialog.FileName;
            //        var newFile = new FileInfo(exportFilePath);
            //        if (File.Exists(exportFilePath))
            //        {
            //            File.Delete(exportFilePath);
            //        }
            //        ExcelPackage excel = new ExcelPackage(newFile);
            //        var workSheet = excel.Workbook.Worksheets.Add("List Element");
            //        workSheet.TabColor = System.Drawing.Color.Black;
            //        workSheet.DefaultRowHeight = 12;
            //        workSheet.Row(1).Height = 20;
            //        workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //        workSheet.Row(1).Style.Font.Bold = true;
            //        workSheet.Cells[1, 1].Value = "Name";
            //        workSheet.Cells[1, 2].Value = "Id";
            //        workSheet.Cells[1, 3].Value = "Work Plane";
            //        int recordIndex = 5;
            //        foreach (var item in familyinstances)
            //        {
            //            workSheet.Cells[recordIndex, 1].Value = item.Name;
            //            workSheet.Cells[recordIndex, 2].Value = item.ID;
            //            workSheet.Cells[recordIndex, 3].Value = item.Workplane;
            //            recordIndex++;
            //        }
            //        workSheet.Column(1).AutoFit();
            //        workSheet.Column(2).AutoFit();
            //        workSheet.Column(3).AutoFit();
            //        excel.Save();
            //    }
            //}
            #endregion
            _Application application = (Microsoft.Office.Interop.Excel.Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
            _Workbook workbook = application.Workbooks.Add(Type.Missing);
            int count = workbook.Sheets.Count;
            _Worksheet worksheet = (_Worksheet)workbook.Worksheets.Add((dynamic)workbook.Sheets[count], Type.Missing, Type.Missing, Type.Missing);
            worksheet.Name = "List Element";
            dynamic val = worksheet.Range[(dynamic)worksheet.Cells[1, 1], (dynamic)worksheet.Cells[1, 5]];
            val.Font.Bold = true;
            val.Merge();
            val = worksheet.Range[(dynamic)worksheet.Cells[2, 1], (dynamic)worksheet.Cells[2, 5]];
            val.Font.Bold = true;
            val.Merge();
            worksheet.Cells[3, 1] = "Name";
            worksheet.Cells[3, 2] = "Id";
            worksheet.Cells[3, 3] = "Work Plane";
            val = worksheet.Range[(dynamic)worksheet.Cells[3, 1], (dynamic)worksheet.Cells[3, 4]];
            val.Font.Bold = true;
            int num = 5;
            foreach (var item in familyinstances)
            {
                worksheet.Cells[num, 1] = item.Name;
                worksheet.Cells[num, 2] = item.ID;
                worksheet.Cells[num, 3] = item.Workplane;
                num++;
            }
            application.Visible = true;
            application.WindowState = XlWindowState.xlMaximized;
        }
        public void Showview(Document doc,UIDocument uidoc, ICollection<ElementId> elementIds,string viewname)
        {
            //View3D view = Get3Dview(doc);
            //List<XYZ> pointsmax = new List<XYZ>();
            //List<XYZ> pointsmin = new List<XYZ>();
            //foreach (var i in elementIds)
            //{
            //    Element element = doc.GetElement(i);
            //    BoundingBoxXYZ boxXYZ = element.get_BoundingBox(view);
            //    XYZ max = boxXYZ.Max;
            //    XYZ min = boxXYZ.Min;
            //    pointsmax.Add(max);
            //    pointsmin.Add(min);
            //}
            //var Bpoint = new Maxpoint(pointsmax);
            //var Vpoint = new Minpoint(pointsmin);
            //XYZ Maxpoint = new XYZ(Bpoint.Xmax, Bpoint.Ymax, Bpoint.Zmax);
            //XYZ Minpoint = new XYZ(Vpoint.Xmin, Vpoint.Ymin, Vpoint.Zmin);
            //BoundingBoxXYZ viewSectionBox = new BoundingBoxXYZ();
            //viewSectionBox.Max = Maxpoint;
            //viewSectionBox.Min = Minpoint;
            //using (Transaction tx = new Transaction(doc))
            //{
            //    tx.Start("Move And Resize Section Box");
            //    view.SetSectionBox(viewSectionBox);
            //    tx.Commit();
            //}
            //uidoc.ActiveView = view;
            //uidoc.Selection.SetElementIds(elementIds);
            //uidoc.RefreshActiveView();
            //uidoc.ShowElements(elementIds);
            SetBoundingBox(doc, elementIds);
            uidoc.Selection.SetElementIds(elementIds);
            foreach (UIView uiview in uidoc.GetOpenUIViews())
            {
                Autodesk.Revit.DB.View view = uidoc.Document.GetElement(uiview.ViewId) as Autodesk.Revit.DB.View;
                bool flag = ((view != null) ? view.Name : null) == viewname;
                if (flag)
                {
                    uiview.ZoomToFit();
                    break;
                }
            }
        }
        public void SetBoundingBox(Document doc, ICollection<ElementId> ids)
        {
            try
            {
                View3D view3D = doc.ActiveView as View3D;
                bool flag = view3D == null;
                if (flag)
                {
                    TaskDialog.Show("Error", "Please go to 3D view");
                }
                else
                {
                    XYZ xyz = XYZ.Zero;
                    XYZ xyz2 = XYZ.Zero;
                    foreach (ElementId id in ids)
                    {
                        FamilyInstance familyInstance = doc.GetElement(id) as FamilyInstance;
                        bool flag2 = familyInstance == null;
                        if (!flag2)
                        {
                            BoundingBoxXYZ boundingBoxXYZ = familyInstance.get_BoundingBox(view3D);
                            XYZ min = boundingBoxXYZ.Min;
                            XYZ max = boundingBoxXYZ.Max;
                            bool flag3 = xyz.IsAlmostEqualTo(XYZ.Zero);
                            if (flag3)
                            {
                                xyz = min;
                            }
                            else
                            {
                                xyz = new XYZ(Math.Min(xyz.X, min.X), Math.Min(xyz.Y, min.Y), Math.Min(xyz.Z, min.Z));
                            }
                            bool flag4 = xyz2.IsAlmostEqualTo(XYZ.Zero);
                            if (flag4)
                            {
                                xyz2 = max;
                            }
                            else
                            {
                                xyz2 = new XYZ(Math.Max(xyz2.X, max.X), Math.Max(xyz2.Y, max.Y), Math.Max(xyz2.Z, max.Z));
                            }
                        }
                    }
                    Transaction transaction = new Transaction(doc, "SetBoundingBox");
                    transaction.Start();
                    BoundingBoxXYZ boundingBoxXYZ2 = new BoundingBoxXYZ();
                    boundingBoxXYZ2.Min = xyz - new XYZ(1.0, 1.0, 1.0);
                    boundingBoxXYZ2.Max = xyz2 + new XYZ(1.0, 1.0, 1.0);
                    view3D.IsSectionBoxActive = true;
                    view3D.SetSectionBox(boundingBoxXYZ2);
                    transaction.Commit();
                }
            }
            catch
            {

            }
        }
        public View3D Get3Dview(Document doc)
        {
            View3D view3D = null;
            List<View3D> view3Ds = new List<View3D>();
            var col = new FilteredElementCollector(doc).OfClass(typeof(View3D)).WhereElementIsNotElementType().Cast<View3D>().ToList();
            foreach (var i in col)
            {
                if (i.IsTemplate) continue;
                view3Ds.Add(i);
            }
            view3D = view3Ds.First();
            return view3D;
        }
    }
    #region Codehandling
    public class ElementMgr
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
        public ElementMgr(Document doc)
        {
            //GetAllElement(doc);
            Getallconnection(doc);
            Getallvoid(doc);
            Getallgenericmodel(doc);
            GetallStructuralFramming(doc);
            Getallspecialityequipment(doc);
            dicmrg = (dic.Union(dic2).ToDictionary(pair => pair.Key, pair => pair.Value)).Union(dic3).ToDictionary(pair => pair.Key, pair => pair.Value).Union(dic4).ToDictionary(pair => pair.Key, pair => pair.Value).Union(dic5).ToDictionary(pair => pair.Key, pair => pair.Value);
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
            foreach (var i in Conn)
            {
                if (dic3.ContainsKey("(" + i.Symbol.FamilyName + ")" + " " + i.Name))
                {
                    var ele = i.SuperComponent;
                    if (ele == null)
                    {
                        dic3["(" + i.Symbol.FamilyName + ")" + " " + i.Name].Add(i);
                    }
                }
                else
                {
                    var ele = i.SuperComponent;
                    if (ele == null)
                    {
                        dic3.Add("(" + i.Symbol.FamilyName + ")" + " " + i.Name, new List<FamilyInstance> { i });
                    }
                }
            }
            dic3.OrderByDescending(x => x.Key).ToList();
            foreach (var y in dic3)
            {
                AssortElement(y.Key, "Structural Framming");
            }
        }
        private void Getallspecialityequipment(Document doc)
        {
            var Conn = new FilteredElementCollector(doc, listspecial).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Autodesk.Revit.DB.Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var vla = pa.AsString();
                    if (!vla.Contains("CONNECTION") && !vla.Contains("REBAR") && !vla.Contains("LIFTING"))
                    {
                        if (dic5.ContainsKey("(" + i.Symbol.FamilyName + ")" + " " + i.Name))
                        {
                            var ele = i.SuperComponent;
                            if (ele == null)
                            {
                                dic5["(" + i.Symbol.FamilyName + ")" + " " + i.Name].Add(i);
                            }
                        }
                        else
                        {
                            var ele = i.SuperComponent;
                            if (ele == null)
                            {
                                dic5.Add("(" + i.Symbol.FamilyName + ")" + " " + i.Name, new List<FamilyInstance> { i });
                            }
                        }
                    }
                }
                else
                {
                    if (dic5.ContainsKey("(" + i.Symbol.FamilyName + ")" + " " + i.Name))
                    {
                        var ele = i.SuperComponent;
                        if (ele == null)
                        {
                            dic5["(" + i.Symbol.FamilyName + ")" + " " + i.Name].Add(i);
                        }
                    }
                    else
                    {
                        var ele = i.SuperComponent;
                        if (ele == null)
                        {
                            dic5.Add("(" + i.Symbol.FamilyName + ")" + " " + i.Name, new List<FamilyInstance> { i });
                        }
                    }
                }
            }
            dic5.OrderByDescending(x => x.Key).ToList();
            foreach (var y in dic5)
            {
                AssortElement(y.Key, "Speciality Equipment");
            }
        }
        private void Getallconnection(Document doc)
        {
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Autodesk.Revit.DB.Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var vla = pa.AsString();
                    if (vla.Contains("CONNECTION"))
                    {
                        if (dic.ContainsKey(i.Name))
                        {
                            dic[i.Name].Add(i);
                        }
                        else
                        {
                            dic.Add(i.Name, new List<FamilyInstance> { i });
                        }
                    }
                    else
                    {
                        listspecial.Add(i.Id);
                    }
                }
            }
            var list = dic.Keys.ToList();
            list.Sort();
            foreach (var y in list)
            {
                AssortElement(y, "Connections");
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
    public class FamilyinstanceInfo
    {
        public ElementId ID { get; set; }
        public string Workplane { get; set; }
        public string Name { get; set; }
        public XYZ location { get; set; }
        public FamilyinstanceInfo(FamilyInstance familyInstance)
        {
            ID = familyInstance.Id;
            Autodesk.Revit.DB.Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
            if (parameter != null)
            {
                var value = parameter.AsString();
                if (value != null)
                {
                    Workplane = value;
                }
                else
                {
                    Workplane = "";
                }
            }
            if (familyInstance.Name.Contains("CONN"))
            {
                Name = familyInstance.Name;
            }
            else
            {
                Name = "(" + familyInstance.Symbol.FamilyName + ")" + " " + familyInstance.Name;
            }
            LocationPoint locationPoint = familyInstance.Location as LocationPoint;
            location = locationPoint.Point;
        }
    }
    public class UpdateGroupinstance
    {
        public static UpdateGroupinstance _instance;
        private UpdateGroupinstance()
        {

        }
        public static UpdateGroupinstance Instance => _instance ?? (_instance = new UpdateGroupinstance());
        public Dictionary<string, GroupInstanceInfo> Filterlist(List<GroupInstanceInfo> list)
        {
            Dictionary<string, List<GroupInstanceInfo>> dic1 = SortInstance(list);
            Dictionary<string, GroupInstanceInfo> dic = new Dictionary<string, GroupInstanceInfo>();
            foreach (string text in dic1.Keys)
            {
                var bm = dic1[text];
                for (int i = 0; i < dic1[text].Count; i++)
                {
                    var item = dic1[text][i];
                    string key1 = item.Bassic + item.Name;
                    if (dic.ContainsKey(key1))
                    {
                        dic[key1].Name = dic[key1].Name;
                        dic[key1].ID = dic[key1].ID + ";" + item.ID;
                        dic[key1].Workplane = dic[key1].Workplane + ";" + item.Workplane;
                    }
                    else
                    {
                        dic.Add(key1, item);
                    }
                }
            }
            return dic;
        }
        Dictionary<string, List<GroupInstanceInfo>> SortInstance(List<GroupInstanceInfo> list)
        {
            Dictionary<string, List<GroupInstanceInfo>> dic = new Dictionary<string, List<GroupInstanceInfo>>();
            foreach (var item in list)
            {
                if (dic.ContainsKey(item.Name))
                {
                    dic[item.Name].Add(item);
                }
                else
                {
                    dic.Add(item.Name, new List<GroupInstanceInfo> { item });
                }
            }
            return dic;
        }
        Dictionary<string, List<GroupInstanceInfo>> SortByorin(List<GroupInstanceInfo> list)
        {
            Dictionary<string, List<GroupInstanceInfo>> dic1 = SortInstance(list);
            Dictionary<string, List<GroupInstanceInfo>> dic = new Dictionary<string, List<GroupInstanceInfo>>();
            Dictionary<string, List<GroupInstanceInfo>> dic2 = new Dictionary<string, List<GroupInstanceInfo>>();
            foreach (string text in dic1.Keys)
            {
                for (int i = 0; i < dic1[text].Count; i++)
                {
                    var item = dic1[text][i];
                    string locmet = item.Bassic + item.Name;
                    if (dic.ContainsKey(locmet))
                    {
                        dic[locmet].Add(item);
                    }
                    else
                    {
                        dic.Add(locmet, new List<GroupInstanceInfo> { item });
                    }
                }
            }
            return dic;
        }
    }
    public class GroupInstanceInfo
    {
        public string ID { get; set; }
        public string Workplane { get; set; }
        public string Name { get; set; }
        public string Handing { get; set; }
        public string Bassic { get; }
        public XYZ location { get; set; }
        public XYZ Origintranform { get; }
        public Transform Transform { get; }
        public GroupInstanceInfo(FamilyInstance familyInstance)
        {
            ID = familyInstance.Id.ToString();
            Autodesk.Revit.DB.Parameter parameter = familyInstance.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
            if (parameter != null)
            {
                var value = parameter.AsString();
                if (value != null)
                {
                    Workplane = value;
                }
                else
                {
                    Workplane = "";
                }
            }

            if (familyInstance.Name.Contains("CONN"))
            {
                Name = familyInstance.Name;
            }
            else
            {
                Name = "(" + familyInstance.Symbol.FamilyName + ")" + " " + familyInstance.Name;
            }
            location = (familyInstance.Location as LocationPoint).Point;
            Origintranform = familyInstance.GetTransform().Origin;
            Handing = familyInstance.HandOrientation.ToString() + familyInstance.Name;
            Transform = familyInstance.GetTransform();
            Bassic = ((familyInstance.GetTransform().BasisX) + ";" + (familyInstance.GetTransform().BasisY) + ";" + (familyInstance.GetTransform().BasisZ)).ToString();
        }
    }
    public class Maxpoint
    {
        public double Xmax { get; set; }
        public double Ymax { get; set; }
        public double Zmax { get; set; }
        public Maxpoint(List<XYZ> list)
        {
            List<double> X = new List<double>();
            List<double> Y = new List<double>();
            List<double> Z = new List<double>();
            foreach (var i in list)
            {
                X.Add(i.X);
                Y.Add(i.Y);
                Z.Add(i.Z);
            }
            Sortlist(X);
            Sortlist(Y);
            Sortlist(Z);
            Xmax = X.Last();
            Ymax = Y.Last();
            Zmax = Z.Last();
        }
        private void Sortlist(List<double> Xyzs)
        {
            for (int i = 0; i < Xyzs.Count; i++)
            {
                for (int j = 0; j < Xyzs.Count; j++)
                {
                    if (Xyzs[i] < Xyzs[j])
                    {
                        var temp = Xyzs[i];
                        Xyzs[i] = Xyzs[j];
                        Xyzs[j] = temp;
                    }
                }
            }
        }
    }
    public class Minpoint
    {
        public double Xmin { get; set; }
        public double Ymin { get; set; }
        public double Zmin { get; set; }
        public Minpoint(List<XYZ> list)
        {
            List<double> X = new List<double>();
            List<double> Y = new List<double>();
            List<double> Z = new List<double>();
            foreach (var i in list)
            {
                X.Add(i.X);
                Y.Add(i.Y);
                Z.Add(i.Z);
            }
            Sortlist(X);
            Sortlist(Y);
            Sortlist(Z);
            Xmin = X.First();
            Ymin = Y.First();
            Zmin = Z.First();
        }
        private void Sortlist(List<double> Xyzs)
        {
            for (int i = 0; i < Xyzs.Count; i++)
            {
                for (int j = 0; j < Xyzs.Count; j++)
                {
                    if (Xyzs[i] < Xyzs[j])
                    {
                        var temp = Xyzs[i];
                        Xyzs[i] = Xyzs[j];
                        Xyzs[j] = temp;
                    }
                }
            }
        }
    }
    #endregion
}
