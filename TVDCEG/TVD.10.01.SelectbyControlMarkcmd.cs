#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class SelectbyControlMarkcmd : IExternalCommand
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
            var ids = Filterelement(doc, sel);
            sel.SetElementIds(ids);
            return Result.Succeeded;
        }
        public ICollection<ElementId> GetElementsameMark(Document doc, FamilyInstance familyInstance)
        {
            ICollection<ElementId> collections = new List<ElementId>();
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            try
            {
                AssemblyInstance assemblyInstance = doc.GetElement(familyInstance.AssemblyInstanceId) as AssemblyInstance;
                ICollection<ElementId> elementIds = assemblyInstance.GetMemberIds();
                Parameter controlmark = familyInstance.LookupParameter("CONTROL_MARK");
                if (controlmark != null)
                {
                    var col1 = (from FamilyInstance x in new FilteredElementCollector(doc, elementIds).OfClass(typeof(FamilyInstance)) where x.LookupParameter("CONTROL_MARK") != null select x).ToList();
                    var col2 = (from FamilyInstance y in col1 where y.LookupParameter("CONTROL_MARK").AsString() == controlmark.AsString() select y.Id).ToList();
                    col2.ForEach(x => collections.Add(x));
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show(ex.Message, "Can't find Paramter CONTROL_MARK");
            }
            return collections;
        }
        public List<ElementId> Filterelement(Document doc,Selection sel)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            try
            {
                IList<Reference> rfs = sel.PickObjects(ObjectType.Element);
                rfs.ToList().ForEach(x => list.Add((FamilyInstance)doc.GetElement(x)));
            }
            catch (Exception)
            {
                
            }
            List<ElementId> listids = new List<ElementId>();
            foreach (var item in list)
            {
                var op = GetElementsameMark(doc, item);
                op.ToList().ForEach(y => listids.Add(y));
            }
            return listids;
        }
    }

}
