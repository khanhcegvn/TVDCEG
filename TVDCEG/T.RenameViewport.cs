#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class RenameViewport : IExternalCommand
    {
        public Document doc;
        public List<Grid> listgrid = new List<Grid>();
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
            ChangeName(doc);
            return Result.Succeeded;
        }
        public void ChangeName(Document doc)
        {
            List<ViewSheet> viewSheets = GetViewSheets(doc);
            ProgressBarform progressBarform = new ProgressBarform(viewSheets.Count, "Loading...");
            progressBarform.Show();
            foreach (ViewSheet sheet in viewSheets)
            {
                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                var gi = sheet.GetAllPlacedViews();
                foreach (var i in gi)
                {
                    Element ele = doc.GetElement(i);
                    View view = ele as View;
                    string Sname = view.Name;
                    using (Transaction tran = new Transaction(doc, "Rename"))
                    {
                        tran.Start();
                        if (Sname.EndsWith("Copy 2"))
                        {
                            string name2 = Sname.Replace("Copy 2", "S");
                            view.Name = name2;
                        }
                        if (Sname.EndsWith("Copy 1"))
                        {
                            string name2 = Sname.Replace("Copy 1", "S");
                            view.Name = name2;
                        }
                        tran.Commit();
                    }
                }
            }
            progressBarform.Close();
        }
        public List<ViewSheet> GetViewSheets(Document doc)
        {
            List<ViewSheet> viewSheets = new List<ViewSheet>();
            FilteredElementCollector col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(ViewSheet));
            var bn = col.ToElements();
            foreach (var i in bn)
            {
                ViewSheet sheet = i as ViewSheet;
                if (sheet.SheetNumber.Contains("PC12"))
                {
                    viewSheets.Add(sheet);
                }
            }
            return viewSheets;
        }
    }
}
