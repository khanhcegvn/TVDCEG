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
using System.Threading;
using TVDCEG.Ultis;
using System.Security.Permissions;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class ManagerSectioncmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        private ExternalEvent _exEvent;
        public SettingManagerSection Setting;
        public ManagerSectionModel ViewModel;
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
            Setting = SettingManagerSection.Instance.GetSetting();
            if(Setting.FamilySymbol==null&&Setting.Parameter==null)
            {
                var form = new FrmSettingManagerSection(this, doc);
                form.ShowDialog();
            }
            ViewModel = new ManagerSectionModel(doc, Setting);
            Run(doc);
            return Result.Succeeded;
        }
        public void Run(Document doc)
        {
            Setting = SettingManagerSection.Instance.GetSetting();
            ViewModel = new ManagerSectionModel(doc, Setting);
            var form = new FrmManagerSection(this, doc);
            this._exEvent = ExternalEvent.Create((IExternalEventHandler)new ManagerSectionEvent(this, doc));
            form.Show();
            form.ExEvent = this._exEvent;
        }
    }
    public class SettingManagerSection
    {
        private static SettingManagerSection _instance;
        private SettingManagerSection()
        {

        }
        public static SettingManagerSection Instance => _instance ?? (_instance = new SettingManagerSection());
        public string FamilySymbol { get; set; }
        public string Parameter { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.47.SettingManagerSection";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingManagerSection.json";
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

        public SettingManagerSection GetSetting()
        {
            SettingManagerSection setting = SettingExtension.GetSetting<SettingManagerSection>(GetFullFileName());
            if (setting == null) setting = new SettingManagerSection();
            return setting;
        }
    }
    public class ObjectSheetManager
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public ObjectSheetManager(string name,ElementId id)
        {
            Name = name;
            Id = id.IntegerValue;
        }
    }
    public class ObjectSectionManager
    {
        public string Name { get; set; }
        public ElementId Id { get; set; }
        public ObjectSectionManager(string name,ElementId id)
        {
            Name = name;
            Id = id;
        }
    }
}
