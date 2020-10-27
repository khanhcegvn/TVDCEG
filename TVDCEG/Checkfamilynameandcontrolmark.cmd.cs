#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Checkfamilynameandcontrolmarkcmd : IExternalCommand
    {
        public Document doc;
        public Dictionary<string, Partinfo> listpart = new Dictionary<string, Partinfo>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            listpart = Checkpartinfo.Instance.Conflictlist(doc);
            var form = new FrmCheckNameAndControlMark(this);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
    public class Partinfo
    {
        public int Id
        {
            get;
            set;
        }

        public string Symbol
        {
            get;
            set;
        }

        public string Description
        {
            get;
            set;
        }
        public string Draw
        {
            get;
            set;
        }
        public string Connection
        {
            get;
            set;
        }
        public string PartOwner
        {
            get;
            set;
        }
        public string ProductName
        {
            get;
            set;
        }
        public Partinfo()
        {

        }
        public Partinfo(FamilyInstance instance)
        {
            Id = instance.Id.IntegerValue;
            Symbol = instance.Symbol.Name;
        }
    }
    public class Checkpartinfo
    {
        private static Checkpartinfo _instance;
        public static Checkpartinfo Instance => _instance ?? (_instance = new Checkpartinfo());
        public Dictionary<string, Partinfo> GetPartDictionary(Document doc)
        {
            Dictionary<string, Partinfo> dictionary = new Dictionary<string, Partinfo>();
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            IList<Element> list = filteredElementCollector.OfClass(typeof(FamilyInstance)).ToElements();
            foreach (Element item in list)
            {
                FamilyInstance familyInstance = item as FamilyInstance;
                FamilyInstance familyInstance2 = familyInstance?.SuperComponent as FamilyInstance;
                if (familyInstance2 != null && familyInstance2.SuperComponent == null)
                {
                    Autodesk.Revit.DB.Parameter parameter = familyInstance.Symbol.LookupParameter("CONTROL_MARK");
                    if (parameter != null && parameter.StorageType == StorageType.String)
                    {
                        string text = parameter.AsString();
                        if (!string.IsNullOrEmpty(text))
                        {
                            string name = familyInstance2.Symbol.Name;
                            string text2 = familyInstance.LookupParameter("BOM_PRODUCT_HOST")?.AsString() ?? "";
                            if (dictionary.ContainsKey(text))
                            {
                                if (string.IsNullOrEmpty(dictionary[text].ProductName))
                                {
                                    dictionary[text].ProductName = text2;
                                }
                                else if (!dictionary[text].ProductName.Contains(text2))
                                {
                                    dictionary[text].ProductName = dictionary[text].ProductName + "; " + text2;
                                }
                                if (dictionary[text].Connection == null)
                                {
                                    dictionary[text].Connection = name;
                                }
                                else if (!dictionary[text].Connection.Contains(name))
                                {
                                    dictionary[text].Connection = dictionary[text].Connection + "; " + name;
                                }
                            }
                            else
                            {
                                dictionary[text] = new Partinfo
                                {
                                    ProductName = text2
                                };
                                string description = familyInstance.Symbol.LookupParameter("IDENTITY_DESCRIPTION")?.AsString();
                                dictionary[text].Description = description;
                                dictionary[text].Connection = name;
                            }
                        }
                    }
                }
            }
            return dictionary;
        }
        public Dictionary<string, Partinfo> Conflictlist(Document doc)
        {
            Dictionary<string, Partinfo> dic = new Dictionary<string, Partinfo>();
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            IList<Element> list = filteredElementCollector.OfClass(typeof(FamilyInstance)).ToElements();
            foreach (var item in list)
            {
                FamilyInstance familyInstance = item as FamilyInstance;
                FamilyInstance familyInstance2 = familyInstance?.SuperComponent as FamilyInstance;
                if (familyInstance2 != null && familyInstance2.SuperComponent == null)
                {
                    Autodesk.Revit.DB.Parameter parameter = familyInstance.Symbol.LookupParameter("CONTROL_MARK");
                    if (parameter != null && parameter.StorageType == StorageType.String)
                    {
                        string text = parameter.AsString();
                        if (!string.IsNullOrEmpty(text))
                        {
                            if (text != familyInstance.Name)
                            {
                                string name = familyInstance2.Symbol.Name;
                                string text2 = familyInstance.LookupParameter("BOM_PRODUCT_HOST")?.AsString() ?? "";
                                if (dic.ContainsKey(text))
                                {
                                    if (string.IsNullOrEmpty(dic[text].ProductName))
                                    {
                                        dic[text].ProductName = text2;
                                    }
                                    else if (!dic[text].ProductName.Contains(text2))
                                    {
                                        dic[text].ProductName = dic[text].ProductName + "; " + text2;
                                    }
                                    if (dic[text].Connection == null)
                                    {
                                        dic[text].Connection = name;
                                    }
                                    else if (!dic[text].Connection.Contains(name))
                                    {
                                        dic[text].Connection = dic[text].Connection + "; " + name;
                                    }
                                    if (dic[text].PartOwner == null)
                                    {
                                        dic[text].PartOwner = familyInstance.Name;
                                    }
                                    else if (!dic[text].PartOwner.Contains(familyInstance.Name))
                                    {
                                        dic[text].PartOwner = dic[text].PartOwner + "; " + familyInstance.Name;
                                    }
                                }
                                else
                                {
                                    dic[text] = new Partinfo
                                    {
                                        ProductName = text2
                                    };
                                    string description = familyInstance.Symbol.LookupParameter("IDENTITY_DESCRIPTION")?.AsString();
                                    dic[text].Description = description;
                                    dic[text].Connection = name;
                                    dic[text].PartOwner = familyInstance.Name;
                                }
                            }
                        }
                    }
                }
            }
            return dic;
        }
        public void ExportExcel(Dictionary<string, Partinfo> partlist, string filepath)
        {
            //ExcelPackage excel = new ExcelPackage();
            //var workSheet = excel.Workbook.Worksheets.Add("Sheet1");
            //workSheet.TabColor = System.Drawing.Color.Black;
            //workSheet.DefaultRowHeight = 12;
            ////Header of table  
            ////  
            //workSheet.Row(1).Height = 20;
            //workSheet.Row(1).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            //workSheet.Row(1).Style.Font.Bold = true;
            //workSheet.Cells[1, 1].Value = "PART NAME";
            //workSheet.Cells[1, 2].Value = "DESCRIPTION";
            //workSheet.Cells[1, 3].Value = "CONNECTION";
            //workSheet.Cells[1, 4].Value = "PRODUCT";
            //int recordIndex = 5;
            //List<string> list = new List<string>(partlist.Keys);
            //list.Sort();
            //foreach (var item in list)
            //{
            //    workSheet.Cells[recordIndex, 1].Value = item;
            //    workSheet.Cells[recordIndex, 2].Value = partlist[item].Description;
            //    workSheet.Cells[recordIndex, 3].Value = partlist[item].Connection;
            //    workSheet.Cells[recordIndex, 4].Value = partlist[item].PartOwner;
            //    workSheet.Cells[recordIndex, 5].Value = partlist[item].ProductName;
            //    recordIndex++;
            //}
            //workSheet.Column(1).AutoFit();
            //workSheet.Column(2).AutoFit();
            //workSheet.Column(3).AutoFit();
            //workSheet.Column(4).AutoFit();
            //Stream stream = excel.Stream;
            //excel.Save();
            //this.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //this.Response.AddHeader(
            //          "content-disposition",
            //          string.Format("attachment;  filename={0}", "ExcellData.xlsx"));
            //this.Response.BinaryWrite(package.GetAsByteArray());
            _Application application = (Microsoft.Office.Interop.Excel.Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
            _Workbook workbook = application.Workbooks.Add(Type.Missing);
            int count = workbook.Sheets.Count;
            _Worksheet worksheet = (_Worksheet)workbook.Worksheets.Add((dynamic)workbook.Sheets[count], Type.Missing, Type.Missing, Type.Missing);
            worksheet.Name = "CONNECTION LIST";
            dynamic val = worksheet.Range[(dynamic)worksheet.Cells[1, 1], (dynamic)worksheet.Cells[1, 5]];
            val.Font.Bold = true;
            val.Merge();
            val = worksheet.Range[(dynamic)worksheet.Cells[2, 1], (dynamic)worksheet.Cells[2, 5]];
            val.Font.Bold = true;
            val.Merge();
            List<string> list = new List<string>(partlist.Keys);
            list.Sort();
            worksheet.Cells[3, 1] = "PART #";
            worksheet.Cells[3, 2] = "DESCRIPTION";
            worksheet.Cells[3, 3] = "CONNECTION";
            worksheet.Cells[3, 4] = "PART OWNER";
            worksheet.Cells[3, 5] = "PRODUCT";
            val = worksheet.Range[(dynamic)worksheet.Cells[3, 1], (dynamic)worksheet.Cells[3, 4]];
            val.Font.Bold = true;
            int num = 5;
            foreach (string item in list)
            {
                worksheet.Cells[num, 1] = item;
                worksheet.Cells[num, 2] = partlist[item].Description;
                worksheet.Cells[num, 3] = partlist[item].Connection;
                worksheet.Cells[num, 4] = partlist[item].PartOwner;
                worksheet.Cells[num, 5] = partlist[item].ProductName;
                num++;
            }
            application.Visible = true;
            application.WindowState = XlWindowState.xlMaximized;
        }
    }
}
