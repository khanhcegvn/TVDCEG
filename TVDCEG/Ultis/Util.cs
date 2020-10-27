using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class Util
    {
        private const double _eps = 1E-09;

        private const double _minimumSlope = 0.3;

        private const double _convertFootToMm = 304.79999999999995;

        private const double _convertFootToMeter = 0.30479999999999996;

        private const double _convertCubicFootToCubicMeter = 0.02831684659199999;

        private const double _convertSquareFootToSquareMeter = 0.092903039999999978;

        private const double kPi = 3.1415926535897931;

        public static double Eps => 1E-09;

        public static double MinLineLength => 1E-09;

        public static double TolPointOnPlane => 1E-09;

        public static bool IsZero(double a, double tolerance)
        {
            return tolerance > Math.Abs(a);
        }

        public static bool IsZero(double a)
        {
            return IsZero(a, 1E-09);
        }

        public static bool IsEqual(double a, double b)
        {
            return IsZero(b - a);
        }

        public static int Compare(double a, double b)
        {
            return (!IsEqual(a, b)) ? ((!(a < b)) ? 1 : (-1)) : 0;
        }

        public static int Compare(XYZ p, XYZ q)
        {
            int num = Compare(p.X, q.X);
            if (num == 0)
            {
                num = Compare(p.Y, q.Y);
                if (num == 0)
                {
                    num = Compare(p.Z, q.Z);
                }
            }
            return num;
        }
        public static bool Nguochuong(XYZ p, XYZ q)
        {
            var dodaip = Math.Sqrt(Math.Pow(p.X, 2) + Math.Pow(p.Y, 2) + Math.Pow(p.Z, 2));
            var dodaiq = Math.Sqrt(Math.Pow(q.X, 2) + Math.Pow(q.Y, 2) + Math.Pow(q.Z, 2));
            if (p.CrossProduct(q).IsZeroLength() && p.DotProduct(q) / (dodaip * dodaiq) == -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static int Compare(Plane a, Plane b)
        {
            int num = Compare(a.Normal, b.Normal);
            if (num == 0)
            {
                num = Compare(a.SignedDistanceTo(XYZ.Zero), b.SignedDistanceTo(XYZ.Zero));
                if (num == 0)
                {
                    num = Compare(a.XVec.AngleOnPlaneTo(b.XVec, b.Normal), 0.0);
                }
            }
            return num;
        }

        public static bool IsEqual(XYZ p, XYZ q)
        {
            return Compare(p, q) == 0;
        }

        public static bool BoundingBoxXyzContains(BoundingBoxXYZ bb, XYZ p)
        {
            return 0 < Compare(bb.Min, p) && 0 < Compare(p, bb.Max);
        }

        public static bool IsPerpendicular(XYZ v, XYZ w)
        {
            double length = v.GetLength();
            double length2 = v.GetLength();
            double num = Math.Abs(v.DotProduct(w));
            return 1E-09 < length && 1E-09 < length2 && 1E-09 > num;
        }

        public static bool IsParallel(XYZ p, XYZ q)
        {
            return p.CrossProduct(q).IsZeroLength();
        }

        public static bool IsHorizontal(XYZ v)
        {
            return IsZero(v.Z);
        }

        public static bool IsHorizontal(Edge e)
        {
            XYZ right = e.Evaluate(0.0);
            XYZ left = e.Evaluate(1.0);
            return IsHorizontal(left - right);
        }

        public static bool IsHorizontal(PlanarFace f)
        {
            return IsVertical(f.FaceNormal);
        }

        public static bool IsVertical(XYZ v)
        {
            return IsZero(v.X) && IsZero(v.Y);
        }

        public static bool IsVertical(XYZ v, double tolerance)
        {
            return IsZero(v.X, tolerance) && IsZero(v.Y, tolerance);
        }

        public static bool IsVertical(PlanarFace f)
        {
            return IsHorizontal(f.FaceNormal);
        }

        public static bool IsVertical(CylindricalFace f)
        {
            return IsVertical(f.Axis);
        }

        public static bool PointsUpwards(XYZ v)
        {
            double num = v.X * v.X + v.Y * v.Y;
            double num2 = v.Z * v.Z;
            return 0.0 < v.Z && 0.3 < num2 / num;
        }

        public static double Max(double[] a)
        {
            Debug.Assert(1 == a.Rank, "expected one-dimensional array");
            Debug.Assert(a.GetLowerBound(0) == 0, "expected zero-based array");
            Debug.Assert(0 < a.GetUpperBound(0), "expected non-empty array");
            double num = a[0];
            for (int i = 1; i <= a.GetUpperBound(0); i++)
            {
                if (num < a[i])
                {
                    num = a[i];
                }
            }
            return num;
        }

        public static double MmToFoot(double mm)
        {
            return mm / 304.79999999999995;
        }

        public static double MeterToFoot(double metter)
        {
            return metter / 0.30479999999999996;
        }

        public static double FootToMm(double feet)
        {
            return feet * 304.79999999999995;
        }

        public static double CubicFootToCubicMeter(double cubicFoot)
        {
            return cubicFoot * 0.02831684659199999;
        }

        public static double SquareFootToSquareMeter(double squareFoot)
        {
            return squareFoot * 0.092903039999999978;
        }

        public static double RadiansToDegrees(double rads)
        {
            return rads * 57.295779513082323;
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * 0.017453292519943295;
        }

        public static XYZ Midpoint(XYZ p, XYZ q)
        {
            return 0.5 * (p + q);
        }

        public static XYZ Midpoint(Line line)
        {
            return Midpoint(line.GetEndPoint(0), line.GetEndPoint(1));
        }

        public static XYZ Normal(Line line)
        {
            XYZ endPoint = line.GetEndPoint(0);
            XYZ endPoint2 = line.GetEndPoint(1);
            XYZ xYZ = endPoint2 - endPoint;
            return xYZ.CrossProduct(XYZ.BasisZ).Normalize();
        }

        public static List<XYZ> GetBottomCorners(BoundingBoxXYZ b)
        {
            double z = b.Min.Z;
            return new List<XYZ>
        {
            new XYZ(b.Min.X, b.Min.Y, z),
            new XYZ(b.Max.X, b.Min.Y, z),
            new XYZ(b.Max.X, b.Max.Y, z),
            new XYZ(b.Min.X, b.Max.Y, z)
        };
        }

        public static XYZ Intersection(Curve c1, Curve c2)
        {
            XYZ endPoint = c1.GetEndPoint(0);
            XYZ endPoint2 = c1.GetEndPoint(1);
            XYZ endPoint3 = c2.GetEndPoint(0);
            XYZ endPoint4 = c2.GetEndPoint(1);
            XYZ xYZ = endPoint2 - endPoint;
            XYZ xYZ2 = endPoint4 - endPoint3;
            XYZ xYZ3 = endPoint3 - endPoint;
            XYZ result = null;
            double num = (xYZ2.X * xYZ3.Y - xYZ2.Y * xYZ3.X) / (xYZ2.X * xYZ.Y - xYZ2.Y * xYZ.X);
            if (!double.IsInfinity(num))
            {
                double x = endPoint.X + num * xYZ.X;
                double y = endPoint.Y + num * xYZ.Y;
                result = new XYZ(x, y, 0.0);
            }
            return result;
        }

        public static Solid CreateSphereAt(XYZ centre, double radius)
        {
            Frame coordinateFrame = new Frame(centre, XYZ.BasisX, XYZ.BasisY, XYZ.BasisZ);
            Arc arc = Arc.Create(centre - radius * XYZ.BasisZ, centre + radius * XYZ.BasisZ, centre + radius * XYZ.BasisX);
            Line curve = Line.CreateBound(arc.GetEndPoint(1), arc.GetEndPoint(0));
            CurveLoop curveLoop = new CurveLoop();
            curveLoop.Append(arc);
            curveLoop.Append(curve);
            List<CurveLoop> list = new List<CurveLoop>(1);
            list.Add(curveLoop);
            return GeometryCreationUtilities.CreateRevolvedGeometry(coordinateFrame, list, 0.0, 6.2831853071795862);
        }

        public static Solid CreateCube(double d)
        {
            return CreateRectangularPrism(XYZ.Zero, d, d, d);
        }

        public static Solid CreateRectangularPrism(XYZ center, double d1, double d2, double d3)
        {
            List<Curve> list = new List<Curve>();
            XYZ xYZ = new XYZ((0.0 - d1) / 2.0, (0.0 - d2) / 2.0, (0.0 - d3) / 2.0);
            XYZ xYZ2 = new XYZ((0.0 - d1) / 2.0, d2 / 2.0, (0.0 - d3) / 2.0);
            XYZ xYZ3 = new XYZ(d1 / 2.0, d2 / 2.0, (0.0 - d3) / 2.0);
            XYZ xYZ4 = new XYZ(d1 / 2.0, (0.0 - d2) / 2.0, (0.0 - d3) / 2.0);
            list.Add(Line.CreateBound(xYZ, xYZ2));
            list.Add(Line.CreateBound(xYZ2, xYZ3));
            list.Add(Line.CreateBound(xYZ3, xYZ4));
            list.Add(Line.CreateBound(xYZ4, xYZ));
            CurveLoop curveLoop = CurveLoop.Create(list);
            SolidOptions solidOptions = new SolidOptions(ElementId.InvalidElementId, ElementId.InvalidElementId);
            return GeometryCreationUtilities.CreateExtrusionGeometry(new CurveLoop[1]
            {
            curveLoop
            }, XYZ.BasisZ, d3, solidOptions);
        }

        public static Solid CreateSolidFromBoundingBox(Solid inputSolid)
        {
            BoundingBoxXYZ boundingBox = inputSolid.GetBoundingBox();
            XYZ xYZ = new XYZ(boundingBox.Min.X, boundingBox.Min.Y, boundingBox.Min.Z);
            XYZ xYZ2 = new XYZ(boundingBox.Max.X, boundingBox.Min.Y, boundingBox.Min.Z);
            XYZ xYZ3 = new XYZ(boundingBox.Max.X, boundingBox.Max.Y, boundingBox.Min.Z);
            XYZ xYZ4 = new XYZ(boundingBox.Min.X, boundingBox.Max.Y, boundingBox.Min.Z);
            Line item = Line.CreateBound(xYZ, xYZ2);
            Line item2 = Line.CreateBound(xYZ2, xYZ3);
            Line item3 = Line.CreateBound(xYZ3, xYZ4);
            Line item4 = Line.CreateBound(xYZ4, xYZ);
            List<Curve> list = new List<Curve>();
            list.Add(item);
            list.Add(item2);
            list.Add(item3);
            list.Add(item4);
            double extrusionDist = boundingBox.Max.Z - boundingBox.Min.Z;
            CurveLoop item5 = CurveLoop.Create(list);
            List<CurveLoop> list2 = new List<CurveLoop>();
            list2.Add(item5);
            Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(list2, XYZ.BasisZ, extrusionDist);
            return SolidUtils.CreateTransformed(solid, boundingBox.Transform);
        }

        public static XYZ Greater(XYZ pt1, XYZ pt2, XYZ axis)
        {
            XYZ result = new XYZ();
            if (axis.Equals(XYZ.BasisX))
            {
                result = ((!(pt1.X > pt2.X)) ? pt2 : pt1);
            }
            if (axis.Equals(XYZ.BasisY))
            {
                result = ((!(pt1.Y > pt2.Y)) ? pt2 : pt1);
            }
            if (axis.Equals(XYZ.BasisZ))
            {
                result = ((!(pt1.Z > pt2.Z)) ? pt2 : pt1);
            }
            return result;
        }

        public static XYZ GetClosestPt(XYZ pt, List<XYZ> pts)
        {
            XYZ xYZ = new XYZ();
            double num = 0.0;
            foreach (XYZ pt2 in pts)
            {
                if (!pt.Equals(pt2))
                {
                    double num2 = Math.Sqrt(Math.Pow(pt.X - pt2.X, 2.0) + Math.Pow(pt.Y - pt2.Y, 2.0) + Math.Pow(pt.Z - pt2.Z, 2.0));
                    if (xYZ.IsZeroLength())
                    {
                        num = num2;
                        xYZ = pt2;
                    }
                    else if (num2 < num)
                    {
                        num = num2;
                        xYZ = pt2;
                    }
                }
            }
            return xYZ;
        }
        private static string CleanFileName(this string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
