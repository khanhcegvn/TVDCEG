#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
#endregion

namespace TVDCEG
{
    public class HideElementbyworkplaneandlevel
    {
        private static HideElementbyworkplaneandlevel _instance;
        private HideElementbyworkplaneandlevel()
        {

        }
        public static HideElementbyworkplaneandlevel Instance => _instance ?? (_instance = new HideElementbyworkplaneandlevel());
        private void GetallStructuralFramming(Document doc, View view, ref List<FamilyInstance> list)
        {
            var Conn = new FilteredElementCollector(doc, view.Id).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in Conn)
            {
                list.Add(item);
            }
        }
        public ICollection<ElementId> GetAllNestedFamily(Document doc, List<ElementId> ids)
        {
            ICollection<ElementId> collection = new Collection<ElementId>();
            foreach (ElementId elementId in ids)
            {
                collection.Add(elementId);
                FamilyInstance familyInstance = doc.GetElement(elementId) as FamilyInstance;
                bool flag = familyInstance == null;
                if (!flag)
                {
                    foreach (ElementId item in familyInstance.GetSubComponentIds())
                    {
                        collection.Add(item);
                    }
                }
            }
            return collection;
        }
        private void Getallconnection(Document doc, View view, ref List<FamilyInstance> list)
        {
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
                        list.Add(i);
                    }
                }
            }
        }
        public List<FamilyInstance> AllElementbylevel(Document doc, View view)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            Getallconnection(doc, view, ref list);
            GetallStructuralFramming(doc, view, ref list);
            return list;
        }
        public Dictionary<string, List<FamilyInstance>> Filterelementbylevel(Document doc, View view)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            List<FamilyInstance> list = AllElementbylevel(doc, view);
            foreach (var item in list)
            {
                Parameter pa = item.get_Parameter(BuiltInParameter.SKETCH_PLANE_PARAM);
                if (pa != null)
                {
                    string value = pa.AsString();
                    if (dic.ContainsKey(value))
                    {
                        dic[value].Add(item);
                    }
                    else
                    {
                        dic.Add(value, new List<FamilyInstance> { item });
                    }
                }
            }
            return dic;
        }
        public void Hidelementinview(Document doc, View view, ICollection<ElementId> elementIds)
        {
            List<ElementId> ids = new List<ElementId>();
            foreach (var item in elementIds)
            {
                if (doc.GetElement(item).IsHidden(view)) continue;
                else
                {
                    ids.Add(item);
                }
            }
            using (Transaction tx = new Transaction(doc, "Hide element by level/ workplane"))
            {
                tx.Start();
                view.HideElements(ids);
                tx.Commit();
            }
        }
    }
}
