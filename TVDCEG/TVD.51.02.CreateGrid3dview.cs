using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Markup.Localizer;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    public class CreateGrid3dview
    {
        private static CreateGrid3dview _instance;
        private CreateGrid3dview()
        {

        }
        public static CreateGrid3dview Instance => _instance ?? (_instance = new CreateGrid3dview());
        public FamilySymbol GetSymbol(Document doc, SettingCreateGrid3d Setting)
        {
            FamilySymbol symbol = null;
            if (Setting.Projectname == doc.Title)
            {
                Element elem = doc.GetElement(Setting.Idsymbol);
                symbol = elem as FamilySymbol;
                if (symbol == null)
                {
                    Setting.Projectname = doc.Title;
                    var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericAnnotation).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().ToList();
                    foreach (var item in col)
                    {
                        if (item.FamilyName == "3D Grid")
                        {
                            symbol = item;
                        }
                    }
                    if (symbol != null)
                    {
                        Setting.Idsymbol = symbol.UniqueId;
                        Setting.SaveSetting();
                    }
                    else
                    {
                        Transaction tran = new Transaction(doc, "Load Family");
                        tran.Start();
                        string Path = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Family\3DGrid.rfa";
                        doc.LoadFamilySymbol(Path, "650mm Bubble - Single", out symbol);
                        if ((!symbol.IsActive) && (symbol != null))
                        { symbol.Activate(); doc.Regenerate(); }
                        Setting.Idsymbol = symbol.UniqueId;
                        Setting.SaveSetting();                      
                        tran.Commit();
                    }
                }
            }
            else
            {
                Transaction tran = new Transaction(doc, "Load Family");
                tran.Start();
                string Path = @"C:\ProgramData\Autodesk\ApplicationPlugins\TVD\Family\3DGrid.rfa";
                doc.LoadFamilySymbol(Path, "650mm Bubble - Single", out symbol);
                // check symbol da active chua, moi lan load family moi phai regenerate lai project
                if ((!symbol.IsActive) && (symbol != null))
                { symbol.Activate(); doc.Regenerate(); }
                Setting.Idsymbol = symbol.UniqueId;
                Setting.Projectname = doc.Title;
                Setting.SaveSetting();            
                tran.Commit();
            }
            return symbol;
        }

        [Obsolete]
        public void Create3dGrid(Document doc, FamilySymbol symbol)
        {
            var alllevel = Getmodelelement.GetallFloorplan(doc);
            var listgrids = Getmodelelement.GetAllGrid(doc);
            foreach (var item in listgrids)
            {
                Line line = item.Curve as Line;
                string namegrid = item.Name;
                var instance = doc.Create.NewFamilyInstance(item.Curve, symbol, alllevel.First().GenLevel, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
                var text = instance.LookupParameter("Text");
                text.SetParameterValue(namegrid);
                instance.Pinned = true;
            }
            ParameterFilterElement parameterFilterElement;
            CreateFilterRule3dGrid(doc, out parameterFilterElement);
            AddFilterRule(doc, parameterFilterElement);
        }
        public void Deleteolddata(Document doc)
        {
            using (Transaction tran = new Transaction(doc,"Delete old data"))
            {
                tran.Start();
                var collector = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>() where x.Name.Contains("650mm Bubble - Single") select x.Id).ToList();
                doc.Delete(collector);
                doc.Regenerate();
                tran.Commit();
            }
        }
        //Add new filter rule
        [Obsolete]
        public void CreateFilterRule3dGrid(Document doc,out ParameterFilterElement parameterFilterElement)
        {
            if(CheckFilter(doc,out parameterFilterElement))
            {
               
            }
            else
            {
                List<ElementId> categories = new List<ElementId>();
                categories.Add(new ElementId(BuiltInCategory.OST_GenericModel));
                List<FilterRule> filterRules = new List<FilterRule>();
                // Create filter element assocated to the input categories
                parameterFilterElement = ParameterFilterElement.Create(doc, "Grid 3D", categories);
                var collector = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>() where x.Name.Contains("650mm Bubble - Single") select x).ToList();
                FamilyInstance familyInstance = collector.First();

                if (familyInstance != null)
                {
                    ElementId sharedParamId = familyInstance.get_Parameter(BuiltInParameter.ALL_MODEL_FAMILY_NAME).Id;

                    filterRules.Add(ParameterFilterRuleFactory.CreateContainsRule(sharedParamId, "3D Grid", true));
                }

                parameterFilterElement.SetRules(filterRules);
                // Apply filter to view
                //doc.ActiveView.AddFilter(parameterFilterElement.Id);
                //doc.ActiveView.SetFilterVisibility(parameterFilterElement.Id, false);
            }
        }
        public void AddFilterRule(Document doc, ParameterFilterElement parameterFilterElement)
        {
            List<View> ListViewTemplate = Getmodelelement.GetallViewTeamplate(doc);
            foreach (View item in ListViewTemplate)
            {
                if(CheckFilterinview(doc,item))
                {
                    continue;
                }
                else
                {
                    item.AddFilter(parameterFilterElement.Id);
                    if (doc.ActiveView.ViewTemplateId == item.ViewTemplateId)
                    {
                        item.SetFilterVisibility(parameterFilterElement.Id, true);
                    }
                    else
                    {
                        item.SetFilterVisibility(parameterFilterElement.Id, false);
                    }
                }
            }
        }
        public bool CheckFilterinview(Document doc,View view)
        {
            bool flag = false;
            var col =(from x in view.GetFilters() select doc.GetElement(x).Name).ToList();
            foreach (var item in col)
            {
                if(item.Equals("Grid 3D"))
                {
                    flag = true;
                }
            }
            return flag;
        }
        public bool CheckFilter(Document doc, out ParameterFilterElement parameterFilterElement)
        {
            bool flag = false;
            parameterFilterElement = null;
            var col = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().ToList();
            foreach (var item in col)
            {
                if (item.Name == "Grid 3D")
                {
                    parameterFilterElement = item;
                    return flag = true;
                }
                if (flag)
                {
                    break;
                }
            }
            return flag;
        }
    }
    public class SettingCreateGrid3d
    {
        private static SettingCreateGrid3d _intance;
        private SettingCreateGrid3d()
        {

        }
        public static SettingCreateGrid3d Instance => _intance ?? (_intance = new SettingCreateGrid3d());
        public string Idsymbol { get; set; }
        public string Projectname { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.51.SettingCreateGrid3d";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingCreateGrid3d.json";
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

        public SettingCreateGrid3d GetSetting()
        {
            SettingCreateGrid3d setting = SettingExtension.GetSetting<SettingCreateGrid3d>(GetFullFileName());
            if (setting == null) setting = new SettingCreateGrid3d();
            return setting;
        }
    }
}
