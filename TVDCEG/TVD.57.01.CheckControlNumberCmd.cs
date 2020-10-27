using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.CEG_INFOR;
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CheckControlNumberCmd : IExternalCommand
    {
        public List<ElementId> listproductid = new List<ElementId>();
        private ExternalEvent _exEvent;
        public List<CEG_Product> list = new List<CEG_Product>();
        public List<CEG_Product> listRenumber = new List<CEG_Product>();
        public int flag;
        public int number;
        public int increase;
        public UIDocument uidoc;
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            uidoc = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            this._exEvent = ExternalEvent.Create((IExternalEventHandler)new CheckControlNumberEvent(this, doc));
            var form = new FrmCheckControlNumber(this, doc);
            form.Show();
            form.ExEvent = this._exEvent;
            return Result.Succeeded;
        }
    }
}
