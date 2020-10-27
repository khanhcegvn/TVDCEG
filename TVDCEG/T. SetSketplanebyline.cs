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
    public class SetSketchplanebyline : IExternalCommand
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
            var rf = sel.PickObjects(ObjectType.Edge);
            List<Curve> list = new List<Curve>();
            foreach (var i in rf)
            {
                var g = i.ElementId;
                var ele = doc.GetElement(g).GetGeometryObjectFromReference(i) as Edge;
                var curve = ele.AsCurve();
                list.Add(curve);
            }
            using (Transaction t = new Transaction(doc, "set"))
            {
                t.Start();
                Plane plane = Plane.CreateByThreePoints(list[0].GetEndPoint(0), list[0].GetEndPoint(1), list[1].Evaluate(0.05, true));
                SketchPlane stk = SketchPlane.Create(doc, plane);
                doc.ActiveView.SketchPlane = stk;
                t.Commit();
            }
            return Result.Succeeded;
        }
    }
}
