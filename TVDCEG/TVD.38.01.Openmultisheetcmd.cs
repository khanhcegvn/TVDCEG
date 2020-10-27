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
    public class Openmultisheetcmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public Dictionary<string, ViewSheet> dic_sheet = new Dictionary<string, ViewSheet>();
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
            dic_sheet = GetViewSheets(doc);
            using (var form = new FrmOpenmultisheet(this, doc))
            {
                if (form.ShowDialog() != true && form.check == true)
                {
                    OpenMultisheet(form.viewsheettarget);
                }
                return Result.Succeeded;
            }
        }
        public Dictionary<string, ViewSheet> GetViewSheets(Document doc)
        {
            Dictionary<string, ViewSheet> viewSheets = new Dictionary<string, ViewSheet>();
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(ViewSheet)).Cast<ViewSheet>().ToList();
            col.OrderBy(x => x.Name).ToList();
            foreach (var i in col)
            {
                ViewSheet sheet = i as ViewSheet;
                var nameview = sheet.SheetNumber + " - " + sheet.Name;
                if (viewSheets.ContainsKey(nameview)) continue;
                viewSheets.Add(nameview, sheet);
            }
            return viewSheets;
        }
        public void OpenMultisheet(List<ViewSheet> viewSheets)
        {
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(viewSheets.Count, "Opening...");
            progressbarWPF.Show();
            foreach (var item in viewSheets)
            {
                progressbarWPF.Giatri();
                if (!progressbarWPF.iscontinue)
                {
                    break;
                }
                uidoc.ActiveView = item;
            }
            progressbarWPF.Close();
        }
    }
}
