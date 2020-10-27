using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG
{
    public class lbr_view
    {
        public List<View> GetViews(Document doc)
        {
            List<View> listview_after = new List<View>();
            List<View> listview =(from x in new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(View)).ToElements().Cast<View>() where !x.IsTemplate select x).ToList();
            foreach (View view in listview)
            {
                if (view.ViewType == ViewType.DraftingView ||
                    view.ViewType == ViewType.Legend||
                    view.ViewType==ViewType.Schedule)
                {
                    if(view.Name.Contains("Revision Schedule"))
                    {
                        continue;
                    }
                    if (view.IsTemplate) continue;
                    listview_after.Add(view);
                }
            }
            return listview_after;
        }
    }
}
