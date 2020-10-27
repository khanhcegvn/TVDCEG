using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TVDCEG.Ultis;
using TVDCEG.WPF;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class SelectByControlMark : IExternalCommand
    {
        public Dictionary<string, List<FamilyInstance>> listinstance = new Dictionary<string, List<FamilyInstance>>();
        public List<View3D> list3d = new List<View3D>();
        public SettingSelecelementbymark Setting;
        public View3D Viewsorce;
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
            Selection sel = uidoc.Selection;
            listinstance = FilterelementByMark(doc);
            list3d = Get3Dview(doc);
            Setting = SettingSelecelementbymark.Instance.GetSetting();
            //FrmSelectByMark form = new FrmSelectByMark(this, doc);
            //form.ShowDialog();
            using (FrmSelectByMark form = new FrmSelectByMark(this, doc))
            {
                if (form.ShowDialog() == false)
                {
                    if (form.ids.Count != 0)
                    {
                        sel.SetElementIds(form.ids);
                        ICollection<ElementId> elementIds = new List<ElementId>();
                        Showview(doc, form.ids, form._view3d);
                    }
                }
            }
            return Result.Succeeded;
        }
        public Dictionary<string, List<FamilyInstance>> FilterelementByMark(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> list = new Dictionary<string, List<FamilyInstance>>();
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in col)
            {
                Parameter parameter = i.LookupParameter("CONTROL_MARK");
                if (parameter != null)
                {
                    var value = parameter.AsString();
                    if (value != null)
                    {
                        if (list.ContainsKey(value))
                        {
                            list[value].Add(i);
                        }
                        else
                        {
                            list.Add(value, new List<FamilyInstance> { i });
                        }
                    }
                }
            }
            return list;
        }
        public void Showview(Document doc, ICollection<ElementId> elementIds, View3D view)
        {
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
            XYZ Maxpoint = new XYZ(Bpoint.Xmax+5, Bpoint.Ymax+5, Bpoint.Zmax+5);
            XYZ Minpoint = new XYZ(Vpoint.Xmin+5, Vpoint.Ymin+5, Vpoint.Zmin+5);
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
            var col2 = (from View3D y in col where !y.IsTemplate select y).Cast<View3D>().ToList();
            return col2;
        }
    }
    public class SettingSelecelementbymark
    {
        private static SettingSelecelementbymark _intance;
        private SettingSelecelementbymark()
        {

        }
        public static SettingSelecelementbymark Instance => _intance ?? (_intance = new SettingSelecelementbymark());
        public string View3d { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.15.SettingSelecelementbymark";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingSelecelementbymark.json";
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

        public SettingSelecelementbymark GetSetting()
        {
            SettingSelecelementbymark setting = SettingExtension.GetSetting<SettingSelecelementbymark>(GetFullFileName());
            if (setting == null) setting = new SettingSelecelementbymark();
            return setting;
        }
    }
}
