using System;
using System.IO;
using System.Linq;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class SelectElementsinRowCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;
            Selection sel = activeUIDocument.Selection;
            FamilyInstance wall1 = (FamilyInstance)doc.GetElement(sel.PickObject(ObjectType.Element, new InstanceFilter(), "Select Element First"));
            FamilyInstance wall2 = (FamilyInstance)doc.GetElement(sel.PickObject(ObjectType.Element, new InstanceFilter(), "Select Element Second"));
            LocationPoint locationPoint = (LocationPoint)wall1.Location;
            XYZ xyz = (locationPoint != null) ? locationPoint.Point : null;
            LocationPoint locationPoint2 = (LocationPoint)wall2.Location;
            XYZ xyz2 = (locationPoint2 != null) ? locationPoint2.Point : null;
            XYZ direction = (xyz - xyz2).Normalize();
            var listwall = (from x in AutodimElement.Instance.GetFamilyInstances2(doc, doc.ActiveView, wall1, wall2, direction) select x.Id).ToList();
            sel.SetElementIds(listwall);
            return Result.Succeeded;
        }
    }
}
