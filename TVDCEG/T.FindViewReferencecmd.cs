#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class FindViewReferencecmd : IExternalCommand
    {
        public Document doc;
        public List<Element> listelement = new List<Element>();
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

            return Result.Succeeded;
        }
        public List<Element> GetElements(Document doc)
        {
            List<Element> list = new List<Element>();
            FilteredElementCollector col = new FilteredElementCollector(doc, doc.ActiveView.Id);
            var tui = col.ToElements();
            foreach (var i in tui)
            {
                list.Add(i);
            }
            return list;
        }
    }
}
