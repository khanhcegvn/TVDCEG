using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    public class SetValueTemplateSheet
    {
        private static SetValueTemplateSheet _instance;
        private SetValueTemplateSheet()
        {

        }
        public static SetValueTemplateSheet Instance => _instance ?? (_instance = new SetValueTemplateSheet());
        public FamilyInstance Gettileblock(Document doc)
        {
            FamilyInstance familyInstance = null;
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_TitleBlocks).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in col)
            {
                if (i.Category.ToBuiltinCategory() == BuiltInCategory.OST_TitleBlocks)
                {
                    familyInstance = i;
                }
            }
            return familyInstance;
        }
        //lay parameter va value 
        public CegParameterSet GetParameter(Document doc, FamilyInstance Titleblock)
        {
            CegParameterSet cegParameterSet = new CegParameterSet();
            if (doc.ActiveView.IsViewSheet())
            {
                ViewSheet viewSheet = doc.ActiveView as ViewSheet;
                string sheetName = GetSheetName(viewSheet);
                foreach (object obj in viewSheet.Parameters)
                {
                    Parameter parameter = (Parameter)obj;
                    bool isReadOnly = parameter.IsReadOnly;
                    if (!isReadOnly)
                    {
                        string name = parameter.Definition.Name;
                        CegParameterInfo value = new CegParameterInfo(parameter);
                        cegParameterSet.Parameters[name] = value;
                    }
                }
            }
            return cegParameterSet;
        }
        public void SaveTemplateSheet(Document doc,CegParameterSet cegParameterSet,string filename,SettingSetValueSheet setting)
        {
            string path = setting.GetFullFileName(filename);
            string contents = JsonConvert.SerializeObject(cegParameterSet, Formatting.Indented);
            File.WriteAllText(path, contents);
        }
        // lay sheetname
        public string GetSheetName(ViewSheet viewSheet)
        {
            Document document = viewSheet.Document;
            string text = viewSheet.SheetNumber + "-" + viewSheet.Name;
            bool isAssemblyView = viewSheet.IsAssemblyView;
            if (isAssemblyView)
            {
                AssemblyInstance assemblyInstance = document.GetElement(viewSheet.AssociatedAssemblyInstanceId) as AssemblyInstance;
                bool flag = assemblyInstance != null;
                if (flag)
                {
                    text = text + "-" + assemblyInstance.AssemblyTypeName;
                }
            }
            return text;
        }
    }
    public class SettingSetValueSheet
    {
        private static SettingSetValueSheet _instance;
        private SettingSetValueSheet()
        {

        }
        public static SettingSetValueSheet Instance => _instance ?? (_instance = new SettingSetValueSheet());
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.53.SettingSetValueSheet";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFullFileName(string filename)
        {
            return GetFolderPath() + "\\" + filename + ".json";
        }

        public void SaveSetting(string filename)
        {
            SettingExtension.SaveSetting(this, GetFullFileName(filename));
        }

        public SettingSetValueSheet GetSetting(string filename)
        {
            DirectoryInfo d = new DirectoryInfo(GetFolderPath());
            var p = d.GetFiles().ToList();
            List<string> list = new List<string>();
            p.ForEach(x => list.Add(x.Name));
            if (list.Count == 0)
            {
                SettingSetValueSheet setting = SettingExtension.GetSetting<SettingSetValueSheet>(GetFullFileName("%^^"));
                if (setting == null) setting = new SettingSetValueSheet();
                return setting;
            }
            else
            {
                var fg = GetFullFileName(filename);
                SettingSetValueSheet setting = SettingExtension.GetSetting<SettingSetValueSheet>(GetFullFileName(filename));
                if (setting == null) setting = new SettingSetValueSheet();
                return setting;
            }
        }
    }
}
