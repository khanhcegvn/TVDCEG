using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace TVDCEG.LBR
{
    public static class GeometryHelper
    {
        #region Geometry Element
        public static List<Line> LinesGeometry(this Element element, Document doc)
        {
            var lines = new List<Line>();
            Options option = new Options();
            if (element.Document.ActiveView != null)
            {
                option.View = element.Document.ActiveView;
                option.IncludeNonVisibleObjects = true;
            }
            var geoEle = element.get_Geometry(option);
            if (geoEle == null) return lines;
            foreach (GeometryObject geometryObject in geoEle)
            {
                GeometryInstance geometryInstance = geometryObject as GeometryInstance;
                bool flag2 = null != geometryInstance;
                if (flag2)
                {
                    GeometryElement symbolGeometry = geometryInstance.GetSymbolGeometry();
                    foreach (GeometryObject geometryObject2 in symbolGeometry)
                    {
                        Line line = geometryObject2 as Line;
                        bool flag3 = null == line;
                        if (!flag3)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }
            foreach (GeometryObject geometryObject in geoEle)
            {
                if (geometryObject is Line)
                {
                    lines.Add(geometryObject as Line);
                }
            }
            return lines;
        }
        public static List<Line> LinesGeometry(this FamilyInstance element, Document doc)
        {
            var lines = new List<Line>();
            Options option = new Options();
            if (element.Document.ActiveView != null)
            {
                option.View = element.Document.ActiveView;
                option.IncludeNonVisibleObjects = true;
            }
            var geoEle = element.get_Geometry(option);
            if (geoEle == null) return lines;
            foreach (GeometryObject geometryObject in geoEle)
            {
                GeometryInstance geometryInstance = geometryObject as GeometryInstance;
                bool flag2 = null != geometryInstance;
                if (flag2)
                {
                    GeometryElement symbolGeometry = geometryInstance.GetSymbolGeometry();
                    foreach (GeometryObject geometryObject2 in symbolGeometry)
                    {
                        Line line = geometryObject2 as Line;
                        bool flag3 = null == line;
                        if (!flag3)
                        {
                            lines.Add(line);
                        }
                    }
                }
            }
            foreach (GeometryObject geometryObject in geoEle)
            {
                if (geometryObject is Line)
                {
                    lines.Add(geometryObject as Line);
                }
            }
            return lines;
        }
        public static XYZ GetMidPointOfEdge(this Edge edge)
        {
            var curve = edge.AsCurve();
            return (curve.GetEndPoint(0) * 0.5 + curve.GetEndPoint(1) * 0.5);
        }
        public static XYZ GetCurveDirection(this Curve curve)
        {
            var e0 = curve.GetEndPoint(0);
            var e1 = curve.GetEndPoint(1);
            return (e0 - e1).Normalize();
        }
        public static bool IsSameCurve(this Curve c1, Curve c2)
        {
            var rs = false;
            var a = c1.GetEndPoint(0);
            var b = c1.GetEndPoint(1);
            var c = c2.GetEndPoint(0);
            var d = c2.GetEndPoint(1);

            if (a.IsAlmostEqualTo(c, 0.0001) && b.IsAlmostEqualTo(d, 0.0001))
            {
                rs = true;
            }
            if (a.IsAlmostEqualTo(d, 0.0001) && b.IsAlmostEqualTo(c, 0.0001))
            {
                rs = true;
            }
            return rs;
        }


        #endregion


    }
}
