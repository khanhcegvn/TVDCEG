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
    public class AddSuffixControlMarkErection: ConstructorSingleton<AddSuffixControlMarkErection>
    {
        public Dictionary<string, Dictionary<string, List<FamilyInstance>>> dic;
        public Dictionary<string, Dictionary<string, List<IndependentTag>>> dic2 = new Dictionary<string, Dictionary<string, List<IndependentTag>>>();
        //Get all sheet Erection
        public List<ViewSheet> GetAllSheetErection(Document doc)
        {
            var col = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Sheets).OfClass(typeof(ViewSheet)).Cast<ViewSheet>() where !x.IsAssemblyView select x).ToList();
            return col;
        }
        //Get all view place on sheet
        public List<View> GetAllViewPlaceOnSheet(Document doc, ViewSheet sheet)
        {
            ISet<ElementId> Ids = sheet.GetAllPlacedViews();
            List<View> col = (from x in Ids select doc.GetElement(x)).Cast<View>().ToList();
            var list = (from x in col where x.ViewType == ViewType.FloorPlan || x.ViewType == ViewType.Section select x).ToList();
            return list;
        }
        //Get all products in view
        public List<FamilyInstance> GetAllProductInview(Document doc, View view)
        {
            var col = new FilteredElementCollector(doc, view.Id).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            return col;
        }
        public void Running(Document doc)
        {

            List<ViewSheet> ListSheets = GetAllSheetErection(doc);
            using (Transaction tran = new Transaction(doc, "Add Suffix ControlNumber"))
            {
                tran.Start();
                ProgressBarform progressBarform = new ProgressBarform(ListSheets.Count, "Running...");
                progressBarform.Show();
                foreach (ViewSheet sheet in ListSheets)
                {
                    progressBarform.giatri();
                    if (progressBarform.iscontinue == false)
                    {
                        break;
                    }
                    string sheetNumber = sheet.SheetNumber;
                    List<View> Views = GetAllViewPlaceOnSheet(doc, sheet);
                    foreach (View view in Views)
                    {
                        List<FamilyInstance> Products = GetAllProductInview(doc, view);
                        foreach (FamilyInstance product in Products)
                        {
                            FamilyInstance _product = product.GetSuperInstances();
                            Parameter Control_number = _product.LookupParameter("CONTROL_NUMBER");
                            if (Control_number != null)
                            {
                                Control_number.Set(sheetNumber + Control_number.AsString());
                            }
                        }
                    }
                }
                progressBarform.Close();
                tran.Commit();
            }
        }
        //Đảo ngược chuỗi string
        public string RevertString(string Str)
        {
            string str = "";
            int l = Str.Length - 1;
            for (int i = l; i >= 0; i--)
            {
                str = str + Str[i];
            }
            return str;
        }
        public void Crash(Document doc)
        {
            List<ViewSheet> ListSheets = GetAllSheetErection(doc);
            using (Transaction tran = new Transaction(doc, "Add Suffix ControlNumber"))
            {
                tran.Start();
                ProgressBarform progressBarform = new ProgressBarform(ListSheets.Count, "Running...");
                progressBarform.Show();
                foreach (ViewSheet sheet in ListSheets)
                {
                    progressBarform.giatri();
                    if (progressBarform.iscontinue == false)
                    {
                        break;
                    }
                    string sheetNumber = sheet.SheetNumber.Split('-').Last().Replace(".","");
                    
                    List<View> Views = GetAllViewPlaceOnSheet(doc, sheet);
                    foreach (View view in Views)
                    {
                        Dictionary<string, List<IndependentTag>> dic4 = new Dictionary<string, List<IndependentTag>>();
                        List<IndependentTag> independentTags = FindsymboltaginView(doc, view);
                        foreach (var tag in independentTags)
                        {
                            FamilyInstance instancesource = doc.GetElement(tag.TaggedLocalElementId) as FamilyInstance;
                            if (instancesource.Category.Name != null)
                            {
                                if (instancesource.Category.Name.Contains("Structural Framing"))
                                {
                                    FamilyInstance _product = instancesource.GetSuperInstances();
                                    Parameter Control_number = _product.LookupParameter("CONTROL_NUMBER");
                                    string stringrevert = RevertString(Control_number.AsString());
                                    string value = RevertString(stringrevert.Substring(0, 3));
                                    if (Control_number != null)
                                    {
                                        Control_number.Set(sheetNumber + value);
                                    }
                                }
                            }
                        }
                        //foreach (IndependentTag tag in independentTags)
                        //{
                        //    if (dic4.ContainsKey(tag.Name))
                        //    {
                        //        dic4[tag.Name].Add(tag);
                        //    }
                        //    else
                        //    {
                        //        dic4.Add(tag.Name, new List<IndependentTag> { tag });
                        //    }
                        //}
                        //if(dic2.ContainsKey(view.Name))
                        //{
                        //    continue;
                        //}
                        //else
                        //{
                        //    dic2.Add(view.Name, dic4);
                        //}
                        //dic2.Add(view.Name, dic4);
                    }
                }
                progressBarform.Close();
                tran.Commit();
            }
        }
        public static Dictionary<string, List<FamilyInstance>> Findsymboltag(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).Where(x => x.Category.Name == "Multi-Category Tags" || x.Category.Name == "Structural Framing Tags").Cast<IndependentTag>().ToList();
            foreach (var tag in col)
            {
                FamilyInstance instancesource = doc.GetElement(tag.TaggedLocalElementId) as FamilyInstance;
                if (instancesource.Category.Name != null)
                {
                    if (instancesource.Category.Name.Contains("Structural Framing"))
                    {
                        if (dic.ContainsKey(tag.Name))
                        {
                            dic[tag.Name].Add(instancesource);
                        }
                        else
                        {
                            dic.Add(tag.Name, new List<FamilyInstance> { instancesource });
                        }
                    }
                }
            }
            return dic;
        }
        public List<IndependentTag> FindsymboltaginView(Document doc, View view)
        {
            var col = new FilteredElementCollector(doc, view.Id).WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).Where(x => x.Category.Name == "Multi-Category Tags" || x.Category.Name == "Structural Framing Tags").Cast<IndependentTag>().ToList();
            return col;
        }
    }

}
