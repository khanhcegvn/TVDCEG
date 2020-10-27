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
    public class HideElementbyworkplaneandlevelcmd : IExternalCommand
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
            var form = new FrmHideElementbyworkplaneandlevel(doc);
            _event = ExternalEvent.Create((IExternalEventHandler)new HideElementbyworkplaneandlevelEvent(doc, HideElementbyworkplaneandlevel.Instance, form));
            form.Show();
            form.ExEvent = this._event;
            return Result.Succeeded;
        }
    }
}
