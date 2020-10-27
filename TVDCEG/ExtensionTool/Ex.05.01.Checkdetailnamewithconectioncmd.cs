using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace TVDCEG.ExtensionTool
{
    [Transaction(TransactionMode.Manual)]
    public class Checkdetailnamewithconectioncmd : IExternalCommand
    {
        public List<string> listsource = new List<string>();
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;

            var Conns = Checkdetailnamewithconection.Instance.Getallconnection(doc);
            var detailsname = Checkdetailnamewithconection.Instance.GetDetailView(doc);
            var list = Conns.Keys.ToList();
            listsource = Checkdetailnamewithconection.Instance.Report(detailsname, list);

            var form = new FrmCheckdetailnamewithconection(this);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
