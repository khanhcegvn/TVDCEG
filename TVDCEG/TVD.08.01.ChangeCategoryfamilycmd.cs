#region Namespaces
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using Application = Autodesk.Revit.ApplicationServices.Application;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class ChangeCategoryfamilycmd : IExternalCommand
    {
        public UIDocument uidoc;
        public Selection sel;
        public List<FamilySymbol> listfamilysymbol = new List<FamilySymbol>();
        public Dictionary<string, Category> dic = new Dictionary<string, Category>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            sel = uidoc.Selection;
            listfamilysymbol = ChangeCategoryfamily.Instance.Getallfamily(doc);
            dic = ChangeCategoryfamily.Instance.TypeCategory(doc);
            var form = new FrmChangeCategoryFamily(this);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
