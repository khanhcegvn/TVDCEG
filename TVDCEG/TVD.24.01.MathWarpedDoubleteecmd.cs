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
    public class MathWarpedDoubleteecmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public Dictionary<string, List<DoubleTee>> dic = new Dictionary<string, List<DoubleTee>>();
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
            dic = MathWarpedDoubletee.Instance.GetDoubleTee(doc);
            var form = new FrmMathDoubleTee(this);
            if (form.ShowDialog() != true)
            {
                if (form.check == 1)
                {
                    MathWarpedDoubletee.Instance.MathValue(doc, form.list1, form.dic1);
                }
                if (form.check == 2)
                {
                    MathWarpedDoubletee.Instance.Excute(doc, sel);
                }
            }
            return Result.Succeeded;
        }
    }
}
