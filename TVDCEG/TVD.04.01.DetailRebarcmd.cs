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
    public class DetailRebarcmd : IExternalCommand
    {
        public Document doc;
        public List<Grid> listgrid = new List<Grid>();
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
            DetailRebar.Instance.Createdrangting(doc, sel, uidoc);
            return Result.Succeeded;
        }
    }
}
