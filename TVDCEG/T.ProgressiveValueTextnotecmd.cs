using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Text.RegularExpressions;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class ProgressiveValueTextnotecmd : IExternalCommand
    {
        public Document doc;
        public int a = 1;
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
            bool iscontinue = true;
            Reference rf = sel.PickObject(ObjectType.Element, new FilterSymbolTextNote(), "Select symbol");
            var e = doc.GetElement(rf);
            Parameter pa1 = e.LookupParameter("KEYNOTE");
            string value = pa1.AsString();
            var so = TachSo(value);
            var sosau = so + a;
            try
            {
                while (iscontinue)
                {
                    Reference Symbolpick = sel.PickObject(ObjectType.Element, new FilterSymbolTextNote(), "Select symbol");
                    var fg = doc.GetElement(Symbolpick);
                    Parameter pal = fg.LookupParameter("KEYNOTE");
                    if (pal != null)
                    {
                        using (Transaction tx = new Transaction(doc))
                        {
                            tx.Start("Value Override");
                            pal.Set(sosau.ToString());
                            tx.Commit();
                            sosau++;
                        }
                    }
                }
            }
            catch
            {
                iscontinue = false;
                return Result.Succeeded;
            }
            return Result.Succeeded;
        }
        int TachSo(string input)
        {
            int i = 1;
            string[] numbers = Regex.Split(input, @"\D+");
            foreach (string value in numbers)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    i = int.Parse(value);
                }
            }
            return i;
        }
    }

}
