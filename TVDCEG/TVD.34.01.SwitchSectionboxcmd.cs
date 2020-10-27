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
    public class SwitchSectionboxcmd : IExternalCommand
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
            if (doc.ActiveView.ViewType == ViewType.ThreeD)
            {
                Parameter sectionbox = doc.ActiveView.LookupParameter("Section Box");
                using (Transaction tran = new Transaction(doc, "Modify element attributes"))
                {
                    tran.Start();
                    if (sectionbox != null)
                    {
                        if (sectionbox.AsInteger() == 0) sectionbox.Set(1);
                        else if (sectionbox.AsInteger() == 1)
                        {
                            sectionbox.Set(0);
                        }
                    }
                    tran.Commit();
                }
                return Result.Succeeded;
            }
            else
            {
                TaskDialog.Show("Ivention EXT:", "Please go to 3d view");
                return Result.Cancelled;
            }
        }
    }
}
