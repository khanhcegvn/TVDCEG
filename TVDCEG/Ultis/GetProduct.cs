using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.Ultis
{
   public static class GetProduct
    {
        public static Dictionary<string, List<FamilyInstance>> GetAllProduct(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            List<FamilyInstance> Col1 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in Col1)
            {
                Parameter pa = item.LookupParameter("CONTROL_MARK");
                if(pa!=null)
                {
                    string control_mark = pa.AsString();
                    {
                        if (dic.ContainsKey(control_mark))
                        {
                            dic[control_mark].Add(item);
                        }
                        else
                        {
                            dic.Add(control_mark, new List<FamilyInstance> { item });
                        }
                    }
                }
            }
            return dic;
        }
        public static List<FamilyInstance> GetAllproduct(Document doc)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            List<FamilyInstance> Col1 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in Col1)
            {
                Parameter pa = item.LookupParameter("CONTROL_MARK");
                if (pa != null)
                {
                    list.Add(item);
                }
            }
            return list;
        }
    }
}
