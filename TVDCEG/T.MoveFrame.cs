#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class MoveFramecmd : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            Selection sel = uidoc.Selection;

            Reference rf = sel.PickObject(ObjectType.Element);
            FamilyInstance familyInstance = doc.GetElement(rf) as FamilyInstance;
            XYZ point1 = sel.PickPoint();
            XYZ point2 = sel.PickPoint();
            MoveElement(doc, familyInstance, point1, point2);
            return Result.Succeeded;
        }
        public void MoveElement(Document doc, FamilyInstance familyInstance, XYZ point1, XYZ point2)
        {
            Line line = Line.CreateBound(point1, point2);
            XYZ direc = line.Direction * 0.010417;
            using (Transaction tr = new Transaction(doc, "Move element"))
            {
                tr.Start();
                ElementTransformUtils.MoveElement(doc, familyInstance.Id, direc);
                tr.Commit();
            }
        }
    }
}
