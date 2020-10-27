using Autodesk.Revit.DB;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class ViewSheethelper
    {
        public static bool IsonSheet(this ViewSheet sheet, View view)
        {
            var allview = sheet.GetAllPlacedViews();
            var gh = allview.Where(x => x == view.Id).Cast<ElementId>().ToList();
            if (gh.Count != 0) return true;
            else return false;
        }
        public static bool IsSheetAssembly(this ViewSheet vs)
        {
            var t = vs.AssociatedAssemblyInstanceId;
            if (t.IntegerValue != -1) return true;
            else return false;
        }
    }
}
