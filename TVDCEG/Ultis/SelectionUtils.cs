using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class SelectionUtils
    {
        public static List<Element> IdsToElements(this Document doc, ICollection<ElementId> eIds)
        {
            var eles = new List<Element>();
            foreach (ElementId eId in (IEnumerable<ElementId>)eIds)
                eles.Add(doc.GetElement(eId));
            return eles;
        }

        public static Element PickSingleElement(this UIDocument uidoc, string promtString)
        {
            try
            {
                var rf = uidoc.Selection.PickObject(ObjectType.Element, promtString);
                return uidoc.Document.GetElement(rf);
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return null;
            }
        }

        public static List<FamilyInstance> AllFamilyInstanceOfType(this Document doc, string symbolName)
        {
            var col = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().Cast<FamilyInstance>();
            return col.Where(x => x.Symbol.Name == symbolName).ToList();
        }

        public static List<FamilyInstance> AllFamilyInstanceOfTypeInActiveView(this Document doc, string symbolName)
        {
            var col = new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(FamilyInstance)).WhereElementIsNotElementType().Cast<FamilyInstance>();
            return col.Where(x => x.Symbol.Name == symbolName).ToList();
        }

        public static List<Grid> AllGrids(this Document doc)
        {
            return new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(Grid)).WhereElementIsNotElementType().ToElements().Cast<Grid>().ToList();
        }

        public static List<Element> AllLevels(this Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(Level)).WhereElementIsNotElementType().ToElements().ToList();
        }

        public static List<Element> AllReferencePlanes(this Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(ReferencePlane)).WhereElementIsNotElementType().ToElements().ToList();
        }

        public static List<Element> AllStructuralFraming(this Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsNotElementType().ToElements().ToList();
        }

        public static List<Element> AllStructuralFoundations(this Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFoundation).WhereElementIsNotElementType().ToElements().ToList();
        }

        public static List<Element> AllStructuralRebar(this Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Rebar).WhereElementIsNotElementType().ToElements().ToList();
        }

        public static List<Element> AllStructuralColumns(this Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralColumns).WhereElementIsNotElementType().ToElements().ToList();
        }

        public static List<Element> AllFloors(this Document doc)
        {
            return new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_Floors).WhereElementIsNotElementType().ToElements().ToList();
        }

        public static List<Element> AllViewports(this Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(Viewport)).WhereElementIsNotElementType().ToElements().ToList();
        }
        public static List<ScheduleSheetInstance> AllScheduleSheetInstancesInView(this Document doc, ViewSheet viewSheet)
        {
            return new FilteredElementCollector(doc, viewSheet.Id).OfClass(typeof(ScheduleSheetInstance)).WhereElementIsNotElementType().Cast<ScheduleSheetInstance>().ToList();
        }

        public static List<View> AllViews(this Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(View)).WhereElementIsNotElementType().ToElements().Cast<View>().ToList();
        }
        public static List<ViewSheet> AllViewSheet(this Document doc)
        {
            return new FilteredElementCollector(doc).OfClass(typeof(ViewSheet)).WhereElementIsNotElementType().ToElements().Cast<ViewSheet>().ToList();
        }

        //----------------------------------------------------------------------------------------------------------
        public static List<Element> ElementsOfTypeInView(this Document doc, Type t, View v)
        {
            List<Element> elementList = new List<Element>();
            FilteredElementIterator elementIterator = new FilteredElementCollector(doc, v.Id).OfClass(t).WhereElementIsNotElementType().GetElementIterator();
            while (elementIterator.MoveNext())
            {
                Element current = elementIterator.Current;
                if (current.GetType() == t)
                    elementList.Add(current);
            }
            return elementList;
        }

        public static List<Element> GetFromActiveViewByBIC(Document doc, BuiltInCategory bic)
        {
            return GetFromActiveViewByBICs(doc, new BuiltInCategory[1]
            {
                bic
            });
        }

        public static List<Element> GetFromActiveViewByBICs(Document doc, BuiltInCategory[] bics)
        {
            return GetFromViewByBICs(doc, doc.ActiveView, bics);
        }

        public static List<Element> GetFromViewByType(this Document doc, Type t, View v)
        {
            List<Element> elementList = new List<Element>();
            FilteredElementIterator elementIterator = new FilteredElementCollector(doc, v.Id).OfClass(t).WhereElementIsNotElementType().GetElementIterator();
            while (elementIterator.MoveNext())
            {
                Element current = elementIterator.Current;
                if (current != null && current.GetType() == t)
                    elementList.Add(current);
            }
            return elementList;
        }

        public static List<Element> GetFromViewByBICs(this Document doc, View v, BuiltInCategory[] bics)
        {
            IList<ElementFilter> elementFilterList = new List<ElementFilter>(bics.Length);
            foreach (var builtInCategory1 in bics)
            {
                var bic = (int)builtInCategory1;
                var builtInCategory = (BuiltInCategory)bic;
                elementFilterList.Add(new ElementCategoryFilter(builtInCategory));
            }
            var logicalOrFilter = new LogicalOrFilter(elementFilterList);
            var elementCollector = new FilteredElementCollector(doc, v.Id);
            elementCollector.WherePasses(logicalOrFilter).WhereElementIsNotElementType();
            return elementCollector.ToList();
        }

        public static List<Element> GetElementsFromReferences(this Document doc, IList<Reference> references)
        {
            var eles = new List<Element>();
            foreach (var rf in references)
            {
                var ele = doc.GetElement(rf);
                if (ele != null) eles.Add(ele);
            }
            return eles;
        }

        public static List<Element> GetFromModelByBICs(Document doc, BuiltInCategory[] bics)
        {
            IList<ElementFilter> elementFilterList = new List<ElementFilter>(((IEnumerable<BuiltInCategory>)bics).Count<BuiltInCategory>());
            foreach (int bic in bics)
            {
                BuiltInCategory builtInCategory = (BuiltInCategory)bic;
                elementFilterList.Add(new ElementCategoryFilter(builtInCategory));
            }
            LogicalOrFilter logicalOrFilter = new LogicalOrFilter(elementFilterList);
            FilteredElementCollector elementCollector = new FilteredElementCollector(doc);
            elementCollector.WherePasses(logicalOrFilter).WhereElementIsNotElementType();
            return ((IEnumerable<Element>)elementCollector).ToList<Element>();
        }



        public static List<Element> GetSelectedElements(this Document doc, ICollection<ElementId> eIds)
        {
            var eles = new List<Element>();
            foreach (ElementId eId in (IEnumerable<ElementId>)eIds)
                eles.Add(doc.GetElement(eId));
            return eles;
        }



    }
}
