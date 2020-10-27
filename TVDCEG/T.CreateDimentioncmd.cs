#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CreateDimentioncmd : IExternalCommand
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
            Reference rf = sel.PickObject(ObjectType.Element);
            ElementtransformToCopy kl = new ElementtransformToCopy();
            Element ele = doc.GetElement(rf);
            FamilyInstance familyInstance = ele as FamilyInstance;
            FamilyInstance flat = kl.GetFlat(doc, familyInstance);
            var t1 = flat.GetReferences(FamilyInstanceReferenceType.StrongReference).First();
            var t2 = flat.GetReferences(FamilyInstanceReferenceType.StrongReference).First();
            Transform transform = flat.GetTransform();
            EdgeArray edgeArray = kl.FlEdgeArray(flat);
            List<Edge> list = new List<Edge>();
            foreach (Edge i in edgeArray)
            {
                list.Add(i);
            }
            list.OrderBy(i => i.ApproximateLength).ToList();
            Edge max = list.Last();
            Curve cure = max.AsCurve();
            Line lop = cure as Line;
            ReferenceArray referenceArray = new ReferenceArray();
            XYZ startpoint = lop.GetEndPoint(0);
            XYZ endpoint = lop.GetEndPoint(1);
            using (Transaction tr = new Transaction(doc, "cre"))
            {
                tr.Start();
                Line line = Line.CreateBound(transform.OfPoint(startpoint), transform.OfPoint(endpoint));
                DetailLine detailLine = doc.Create.NewDetailCurve(doc.ActiveView, line) as DetailLine;
                referenceArray.Append(t1);
                referenceArray.Append(t2);
                var t = doc.Create.NewDimension(doc.ActiveView, line, referenceArray);
                tr.Commit();
            }
            return Result.Succeeded;
        }

    }
}
