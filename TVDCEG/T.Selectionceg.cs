using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;

namespace TVDCEG
{
    public class Selectionceg
    {
        public static List<FamilyInstance> SelectionbyControl_Mark(Document doc, FamilyInstance familyInstance)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            Parameter pa = familyInstance.LookupParameter("CONTROL_MARK");
            string HH = pa.AsString();
            FilteredElementCollector col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_StructuralFraming);
            var tp = col.ToElements();
            foreach (var t in tp)
            {
                FamilyInstance gh = doc.GetElement(t.Id) as FamilyInstance;
                Parameter pl = gh.LookupParameter("CONTROL_MARK");
                string hu = pl.AsString();
                if (hu == HH)
                {
                    if (gh.Id != familyInstance.Id)
                    {
                        list.Add(gh);
                    }
                }
            }
            return list;
        }
    }
    public class AssemblySelectionfilter : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name == "Assemblies")
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class Filterdimention : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Dimensions)
                return true;
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class FilterBrick : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Name.Contains("BRICK"))
                return true;
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class Filterstructuralframing : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name.Contains("Framing"))
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class FilterSymbolTextNote : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericAnnotation)
                return true;
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }
    public class FilterSpecialEquipment : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name.Contains("Specialty"))
            {
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            return false;
        }
    }

}
