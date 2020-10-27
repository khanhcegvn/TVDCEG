using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class CurveHelper
    {
        public static XYZ IntersectionPoint(this Curve c1, Curve c2)
        {
            Autodesk.Revit.DB.XYZ endPoint = c1.GetEndPoint(0);
            Autodesk.Revit.DB.XYZ endPoint2 = c1.GetEndPoint(1);
            Autodesk.Revit.DB.XYZ endPoint3 = c2.GetEndPoint(0);
            Autodesk.Revit.DB.XYZ endPoint4 = c2.GetEndPoint(1);
            Autodesk.Revit.DB.XYZ xYZ = endPoint2 - endPoint;
            Autodesk.Revit.DB.XYZ xYZ2 = endPoint4 - endPoint3;
            Autodesk.Revit.DB.XYZ xYZ3 = endPoint3 - endPoint;
            Autodesk.Revit.DB.XYZ result = null;
            double num = (xYZ2.X * xYZ3.Y - xYZ2.Y * xYZ3.X) / (xYZ2.X * xYZ.Y - xYZ2.Y * xYZ.X);
            if (!double.IsInfinity(num))
            {
                double x = endPoint.X + num * xYZ.X;
                double y = endPoint.Y + num * xYZ.Y;
                result = new Autodesk.Revit.DB.XYZ(x, y, 0.0);
            }
            return result;
        }

        public static XYZ MidPoint(this Curve curve)
        {
            var p1 = curve.GetStartPoint();
            var p2 = curve.GetLastPoint();

            return (p1 + p2) / 2;
        }
        public static bool OnPlane(this Curve curve, Plane plane)
        {
            UV uuv = new UV();
            double i1;
            plane.Project(curve.GetEndPoint(0), out uuv, out i1);
            double i2;
            plane.Project(curve.GetEndPoint(1), out uuv, out i2);
            if (i1 == 0 && i2 == 0) return true;
            else return false;
        }
        public static Line Linemax(this List<Line> list)
        {
            Line max = list.First();
            for (int i = 0; i < list.Count; i++)
            {
                if(max.Length<list[i].Length)
                {
                    max = list[i];
                }
            }
            return max;
        }
    }
}
