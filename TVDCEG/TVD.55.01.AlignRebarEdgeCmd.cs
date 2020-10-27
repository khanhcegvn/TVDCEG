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
    public class AlignRebarEdgeCmd : IExternalCommand
    {
        [Obsolete]
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication application = commandData.Application;
            UIDocument activeUIDocument = application.ActiveUIDocument;
            Application application2 = application.Application;
            Document doc = activeUIDocument.Document;
            Selection sel = activeUIDocument.Selection;
            using (var form = new FrmAlignRebar())
            {
                if (form.ShowDialog() != System.Windows.Forms.DialogResult.OK && form.check)
                {
                    using (Transaction tran = new Transaction(doc, "Align Rebar"))
                    {
                        tran.Start();
                        //IList<Reference> listreference = sel.PickObjects(ObjectType.Element).ToList();
                        IList<Element> listreference = sel.PickElementsByRectangle(new CegRebarFilter(), "Select Rebar").ToList();
                        Reference rf2 = sel.PickObject(ObjectType.Edge);
                        FamilyInstance fa2 = doc.GetElement(rf2.ElementId) as FamilyInstance;
                        Transform transform = fa2.GetTransform();
                        Curve bed = (doc.GetElement(rf2.ElementId).GetGeometryObjectFromReference(rf2) as Edge).AsCurve();
                        foreach (var rf in listreference)
                        {
                            //Element ele = doc.GetElement(rf);
                            //FamilyInstance fa1 = ele as FamilyInstance;
                            FamilyInstance fa1 = rf as FamilyInstance;
                            AlignRebarEdge.Instance.SetValue(doc, bed as Line, fa1, form.space);
                        }
                        tran.Commit();
                    }
                    return Result.Succeeded;
                }
                else
                {
                    return Result.Cancelled;
                }
            }

            //using (Transaction tran = new Transaction(doc, "Create detail line"))
            //{
            //    tran.Start();
            //    Plane plane = Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin);
            //    PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
            //    Line line = plane.ProjectLineOnPlane(bed as Line);
            //    doc.Create.NewDetailCurve(doc.ActiveView, line);
            //    tran.Commit();
            //}
            //return Result.Succeeded;
        }
    }
}
