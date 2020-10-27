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
    public class SetOrienViewcmd : IExternalCommand
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
            Reference rf = sel.PickObject(ObjectType.Face, "Select Face");
            Element ele = doc.GetElement(rf);
            GeometryObject geoobject = ele.GetGeometryObjectFromReference(rf);
            PlanarFace face = geoobject as PlanarFace;
            Plane plane = face.Faceby3pointPlane();
            XYZ direction = face.ComputeNormal(UV.Zero);
            using (Transaction tran = new Transaction(doc, "Set view by face"))
            {
                tran.Start();
                SketchPlane skt = SketchPlane.Create(doc, plane);
                View3D view3d = doc.ActiveView as View3D;
                view3d.OrientTo(plane.Normal);
                doc.ActiveView.SketchPlane = skt;
                uidoc.RefreshActiveView();
                uidoc.ShowElements(ele.Id);
                tran.Commit();
            }
            return Result.Succeeded;
        }
    }
}
