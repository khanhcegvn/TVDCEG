#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Renamesheetcmd : IExternalCommand
    {
        public static Dictionary<string, ElementId> dicViews = new Dictionary<string, ElementId>();
        //public static IList<Data> lstViews = new List<Data>()
        public static IList<string> ls = new List<string>();
        public static IList<DataRename> ListSheet = new List<DataRename>();
        public static IList<Element> ListAssembly = new List<Element>();
        public static Dictionary<string, ElementId> dic = new Dictionary<string, ElementId>();
        public Dictionary<string, List<Element>> numberdic;


        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            FilteredElementCollector ASEM
              = new FilteredElementCollector(doc)
                .WhereElementIsNotElementType()
                .OfCategory(BuiltInCategory.INVALID).OfClass(typeof(ViewSheet));

            foreach (var ii in ASEM.ToElements())
            {
                var v1 = ii as ViewSheet;
                string h = ii.Name;
                var objType = doc.GetElement(v1.GetTypeId()) as ElementType;
                if (objType == null || !ISASSEMBLYSHEET(v1))
                {
                    continue;
                }
                if (v1 != null)
                {
                    var c = v1.SheetNumber;

                    ElementId e2 = v1.Id;
                    var ass = doc.GetElement(v1.AssociatedAssemblyInstanceId);
                    ListSheet.Add(new DataRename(e2, h, c, ass.Name));
                }

            }

            var form = new RenameSheet(this, doc);
            form.ShowDialog();
            if (form.ttrue == true)
            {
                ElementId newid = new ElementId(form.Gotoid);
                ViewSheet vs = doc.GetElement(newid) as ViewSheet;
                uidoc.ActiveView = vs;
            }
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Transaction Name");

                tx.Commit();
            }

            return Result.Succeeded;
        }
        bool ISASSEMBLYSHEET(ViewSheet VS)
        {
            var ass = VS.AssociatedAssemblyInstanceId;
            if (ass.IntegerValue != -1) return true;
            else return false;

        }
    }
    public class DataRename
    {
        public ElementId Sheet { get; set; }
        public string SheetName { get; set; }
        public string SheetNumber { get; set; }
        public string RAssemblyname { get; set; }
        public DataRename(ElementId a, string b, string c, string d)
        {
            Sheet = a;
            SheetName = b;
            SheetNumber = c;
            RAssemblyname = d;
        }
    }

}
