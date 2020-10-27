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
using System.Threading;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CreateGridcmd : IExternalCommand
    {
        public Document doc;
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
            List<Grid> GridX = new List<Grid>();
            List<Grid> GridY = new List<Grid>();
            using (var form = new FrmCreateGrid(this))
            {
                form.ShowDialog();
                if (form.check)
                {
                    var hx = form.DataAxisX;
                    var hy = form.DataAxisY;
                    XYZ point = sel.PickPoint();
                    CreateGrid.Instance.NewGridX(doc, point, form.namegridX, hx, hy, ref GridX);
                    CreateGrid.Instance.NewGridY(doc, point, form.namegridY, hy, hx, ref GridY);
                    CreateGrid.Instance.DimGrids(doc, doc.ActiveView, 320, 500);
                }
            }
            return Result.Succeeded;
        }
    }
}
