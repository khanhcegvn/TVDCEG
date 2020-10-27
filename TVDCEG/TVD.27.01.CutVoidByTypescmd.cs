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
    public class CutVoidByTypescmd : IExternalCommand
    {
        public Document doc;
        public ExternalEvent _event;
        public UIDocument uidoc;
        public Selection sel;
        public CutVoidByTypes cutting;
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
            List<FamilyInstance> listvoids = new List<FamilyInstance>();
            foreach (var item in sel.GetElementIds())
            {
                FamilyInstance familyInstance = doc.GetElement(item) as FamilyInstance;
                listvoids.Add(familyInstance);
            }
            if(listvoids.Count!=0)
            {
                cutting = new CutVoidByTypes(doc);
                using (var form = new FrmCutvoidbytypes(this, doc))
                {
                    if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                    {
                        if (form.list.Count != 0)
                        {
                            if(form.cut)
                            {
                                cutting.Cutting(doc, listvoids, form.list);
                            }
                            if(form.uncut)
                            {
                                cutting.UnCut(doc, listvoids, form.list);
                            }
                        }
                    }
                }
            }
            else
            {
                TaskDialog.Show("Error", "Select void");
            }
            return Result.Succeeded;
        }
    }
}
