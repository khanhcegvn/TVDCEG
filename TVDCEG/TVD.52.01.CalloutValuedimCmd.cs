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
using TVDCEG.LBR;
using TVDCEG.Ultis;

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CalloutValuedimCmd : IExternalCommand
    {
        public bool iscontinue = true;
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;
            Selection sel = activeUIDocument.Selection;
            //while (iscontinue)
            //{
            //    try
            //    {
            //        Reference reference = sel.PickObject(ObjectType.Element,/* new Filterdimention(),*/ "Select Dimension");
            //        Dimension dimension = doc.GetElement(reference) as Dimension;
            //        CSAZDimCallout dim = new CSAZDimCallout(doc, dimension, new XYZ(1, 0, 0));
            //        CreateTextNote(doc, dim);
            //    }
            //    catch
            //    {
            //        iscontinue = false;
            //    }
            //}
            Reference reference = sel.PickObject(ObjectType.Element,/* new Filterdimention(),*/ "Select Dimension");
            Dimension dimension = doc.GetElement(reference) as Dimension;
            CSAZDimCallout dim = new CSAZDimCallout(doc, dimension, new XYZ(1, 0, 0));
            TextNote textNote = null;
            CreateTextNote(doc, dim, sel, ref textNote);
            Transaction tran = new Transaction(doc, "Create Line");
            tran.Start();
            PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
            double value = textNote.GetMaximumAllowedWidth();
            XYZ p1 = Timdiemdimngang(dim.Endpoint, dim.VectorDirection, 3);
            var dline = doc.Create.NewDetailCurve(doc.ActiveView, Line.CreateBound(pLane3D.ProjectPointOnPlane(dim.Endpoint), pLane3D.ProjectPointOnPlane(p1)));

            tran.Commit();
            return Result.Succeeded;
        }
        public void CreateTextNote(Document doc, CSAZDimCallout dim, Selection sel, ref TextNote textNote)
        {
            var typetextnote = (from TextNoteType x in new FilteredElementCollector(doc).OfClass(typeof(TextNoteType)) where x.Name == "3/32\" Arial" select x).First();
            XYZ Point = sel.PickPoint();
            double SpaceToStartPoint = Point.DistanceTo(dim.Startpoint);
            double SpaceToEndPoint = Point.DistanceTo(dim.Endpoint);
            if (SpaceToEndPoint < SpaceToStartPoint)
            {
                using (Transaction tran = new Transaction(doc, "Create Text"))
                {
                    tran.Start();
                    textNote = TextNote.Create(doc, doc.ActiveView.Id, dim.Endpoint, dim.Numbersegment + " " + dim.ControlMark, typetextnote.Id);

                    tran.Commit();
                }
            }
            else
            {
                using (Transaction tran = new Transaction(doc, "Create Text"))
                {
                    tran.Start();
                    textNote = TextNote.Create(doc, doc.ActiveView.Id, dim.Startpoint, dim.Numbersegment + " " + dim.ControlMark, typetextnote.Id);
                    double value = textNote.Width;
                    XYZ p1 = Timdiemdimngang(dim.Startpoint, dim.VectorDirection, value);
                    doc.Create.NewDetailCurve(doc.ActiveView, Line.CreateBound(dim.Startpoint, p1));
                    tran.Commit();
                }
            }
        }
        public XYZ Timdiemdimngang(XYZ A, XYZ V, double vm)
        {
            XYZ diem = null;
            //var val = UnitUtils.Convert(vm, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
            var val = vm;
            var tong = Math.Pow(V.X, 2) + Math.Pow(V.Y, 2) + Math.Pow(V.Z, 2);
            var x1 = A.X + ((V.X) * val / (tong));
            var y1 = A.Y + ((V.Y) * val / (tong));
            var z1 = A.Z + ((V.Z) * val / (tong));
            var x2 = A.X - ((V.X) * val / (tong));
            var y2 = A.Y - ((V.Y) * val / (tong));
            var z2 = A.Z - ((V.Z) * val / (tong));
            XYZ diem1 = new XYZ(x1, y1, z1);
            XYZ diem2 = new XYZ(x2, y2, z2);
            XYZ Adiem1 = new XYZ(x1 - A.X, y1 - A.Y, z1 - A.Z);
            XYZ Adiem2 = new XYZ(x2 - A.X, y2 - A.Y, z2 - A.Z);
            if (Util.Nguochuong(Adiem1, V))
            {
                diem = diem1;
            }
            if (Util.Nguochuong(Adiem2, V))
            {
                diem = diem2;
            }
            return diem;
        }
        public XYZ Timdiemthu3(XYZ p1, XYZ p2)
        {
            XYZ px = XYZ.Zero;
            XYZ r1 = new XYZ(p1.X, p2.Y, p2.Z);
            XYZ vector1 = p1 - r1;
            XYZ vector2 = p2 - r1;
            if (vector1.IsPerpendicular(vector2))
            {
                px = r1;
            }
            return px;
        }
    }
}
