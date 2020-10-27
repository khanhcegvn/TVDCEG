using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.LBR;

namespace TVDCEG
{
    internal class RebarSymbolCreatorController : RvtController
    {
        public RebarSymbolCreatorController(UIApplication uiapp) : base(uiapp)
        {
        }
        public override void Execute()
        {
            bool flag = !(base.ActiveView is ViewSheet);
            if (flag)
            {
                TaskDialog.Show("CEG-Warming", "Please go to a ViewSheet to run the tool.");
            }
            else
            {
                var dic2 = Sortdic(this.Dic, viewSchedule);
                //viewSchedule.Export(@"C:\Users\toanvd\Desktop\EXPORT", viewSchedule.Name, new ViewScheduleExportOptions());
                foreach (KeyValuePair<string, FamilyInstance> pair in dic2)
                {
                    this.CreateRebarSymbol(pair);
                }
            }
        }
        public override void ReadData()
        {
            Reference reference = base.RSelection.PickObject(ObjectType.Element, new SelectionFilter(typeof(ScheduleSheetInstance)));
            ScheduleSheetInstance scheduleSheetInstance = base.Doc.GetElement(reference) as ScheduleSheetInstance;
            bool flag = scheduleSheetInstance == null;
            if (!flag)
            {
                viewSchedule = base.Doc.GetElement(scheduleSheetInstance.ScheduleId) as ViewSchedule;
                bool flag2 = viewSchedule == null;
                if (!flag2)
                {
                    List<FamilyInstance> list = new FilteredElementCollector(base.Doc, scheduleSheetInstance.ScheduleId).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList<FamilyInstance>();
                    foreach (FamilyInstance familyInstance in list)
                    {
                        FamilySymbol symbol = familyInstance.Symbol;
                        bool flag3 = symbol == null;
                        if (!flag3)
                        {
                            Parameter parameter = symbol.LookupParameter("SORTING_ORDER");
                            bool flag4 = parameter == null;
                            if (!flag4)
                            {
                                string a = parameter.AsValueString();
                                bool flag5 = a == "405";
                                if (flag5)
                                {
                                    Parameter parameter2 = familyInstance.LookupParameter("CONTROL_MARK");
                                    bool flag6 = parameter2 == null;
                                    if (!flag6)
                                    {
                                        string text = parameter2.AsString();
                                        bool flag7 = text == null;
                                        if (!flag7)
                                        {
                                            bool flag8 = this.Dic.ContainsKey(text);
                                            if (!flag8)
                                            {
                                                this.Dic.Add(text, familyInstance);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void CreateRebarSymbol(KeyValuePair<string, FamilyInstance> pair)
        {
            FamilySymbol symbol = pair.Value.Symbol;
            bool flag = symbol == null;
            if (!flag)
            {
                Parameter parameter = symbol.LookupParameter("Type Comments");
                bool flag2 = parameter == null;
                if (!flag2)
                {
                    string symbolName = parameter.AsString();
                    FamilySymbol rebarSymbol2D = this.GetRebarSymbol2D(symbolName, "999");
                    bool flag3 = rebarSymbol2D == null;
                    if (!flag3)
                    {
                        XYZ origin = base.RSelection.PickPoint();
                        FamilyInstance rebar2dSymbol = base.Doc.Create.NewFamilyInstance(origin, rebarSymbol2D, base.ActiveView);
                        this.CopyParameterValue(rebar2dSymbol, pair.Value);
                    }
                }
            }
        }
        private Dictionary<string, FamilyInstance> Sortdic(Dictionary<string, FamilyInstance> dic, ViewSchedule viewSchedule)
        {
            Dictionary<string, FamilyInstance> dic2 = new Dictionary<string, FamilyInstance>();
            var keys = dic.Keys.ToList();
            List<string> listtexts = new List<string>();
            List<string> Newtexts = new List<string>();
            TableData tableData = viewSchedule.GetTableData();
            TableSectionData sectionData = tableData.GetSectionData(SectionType.Body);
            int numberOfRows = sectionData.NumberOfRows;
            int numberOfColumns = sectionData.NumberOfColumns;
            //for (int i = 0; i < numberOfRows; i++)
            //{
            //	for (int j = 0; j < numberOfColumns; j++)
            //	{
            //		string cellText = viewSchedule.GetCellText(SectionType.Body, i, j);
            //		listtexts.Add(cellText);
            //	}
            //}
            for (int i = 0; i < numberOfRows; i++)
            {
                string cellText = viewSchedule.GetCellText(SectionType.Body, i, 0);
                listtexts.Add(cellText);
            }
            foreach (var item in listtexts)
            {
                foreach (var item2 in keys)
                {
                    if (item.Equals(item2))
                    {
                        Newtexts.Add(item);
                    }
                }
            }
            foreach (var item in Newtexts)
            {
                dic2.Add(item, dic[item]);
            }
            return dic2;
        }
        private FamilySymbol GetRebarSymbol2D(string symbolName, string orderNumber)
        {
            FamilySymbol result = null;
            IEnumerable<FamilySymbol> enumerable = (from x in new FilteredElementCollector(base.Doc).OfCategory(BuiltInCategory.OST_GenericAnnotation).OfClass(typeof(FamilySymbol))
                                                    where x.Name == symbolName
                                                    select x).Cast<FamilySymbol>();
            foreach (FamilySymbol familySymbol in enumerable)
            {
                Parameter parameter = familySymbol.LookupParameter("SORTING_ORDER");
                bool flag = parameter == null;
                if (!flag)
                {
                    string a = parameter.AsString();
                    bool flag2 = a == orderNumber;
                    if (flag2)
                    {
                        result = familySymbol;
                    }
                }
            }
            return result;
        }
        private void CopyParameterValue(FamilyInstance rebar2dSymbol, FamilyInstance rebar)
        {
            IList<Parameter> orderedParameters = rebar2dSymbol.GetOrderedParameters();
            foreach (Parameter parameter in orderedParameters)
            {
                bool isReadOnly = parameter.IsReadOnly;
                if (!isReadOnly)
                {
                    string name = parameter.Definition.Name;
                    bool flag = name != "SORTING_ORDER";
                    if (flag)
                    {
                        Parameter parameter2 = rebar.LookupParameter(name);
                        bool flag2 = parameter2 == null;
                        if (!flag2)
                        {
                            bool flag3 = parameter2.StorageType == StorageType.Double && parameter.StorageType == StorageType.Double;
                            if (flag3)
                            {
                                double value = parameter2.AsDouble();
                                parameter.Set(value);
                            }
                            else
                            {
                                bool flag4 = parameter2.StorageType == StorageType.Integer && parameter.StorageType == StorageType.Integer;
                                if (flag4)
                                {
                                    int value2 = parameter2.AsInteger();
                                    parameter.Set(value2);
                                }
                                else
                                {
                                    bool flag5 = parameter2.StorageType == StorageType.String && parameter.StorageType == StorageType.String;
                                    if (flag5)
                                    {
                                        string value3 = parameter2.AsString();
                                        parameter.Set(value3);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public override void LoadSettings()
        {
        }

        public override void SaveSettings()
        {
        }

        private Dictionary<string, FamilyInstance> Dic = new Dictionary<string, FamilyInstance>();
        private ViewSchedule viewSchedule;
    }
}
