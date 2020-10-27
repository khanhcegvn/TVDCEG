using Autodesk.Revit.DB;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class ViewUtils
    {
        public static FamilyInstance GetTitleBlock(this ViewSheet viewSheet, Document doc)
        {
            var fi = new FilteredElementCollector(doc, viewSheet.Id).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_TitleBlocks)
                .OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().First();
            return fi;
        }
        public static bool IsAssemblyview(this View view)
        {
            if (view.AssociatedAssemblyInstanceId.IntegerValue != -1) return true;
            else return false;
        }
        public static bool IsonView(this View view, Document doc, Element element)
        {
            var col = new FilteredElementCollector(doc, view.Id).ToList();
            var gh = col.Where(x => x.Id == element.Id).ToList();
            if (gh.Count != 0) return true;
            else return false;
        }
        public static bool IsViewSheet(this View view)
        {
            ViewSheet sheet = view as ViewSheet;
            if(sheet!=null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
