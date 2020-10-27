using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG
{
    public class Insulationlbr
    {
        public static Insulationlbr _instance;
        private Insulationlbr()
        {

        }
        public static Insulationlbr Instance => _instance ?? (_instance = new Insulationlbr());
        public FamilyInstance GetWall(Document doc, AssemblyInstance assembly)
        {
            ICollection<ElementId> Memberid = assembly.GetMemberIds();
            FamilyInstance instance = null;
            foreach (ElementId i in Memberid)
            {
                FamilyInstance familyInstance = doc.GetElement(i) as FamilyInstance;
                if (familyInstance != null)
                {
                    string familyName = familyInstance.Symbol.Category.Name;
                    if (familyName.Equals("Structural Framing"))
                    {
                        instance = familyInstance;
                    }
                }
            }
            return instance;
        }
        public FamilyInstance Getinsulation(Document doc, AssemblyInstance assembly)
        {
            ICollection<ElementId> Memberid = assembly.GetMemberIds();
            FamilyInstance instance = null;
            List<FamilyInstance> listinstance = new List<FamilyInstance>();
            foreach (ElementId i in Memberid)
            {
                FamilyInstance familyInstance = doc.GetElement(i) as FamilyInstance;
                if (familyInstance != null)
                {
                    string familyName = familyInstance.Symbol.Category.Name;
                    if (familyName.Equals("Structural Framing"))
                    {
                        var gh = familyInstance.GetSubComponentIds();
                        foreach (var j in gh)
                        {
                            Element op = doc.GetElement(j);
                            if (op.Name.Contains("BEADBOARD_INSULATION"))
                            {
                                FamilyInstance KJ = op as FamilyInstance;
                                listinstance.Add(KJ);
                            }
                        }
                    }
                }
            }
            listinstance.OrderBy(x => x.LookupParameter("DIM_WIDTH").AsDouble()).ToList();
            instance = listinstance.First();
            return instance;
        }
        public PlanarFace GetFaceTopWall(Document doc, FamilyInstance familyInstance)
        {
            PlanarFace topFace = null;
            ElementtransformToCopy transfg = new ElementtransformToCopy();
            Transform transform = familyInstance.GetTransform();
            List<PlanarFace> faces = transfg.FlFaces(familyInstance);
            double area = double.MinValue;
            foreach (PlanarFace face in faces)
            {
                if (transform.OfVector(face.FaceNormal).CrossProduct(doc.ActiveView.ViewDirection).GetLength() < 0.001)
                {
                    double a = face.Area;
                    if (a > area)
                    {
                        topFace = face;
                        area = a;
                    }
                }
            }
            return topFace;
        }
        public View CreateDraftingView(Document doc)
        {
            View viewsoure = doc.ActiveView;
            View viewout = null;
            ElementId idt = viewsoure.AssociatedAssemblyInstanceId;
            Element ele = doc.GetElement(idt);
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            collector.OfClass(typeof(ViewFamilyType));
            ViewFamilyType viewFamilyType = collector.Cast<ViewFamilyType>().First(vft => vft.ViewFamily == ViewFamily.Drafting);
            using (Transaction t = new Transaction(doc, "Create view"))
            {
                t.Start();
                ViewDrafting drafting = ViewDrafting.Create(doc, viewFamilyType.Id);
                viewout = drafting as View;
                viewout.Name = ele.Name + " " + "Insulation Layout";
                viewout.Scale = 24;
                t.Commit();
            }
            return (viewout);
        }
        public List<ElementId> CreateDetailline(Document doc, FamilyInstance familyInstance, EdgeArrayArray edgeArrayArray)
        {
            List<ElementId> list = new List<ElementId>();
            Transform transform = familyInstance.GetTransform();
            foreach (EdgeArray loop in edgeArrayArray)
            {
                foreach (Edge edge in loop)
                {
                    using (Transaction tran = new Transaction(doc, "create line"))
                    {
                        tran.Start();
                        Curve curve = edge.AsCurve();
                        XYZ pointstart = curve.GetEndPoint(0);
                        XYZ pointend = curve.GetEndPoint(1);
                        Line line2 = Line.CreateBound(transform.OfPoint(pointstart), transform.OfPoint(pointend));
                        Line line = curve as Line;
                        XYZ point1 = line.Origin;
                        View view = doc.ActiveView;
                        DetailLine detailLine = doc.Create.NewDetailCurve(view, line2) as DetailLine;
                        list.Add(detailLine.Id);
                        tran.Commit();
                    }
                }
            }
            return list;
        }
        public List<ViewDrafting> Getdraftingonprj(Document doc)
        {
            List<ViewDrafting> m_ViewListStr = new List<ViewDrafting>();
            FilteredElementCollector val = new FilteredElementCollector(doc);
            IList<Element> list = val.OfClass(typeof(ViewDrafting)).ToElements();
            foreach (Element item in list)
            {
                View val2 = item as View;
                if (val2 != null && val2.CanBePrinted && (int)val2.ViewType != 6)
                {
                    m_ViewListStr.Add(val2 as ViewDrafting);
                }
            }
            return m_ViewListStr;
        }
        public Rectangleslbr DrawingFaceTop(Document doc, AssemblyInstance assemblyInstance)
        {
            FamilyInstance familyInstance = GetWall(doc, assemblyInstance);
            PlanarFace Topface = GetFaceTopWall(doc, familyInstance);
            List<XYZ> points = new List<XYZ>();
            List<CurveLoop> curveloop = new List<CurveLoop>();
            Transform transform = familyInstance.GetTransform();
            EdgeArrayArray list1 = Topface.EdgeLoops;
            foreach (EdgeArray edgeArray in list1)
            {
                foreach (Edge edge in edgeArray)
                {
                    Curve curve = edge.AsCurve();
                    XYZ point1 = curve.GetEndPoint(0);
                    XYZ point2 = curve.GetEndPoint(1);
                    points.Add(point1);
                    points.Add(point2);
                    Line line = Line.CreateBound(transform.OfPoint(point1), transform.OfPoint(point2));
                    using (Transaction t = new Transaction(doc, "Drawing Top Face"))
                    {
                        t.Start();
                        DetailLine detailLine = doc.Create.NewDetailCurve(doc.ActiveView, line) as DetailLine;
                        t.Commit();
                    }
                }
            }
            points = Removeduplicatepoint(points);
            IList<CurveLoop> loops = Topface.GetEdgesAsCurveLoops();
            foreach (var i in loops)
            {
                curveloop.Add(i);
            }
            List<Rectangleslbr> rec = Rectangleslbr.GetRectangles(doc, curveloop);
            Rectangleslbr rec1 = rec.First();
            return rec1;
        }
        //get all point of all detailline intersec
        public List<XYZ> GetIntersectXYZoncurve(Document doc, ICollection<ElementId> ids)
        {
            List<Line> lines = new List<Line>();
            List<XYZ> list3 = new List<XYZ>();
            foreach (ElementId i in ids)
            {
                DetailLine m = doc.GetElement(i) as DetailLine;
                Line gh = m.GeometryCurve as Line;
                lines.Add(gh);
            }
            List<XYZ> listpoint = new List<XYZ>();
            List<XYZ> newlistpoint = new List<XYZ>();
            List<XYZ> jk = new List<XYZ>();
            foreach (Line i in lines)
            {
                Curve curve = i as Curve;
                IntersectionResultArray resultArray;
                foreach (var a in lines)
                {
                    SetComparisonResult result = curve.Intersect(a as Curve, out resultArray);
                    if (result != SetComparisonResult.Overlap) continue;
                    if (resultArray == null || resultArray.Size != 1) continue;
                    IntersectionResult result1 = resultArray.get_Item(0);
                    XYZ pointcor = result1.XYZPoint;
                    XYZ pointadd = new XYZ(Math.Round(pointcor.X, 3), Math.Round(pointcor.Y, 3), Math.Round(pointcor.Z, 3));
                    listpoint.Add(pointadd);
                }
            }
            newlistpoint = listpoint.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z).ToList();
            list3 = Removeduplicatepoint(newlistpoint);
            return list3;
        }
        public List<XYZ> Removeduplicatepoint(List<XYZ> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = i - 1; j < i; j++)
                {
                    XYZ point1 = list[i];
                    XYZ point2 = list[j];
                    if (point1.X == point2.X & point1.Y == point2.Y && point1.Z == point2.Z)
                    {
                        list.RemoveAt(j);
                        i--;
                    }
                }
            }
            return list;
        }
        public bool Intersection(Line l, Line ll, out XYZ point)
        {
            point = XYZ.Zero;
            IntersectionResultArray resultArray;
            if (l.Intersect(ll, out resultArray) != SetComparisonResult.Overlap)
                return false;
            if (resultArray == null || resultArray.Size != 1)
                return false;
            point = resultArray.get_Item(0).XYZPoint;
            return true;
        }
        public List<Curve> Edgeintersection(Curve curve1, List<Curve> curves)
        {
            List<Curve> listout = new List<Curve>();
            IntersectionResultArray resultArray;
            foreach (Curve i in curves)
            {
                if (curve1.Intersect(i, out resultArray) != SetComparisonResult.Overlap) continue;
                if (resultArray == null || resultArray.Size != 1) continue;
                IntersectionResult result1 = resultArray.get_Item(0);
                if (result1 != null)
                {
                    listout.Add(i);
                }
            }
            return listout;
        }
        public void Checkintersectionline(Document doc, List<DetailLine> lines)
        {
            for (int i = 1; i < lines.Count; i++)
            {
                DetailLine dline1 = lines[i];
                DetailLine dline2 = lines[i - 1];
                Line line1 = dline1.GeometryCurve as Line;
                Line line2 = dline2.GeometryCurve as Line;
                if (line1.Origin.CrossProduct(line2.Origin).IsZeroLength())
                {
                    ElementTransformUtils.MoveElement(doc, dline2.Id, new XYZ(5, 0, 0));
                }
            }
        }
        public XYZ PointDistancePointMinZhightpoint(XYZ point, List<XYZ> list)
        {
            XYZ XPoint = null;
            double distancemin = 100;
            for (int i = 0; i < list.Count; i++)
            {
                XYZ point1 = list[i];
                double kc = point.DistanceTo(point1);
                if (kc > 0.001 && kc < distancemin)
                {
                    if (point1.Z > point.Z)
                    {
                        distancemin = kc;
                        XPoint = point1;
                    }
                }
            }
            return XPoint;
        }

        public ICollection<ElementId> Copyline(Document doc, ICollection<ElementId> ids)
        {
            ICollection<ElementId> sorces = new List<ElementId>();
            using (Transaction tran = new Transaction(doc, "copy"))
            {
                tran.Start();
                foreach (ElementId i in ids)
                {
                    var t = ElementTransformUtils.CopyElement(doc, i, new XYZ(0, 0, -30));
                    foreach (var g in t)
                    {
                        sorces.Add(g);
                    }
                }
                tran.Commit();
            }
            return sorces;
        }
        // get all annotion symbol
        public List<FamilySymbol> GetFamilySymbols(Document doc)
        {
            List<FamilySymbol> list = new List<FamilySymbol>();
            var families = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(Family)).ToElements().Cast<Family>().ToList();
            foreach (Family Instance in families)
            {
                ISet<ElementId> eleid = Instance.GetFamilySymbolIds();
                foreach (var i in eleid)
                {
                    Element elemtype = doc.GetElement(i);
                    FamilySymbol fasy = elemtype as FamilySymbol;
                    if (fasy.Family.FamilyCategory.Name != null && fasy.Family.FamilyCategory.Name.Contains("Generic Annotations"))
                    {
                        list.Add(fasy);
                    }
                }
            }
            return list;
        }
        public void PlaceSymmbolonPoint(Document doc, List<FamilySymbol> familySymbols, List<XYZ> xYZs)
        {
            FamilySymbol sym_ = null;
            foreach (var i in familySymbols)
            {
                if (i.Name.Contains("TIE COUNT"))
                {
                    sym_ = i;
                }
            }
            for (int i = 0; i < xYZs.Count; i++)
            {
                XYZ POINT = xYZs[i];
                Transaction tran = new Transaction(doc, "ss");
                tran.Start();
                var TagSym = doc.Create.NewFamilyInstance(POINT, sym_, doc.ActiveView);
                Parameter P1 = TagSym.LookupParameter("KEYNOTE");
                P1.Set(i.ToString());
                tran.Commit();
            }
        }
        public void PlaceSymmbolonRectangles(Document doc, List<FamilySymbol> familySymbols, List<Rectangleslbr> rectangles)
        {
            FamilySymbol sym_ = null;
            foreach (var i in familySymbols)
            {
                if (i.Name.Contains("TIE COUNT"))
                {
                    sym_ = i;
                }
            }
            //foreach(var T in xYZs)
            //{
            //    Transaction tran = new Transaction(doc, "ss");
            //    tran.Start();
            //    doc.Create.NewFamilyInstance(T, sym_, doc.ActiveView);
            //    tran.Commit();
            //}
            for (int i = 0; i < rectangles.Count; i++)
            {
                Rectangleslbr rec = rectangles[i];
                Transaction tran = new Transaction(doc, "ss");
                tran.Start();
                var TagSym = doc.Create.NewFamilyInstance(rec.Center, sym_, doc.ActiveView);
                //Parameter P1 = TagSym.LookupParameter("KEYNOTE");
                //P1.Set(i.ToString());
                tran.Commit();
            }
        }
    }
    public class Filterdetailline : ISelectionFilter
    {
        public bool AllowElement(Element element)
        {
            if (element.Category.Name == "Lines")
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
    public class Rectangleslbr
    {
        private static List<Line> listline = new List<Line>();
        private List<XYZ> allpoints = new List<XYZ>();
        private XYZ pointupleft;
        private XYZ pointdowleft;
        private XYZ pointupright;
        private XYZ pointdowright;
        private XYZ center;
        private List<Line> boundline = new List<Line>();
        private double area;
        private double ax;
        private double bx;
        private Curve aright;
        private Curve aleft;
        private Curve btop;
        private Curve bbot;
        public XYZ Center
        {
            get
            {
                return center;
            }
            set
            {
                center = value;
            }
        }
        public List<Line> Boundline
        {
            get
            {
                return boundline;
            }
        }
        public XYZ Pointupleft
        {
            get
            {
                return pointupleft;
            }
        }
        public XYZ Pointdowleft
        {
            get
            {
                return pointdowleft;
            }
        }
        public XYZ Pointupright
        {
            get
            {
                return pointupright;
            }
        }
        public XYZ Pointdowright
        {
            get
            {
                return pointdowright;
            }
        }
        public List<XYZ> Allpoints
        {
            get
            {
                return allpoints;
            }
        }
        public double Area
        {
            get
            {
                return area;
            }
        }
        public double a
        {
            get
            {
                return ax;
            }
        }
        public double b
        {
            get
            {
                return bx;
            }
        }
        public Curve Aright
        {
            get
            {
                return aright;
            }
        }
        public Curve Aleft
        {
            get
            {
                return aleft;
            }
        }
        public Curve Btop
        {
            get
            {
                return btop;
            }
        }
        public Curve Bbot
        {
            get
            {
                return bbot;
            }
        }
        public Rectangleslbr(Curve line1, Curve line2, Curve line3, Curve line4)
        {
            List<XYZ> listbehind = new List<XYZ>();
            XYZ point1 = line2.GetEndPoint(0);
            XYZ point2 = line2.GetEndPoint(1);
            XYZ point3 = line4.GetEndPoint(0);
            XYZ point4 = line4.GetEndPoint(1);
            aright = line1;
            aleft = line3;
            btop = line2;
            bbot = line4;
            Center = (point1 + point2 + point3 + point4) / 4;
            allpoints.Add(point1);
            allpoints.Add(point2);
            allpoints.Add(point3);
            allpoints.Add(point4);
            boundline.Add(line1 as Line);
            boundline.Add(line2 as Line);
            boundline.Add(line3 as Line);
            boundline.Add(line4 as Line);
            area = line1.Length * line2.Length;
            if (line1.Length < line2.Length)
            {
                ax = line1.Length;
                bx = line2.Length;
            }
            else
            {
                ax = line2.Length;
                bx = line1.Length;
            }
            foreach (XYZ point in Allpoints)
            {
                if (point.X > Center.X && point.Z > Center.Z)
                {
                    pointupleft = point;
                }
                if (point.X > Center.X && point.Z < Center.Z)
                {
                    pointdowleft = point;
                }
                if (point.X < Center.X && point.Z > Center.Z)
                {
                    pointupright = point;
                }
                if (point.X < Center.X && point.Z < Center.Z)
                {
                    pointdowright = point;
                }
                if (pointupleft == null || pointupright == null || pointdowleft == null || pointdowright == null)
                {
                    if (point.Y > Center.Y && point.Z > Center.Z)
                    {
                        pointupleft = point;
                    }
                    if (point.Y > Center.Y && point.Z < Center.Z)
                    {
                        pointdowleft = point;
                    }
                    if (point.Y < Center.Y && point.Z > Center.Z)
                    {
                        pointupright = point;
                    }
                    if (point.Y < Center.Y && point.Z < Center.Z)
                    {
                        pointdowright = point;
                    }
                }
            }
        }
        private static List<XYZ> Removeduplicatepoint(List<XYZ> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = i - 1; j < i; j++)
                {
                    XYZ point1 = list[i];
                    XYZ point2 = list[j];
                    if (point1.X == point2.X & point1.Y == point2.Y && point1.Z == point2.Z)
                    {
                        list.RemoveAt(j);
                        i--;
                    }
                }
            }
            return list;
        }
        public static List<Rectangleslbr> CreateRectangle(Document doc, List<XYZ> points)
        {
            List<CurveLoop> listcurveloops = new List<CurveLoop>();
            foreach (XYZ pointsource in points)
            {
                CurveLoop loop = new CurveLoop();
                XYZ point1 = new XYZ();
                XYZ point2 = new XYZ();
                XYZ point3 = new XYZ();
                point1 = PointDistancePointMinZvector(pointsource, points);
                if (point1 != null)
                {
                    point2 = PointDistancePointMinXvector(pointsource, point1, points);
                    if (point2 != null)
                    {
                        point3 = PointDistancePointMinXlowpoint(pointsource, point2, points);
                    }
                }
                using (Transaction t = new Transaction(doc, "ss"))
                {
                    t.Start();
                    if (point1 != null && point2 != null && point3 != null)
                    {
                        if (Equalspoint(pointsource, point1) == true)
                        {
                            Line line1 = Line.CreateBound(pointsource, point1);
                            loop.Append(line1);
                            listline.Add(line1);
                        }
                        if (Equalspoint(point1, point2) == true)
                        {
                            Line line2 = Line.CreateBound(point1, point2);
                            loop.Append(line2);
                            listline.Add(line2);
                        }
                        if (Equalspoint(point2, point3) == true)
                        {
                            Line line3 = Line.CreateBound(point2, point3);
                            loop.Append(line3);
                            listline.Add(line3);
                        }
                        if (Equalspoint(point3, pointsource) == true)
                        {
                            Line line4 = Line.CreateBound(point3, pointsource);
                            loop.Append(line4);
                            listline.Add(line4);
                        }
                    }
                    t.Commit();
                }
                listcurveloops.Add(loop);
            }
            List<Rectangleslbr> list = GetRectangles(doc, listcurveloops);
            list = Fillterinsulation(list);
            return list;
        }
        public static List<Rectangleslbr> CreateRectangle2(Document doc, List<XYZ> points)
        {
            List<CurveLoop> listcurveloops = new List<CurveLoop>();
            foreach (XYZ pointsource in points)
            {
                CurveLoop loop = new CurveLoop();
                XYZ point1 = new XYZ();
                XYZ point2 = new XYZ();
                XYZ point3 = new XYZ();
                point1 = PointDistancePointMinZvector(pointsource, points);
                if (point1 != null)
                {
                    point2 = PointDistancePointMinYvector(pointsource, point1, points);
                    if (point2 != null)
                    {
                        point3 = PointDistancePointMinYlowpoint(pointsource, point2, points);
                    }
                }
                using (Transaction t = new Transaction(doc, "ss"))
                {
                    t.Start();
                    if (point1 != null && point2 != null && point3 != null)
                    {
                        if (Equalspoint(pointsource, point1) == true)
                        {
                            Line line1 = Line.CreateBound(pointsource, point1);
                            loop.Append(line1);
                            listline.Add(line1);
                        }
                        if (Equalspoint(point1, point2) == true)
                        {
                            Line line2 = Line.CreateBound(point1, point2);
                            loop.Append(line2);
                            listline.Add(line2);
                        }
                        if (Equalspoint(point2, point3) == true)
                        {
                            Line line3 = Line.CreateBound(point2, point3);
                            loop.Append(line3);
                            listline.Add(line3);
                        }
                        if (Equalspoint(point3, pointsource) == true)
                        {
                            Line line4 = Line.CreateBound(point3, pointsource);
                            loop.Append(line4);
                            listline.Add(line4);
                        }
                    }
                    t.Commit();
                }
                listcurveloops.Add(loop);
            }
            List<Rectangleslbr> list = GetRectangles(doc, listcurveloops);
            list = Fillterinsulation(list);
            return list;
        }
        public static bool Equalspoint(XYZ point1, XYZ point2)
        {
            bool tagle = false;
            if (point1.X != point2.X || point1.Y != point2.Y || point1.Z != point2.Z)
            {
                tagle = true;
            }
            return tagle;
        }
        public static XYZ PointDistancePointMinZvector(XYZ point, List<XYZ> list)
        {
            XYZ XPoint = null;
            double distancemin = 100;
            for (int i = 0; i < list.Count; i++)
            {
                XYZ point1 = list[i];
                double kc = point.DistanceTo(point1);
                if (kc > 0.001 && kc < distancemin)
                {
                    if (point.Z > 0 && point1.Z > 0)
                    {
                        if (point.Z < point1.Z && point1.X == point.X)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                    }
                    if (point.Z < 0 && point1.Z < 0)
                    {
                        if (point.Z > point1.Z && point1.X == point.X)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                    }
                }
            }
            return XPoint;
        }
        public static XYZ PointDistancePointMinXvector(XYZ pointsor, XYZ point, List<XYZ> list)
        {
            XYZ XPoint = null;
            XYZ pointflu = null;
            double distancemin = 100;
            for (int i = 0; i < list.Count; i++)
            {
                XYZ point1 = list[i];
                double kc = point.DistanceTo(point1);
                if (kc > 0.001 && kc < distancemin)
                {
                    if (point.X > 0)
                    {
                        if (point1.X > 0 && Math.Abs(point.X) > Math.Abs(point1.X) && point1.Z == point.Z)
                        {
                            pointflu = PointDistancePointMinXlowpoint(pointsor, point1, list);
                            if (pointflu != null)
                            {
                                distancemin = kc;
                                XPoint = point1;
                            }
                        }
                        if (point1.X < 0 && point1.Z == point.Z)
                        {
                            pointflu = PointDistancePointMinXlowpoint(pointsor, point1, list);
                            if (pointflu != null)
                            {
                                distancemin = kc;
                                XPoint = point1;
                            }
                        }
                    }
                    if (point.X < 0)
                    {
                        if (point1.X < 0 && Math.Abs(point.X) > Math.Abs(point1.X) && point1.Z == point.Z)
                        {
                            pointflu = PointDistancePointMinXlowpoint(pointsor, point1, list);
                            if (pointflu != null)
                            {
                                distancemin = kc;
                                XPoint = point1;
                            }
                        }
                    }
                }
            }
            return XPoint;
        }
        public static XYZ PointDistancePointMinYvector(XYZ pointsor, XYZ point, List<XYZ> list)
        {
            XYZ XPoint = null;
            XYZ pointflu = null;
            double distancemin = 100;
            for (int i = 0; i < list.Count; i++)
            {
                XYZ point1 = list[i];
                double kc = point.DistanceTo(point1);
                if (kc > 0.001 && kc < distancemin)
                {
                    if (point.Y > 0)
                    {
                        if (point1.Y > 0 && Math.Abs(point.Y) > Math.Abs(point1.Y) && point1.Z == point.Z)
                        {
                            pointflu = PointDistancePointMinYlowpoint(pointsor, point1, list);
                            if (pointflu != null)
                            {
                                distancemin = kc;
                                XPoint = point1;
                            }
                        }
                        if (point1.Y < 0 && point1.Z == point.Z)
                        {
                            pointflu = PointDistancePointMinYlowpoint(pointsor, point1, list);
                            if (pointflu != null)
                            {
                                distancemin = kc;
                                XPoint = point1;
                            }
                        }
                    }
                    if (point.Y < 0)
                    {
                        if (point1.Y < 0 && Math.Abs(point.Y) > Math.Abs(point1.Y) && point1.Z == point.Z)
                        {
                            pointflu = PointDistancePointMinYlowpoint(pointsor, point1, list);
                            if (pointflu != null)
                            {
                                distancemin = kc;
                                XPoint = point1;
                            }
                        }
                    }
                }
            }
            return XPoint;
        }
        public static XYZ PointDistancePointMinXlowpoint(XYZ pointsor, XYZ point, List<XYZ> list)
        {
            XYZ XPoint = null;
            double distancemin = 100;
            for (int i = 0; i < list.Count; i++)
            {
                XYZ point1 = list[i];
                double kc = point.DistanceTo(point1);
                if (kc > 0.001 && kc < distancemin)
                {
                    if (point.Z > 0)
                    {
                        if (point1.Z > 0 && Math.Abs(point.Z) > Math.Abs(point1.Z) && point1.X == point.X)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                        if (point1.Z < 0 && point1.X == point.X)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                    }
                    if (point.Z < 0)
                    {
                        distancemin = kc;
                        XPoint = point1;
                    }
                }
            }
            return XPoint;
        }
        public static XYZ PointDistancePointMinYlowpoint(XYZ pointsor, XYZ point, List<XYZ> list)
        {
            XYZ XPoint = null;
            double distancemin = 100;
            for (int i = 0; i < list.Count; i++)
            {
                XYZ point1 = list[i];
                double kc = point.DistanceTo(point1);
                if (kc > 0.001 && kc < distancemin)
                {
                    if (point.Z > 0)
                    {
                        if (point1.Z > 0 && Math.Abs(point.Z) > Math.Abs(point1.Z) && point1.Y == point.Y)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                        if (point1.Z < 0 && point1.Y == point.Y)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                    }
                    if (point.Z < 0)
                    {
                        distancemin = kc;
                        XPoint = point1;
                    }
                }
            }
            return XPoint;
        }
        public static XYZ PointDistancePointMinXunder(XYZ pointsor, XYZ point, List<XYZ> list)
        {
            XYZ XPoint = null;
            double distancemin = 100;
            for (int i = 0; i < list.Count; i++)
            {
                XYZ point1 = list[i];
                double kc = point.DistanceTo(point1);
                if (kc > 0.001 && kc < distancemin)
                {
                    if (point.Z > 0)
                    {
                        if (point1.Z < point.Z && point1.Z == pointsor.Z && point1.X == point.X)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                    }
                    if (point.Z < 0)
                    {
                        if (point1.Z > point.Z && point1.Z == pointsor.Z && point1.X == point.X)
                        {
                            distancemin = kc;
                            XPoint = point1;
                        }
                    }
                }
            }
            return XPoint;
        }
        public static List<Rectangleslbr> GetRectangles(Document doc, List<CurveLoop> listloops)
        {
            List<Rectangleslbr> rectangleExtensions = new List<Rectangleslbr>();
            Plane plane = Plane.CreateByOriginAndBasis(doc.ActiveView.Origin, XYZ.BasisX, XYZ.BasisZ);
            foreach (var curveloop in listloops)
            {

                if (Checkrectangle(doc, curveloop, plane) == true)
                {
                    curveloop.ToList().RemoveAt(0);
                    curveloop.ToList().RemoveAt(1);
                    curveloop.ToList().RemoveAt(2);
                    curveloop.ToList().RemoveAt(3);
                    Curve c1 = curveloop.ToList()[0];
                    Curve c2 = curveloop.ToList()[1];
                    Curve c3 = curveloop.ToList()[2];
                    Curve c4 = curveloop.ToList()[3];
                    Rectangleslbr rec = new Rectangleslbr(c1, c2, c3, c4);
                    if (rec.Pointupright != null && rec.Pointupleft != null && rec.Pointdowright != null && rec.Pointdowleft != null)
                    {
                        rectangleExtensions.Add(rec);
                    }
                }
            }
            return rectangleExtensions;
        }
        public static bool Checkrectangle(Document doc, CurveLoop curveloop, Plane plane)
        {
            bool flag = false;
            if (curveloop.IsOpen() == false)
            {
                var j = curveloop.IsRectangular(plane);
                if (j == true)
                {
                    flag = true;
                }
            }
            return flag;
        }
        public static ICollection<ElementId> LayoutInsulationvert(Document doc, List<List<Rectangleslbr>> listrecs)
        {
            FamilySymbol symbol_ = FindSymbol_(doc);
            ICollection<ElementId> copyid = new List<ElementId>();
            for (int i = 0, j = 20; i < listrecs.Count; i++, j += 3)
            {
                List<Rectangleslbr> list1 = listrecs[i];
                for (int k = 0, q = 5; k < list1.Count; k++, q += 2)
                {
                    var rec = list1[k];
                    using (Transaction t = new Transaction(doc, "Createdetail"))
                    {
                        t.Start();
                        ICollection<ElementId> listid = new List<ElementId>();
                        var linesrec = rec.Boundline;
                        var tagsymbol_ = doc.Create.NewFamilyInstance(rec.Center, symbol_, doc.ActiveView);
                        foreach (var lk in linesrec)
                        {
                            DetailLine detailLine = doc.Create.NewDetailCurve(doc.ActiveView, lk) as DetailLine;
                            listid.Add(detailLine.Id);
                            copyid.Add(detailLine.Id);
                            copyid.Add(tagsymbol_.Id);
                        }
                        ElementTransformUtils.MoveElements(doc, listid, new XYZ(0, 0, j));
                        ElementTransformUtils.MoveElement(doc, tagsymbol_.Id, new XYZ(0, 0, j));
                        ElementTransformUtils.MoveElements(doc, listid, new XYZ(q, 0, 0));
                        ElementTransformUtils.MoveElement(doc, tagsymbol_.Id, new XYZ(q, 0, 0));
                        t.Commit();
                    }
                }
            }
            return copyid;
        }
        public static ICollection<ElementId> LayoutInsulationvert2(Document doc, List<List<Rectangleslbr>> listrecs)
        {
            FamilySymbol symbol_ = FindSymbol_(doc);
            ICollection<ElementId> copyid = new List<ElementId>();
            for (int i = 0, j = 20; i < listrecs.Count; i++, j += 3)
            {
                List<Rectangleslbr> list1 = listrecs[i];
                for (int k = 0, q = 5; k < list1.Count; k++, q += 2)
                {
                    var rec = list1[k];
                    using (Transaction t = new Transaction(doc, "Createdetail"))
                    {
                        t.Start();
                        ICollection<ElementId> listid = new List<ElementId>();
                        var linesrec = rec.Boundline;
                        var tagsymbol_ = doc.Create.NewFamilyInstance(rec.Center, symbol_, doc.ActiveView);
                        foreach (var lk in linesrec)
                        {
                            DetailLine detailLine = doc.Create.NewDetailCurve(doc.ActiveView, lk) as DetailLine;
                            listid.Add(detailLine.Id);
                            copyid.Add(detailLine.Id);
                            copyid.Add(tagsymbol_.Id);
                        }
                        ElementTransformUtils.MoveElements(doc, listid, new XYZ(0, 0, j));
                        ElementTransformUtils.MoveElement(doc, tagsymbol_.Id, new XYZ(0, 0, j));
                        ElementTransformUtils.MoveElements(doc, listid, new XYZ(0, q, 0));
                        ElementTransformUtils.MoveElement(doc, tagsymbol_.Id, new XYZ(0, q, 0));
                        t.Commit();
                    }
                }
            }
            return copyid;
        }
        public static FamilySymbol FindSymbol_(Document doc)
        {
            List<FamilySymbol> list = new List<FamilySymbol>();
            var families = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(Family)).ToElements().Cast<Family>().ToList();
            foreach (Family Instance in families)
            {
                ISet<ElementId> eleid = Instance.GetFamilySymbolIds();
                foreach (var i in eleid)
                {
                    Element elemtype = doc.GetElement(i);
                    FamilySymbol fasy = elemtype as FamilySymbol;
                    if (fasy.Family.FamilyCategory.Name != null && fasy.Family.FamilyCategory.Name.Contains("Generic Annotations"))
                    {
                        list.Add(fasy);
                    }
                }
            }
            FamilySymbol sym_ = null;
            foreach (var i in list)
            {
                if (i.Name.Contains("TIE COUNT"))
                {
                    sym_ = i;
                }
            }
            return sym_;
        }
        public static List<Rectangleslbr> Fillterinsulation(List<Rectangleslbr> list)
        {
            List<Rectangleslbr> listout = new List<Rectangleslbr>();
            foreach (var i in list)
            {
                if (i.Area > 16) continue;
                else
                {
                    listout.Add(i);
                }
            }
            listout.OrderBy(x => x.Center.Z).ToList();
            return listout;
        }
        public static List<List<Rectangleslbr>> ListRecVert(List<Rectangleslbr> list)
        {
            List<List<Rectangleslbr>> listrec = new List<List<Rectangleslbr>>();
            List<Rectangleslbr> list1 = new List<Rectangleslbr>();
            List<Rectangleslbr> list2 = new List<Rectangleslbr>();
            List<Rectangleslbr> list3 = new List<Rectangleslbr>();
            List<Rectangleslbr> list4 = new List<Rectangleslbr>();
            List<Rectangleslbr> list5 = new List<Rectangleslbr>();
            List<Rectangleslbr> list6 = new List<Rectangleslbr>();
            List<Rectangleslbr> list7 = new List<Rectangleslbr>();
            List<Rectangleslbr> list8 = new List<Rectangleslbr>();
            List<Rectangleslbr> list9 = new List<Rectangleslbr>();
            List<Rectangleslbr> list10 = new List<Rectangleslbr>();
            List<Rectangleslbr> list11 = new List<Rectangleslbr>();
            if (list.Count != 0)
            {
                list1.Add(list[0]);
                for (int i = 1; i < list.Count; i++)
                {
                    Rectangleslbr rec1 = list1[0];
                    Rectangleslbr rec2 = list[i];
                    if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                    {
                        list1.Add(rec2);
                    }
                }
                List<Rectangleslbr> query1 = list.Except(list1).ToList();
                list1 = Fillterinsulation(list1);
                if (query1.Count != 0)
                {
                    list2.Add(query1[0]);
                    for (int i = 1; i < query1.Count; i++)
                    {
                        Rectangleslbr rec1 = list2[0];
                        Rectangleslbr rec2 = query1[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list2.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query2 = query1.Except(list2).ToList();
                list2 = Fillterinsulation(list2);
                if (query2.Count != 0)
                {
                    list3.Add(query2[0]);
                    for (int i = 1; i < query2.Count; i++)
                    {
                        Rectangleslbr rec1 = list3[0];
                        Rectangleslbr rec2 = query2[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list3.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query3 = query2.Except(list3).ToList();
                list3 = Fillterinsulation(list3);
                if (query3.Count != 0)
                {
                    list4.Add(query3[0]);
                    for (int i = 1; i < query3.Count; i++)
                    {
                        Rectangleslbr rec1 = list4[0];
                        Rectangleslbr rec2 = query3[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list4.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query4 = query3.Except(list4).ToList();
                list4 = Fillterinsulation(list4);
                if (query4.Count != 0)
                {
                    list5.Add(query4[0]);
                    for (int i = 1; i < query4.Count; i++)
                    {
                        Rectangleslbr rec1 = list5[0];
                        Rectangleslbr rec2 = query4[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list5.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query5 = query4.Except(list5).ToList();
                list5 = Fillterinsulation(list5);
                if (query5.Count != 0)
                {
                    list6.Add(query5[0]);
                    for (int i = 1; i < query5.Count; i++)
                    {
                        Rectangleslbr rec1 = list6[0];
                        Rectangleslbr rec2 = query5[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list6.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query6 = query5.Except(list6).ToList();
                list6 = Fillterinsulation(list6);
                if (query6.Count != 0)
                {
                    list7.Add(query6[0]);
                    for (int i = 1; i < query6.Count; i++)
                    {
                        Rectangleslbr rec1 = list7[0];
                        Rectangleslbr rec2 = query6[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list7.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query7 = query6.Except(list7).ToList();
                list7 = Fillterinsulation(list7);
                if (query7.Count != 0)
                {
                    list8.Add(query7[0]);
                    for (int i = 1; i < query7.Count; i++)
                    {
                        Rectangleslbr rec1 = list8[0];
                        Rectangleslbr rec2 = query7[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list8.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query8 = query7.Except(list8).ToList();
                list8 = Fillterinsulation(list8);
                if (query8.Count != 0)
                {
                    list9.Add(query8[0]);
                    for (int i = 1; i < query8.Count; i++)
                    {
                        Rectangleslbr rec1 = list9[0];
                        Rectangleslbr rec2 = query8[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list9.Add(rec2);
                        }
                    }
                }
                List<Rectangleslbr> query9 = query8.Except(list9).ToList();
                list9 = Fillterinsulation(list9);
                if (query9.Count != 0)
                {
                    list10.Add(query9[0]);
                    for (int i = 1; i < query9.Count; i++)
                    {
                        Rectangleslbr rec1 = list10[0];
                        Rectangleslbr rec2 = query9[i];
                        if (rec1.Center.Z == rec2.Center.Z && (rec1.Center.X != rec2.Center.X || rec1.Center.Y != rec2.Center.Y))
                        {
                            list10.Add(rec2);
                        }
                    }
                }
                if (list1.Count != 0)
                {
                    listrec.Add(list1);
                }
                if (list2.Count != 0)
                {
                    listrec.Add(list2);
                }
                if (list3.Count != 0)
                {
                    listrec.Add(list3);
                }
                if (list4.Count != 0)
                {
                    listrec.Add(list4);
                }
                if (list5.Count != 0)
                {
                    listrec.Add(list5);
                }
                if (list6.Count != 0)
                {
                    listrec.Add(list6);
                }
                if (list7.Count != 0)
                {
                    listrec.Add(list7);
                }
                if (list8.Count != 0)
                {
                    listrec.Add(list8);
                }
                if (list9.Count != 0)
                {
                    listrec.Add(list9);
                }
                if (list10.Count != 0)
                {
                    listrec.Add(list10);
                }
            }
            listrec.OrderByDescending(x => x.First().Center.Z).ToList();
            return listrec;
        }
    }
}
