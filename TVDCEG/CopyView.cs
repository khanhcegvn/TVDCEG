#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Linq;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CopyView : IExternalCommand
    {
        public static string vName = "";
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            {

                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Application app = uiapp.Application;
                Document doc = uidoc.Document;
             
                Selection sel = uidoc.Selection;
                var elementId = sel.GetElementIds();

                var element = from eid in elementId select doc.GetElement(eid);
                var vp = element.First() as Viewport;
                vName = vp.Name;
                XYZ p2 = sel.PickPoint("Pick Point to Place View.");
                XYZ pz = XYZ.Zero;

                foreach (Element e in element)
                {
                    if (e is Viewport)
                    {
                        RecreateViewport(doc, e as Viewport, p2);
                        var ol = (e as Viewport).GetBoxOutline();
                        var max = ol.MaximumPoint;
                        var min = ol.MinimumPoint;
                        var aver = (max - min) / 2;
                        var x = aver.X + p2.X;
                         pz = new XYZ(x, p2.Y, p2.Z);
                        

                    }
                    p2 = pz;
                }
                return Result.Succeeded;
            }
        }
        public void RecreateViewport (Document doc, Viewport viewport,XYZ p2)
        {
            // 1.Get XYZ of viewport :
            BoundingBoxXYZ bb = viewport.get_BoundingBox(doc.GetElement(viewport.OwnerViewId) as View);
            XYZ p1 = viewport.GetBoxCenter();
            // 2. Get ViewId (eg. Legend ID) :
            ElementId viewID = viewport.ViewId;
            View view = doc.GetElement(viewID) as View;
            // 3. Get viewsheet ID :
            ElementId vsid = viewport.SheetId;

                using (Transaction t = new Transaction(doc, " viewport"))
                {
                    t.Start();

                    // 4. Delete Viewport has selected
                    //if (viewport.Id != null) doc.Delete(viewport.Id);
                    // 5. Duplicate View :
                    View dupView = null;
                    ElementId newViewId = ElementId.InvalidElementId;
                    if (view.CanViewBeDuplicated(ViewDuplicateOption.WithDetailing))
                    {
                        newViewId = view.Duplicate(ViewDuplicateOption.WithDetailing);
                        dupView = doc.GetElement(newViewId) as View;
                    }
                    // 6. Create Viewport associated with View in sheet :

                    Viewport vp = Viewport.Create(doc, vsid, dupView.Id, p2);

                    t.Commit();
                }
                
            }
    }
}

