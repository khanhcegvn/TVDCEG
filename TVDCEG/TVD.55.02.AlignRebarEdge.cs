using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Shapes;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Analysis;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Newtonsoft.Json;
using TVDCEG.Extension;
using TVDCEG.LBR;
using TVDCEG.Ultis;
using Line = Autodesk.Revit.DB.Line;

namespace TVDCEG
{
    public class AlignRebarEdge : ConstructorSingleton<AlignRebarEdge>
    {
        public void SetValue(Document doc, Autodesk.Revit.DB.Line targetx, FamilyInstance familyInstance, string space)
        {
            PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
            string val1 = string.Empty;

            double kspace = UnitConvert.StringToFeetAndInches(space, out val1);
            Transform transform = familyInstance.GetTransform();
            Line target = Line.CreateBound(pLane3D.ProjectPointOnPlane(targetx.GetStartPoint()), pLane3D.ProjectPointOnPlane(targetx.GetLastPoint()));
            var list = familyInstance.LinesGeometry(doc);
            Line maxp = list.Linemax();
            Line max = Line.CreateBound(transform.OfPoint(maxp.GetStartPoint()), transform.OfPoint(maxp.GetLastPoint()));
            Line lineonplane = Line.CreateBound(pLane3D.ProjectPointOnPlane(max.GetStartPoint()), pLane3D.ProjectPointOnPlane(max.GetLastPoint()));
            XYZ pointintersect = FindIntersectwoline(target, lineonplane);
            Parameter dim_length = familyInstance.LookupParameter("DIM_LENGTH");
            double value = dim_length.AsDouble();
            //XYZ start = pLane3D.ProjectPointOnPlane(max.GetStartPoint());
            //XYZ end = pLane3D.ProjectPointOnPlane(max.GetLastPoint());
            XYZ start = lineonplane.GetStartPoint();
            XYZ end = lineonplane.GetLastPoint();
            double kc1 = target.Distance(pLane3D.ProjectPointOnPlane(start));
            double kc2 = target.Distance(pLane3D.ProjectPointOnPlane(end));

            //Line hg = Line.CreateBound(pointintersect, end);
            //SketchPlane skt = SketchPlane.Create(doc, pLane3D.GetPlane);
            //var ty = doc.Create.NewModelCurve(lineonplane, skt);
            //doc.Create.NewModelCurve(target, skt);
            //doc.Create.NewModelCurve(hg, skt);
            //doc.ActiveView.SketchPlane = skt;
            Line targetXX = Line.CreateBound(target.Origin + 10000.0 * target.Direction, target.Origin - 10000.0 * target.Direction);
            if (kc1 > kc2)
            {
                double length1 = (pointintersect - start).GetLength();
                if (value / 2 < length1)
                {
                    if (length1 < value)
                    {
                        double kc = targetXX.Distance(end);
                        //dim_length.Set(value - kc - kspace);
                        double setk = (value - kc - kspace);
                        string sou1 = setk.DoubleRoundFraction(8);
                        string k;
                        double h = UnitConvert.StringToFeetAndInches(sou1, out k);
                        dim_length.Set(h);
                        XYZ direction = (end - start).Normalize() * (kc + kspace);
                        ElementTransformUtils.MoveElement(doc, familyInstance.Id, -direction);
                    }
                    else
                    {
                        double kc = targetXX.Distance(end);
                        //dim_length.Set(value + kc - kspace);
                        double setk = (value + kc - kspace);
                        string sou1 = setk.DoubleRoundFraction(8);
                        string k;
                        double h = UnitConvert.StringToFeetAndInches(sou1, out k);
                        dim_length.Set(h);
                        XYZ direction = (end - start).Normalize() * (kc - kspace);
                        ElementTransformUtils.MoveElement(doc, familyInstance.Id, direction);
                    }
                }
                else
                {
                    if (length1 < value)
                    {
                        double kc = targetXX.Distance(end);
                        //dim_length.Set(value - (kspace - kc));
                        double setk = (value - (kspace - kc));
                        string sou1 = setk.DoubleRoundFraction(8);
                        string k;
                        double h = UnitConvert.StringToFeetAndInches(sou1, out k);
                        dim_length.Set(h);
                        XYZ direction = (end - start).Normalize() * (kc - kspace);
                        ElementTransformUtils.MoveElement(doc, familyInstance.Id, direction);
                    }
                    else
                    {
                        double kc = targetXX.Distance(end);
                        dim_length.Set(value - (kspace - kc));
                        XYZ direction = (end - start).Normalize() * (kc + kspace);
                        ElementTransformUtils.MoveElement(doc, familyInstance.Id, -direction);
                    }
                }
            }
            else
            {
                double lengthk = (pointintersect - end).GetLength();
                if (lengthk < value)
                {
                    double kc = targetXX.Distance(pLane3D.ProjectPointOnPlane(start));
                    //dim_length.Set(value - kc - kspace);
                    double setk = (value - kc - kspace);
                    string sou1 = setk.DoubleRoundFraction(8);
                    string k;
                    double h = UnitConvert.StringToFeetAndInches(sou1, out k);
                    dim_length.Set(h);
                }
                else
                {
                    //double kc = targetXX.Distance(pLane3D.ProjectPointOnPlane(start));
                    //dim_length.Set(value + kc - kspace);
                    double kc = targetXX.Distance(pLane3D.ProjectPointOnPlane(start));
                    var setk = (value + kc - kspace);
                    string sou1 = setk.DoubleRoundFraction(8);
                    string k;
                    double h = UnitConvert.StringToFeetAndInches(sou1, out k);
                    dim_length.Set(h);
                }
            }
        }
        public XYZ FindIntersectwoline(Line line1, Line line2)
        {
            IntersectionResultArray resultArray;
            if (Line.CreateBound(line2.Origin + 10000.0 * line2.Direction, line2.Origin - 10000.0 * line2.Direction).Intersect((Curve)Line.CreateBound(line1.Origin + 10000.0 * line1.Direction, line1.Origin - 10000.0 * line1.Direction), out resultArray) != SetComparisonResult.Overlap)
                throw new InvalidOperationException("Input lines did not intersect.");
            if (resultArray == null || resultArray.Size != 1)
                throw new InvalidOperationException("Could not extract line intersection point.");
            return resultArray.get_Item(0).XYZPoint;
        }
       
    }

}
