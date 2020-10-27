#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class FindElementonWhereSheetcmd : IExternalCommand
    {
        public Document doc;
        public Selection sel;
        public UIDocument uidoc;
        public List<ElementFindclass> elementFindclasses = new List<ElementFindclass>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            elementFindclasses = FindElementonWhereSheet.Instance.FindElementonSheet(doc);
            elementFindclasses.OrderByDescending(x => x.Control_Number).ToList();
            var form = new FrmFindElementonwhereSheet(this);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }

}
