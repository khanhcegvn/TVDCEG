using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.Ultis;

namespace TVDCEG.ExtensionTool
{
    public class Checkdetailnamewithconection : ConstructorSingleton<Checkdetailnamewithconection>
    {
        public Dictionary<string, List<FamilyInstance>> Getallconnection(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Autodesk.Revit.DB.Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var vla = pa.AsString();
                    if (vla.Contains("CONNECTION"))
                    {
                        if (dic.ContainsKey(i.Name))
                        {
                            dic[i.Name].Add(i);
                        }
                        else
                        {
                            dic.Add(i.Name, new List<FamilyInstance> { i });
                        }
                    }
                }
            }
            return dic;
        }
        public List<string> GetDetailView(Document doc)
        {
            var col = (from x in new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Views).OfClass(typeof(View)).Cast<View>() select x.Name).ToList();
            List<string> list = new List<string>();
            foreach (var item in col)
            {
                if (item.Contains("DET."))
                {
                    list.Add(item);
                }
            }
            return list;
        }
        public List<string> Report(List<string> Views, List<string> lists)
        {
            List<string> reports = new List<string>();
            foreach (var item in Views)
            {
                if (item.Contains("DET."))
                {
                    string sper = item.Replace("DET.", "").Replace(" ", "");
                    foreach (var item2 in lists)
                    {
                        if (item2.Contains("CONN."))
                        {
                            string sper2 = item2.Replace("CONN.", "").Replace(" ", "");
                            if (sper == sper2)
                            {
                                reports.Add(item2);
                            }
                        }
                    }
                }
            }
            var listexpan = lists.Except(reports).ToList();
            return listexpan;
        }
        public void FormatDetailNameOnSheet(Document doc)
        {
            ViewSheet viewsheet = doc.ActiveView as ViewSheet;
            if (viewsheet != null)
            {
                var list = viewsheet.GetAllPlacedViews();
                Transaction tran = new Transaction(doc, "Format view name");
                tran.Start();
                foreach (var item in list)
                {

                }
                tran.Commit();
            }
        }
    }
}
