using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TVDCEG.LBR
{
    public static class XYZHelper
    {
        private const double PrecisionComparison = 1E-06;
        public static bool IsPerpendicular(this XYZ v, XYZ w)
        {
            return 1E-09 < v.GetLength() && 1E-09 < v.GetLength() && 1E-09 > Math.Abs(v.DotProduct(w));
        }
        public static bool IsParallel(this XYZ p, XYZ q)
        {
            return p.CrossProduct(q).IsZeroLength();
        }
        public static int CompareTwoPoint(XYZ first, XYZ second)
        {
            bool flag = MathLib.IsEqual(first.Z, second.Z, 0.0001);
            int result;
            if (flag)
            {
                bool flag2 = MathLib.IsEqual(first.Y, second.Y, 0.0001);
                if (flag2)
                {
                    bool flag3 = MathLib.IsEqual(first.X, second.X, 0.0001);
                    if (flag3)
                    {
                        result = 0;
                    }
                    else
                    {
                        result = ((first.X > second.X) ? 1 : -1);
                    }
                }
                else
                {
                    result = ((first.Y > second.Y) ? 1 : -1);
                }
            }
            else
            {
                result = ((first.Z > second.Z) ? 1 : -1);
            }
            return result;
        }
        public static bool Cungphuong(this XYZ p, XYZ q)
        {
            var dodaip = Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) + Math.Pow(p.Z, 2));
            var dodaiq = Math.Sqrt(Math.Pow(q.X, 2) + Math.Pow(q.Y, 2) + Math.Pow(q.Z, 2));
            if (Math.Abs(p.DotProduct(q) / (dodaip * dodaiq)) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static XYZ AbsVector(XYZ vector)
        {
            XYZ result = vector;
            XYZ first = new XYZ();
            bool flag = XYZHelper.CompareTwoPoint(first, vector) < 0;
            if (flag)
            {
                result = vector;
            }
            bool flag2 = XYZHelper.CompareTwoPoint(first, vector) > 0;
            if (flag2)
            {
                result = -vector;
            }
            return result;
        }
        public static bool IsVertical(this XYZ v, double tolerance)
        {
            if (Util.IsZero(v.X, tolerance))
                return Util.IsZero(v.Y, tolerance);
            return false;
        }
        public static XYZ GetClosestPoint(this XYZ pt, List<XYZ> pts)
        {
            XYZ xyz = new XYZ();
            double num1 = 0.0;
            foreach (XYZ pt1 in pts)
            {
                if (!pt.Equals((object)pt1))
                {
                    double num2 = Math.Sqrt(Math.Pow(pt.X - pt1.X, 2.0) + Math.Pow(pt.Y - pt1.Y, 2.0) + Math.Pow(pt.Z - pt1.Z, 2.0));
                    if (xyz.IsZeroLength())
                    {
                        num1 = num2;
                        xyz = pt1;
                    }
                    else if (num2 < num1)
                    {
                        num1 = num2;
                        xyz = pt1;
                    }
                }
            }
            return xyz;
        }
        public static bool Equalpoint(this XYZ a, XYZ b)
        {
            bool fag = false;
            if (Math.Round(a.X, 3) == Math.Round(b.X, 3) && Math.Round(a.Y, 3) == Math.Round(b.Y, 3) && Math.Round(a.Z, 3) == Math.Round(b.Z, 3))
            {
                fag = true;
            }
            return fag;
        }
        public static bool Iscontains(this XYZ point, List<XYZ> listPoint)
        {
            bool result = false;
            foreach (XYZ item in listPoint)
            {
                if (item.IsAlmostEqualTo(point))
                {
                    result = true;
                }
            }
            return result;
        }

        public static bool IsCodirectionalTo(this XYZ vecThis, XYZ vecTo)
        {
            if (vecTo == null)
            {
                throw new System.ArgumentNullException();
            }
            return Math.Abs(1.0 - vecThis.Normalize().DotProduct(vecTo.Normalize())) < 1E-06;
        }

        public static double DistanceToWithoutZCoord(this XYZ source, XYZ target)
        {
            return new XYZ(source.X, source.Y, 0.0).DistanceTo(new XYZ(target.X, target.Y, 0.0));
        }

        public static XYZ Max(this XYZ source, XYZ target)
        {
            return new XYZ(Math.Max(source.X, target.X), Math.Max(source.Y, target.Y), Math.Max(source.Z, target.Z));
        }

        public static XYZ Min(this XYZ source, XYZ target)
        {
            return new XYZ(Math.Min(source.X, target.X), Math.Min(source.Y, target.Y), Math.Min(source.Z, target.Z));
        }

        public static bool IsOppositeDirectionTo(this XYZ vecThis, XYZ vecTo)
        {
            return MathHelper.IsEqual(-1.0, vecThis.Normalize().DotProduct(vecTo.Normalize()));
        }

        public static bool IsOrthogonalTo(this XYZ vecThis, XYZ vecTo)
        {
            return MathHelper.IsEqual(0.0, vecThis.Normalize().DotProduct(vecTo.Normalize()));
        }

        public static bool IsHorizontal(this XYZ vecThis)
        {
            return vecThis.IsPerpendicular(XYZ.BasisY);
        }

        public static bool IsHorizontal(this XYZ vecThis, View view)
        {
            return vecThis.IsPerpendicular(view.UpDirection);
        }

        public static bool IsVertical(this XYZ vecThis)
        {
            return vecThis.IsPerpendicular(XYZ.BasisX);
        }
        public static bool IsVertical(this XYZ vecThis, View view)
        {
            return vecThis.IsPerpendicular(view.RightDirection);
        }

        public static XYZ GetElementCenter(this Element element)
        {
            BoundingBoxXYZ boundingBoxXYZ = element.get_BoundingBox((View)null);
            XYZ xYZ = boundingBoxXYZ.Max - boundingBoxXYZ.Min;
            return new XYZ(boundingBoxXYZ.Min.X + xYZ.X / 2.0, boundingBoxXYZ.Min.Y + xYZ.Y / 2.0, boundingBoxXYZ.Min.Z + xYZ.Z / 2.0);
        }

        public static XYZ GetElementDirection(this Element element)
        {
            XYZ result = XYZ.Zero;
            if (element.Category.Id.IntegerValue != -2000032 && element.Category.Id.IntegerValue != -2001300)
            {
                LocationCurve locationCurve = element.Location as LocationCurve;
                result = ((locationCurve != null) ? (locationCurve.Curve as Line).Direction : XYZ.BasisZ);
            }
            return result;
        }

        public static XYZ GetCoordinateOfProjectBasePointInInternalCoordinate(Document doc)
        {
            return new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_ProjectBasePoint).ToElements().FirstOrDefault()?.get_BoundingBox(doc.ActiveView).Max;
        }

        public static XYZ GetCoordinateOfSurveyPointInInternalCoordinate(Document doc)
        {
            return new FilteredElementCollector(doc, doc.ActiveView.Id).OfCategory(BuiltInCategory.OST_SharedBasePoint).ToElements().FirstOrDefault()?.get_BoundingBox(doc.ActiveView).Max;
        }

        public static XYZ TransformCoordinateFromInternalCoordinateToSurveyCoordinate(Document doc, XYZ pointToTransform)
        {
            XYZ coordinateOfSurveyPointInInternalCoordinate = GetCoordinateOfSurveyPointInInternalCoordinate(doc);
            return (coordinateOfSurveyPointInInternalCoordinate == null) ? null : pointToTransform.Subtract(coordinateOfSurveyPointInInternalCoordinate);
        }

        public static XYZ TransformCoordinateFromInternalCoordinateToBasePointCoordinate(Document doc, XYZ pointToTransform)
        {
            XYZ coordinateOfProjectBasePointInInternalCoordinate = GetCoordinateOfProjectBasePointInInternalCoordinate(doc);
            return (coordinateOfProjectBasePointInInternalCoordinate == null) ? null : pointToTransform.Subtract(coordinateOfProjectBasePointInInternalCoordinate);
        }

        public static XYZ ConvertCoordinateFromAPIUnitToMillimeterUnit(XYZ pointToConvert)
        {
            double x = UnitUtils.Convert(pointToConvert.X, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
            double y = UnitUtils.Convert(pointToConvert.Y, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
            double z = UnitUtils.Convert(pointToConvert.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
            return new XYZ(x, y, z);
        }

        public static XYZ ConvertCoordinateFromAPIUnitToCentimeterUnit(XYZ pointToConvert)
        {
            double x = UnitUtils.Convert(pointToConvert.X, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_CENTIMETERS);
            double y = UnitUtils.Convert(pointToConvert.Y, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_CENTIMETERS);
            double z = UnitUtils.Convert(pointToConvert.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_CENTIMETERS);
            return new XYZ(x, y, z);
        }

        public static XYZ ConvertCoordinateFromAPIUnitToMeterUnit(XYZ pointToConvert)
        {
            double x = UnitUtils.Convert(pointToConvert.X, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);
            double y = UnitUtils.Convert(pointToConvert.Y, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);
            double z = UnitUtils.Convert(pointToConvert.Z, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_METERS);
            return new XYZ(x, y, z);
        }
        public static XYZ RotateRadians(this XYZ v, double radians)
        {
            var ca = Math.Cos(radians);
            var sa = Math.Sin(radians);
            return new XYZ(ca * v.X - sa * v.Y, sa * v.X + ca * v.Y, v.Z);
        }
        public static XYZ RotateDegree(this XYZ v, double degrees)
        {
            return v.RotateRadians(degrees.D2R());
        }
    }
}
