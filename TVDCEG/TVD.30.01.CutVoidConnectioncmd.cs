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
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CutVoidConnectioncmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
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
            using (var form = new FrmCutVoidConnection(doc))
            {
                if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                {
                    if (form.listcut.Count != 0 && form.listconn.Count != 0)
                    {
                        CutVoidConnection.Instance.Cutting(doc, form.listcut, form.listconn);
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}
