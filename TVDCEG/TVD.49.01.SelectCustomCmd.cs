using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class SelectCustomCmd : IExternalCommand
    {
        public SettingSelectCustom Setting;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document document = activeUIDocument.Document;
            Selection sel = activeUIDocument.Selection;
            Setting = SettingSelectCustom.Instance.GetSetting();
            using (var form = new FrmFilterCustom(this))
            {
                if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    if (form.check)
                    {
                        string name = form.Name;
                        var col = sel.PickElementsByRectangle(new SelectCustomfilter(name));
                        var col2 = (from x in col select x.Id).ToList();
                        sel.SetElementIds(col2);
                        return Result.Succeeded;
                    }
                    else
                    {
                        return Result.Cancelled;
                    }
                }
                else
                {
                    return Result.Cancelled;
                }
            }
        }
    }
    public class SettingSelectCustom
    {
        private static SettingSelectCustom _instance;
        private SettingSelectCustom()
        {

        }
        public static SettingSelectCustom Instance => _instance ?? (_instance = new SettingSelectCustom());
        public string Name { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.49.SettingSelectCustom";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingSelectCustom.json";
        }

        public string GetFullFileName()
        {
            return GetFolderPath() + "\\" + GetFileName();
        }

        public void SaveSetting()
        {
            SettingExtension.SaveSetting(this, GetFullFileName());
        }

        public SettingSelectCustom GetSetting()
        {
            SettingSelectCustom setting = SettingExtension.GetSetting<SettingSelectCustom>(GetFullFileName());
            if (setting == null) setting = new SettingSelectCustom();
            return setting;
        }
    }
}
