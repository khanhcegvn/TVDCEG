#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class ExportRebarSchedulecmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public Dictionary<string, List<CEGRebarInfo>> DicRebar = new Dictionary<string, List<CEGRebarInfo>>();
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
            DicRebar = ExportRebarSchedule.Instance.GetAllRebar(doc);
            var form = new FrmExportRebar(this);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
