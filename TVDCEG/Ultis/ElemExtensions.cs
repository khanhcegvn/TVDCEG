using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;

namespace TVDCEG.LBR
{
    internal static class ElemExtensions
    {
        public static int GetCategoryIndex(this Element elem)
        {
            if (elem != null && elem.Category != null)
            {
                return elem.Category.GetHashCode();
            }
            var val = elem as Family;
            if (val.FamilyCategory != null)
            {
                return val.FamilyCategory.GetHashCode();
            }
            return 0;
        }

        public static List<ElementId> ToElementIds(this List<Element> elems)
        {
            List<ElementId> list = new List<ElementId>();
            foreach (Element elem in elems)
            {
                if (!list.Contains(elem.Id))
                {
                    list.Add(elem.Id);
                }
            }
            return list;
        }

        [Obsolete]
        public static void SetColor(this Element elem, View view, byte r, byte g, byte b)
        {
            try
            {
                Color val = new Color(r, g, b);
                OverrideGraphicSettings val2 = new OverrideGraphicSettings();
                val2.SetProjectionLineColor(val);
                val2.SetProjectionFillColor(val);
                view.SetElementOverrides(elem.Id, val2);
            }
            catch
            {
            }
        }

        public static bool IsUnreferenced(this Element element)
        {
            return element.Location == null;
        }

        public static List<GeometryObject> GetObjects(this GeometryElement ge)
        {
            List<GeometryObject> list = new List<GeometryObject>();
            if (ge != null)
            {
                IEnumerator<GeometryObject> enumerator = ge.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    GeometryObject current = enumerator.Current;
                    if (current != null)
                    {
                        list.Add(current);
                    }
                }
            }
            return list;
        }


        public static ParameterType GetParameterType(this string ParameterTypeName)
        {
            ParameterType result = 0;
            Enum.TryParse(ParameterTypeName, out result);
            return result;
        }

        public static BuiltInCategory ToBuiltinCategory(this Category cat)
        {
            BuiltInCategory result = BuiltInCategory.INVALID;
            try
            {
                result = (BuiltInCategory)Enum.Parse(typeof(BuiltInCategory), ((object)cat.Id).ToString());
                return result;
            }
            catch
            {
                return result;
            }
        }

        public static Category ToCategory(this BuiltInCategory bic, UIDocument uidoc)
        {
            foreach (Category category in uidoc.Document.Settings.Categories)
            {
                Category val = category;
                if (val.Id.GetHashCode() == bic.GetHashCode())
                {
                    return val;
                }
            }
            Category result = null;
            try
            {
                result = uidoc.Document.Settings.Categories
                    .get_Item(bic);
                return result;
            }
            catch (Exception)
            {
                return result;
            }
        }

        public static Category ToCategory(this ElementId id, UIDocument uidoc)
        {
            foreach (Category category in uidoc.Document.Settings.Categories)
            {
                Category val = category;
                if (val.Id.GetHashCode() == id.GetHashCode())
                {
                    return val;
                }
            }
            return null;
        }

        public static LevelType GetLevelType(this Level pLevel)
        {
            ElementId typeId = pLevel.GetTypeId();
            return pLevel.Document.GetElement(typeId) as LevelType;
        }


        public static XYZ GetStartPoint(this Curve curve)
        {
            return curve.Tessellate()[0];
        }

        public static XYZ GetLastPoint(this Curve curve)
        {
            IList<XYZ> list = curve.Tessellate();
            return list[list.Count - 1];
        }

        public static Curve GetSegmentCurve(this BoundarySegment s)
        {
            return s.GetCurve();
        }

        public static List<Element> GetFamilySymbols(this Family family)
        {
            List<Element> list = new List<Element>();
            foreach (ElementId familySymbolId in family.GetFamilySymbolIds())
            {
                Element elementFromDocument = family.Document.GetElement(familySymbolId);
                if (elementFromDocument != null)
                {
                    list.Add(elementFromDocument);
                }
            }
            return list;
        }
    }
}
