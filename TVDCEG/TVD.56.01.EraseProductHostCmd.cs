using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class EraseProductHostcmd : IExternalCommand
    {
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;
            Selection sel = activeUIDocument.Selection;
            ICollection<ElementId> list = sel.GetElementIds();
            using (Transaction tran = new Transaction(doc, "Iven EXT: Erase Product Host"))
            {
                tran.Start();
                foreach (var item in list)
                {
                    FamilyInstance instance = doc.GetElement(item) as FamilyInstance;
                    EraseHost(instance);
                }
                tran.Commit();
            }
            return Result.Succeeded;
        }
        public void EraseHost(FamilyInstance familyInstance)
        {
            Parameter CONSTRUCTION_PRODUCT_HOST = familyInstance.LookupParameter("CONSTRUCTION_PRODUCT_HOST");
            Parameter BOM_PRODUCT_HOST = familyInstance.LookupParameter("BOM_PRODUCT_HOST");
            if (CONSTRUCTION_PRODUCT_HOST != null)
            {
                CONSTRUCTION_PRODUCT_HOST.Set("");
            }
            if (BOM_PRODUCT_HOST != null)
            {
                BOM_PRODUCT_HOST.Set("");
            }
        }
    }
}
