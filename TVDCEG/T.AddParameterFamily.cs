#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class AddParameterFamily : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            if(doc.IsFamilyDocument)
            {
                FrmModify form = new FrmModify(this, doc, app);
                form.ShowDialog();
            }
            else
            {
                TaskDialog.Show("error", "Please go to family document");
            }
            return Result.Succeeded;
        }
    }
}
