#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class TopDTcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public SettingTopDT Setting;
        public bool iscontinue = true;
        public Dictionary<string, ViewSheet> dic_sheet = new Dictionary<string, ViewSheet>();
        public List<TextNoteType> listTextnotes = new List<TextNoteType>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            Setting = SettingTopDT.Instance.GetSetting();
            listTextnotes = Getmodelelement.GetTextNoteTypes(doc);
            using (var form = new FrmTopDT(this))
            {
                if (form.ShowDialog() != true && form.check)
                {
                    while (iscontinue)
                    {
                        Transaction tran = new Transaction(doc, "Callout dimension");
                        tran.Start();
                        try
                        {
                            Reference reference = sel.PickObject(ObjectType.Element, new FilterSpotElevation(), "Select Spot Elevation");
                            var element = doc.GetElement(reference);
                            var top = element.LookupParameter("Single/Upper Value").AsValueString();
                            string val1 = string.Empty;
                            string n = string.Empty;
                            double val2 = double.MinValue;
                            if (!string.IsNullOrEmpty(form.val) && form.tru == "")
                            {
                                val2 = UnitConvert.StringToFeetAndInches(top, out val1) + UnitConvert.StringToFeetAndInches(form.val, out n);
                            }
                            if (!string.IsNullOrEmpty(form.tru) && form.val == "")
                            {
                                val2 = UnitConvert.StringToFeetAndInches(top, out val1) - UnitConvert.StringToFeetAndInches(form.tru, out n);
                            }
                            var val3 = UnitConvert.DoubleToImperial(val2);
                            string empty = UnitConvert.StringToFeetAndInchesformattext(val3);
                            if (!string.IsNullOrEmpty(form.Suffix) && string.IsNullOrEmpty(form.Prefix))
                            {
                                CreteaTextnode(doc, empty + " " + form.Suffix, form.TextNoteType);
                            }
                            if (string.IsNullOrEmpty(form.Suffix) && !string.IsNullOrEmpty(form.Prefix))
                            {
                                CreteaTextnode(doc, form.Prefix + " " + empty, form.TextNoteType);
                            }
                            if (!string.IsNullOrEmpty(form.Suffix) && !string.IsNullOrEmpty(form.Prefix))
                            {
                                CreteaTextnode(doc, form.Prefix + " " + empty + " " + form.Suffix, form.TextNoteType);
                            }
                        }
                        catch (Exception)
                        {
                            this.iscontinue = false;
                        }
                        tran.Commit();
                    }
                }
            }
            return Result.Succeeded;

        }
        public void CreteaTextnode(Document doc, string value, TextNoteType textNoteType)
        {
            ElementId defaultTextTypeId = doc.GetDefaultElementTypeId(ElementTypeGroup.TextNoteType);
            TextNoteOptions opts = new TextNoteOptions(defaultTextTypeId);
            opts.TypeId = textNoteType.Id;
            XYZ point = sel.PickPoint();
            PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
            var P = pLane3D.ProjectPointOnPlane(point);
            TextNote textNote = TextNote.Create(doc, doc.ActiveView.Id, P, value, opts);
        }
    }
    public class FilterSpotElevation : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_SpotElevations) return true;
            else return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    public class SettingTopDT
    {
        private static SettingTopDT _instance;
        private SettingTopDT()
        {

        }
        public static SettingTopDT Instance => _instance ?? (_instance = new SettingTopDT());
        public string Prefix { get; set; }
        public string Suffix { get; set; }
        public string cong { get; set; }
        public string tru { get; set; }
        public string Textnote { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.43.SettingTopDT";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingTopDT.json";
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

        public SettingTopDT GetSetting()
        {
            SettingTopDT setting = SettingExtension.GetSetting<SettingTopDT>(GetFullFileName());
            if (setting == null) setting = new SettingTopDT();
            return setting;
        }
    }
}
