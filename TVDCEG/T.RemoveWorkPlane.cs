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
    public class RemoveWorkPlane : IExternalCommand
    {
        public Document doc;
        public List<Element> listelement = new List<Element>();
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
            IList<Reference> rfs = sel.PickObjects(ObjectType.Element, new FilterSpecialEquipment(), "Selection");
            ICollection<ElementId> idcopy = new List<ElementId>();
            foreach (Reference v in rfs)
            {
                Element ele = doc.GetElement(v);
                idcopy.Add(ele.Id);
            }
            CopyPasteOptions copyPasteOptions = new CopyPasteOptions();
            copyPasteOptions.SetDuplicateTypeNamesHandler(new CopyHandlerRemoveWorplane());
            ICollection<ElementId> newid = new List<ElementId>();
            using (Transaction tran = new Transaction(doc, "ss"))
            {
                tran.Start();
                foreach (ElementId i in idcopy)
                {
                    Element ele = doc.GetElement(i);
                    LocationPoint point = ele.Location as LocationPoint;
                    XYZ tu = point.Point;
                    FamilyInstance familyInstance = ele as FamilyInstance;
                    ICollection<ElementId> listlp = familyInstance.GetSubComponentIds();
                    Transform transform = familyInstance.GetTransform();
                    Transform transform1 = Transform.CreateTranslation(-transform.BasisZ);
                    FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                    IgnoreProcess ignoreProcess = new IgnoreProcess();
                    options.SetClearAfterRollback(true);
                    options.SetFailuresPreprocessor(ignoreProcess);
                    tran.SetFailureHandlingOptions(options);
                    ICollection<ElementId> Listcopy = new List<ElementId>();
                    foreach (ElementId j in listlp)
                    {
                        Listcopy.Add(j);
                    }
                    newid = ElementTransformUtils.CopyElements(doc, Listcopy, doc, transform1, copyPasteOptions);
                    foreach (var gh in newid)
                    {
                        Element eleoop = doc.GetElement(gh);
                        LocationPoint pointyu = eleoop.Location as LocationPoint;
                        XYZ pointg = pointyu.Point;
                        XYZ pointnew = new XYZ(tu.X - pointg.X, tu.Y - pointg.Y, tu.Z - pointg.Z);
                        ElementTransformUtils.MoveElement(doc, gh, pointnew);
                        Element Newele = doc.GetElement(gh);
                        FamilyInstance newinstance = Newele as FamilyInstance;
                    }
                    Listcopy.Remove(i);
                    doc.Delete(ele.Id);
                }
                tran.Commit();
            }
            return Result.Succeeded;
        }
        public List<Element> GetElements(Document doc)
        {
            List<Element> list = new List<Element>();
            FilteredElementCollector col = new FilteredElementCollector(doc, doc.ActiveView.Id);
            var tui = col.ToElements();
            foreach (var i in tui)
            {
                list.Add(i);
            }
            return list;
        }
    }
    public class CopyHandlerRemoveWorplane : IDuplicateTypeNamesHandler
    {
        public DuplicateTypeAction OnDuplicateTypeNamesFound(DuplicateTypeNamesHandlerArgs args)
        {
            return DuplicateTypeAction.UseDestinationTypes;
        }
    }
}
