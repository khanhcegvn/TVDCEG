using System;
using Autodesk.Revit.DB;

namespace TVDCEG.Ultis
{
    public class PLane3D
    {
        public XYZ Origin { get; }
        public XYZ FaceNormal { get; }
        public Plane GetPlane { get; }
        public PLane3D(XYZ origin, XYZ normal)
        {
            this.Origin = origin;
            this.FaceNormal = normal.Normalize();
            this.GetPlane = Plane.CreateByNormalAndOrigin(normal, origin); 
        }
        public double DistancepointtoPlane(XYZ xYZ)
        {
            XYZ xYZ1 = xYZ - Origin;
            return Math.Abs(xYZ1.DotProduct(FaceNormal) / FaceNormal.GetLength());
        }
        public XYZ ProjectVectorOnPlane(XYZ vector)
        {
            XYZ xyz = new XYZ();
            XYZ xyz2 = new XYZ();
            XYZ point = xyz2 + vector;
            XYZ right = this.ProjectPointOnPlane(xyz2);
            XYZ left = this.ProjectPointOnPlane(point);
            return left - right;
        }
        public double DistanceFromPointToPlane(XYZ point)
        {
            XYZ xyz = point - this.Origin;
            return Math.Abs(xyz.DotProduct(this.FaceNormal) / this.FaceNormal.GetLength());
        }
        public XYZ ProjectPointOnPlane(XYZ point)
        {
            XYZ xyz = null;
            double num = this.DistancepointtoPlane(point);
            bool flag = num < double.Epsilon;
            XYZ result;
            if (flag)
            {
                result = point;
            }
            else
            {
                XYZ xyz2 = point + num * this.FaceNormal;
                XYZ xyz3 = point - num * this.FaceNormal;
                XYZ xyz4 = xyz2 - this.Origin;
                XYZ xyz5 = xyz3 - this.Origin;
                double value = 0.0;
                double value2 = 0.0;
                bool flag2 = xyz4.GetLength() < 0.0001;
                if (flag2)
                {
                    xyz = xyz2;
                }
                else
                {
                    value = xyz4.DotProduct(this.FaceNormal) / xyz4.GetLength();
                }
                bool flag3 = xyz5.GetLength() < 0.0001;
                if (flag3)
                {
                    xyz = xyz3;
                }
                else
                {
                    value2 = xyz5.DotProduct(this.FaceNormal) / xyz5.GetLength();
                }
                bool flag4 = Math.Abs(value) < 1E-05;
                if (flag4)
                {
                    xyz = xyz2;
                }
                bool flag5 = Math.Abs(value2) < 1E-05;
                if (flag5)
                {
                    xyz = xyz3;
                }
                result = xyz;
            }
            return result;
        }
    }
}
