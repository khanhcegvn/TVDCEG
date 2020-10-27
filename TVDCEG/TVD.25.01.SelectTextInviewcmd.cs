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
    public class SelectTextInviewcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public List<View> listview = new List<View>();
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
            using (var form = new FrmSelectTextinView(this,doc))
            {
                if (form.ShowDialog() != true)
                {
                    if(form.check==true)
                    {
                        SelectTextInview.Instance.CopyText(doc, form.ids, doc.ActiveView, form.viewstagget);
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}
