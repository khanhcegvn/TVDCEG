using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    public class Checkpartdrawcmd : IExternalCommand
    {
        public UIDocument uidoc;
        public Document doc;
        public Selection sel;
        public Dictionary<string, Partinfo> dic = new Dictionary<string, Partinfo>();
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            uidoc = application.ActiveUIDocument;
            Application application2 = application.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            dic = Checkpartdraw.Instance.GetPartDictionary(doc);
            var form = new FrmCheckpartdraw(doc,this);
            form.ShowDialog();
            return Result.Succeeded;
        }
    }
}
