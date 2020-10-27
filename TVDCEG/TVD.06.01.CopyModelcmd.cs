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
    public class CopyModelcmd : IExternalCommand
    {
        public List<Document> listdoc = new List<Document>();
        public ICollection<ElementId> ids = new List<ElementId>();
        public Document doc;
        public Application app;
        public UIDocument uidoc;
        public List<Grid> listgrid = new List<Grid>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            listdoc = CopyModel.Instance.GetDocuments(app, uidoc);
            ids = sel.GetElementIds();
            var form = new FrmCopyModel(this, doc);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
