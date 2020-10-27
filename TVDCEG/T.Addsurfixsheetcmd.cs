#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using TVDCEG.WPF;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Addsurfixsheetcmd : IExternalCommand
    {
        public Document doc;
        public Dictionary<string, ViewSheet> ListSheet = new Dictionary<string, ViewSheet>();
        [Obsolete]
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
            ListSheet = GetViewSheets(doc);
            using (FrmAddprefixsheetWPF frm = new FrmAddprefixsheetWPF(this, doc))
            {
                if (frm.ShowDialog() == true)
                {
                    if (frm.listsheet1 != null)
                    {
                        Getviewportonsheet(doc, frm.listsheet1, frm.prefix, frm.suffix, frm.SheetNumber_);
                    }
                }
            }
            //using (FrmAddsurfixsheet form = new FrmAddsurfixsheet(this, doc))
            //{
            //    if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
            //    {
            //        if (form.listsheet1 != null)
            //        {
            //            if (form.checkvalue == true)
            //            {
            //                Getviewportonsheet(doc, form.listsheet1, form.prefix, form.suffix, form.SheetNumber_);
            //            }
            //        }
            //    }
            //}
            return Result.Succeeded;
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

        [Obsolete]
        public void Getviewportonsheet(Document doc, List<ViewSheet> viewsheets, string Prefix, string Suffix, string SheetNumber_)
        {
            //ProgressBarform progressBarform = new ProgressBarform(viewsheets.Count, "Loading...");
            //progressBarform.Show();
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(viewsheets.Count, "Loading...");
            progressbarWPF.Show();
            using (Transaction tran = new Transaction(doc, "Rename Sheet"))
            {
                tran.Start();
                foreach (var i in viewsheets)
                {
                    //progressBarform.giatri();
                    //if (progressBarform.iscontinue == false)
                    //{
                    //    break;
                    //}
                    progressbarWPF.Giatri();
                    if (progressbarWPF.iscontinue == false)
                    {
                        break;
                    }
                    i.ViewName = Prefix + " " + i.ViewName + " " + Suffix;
                    if (SheetNumber_.Length != 0)
                    {
                        int so = int.Parse(SheetNumber_);
                        string ten = i.SheetNumber;
                        string newstring = ten.Split('.')[0];
                        string newstring2 = ten.Split('.')[1];
                        int soo = int.Parse(newstring2);
                        int lp = int.Parse(SheetNumber_);
                        int jh = soo + lp;
                        string newsheetnumber = newstring + "." + jh;
                        i.SheetNumber = newsheetnumber;
                    }
                    var y = i.GetAllPlacedViews();
                    foreach (var t in y)
                    {
                        View hh = doc.GetElement(t) as View;
                        hh.ViewName = Prefix + " " + hh.ViewName + " " + Suffix;
                    }
                }
                tran.Commit();
            }
            progressbarWPF.Close();
            //progressBarform.Close();

        }
    }
}
