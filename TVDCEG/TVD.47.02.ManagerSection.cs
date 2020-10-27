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
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    public class ManagerSection
    {
        private static ManagerSection _instance;
        private ManagerSection()
        {

        }
        public static ManagerSection Instance => _instance ?? (_instance = new ManagerSection());
        public Dictionary<string, List<View>> CollectSection(Document doc, string name,string Paramatername,ref List<string> listview)
        {
            var col = GetallAnnotionSymbol(doc, name);
            Dictionary<string,List<View>> dic = new Dictionary<string,List<View>>();
            foreach (var item in col)
            {
                View view = doc.GetElement(item.OwnerViewId) as View;
                Parameter parameter = item.LookupParameter(Paramatername);
                if(parameter!=null)
                {
                    string key = parameter.AsString();
                    if (!string.IsNullOrEmpty(key))
                    {
                        listview.Add(key);
                        if (dic.ContainsKey(key))
                        {
                            dic[key].Add(view);
                        }
                        else
                        {
                            dic.Add(key, new List<View> { view });
                        }
                    }
                }
            }
            RemoveduplicateString(listview);
            return dic;
        }
        public List<FamilyInstance> GetallAnnotionSymbol(Document doc, string name)
        {
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericAnnotation).OfClass(typeof(FamilyInstance)).ToList();
            var col2 = (from x in col where x.Name.Contains(name) select x).Cast<FamilyInstance>().ToList();
            return col2;
        }
        public Dictionary<string,List<Parameter>> GetFamilyAnnotionSymbol(Document doc)
        {
            Dictionary<string, List<Parameter>> dic = new Dictionary<string, List<Parameter>>();
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericAnnotation).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            RemoveFamilyinstanceinlist(col);
            foreach (var item in col)
            {
                List<Parameter> list = new List<Parameter>();
                foreach (Parameter pa in item.ParametersMap)
                {
                    list.Add(pa);
                }
                if (dic.ContainsKey(item.Name)) continue;
                dic.Add(item.Name, list);
            }
            return dic;
        }
        public void RemoveduplicateString(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i+1; j < list.Count; j++)
                {
                    string val1 = list[i];
                    string val2 = list[j];
                    if (val1.Equals(val2))
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
        public void RemoveFamilyinstanceinlist(List<FamilyInstance> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    FamilyInstance val1 = list[i];
                    FamilyInstance val2 = list[j];
                    if (val1.Name.Equals(val2.Name))
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
        }
      
    }
    public class ManagerSectionModel
    {
        public List<string> ListView = new List<string>();
        public Dictionary<string, List<ElementId>> dic = new Dictionary<string, List<ElementId>>();
        public List<ObjectSectionManager> ListObjectSections = new List<ObjectSectionManager>();
        public Dictionary<string, List<ObjectSheetManager>> dic3 = new Dictionary<string, List<ObjectSheetManager>>();
        public ManagerSectionModel(Document doc,SettingManagerSection Setting)
        {
            var col = ManagerSection.Instance.CollectSection(doc, Setting.FamilySymbol, Setting.Parameter, ref ListView);
            //ThreadStart job = new ThreadStart(GetDataSection);
            //Thread thread = new Thread(job);
            //thread.Start();
            GetAllSheet(doc);
            GetDataSection(doc);
            foreach (KeyValuePair<string, List<View>> item in col)
            {
                foreach (View view in item.Value)
                {
                    foreach (KeyValuePair<string, List<ElementId>> val in dic)
                    {
                        foreach (ElementId ele in val.Value)
                        {
                            bool flag = ele == view.Id;
                            if (flag)
                            {
                                if (dic3.ContainsKey(item.Key))
                                {
                                    ObjectSheetManager obj = new ObjectSheetManager(val.Key, ele);
                                    dic3[item.Key].Add(obj);
                                }
                                else
                                {
                                    ObjectSheetManager obj = new ObjectSheetManager(val.Key, ele);
                                    dic3.Add(item.Key, new List<ObjectSheetManager> { obj });
                                }
                            }
                        }
                    }
                }
            }
        }
        public void GetAllSheet(Document doc)
        {
            var col2 = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(ViewSheet)).Cast<ViewSheet>().ToList();
            col2.OrderBy(x => x.Name).ToList();
            foreach (var sheet in col2)
            {
                var list = sheet.GetAllPlacedViews().ToList();
                var nameview = sheet.SheetNumber + " - " + sheet.Name;
                if (dic.ContainsKey(nameview)) continue;
                dic.Add(nameview, list);
            }
        }

        public void GetDataSection(Document doc)
        {
            var col2 = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(View)).Cast<View>().ToList();
            col2.OrderBy(x => x.Name).ToList();
            foreach (var item in ListView)
            {
                foreach (var index in col2)
                {
                    if (item.Equals(index.Name))
                    {
                        ObjectSectionManager objectSection = new ObjectSectionManager(item, index.Id);
                        ListObjectSections.Add(objectSection);
                    }
                }
            }
            ListObjectSections.OrderByDescending(x => x.Name);
        }
        public void Removeduplicateobject(List<ObjectSectionManager> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = i - 1; j < i; j++)
                {
                    ObjectSectionManager val1 = list[i];
                    ObjectSectionManager val2 = list[j];
                    if (val1.Name.Equals(val2.Name))
                    {
                        list.RemoveAt(j);
                        i--;
                    }
                }
            }
        }
    }
}
