#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class CopyConnectionSamegeometrycmd : IExternalCommand
    {
        public Document doc;
        public List<FamilyInstance> listtarget = new List<FamilyInstance>();
        public FamilyInstance elesource = null;
        private UIDocument uidoc;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            ElementtransformToCopy tr = new ElementtransformToCopy();
            Reference ef = sel.PickObject(ObjectType.Element, new Filterstructuralframing(), "Selection element source");
            elesource = doc.GetElement(ef) as FamilyInstance;


            //Reference ef = sel.PickObject(ObjectType.Element, new AssemblySelectionfilter(), "Select Assembly");
            //Element ele = doc.GetElement(ef);
            //AssemblyInstance instance = ele as AssemblyInstance;
            //elesource = ElementtransformToCopy.Elementcopy(doc, instance);

            ICollection<ElementId> elementIds = Elementlistcopy(doc);
            //listtarget = ElementTarget(doc);
            listtarget = Findlisttagget(doc,elesource);
            tr.CopyElementsConnDtee(doc, elesource, listtarget, elementIds);
            return Result.Succeeded;
        }
        public List<FamilyInstance> Findlisttagget(Document doc,FamilyInstance familyInstance)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            var col = new FilteredElementCollector(doc).OfCategory(familyInstance.Category.ToBuiltinCategory()).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            double length = familyInstance.Getlength();
            double StemWidth = familyInstance.GetWidthStemOfDoubletee(doc);
            string symbol = familyInstance.GetSymbol(doc).Name;
            foreach (var item in col)
            {
                if(item.Getlength()==length&&item.GetSymbol(doc).Name.Equals(symbol))
                {
                    if(item.Id.IntegerValue!=familyInstance.Id.IntegerValue)
                    {
                        list.Add(item);
                    }
                }
            }
            return list;
        }
        public FamilyInstance GetSuperInstances(FamilyInstance familyInstance)
        {
            FamilyInstance super = null;
            var superinstance = familyInstance.SuperComponent as FamilyInstance;

            if (superinstance == null)
            {
                super = familyInstance;
            }
            else
            {
                super = superinstance;
            }
            return super;
        }
        public ICollection<ElementId> Elementlistcopy(Document doc)
        {
            ICollection<ElementId> listconnection = new List<ElementId>();
            Selection sel = uidoc.Selection;
            IList<Reference> gh = sel.PickObjects(ObjectType.Element, new FilterSpecialEquipment(), "Selection");
            foreach (var j in gh)
            {
                Element ee = doc.GetElement(j);
                FamilyInstance instance = ee as FamilyInstance;
                FamilyInstance instance1 = GetSuperInstances(instance);
                listconnection.Add(instance1.Id);
            }
            return listconnection;
        }
        public List<FamilyInstance> ElementTarget(Document doc)
        {
            Selection sel = uidoc.Selection;
            IList<Reference> gh = sel.PickObjects(ObjectType.Element, new Filterstructuralframing(), "Selection");
            List<FamilyInstance> instances = new List<FamilyInstance>();
            foreach (var t in gh)
            {
                FamilyInstance op = doc.GetElement(t) as FamilyInstance;
                instances.Add(op);
            }
            return instances;
        }
    }
}
