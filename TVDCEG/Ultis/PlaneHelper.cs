using Autodesk.Revit.DB;
using System;

namespace TVDCEG.LBR
{
    internal static class PlaneHelper
    {
        public static double SignedDistanceTo(this Plane plane, XYZ p)
        {
            XYZ source = p - plane.Origin;
            return Math.Abs(plane.Normal.DotProduct(source));
        }
        //----------------------------------------------------
        public static XYZ ProjectOnto(this Plane plane, XYZ p)
        {
            XYZ source = p - plane.Origin;

            double d = plane.Normal.DotProduct(source);

            XYZ q = p - d * plane.Normal;

            return q;
        }
        public static XYZ ProjectOnto(this XYZ p, Plane plane)
        {

            XYZ source = p - plane.Origin;

            double d = plane.Normal.DotProduct(source);

            XYZ q = p - d * plane.Normal;

            return q;
        }
        //----------------------------------------------------
        public static bool IsPointOnPlane(this Plane plane, XYZ point)
        {
            return Math.Abs(plane.SignedDistanceTo(point)) < 0.0001;
        }
        //----------------------------------------------------
        public static Line ProjectLineOnPlane(this Plane plane, Line line)
        {
            Line l = null;
            var p0 = line.GetEndPoint(0);
            var p1 = line.GetEndPoint(1);
            p0 = p0.ProjectOnto(plane);
            p1 = p1.ProjectOnto(plane);
            l = Line.CreateBound(p0, p1);
            return l;
        }

        //----------------------------------------------------

        //----------------------------------------------------

        //----------------------------------------------------

        //----------------------------------------------------

        //----------------------------------------------------

        //----------------------------------------------------



    }
}
