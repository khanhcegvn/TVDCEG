#region Namespaces
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    public class FindElementonWhereSheet
    {
        private static FindElementonWhereSheet _instance;
        private FindElementonWhereSheet()
        {

        }
        public static FindElementonWhereSheet Instance => _instance ?? (_instance = new FindElementonWhereSheet());

        public List<ElementFindclass> FindElementonSheet(Document doc)
        {
            List<ElementFindclass> list = new List<ElementFindclass>();
            List<ElementFindclass> list1 = new List<ElementFindclass>();
            var listsheet = GetAllSheet(doc);
            var coltag = new FilteredElementCollector(doc).OfClass(typeof(IndependentTag)).Cast<IndependentTag>().ToList();
            coltag.ForEach(x => list.Add(new ElementFindclass(doc, x, listsheet)));
            list1 = (from x in list where x.Control_mark != null /*&& x.Control_Number != null && x.Control_Number != "" */select x).ToList();
            Sortlist(list1);
            return list1;
        }
        List<ViewSheet> GetAllSheet(Document doc)
        {
            var ViewSheets = (from ViewSheet vs in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).OfClass(typeof(ViewSheet)) where !vs.IsSheetAssembly() select vs).ToList();
            return ViewSheets;
        }
        void Sortlist(List<ElementFindclass> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    if (list[i].Control_Number != null && list[j].Control_Number != null && list[i].Control_Number != "" && list[j].Control_Number != "")
                    {
                        if (Int32.Parse(list[i].Control_Number) < Int32.Parse(list[j].Control_Number))
                        {
                            var temp = list[i];
                            list[i] = list[j];
                            list[j] = temp;
                        }
                    }
                }
            }
        }
        public void GotoSheet(UIDocument uidoc, ElementFindclass elementFindclass)
        {
            uidoc.ActiveView = elementFindclass.Sheet;
            //uidoc.Selection.SetElementIds(new List<ElementId> { elementFindclass.ID });
            //uidoc.RefreshActiveView();
        }
        public void Export(ObservableCollection<ElementFindclass> list)
        {
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
            worksheet.Cells[3, 1] = "Control Mark";
            worksheet.Cells[3, 2] = "Control Number";
            worksheet.Cells[3, 3] = "Sheet Number";
            worksheet.Cells[3, 4] = "Sheet Name";
            worksheet.Cells[3, 5] = "View Name";
            val = worksheet.Range[(dynamic)worksheet.Cells[3, 1], (dynamic)worksheet.Cells[3, 5]];
            val.Font.Bold = true;
            int num = 5;
            foreach (var item in list)
            {
                worksheet.Cells[num, 1] = item.Control_mark;
                worksheet.Cells[num, 2] = item.Control_Number;
                worksheet.Cells[num, 3] = item.Sheetnumber;
                worksheet.Cells[num, 4] = item.SheetName;
                worksheet.Cells[num, 5] = item.ViewName;
                num++;
            }
            application.Visible = true;
            application.WindowState = XlWindowState.xlMaximized;
        }
    }
    public class ElementFindclass
    {
        public string Control_mark { get; set; }
        public string Control_Number { get; set; }
        public ElementId ID;
        public ViewSheet Sheet;
        public string SheetName { get; set; }
        public string Sheetnumber { get; set; }
        public string ViewName { get; set; }
        public ElementFindclass(Document doc, IndependentTag independentTag, List<ViewSheet> listsheet)
        {
            bool flag = independentTag == null || independentTag.OwnerViewId == ElementId.InvalidElementId;
            if (!flag)
            {
                var viewid = independentTag.OwnerViewId;
                foreach (var i in listsheet)
                {
                    var allviewonsheet = i.GetAllPlacedViews();
                    foreach (var item in allviewonsheet)
                    {
                        if (item == viewid)
                        {
                            Sheet = i;
                            Element element = independentTag.GetTaggedLocalElement();
                            Autodesk.Revit.DB.Parameter parameter = (element != null) ? element.LookupParameter("CONTROL_NUMBER") : null;
                            Autodesk.Revit.DB.Parameter parameter2 = (element != null) ? element.LookupParameter("CONTROL_MARK") : null;
                            bool flag4 = parameter == null || parameter.StorageType != StorageType.String;
                            if (!flag4)
                            {
                                bool flag5 = parameter2 == null || parameter2.StorageType != StorageType.String;
                                if (!flag5)
                                {
                                    Control_mark = parameter2.AsString();
                                    Control_Number = parameter.AsString();
                                    SheetName = Sheet.Name;
                                    Sheetnumber = Sheet.SheetNumber;
                                    ViewName = doc.GetElement(viewid).Name;
                                    ID = element.Id;
                                }
                            }
                            else
                            {
                                bool flag5 = parameter2 == null || parameter2.StorageType != StorageType.String;
                                if (!flag5)
                                {
                                    Control_mark = parameter2.AsString();
                                    Control_Number = "";
                                    SheetName = Sheet.Name;
                                    Sheetnumber = Sheet.SheetNumber;
                                    ViewName = doc.GetElement(viewid).Name;
                                }
                            }
                        }
                    }
                }
            }
        }
        List<ViewSheet> GetAllSheet(Document doc)
        {
            var ViewSheets = (from ViewSheet vs in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).OfClass(typeof(ViewSheet)) where !vs.IsSheetAssembly() select vs).ToList();
            return ViewSheets;
        }
    }
}
