using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using TVDCEG.CEG_INFOR;
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;
using Line = Autodesk.Revit.DB.Line;

namespace TVDCEG
{
    public class Checkpartdraw : ConstructorSingleton<Checkpartdraw>
    {
        public Dictionary<string, Partinfo> GetPartDictionary(Document doc)
        {
            var legends = Getmodelelement.Getlegends(doc);
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
                                bool flag = (from x in legends where x.Name.Equals(text) select x).ToList().Count != 0;
                                if(flag)
                                {
                                    dictionary[text].Draw = "true";
                                }
                                else
                                {
                                    dictionary[text].Draw = "false";
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
                                bool flag = (from x in legends where x.Name.Equals(text) select x).ToList().Count != 0;
                                if (flag)
                                {
                                    dictionary[text].Draw = "true";
                                }
                                else
                                {
                                    dictionary[text].Draw = "false";
                                }
                            }
                        }
                    }
                }
            }
            return dictionary;
        }
        public void ExportExcel(Dictionary<string, Partinfo> partlist, string filepath)
        {
            _Application application = (Microsoft.Office.Interop.Excel.Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
            _Workbook workbook = application.Workbooks.Add(Type.Missing);
            int count = workbook.Sheets.Count;
            _Worksheet worksheet = (_Worksheet)workbook.Worksheets.Add((dynamic)workbook.Sheets[count], Type.Missing, Type.Missing, Type.Missing);
            worksheet.Name = "Part list";
            dynamic val = worksheet.Range[(dynamic)worksheet.Cells[1, 1], (dynamic)worksheet.Cells[1, 4]];
            //val.Font.Bold = true;
            //val.Merge();
            //val = worksheet.Range[(dynamic)worksheet.Cells[2, 1], (dynamic)worksheet.Cells[2, 5]];
            //val.Font.Bold = true;
            //val.Merge();
            List<string> list = new List<string>(partlist.Keys);
            list.Sort();
            worksheet.Cells[1, 1] = "PART #";
            worksheet.Cells[1, 2] = "DESCRIPTION";
            worksheet.Cells[1, 3] = "CONNECTION";
            worksheet.Cells[1, 4] = "PRODUCT";
            worksheet.Columns.ColumnWidth = 50;
            worksheet.Columns.ColumnWidth = 50;
            val = worksheet.Range[(dynamic)worksheet.Cells[1, 1], (dynamic)worksheet.Cells[1, 4]];
            val.Font.Bold = true;
            int num = 2;
            foreach (string item in list)
            {
                worksheet.Cells[num, 1] = item;
                worksheet.Cells[num, 2] = partlist[item].Description;
                worksheet.Cells[num, 3] = partlist[item].Connection;
                worksheet.Cells[num, 4] = partlist[item].ProductName;
                worksheet.Cells[num, 3].Style.WrapText = true;
                worksheet.Cells[num, 4].Style.WrapText = true;
                if (partlist[item].Draw.Contains("true"))
                {
                    worksheet.Cells[num, 1].Interior.color = System.Drawing.Color.CornflowerBlue;
                    worksheet.Cells[num, 2].Interior.color = System.Drawing.Color.CornflowerBlue;
                    worksheet.Cells[num, 3].Interior.color = System.Drawing.Color.CornflowerBlue;
                    worksheet.Cells[num, 4].Interior.color = System.Drawing.Color.CornflowerBlue;
                }
                num++;
            }
            application.Visible = true;
            application.WindowState = XlWindowState.xlMaximized;
        }
    }
}
