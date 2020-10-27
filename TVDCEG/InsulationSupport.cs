using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TVDCEG.LBR;

namespace TVDCEG
{
    public class InsulationSupport
    {
        public void DrawingShearGridOnFace(Document doc, List<FamilyInstance> sheargrid, FamilyInstance wall)
        {
            using (Transaction tran = new Transaction(doc, "Create Detail line"))
            {
                tran.Start();
                foreach (FamilyInstance instance in sheargrid)
                {
                    Transform transform1 = instance.GetTransform();
                    Curve curve = GetCurveSheargrid(instance);
                    XYZ p1 = curve.GetEndPoint(0);
                    XYZ p2 = curve.GetEndPoint(1);
                    XYZ f1 = new XYZ(p1.X, 0, 0);
                    XYZ f2 = new XYZ(p2.X, 0, 0);
                    Line dline = Line.CreateBound(transform1.OfPoint(f1), transform1.OfPoint(f2));
                    DetailLine line = doc.Create.NewDetailCurve(doc.ActiveView, dline) as DetailLine;
                }
                tran.Commit();
            }
        }
        public Dimension CreateDimentionHolizontal(Document doc, List<DetailLine> detailLines, Line locationdimention, double x)
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Dimension dimension = null;
            foreach (var i in detailLines)
            {
                Line line = i.GeometryCurve as Line;
                referenceArray.Append(line.Reference);
            }
            XYZ starpoint = new XYZ(locationdimention.GetEndPoint(0).X, locationdimention.GetEndPoint(0).Y, locationdimention.GetEndPoint(0).Z + x);
            XYZ endpoint = new XYZ(locationdimention.GetEndPoint(1).X, locationdimention.GetEndPoint(1).Y, locationdimention.GetEndPoint(1).Z + x);
            Line line1 = Line.CreateBound(starpoint, endpoint);
            using (Transaction tran = new Transaction(doc, "Create dimention line"))
            {
                tran.Start();
                dimension = doc.Create.NewDimension(doc.ActiveView, line1, referenceArray);
                tran.Commit();
            }
            return dimension;
        }
        public void CreateLine2Dim(Document doc, Dimension d1, Dimension d2)
        {
            View view = doc.ActiveView;
            XYZ viewDirec = view.ViewDirection;
            Plane plane = Plane.CreateByNormalAndOrigin(viewDirec, view.Origin);
            var l1 = d1.Curve as Line;
            l1.MakeBound(0, 1);
            var l2 = d2.Curve as Line;
            l2.MakeBound(0, 1);
            var line1 = LineOnPlane(l1, plane);
            var line2 = LineOnPlane(l2, plane);
            var inter = ExtendLineIntersection(line1, line2);
            //inter = plane.ProjectOnto(inter);

            XYZ p1 = GetDimensionStartPoint(d1);
            List<XYZ> pts1 = GetDimensionPoints(d1, p1);

            XYZ p2 = GetDimensionStartPoint(d2);
            List<XYZ> pts2 = GetDimensionPoints(d2, p2);

            var p = inter;
            XYZ Point1 = XYZ.Zero;
            XYZ Point2 = XYZ.Zero;

            var min1 = double.MaxValue;

            foreach (var point in pts1)
            {
                var distance = p.DistanceTo(point);
                if (distance < min1)
                {
                    Point1 = point;
                    min1 = distance;
                }
            }

            var min2 = double.MaxValue;
            foreach (var point in pts2)
            {
                var distance = p.DistanceTo(point);
                if (distance < min2)
                {
                    Point2 = point;
                    min2 = distance;
                }
            }
            Point1 = plane.ProjectOnto(Point1);
            Point2 = plane.ProjectOnto(Point2);
            XYZ inter1 = plane.ProjectOnto(inter);
            Line linev = Line.CreateBound(inter1, Point2);
            using (Transaction tx = new Transaction(doc))
            {
                tx.Start("Draw Point Markers");
                doc.Create.NewDetailCurve(view, Line.CreateBound(plane.ProjectOnto(inter), Point1));
                var t = doc.Create.NewDetailCurve(view, linev) as DetailLine;
                var g = t.GeometryCurve.ApproximateLength;
                tx.Commit();
            }

        }
        List<XYZ> GetDimensionPoints(
    Dimension dim,
    XYZ pStart)
        {
            Line dimLine = dim.Curve as Line;
            if (dimLine == null) return null;
            List<XYZ> pts = new List<XYZ>();

            dimLine.MakeBound(0, 1);
            XYZ pt1 = dimLine.GetEndPoint(0);
            XYZ pt2 = dimLine.GetEndPoint(1);
            XYZ direction = pt2.Subtract(pt1).Normalize();

            if (0 == dim.Segments.Size)
            {
                XYZ v = 0.5 * (double)dim.Value * direction;
                pts.Add(pStart - v);
                pts.Add(pStart + v);
            }
            else
            {
                XYZ p = pStart;
                foreach (DimensionSegment seg in dim.Segments)
                {
                    XYZ v = (double)seg.Value * direction;
                    if (0 == pts.Count)
                    {
                        pts.Add(p = (pStart - 0.5 * v));
                    }
                    pts.Add(p = p.Add(v));
                }
            }
            return pts;
        }

        XYZ GetDimensionStartPoint(
    Dimension dim)
        {
            XYZ p = null;

            try
            {
                p = dim.Origin;
            }
            catch (Autodesk.Revit.Exceptions.ApplicationException ex)
            {
                Debug.Assert(ex.Message.Equals("Cannot access this method if this dimension has more than one segment."));

                foreach (DimensionSegment seg in dim.Segments)
                {
                    p = seg.Origin;
                    break;
                }
            }
            return p;
        }
        Line LineOnPlane(Line line, Plane plane)
        {
            var p1 = plane.ProjectOnto(line.GetEndPoint(0));
            var p2 = plane.ProjectOnto(line.GetEndPoint(1));
            return Line.CreateBound(p1, p2);
        }
        public XYZ ExtendLineIntersection(Line l, Line ll)
        {
            IntersectionResultArray resultArray;
            if (Line.CreateBound(l.Origin + 10000.0 * l.Direction, l.Origin - 10000.0 * l.Direction).Intersect((Curve)Line.CreateBound(ll.Origin + 10000.0 * ll.Direction, ll.Origin - 10000.0 * ll.Direction), out resultArray) != SetComparisonResult.Overlap)
                throw new InvalidOperationException("Input lines did not intersect.");
            if (resultArray == null || resultArray.Size != 1)
                throw new InvalidOperationException("Could not extract line intersection point.");
            return resultArray.get_Item(0).XYZPoint;
        }
        public Dimension CreateDimentionVertical(Document doc, List<DetailLine> detailLines, Line locationdimention, double x)
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Dimension dimension = null;
            XYZ starpoint = XYZ.Zero;
            XYZ endpoint = XYZ.Zero;
            foreach (var i in detailLines)
            {
                Line line = i.GeometryCurve as Line;
                referenceArray.Append(line.Reference);
            }
            if (locationdimention.GetEndPoint(0).X < 0)
            {
                starpoint = new XYZ(locationdimention.GetEndPoint(0).X + x, locationdimention.GetEndPoint(0).Y, locationdimention.GetEndPoint(0).Z);
                endpoint = new XYZ(locationdimention.GetEndPoint(1).X + x, locationdimention.GetEndPoint(1).Y, locationdimention.GetEndPoint(1).Z);
            }
            else
            {
                starpoint = new XYZ(locationdimention.GetEndPoint(0).X - x, locationdimention.GetEndPoint(0).Y, locationdimention.GetEndPoint(0).Z);
                endpoint = new XYZ(locationdimention.GetEndPoint(1).X - x, locationdimention.GetEndPoint(1).Y, locationdimention.GetEndPoint(1).Z);
            }
            Line line1 = Line.CreateBound(starpoint, endpoint);
            using (Transaction tran = new Transaction(doc, "Create dimention line"))
            {
                tran.Start();
                dimension = doc.Create.NewDimension(doc.ActiveView, line1, referenceArray);
                tran.Commit();
            }
            return dimension;
        }
        public Dimension CreateDimentionVertical2(Document doc, List<DetailLine> detailLines, Line locationdimention, double x)
        {
            ReferenceArray referenceArray = new ReferenceArray();
            Dimension dimension = null;
            XYZ starpoint = XYZ.Zero;
            XYZ endpoint = XYZ.Zero;
            var right = doc.ActiveView.RightDirection;
            foreach (var i in detailLines)
            {
                Line line = i.GeometryCurve as Line;
                referenceArray.Append(line.Reference);
            }
            if (Math.Floor(right.X) == 0)
            {
                if (right.Y > 0)
                {
                    if (locationdimention.GetEndPoint(0).Y > 0)
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X, locationdimention.GetEndPoint(0).Y + x, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X, locationdimention.GetEndPoint(1).Y + x, locationdimention.GetEndPoint(1).Z);
                    }
                    else
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X, locationdimention.GetEndPoint(0).Y - x, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X, locationdimention.GetEndPoint(1).Y - x, locationdimention.GetEndPoint(1).Z);
                    }
                }
                if (right.Y < 0)
                {
                    if (locationdimention.GetEndPoint(0).Y > 0)
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X, locationdimention.GetEndPoint(0).Y - x, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X, locationdimention.GetEndPoint(1).Y - x, locationdimention.GetEndPoint(1).Z);
                    }
                    else
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X, locationdimention.GetEndPoint(0).Y + x, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X, locationdimention.GetEndPoint(1).Y + x, locationdimention.GetEndPoint(1).Z);
                    }
                }
            }
            if (Math.Floor(right.X) != 0)
            {
                if (right.X > 0)
                {
                    if (locationdimention.GetEndPoint(0).X < 0)
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X + x, locationdimention.GetEndPoint(0).Y, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X + x, locationdimention.GetEndPoint(1).Y, locationdimention.GetEndPoint(1).Z);
                    }
                    else
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X - x, locationdimention.GetEndPoint(0).Y, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X - x, locationdimention.GetEndPoint(1).Y, locationdimention.GetEndPoint(1).Z);
                    }
                }
                if (right.X < 0)
                {
                    if (locationdimention.GetEndPoint(0).X < 0)
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X - x, locationdimention.GetEndPoint(0).Y, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X - x, locationdimention.GetEndPoint(1).Y, locationdimention.GetEndPoint(1).Z);
                    }
                    else
                    {
                        starpoint = new XYZ(locationdimention.GetEndPoint(0).X + x, locationdimention.GetEndPoint(0).Y, locationdimention.GetEndPoint(0).Z);
                        endpoint = new XYZ(locationdimention.GetEndPoint(1).X + x, locationdimention.GetEndPoint(1).Y, locationdimention.GetEndPoint(1).Z);
                    }
                }
            }
            Line line1 = Line.CreateBound(starpoint, endpoint);
            using (Transaction tran = new Transaction(doc, "Create dimention line"))
            {
                tran.Start();
                dimension = doc.Create.NewDimension(doc.ActiveView, line1, referenceArray);
                tran.Commit();
            }
            return dimension;
        }

        public List<FamilyInstance> GetShearGrid(Document doc, AssemblyInstance assemblyInstance)
        {
            List<FamilyInstance> sheargrid = new List<FamilyInstance>();
            ICollection<ElementId> elementIds = assemblyInstance.GetMemberIds();
            foreach (var i in elementIds)
            {
                Element ele = doc.GetElement(i);
                FamilyInstance familyInstance = ele as FamilyInstance;
                if (familyInstance.Symbol.FamilyName == "SHEAR GRID - 3 1-2 x 5-7" || familyInstance.Symbol.FamilyName == "SHEAR GRID - 5 1-2 x 5-7" || familyInstance.Symbol.FamilyName == "SHEAR GRID - 3 1-2 x 2 9 1-2")
                {
                    sheargrid.Add(familyInstance);
                }
            }
            return sheargrid;
        }
        public Curve GetCurveSheargrid(FamilyInstance familyInstance)
        {
            IList<Curve> alllines = new List<Curve>();
            Curve curvesmax = null;
            Options option = new Options();
            option.ComputeReferences = true;
            option.IncludeNonVisibleObjects = false;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    GeometryInstance instance = geoObject as GeometryInstance;
                    if (null != instance)
                    {
                        GeometryElement instanceGeometryElement = instance.GetInstanceGeometry();
                        foreach (GeometryObject o in instanceGeometryElement)
                        {
                            Solid solid = o as Solid;
                            if (solid != null)
                            {
                                EdgeArray edgeArray = solid.Edges;
                                foreach (Edge edge in edgeArray)
                                {
                                    Line line = edge.AsCurve() as Line;
                                    alllines.Add(line);
                                }
                            }
                        }
                        var membercurvmax = alllines.OrderByDescending(x => x.Length).First();
                        curvesmax = membercurvmax;
                    }
                }
            }
            return curvesmax;
        }
        public PlanarFace FindTopWall(FamilyInstance familyInstance)
        {
            PlanarFace planarFace = null;
            List<PlanarFace> face = new List<PlanarFace>();
            Options option = new Options();
            option.ComputeReferences = false;
            option.IncludeNonVisibleObjects = false;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    Solid solid = geoObject as Solid;
                    if (solid != null)
                    {
                        var gh = solid.Volume;
                        FaceArray faces = solid.Faces;
                        foreach (var geoFace in faces)
                        {
                            var g = geoFace.GetType();
                            if (g.Name.Equals("PlanarFace"))
                            {
                                face.Add(geoFace as PlanarFace);
                            }
                        }
                    }
                }
            }
            planarFace = face[0];
            //face.OrderBy(x => x.Area).ToList();
            //List<PlanarFace> gg = face;
            for (int i = 1; i < face.Count; i++)
            {
                var ty = face[i];
                if (planarFace.Area < ty.Area)
                {
                    planarFace = ty;
                }
            }
            return planarFace;
        }
        public PlanarFace FindTopWallother(FamilyInstance familyInstance)
        {
            PlanarFace planarFace = null;
            List<PlanarFace> face = new List<PlanarFace>();
            var listface = AllFaces(familyInstance);
            foreach (var i in listface)
            {
                if (i.GetType().Name.Equals("PlanarFace"))
                {
                    face.Add(i as PlanarFace);
                }
            }
            planarFace = face[0];
            //face.OrderBy(x => x.Area).ToList();
            //List<PlanarFace> gg = face;
            for (int i = 1; i < face.Count; i++)
            {
                var ty = face[i];
                if (planarFace.Area < ty.Area)
                {
                    planarFace = ty;
                }
            }
            return planarFace;
        }
        public void GetFacesFromSymbol(FamilyInstance familyInstance, ref List<Face> faces)
        {
            Options option = new Options();
            option.ComputeReferences = true;
            option.IncludeNonVisibleObjects = true;
            option.DetailLevel = ViewDetailLevel.Undefined;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    GeometryInstance instance = geoObject as Autodesk.Revit.DB.GeometryInstance;
                    if (null != instance)
                    {
                        Transform transform = familyInstance.GetTransform();
                        GeometryElement instanTVDCEGeometryElement = instance.GetSymbolGeometry(transform);
                        foreach (GeometryObject instObj in instanTVDCEGeometryElement)
                        {
                            Solid solid = instObj as Solid;
                            if (solid != null)
                            {
                                var gh = solid.Volume;
                                FaceArray fb = solid.Faces;
                                foreach (var geoFace in fb)
                                {
                                    var g = geoFace.GetType();
                                    if (g.Name.Equals("PlanarFace"))
                                    {
                                        faces.Add(geoFace as PlanarFace);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void GetFaces(FamilyInstance familyInstance, ref List<Face> faces)
        {
            Options option = new Options();
            option.ComputeReferences = true;
            option.IncludeNonVisibleObjects = true;
            option.DetailLevel = ViewDetailLevel.Undefined;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    Solid solid = geoObject as Solid;
                    if (solid != null)
                    {
                        var gh = solid.Volume;
                        FaceArray fB = solid.Faces;
                        foreach (var geoFace in fB)
                        {
                            var g = geoFace.GetType();
                            if (g.Name.Equals("PlanarFace"))
                            {
                                faces.Add(geoFace as PlanarFace);
                            }
                        }
                    }
                }
            }
        }
        public void GetFacesFromNested(FamilyInstance familyInstance, ref List<Face> faces)
        {
            Document doc = familyInstance.Document;
            foreach (ElementId id in familyInstance.GetSubComponentIds())
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                if (instance == null) continue;
                GetFaces(instance, ref faces);
                GetFacesFromSymbol(instance, ref faces);
            }
        }
        public List<Face> AllFaces(FamilyInstance familyInstance)
        {
            List<Face> allFaces = new List<Face>();
            GetFaces(familyInstance, ref allFaces);
            GetFacesFromSymbol(familyInstance, ref allFaces);
            GetFacesFromNested(familyInstance, ref allFaces);
            return allFaces;
        }
        public List<DetailLine> Removelineovelap(List<DetailLine> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                for (int j = i - 1; j < i; j++)
                {
                    Curve curve1 = list[i].GeometryCurve;
                    Curve curve2 = list[i - 1].GeometryCurve;
                    IntersectionResultArray resultArray;
                    if (curve1.Intersect(curve2, out resultArray) != SetComparisonResult.Overlap)
                    {
                        list.RemoveAt(j);
                    }
                    if (resultArray == null || resultArray.Size != 1)
                    {
                        list.RemoveAt(j);
                    }
                }
            }
            return list;
        }
        #region Drawingblockout2
        public List<Rectangleslbr> DrawBlockOut2(Document doc, AssemblyInstance assemblyInstance, FamilyInstance familyInstance, FamilyInstance WALLREC, Rectangleslbr recbo, double space, double insulationlength)
        {
            View view = doc.ActiveView;
            XYZ right = view.RightDirection;
            XYZ Up = view.UpDirection;
            Insulationlbr lbr = Insulationlbr._instance;
            List<Rectangleslbr> listrec = new List<Rectangleslbr>();
            List<Rectangleslbr> rectangleslbrs = new List<Rectangleslbr>();
            List<DetailLine> listline1 = new List<DetailLine>();
            List<DetailLine> listline2 = new List<DetailLine>();
            List<DetailLine> listline3 = new List<DetailLine>();
            List<DetailLine> Dimentioninsulationtop = new List<DetailLine>();
            List<DetailLine> DimentionSheargirdtopandBlockout = new List<DetailLine>();
            List<DetailLine> Dimentiontoptong = new List<DetailLine>();
            List<DetailLine> Dimentionleftinsulation = new List<DetailLine>();
            List<DetailLine> Dimentionleftblockout = new List<DetailLine>();
            List<DetailLine> Dimentionlefttong = new List<DetailLine>();
            Line hozi2 = null;
            Line hozi1 = null;
            //lay 2 canh dai cua recbo 
            List<Rectangleslbr> newlist = new List<Rectangleslbr>();
            List<CurveLoop> curveloop = new List<CurveLoop>();
            PlanarFace topface = FindTopWallother(familyInstance);
            Transform transform = WALLREC.GetTransform();
            DetailCurve canhdaitren = null;
            DetailCurve canhdaiduoi = null;
            Curve recboedge1 = Line.CreateBound(transform.OfPoint(recbo.Pointupright), transform.OfPoint(recbo.Pointupleft));
            Curve recboedge2 = Line.CreateBound(transform.OfPoint(recbo.Pointdowright), transform.OfPoint(recbo.Pointdowleft));
            using (Transaction tr = new Transaction(doc, "create detail line"))
            {
                tr.Start();
                canhdaitren = doc.Create.NewDetailCurve(doc.ActiveView, recboedge1);
                canhdaiduoi = doc.Create.NewDetailCurve(doc.ActiveView, recboedge2);
                tr.Commit();
            }
            listline2.Add(canhdaitren as DetailLine);
            listline3.Add(canhdaiduoi as DetailLine);
            // lay 1 canh ngan cua recbo 
            Line recboedshort1 = Line.CreateBound(transform.OfPoint(recbo.Pointupright), transform.OfPoint(recbo.Pointdowright));
            Line recboedshort2 = Line.CreateBound(transform.OfPoint(recbo.Pointupleft), transform.OfPoint(recbo.Pointdowleft));
            IList<CurveLoop> loops = topface.GetEdgesAsCurveLoops();
            foreach (var i in loops)
            {
                curveloop.Add(i);
            }
            listrec = Rectangleslbr.GetRectangles(doc, curveloop);
            for (int i = 0; i < listrec.Count; i++)
            {
                var rec = listrec[i];
                if (rec.a > 3)
                {
                    if (rec.Aleft.ApproximateLength < 10 && rec.Bbot.ApproximateLength < 10)
                    {
                        newlist.Add(rec);
                    }
                }
            }
            using (Transaction tran = new Transaction(doc, "Createline"))
            {
                tran.Start();
                var rec = newlist[0];
                XYZ pointu1 = recboedge2.GetEndPoint(0);
                XYZ pointu2 = recboedge2.GetEndPoint(1);
                DetailLine detailLine = null;
                DetailLine detailLine1 = null;
                var p1 = new XYZ(rec.Pointupright.X, rec.Pointupright.Y, recboedge1.GetEndPoint(0).Z);
                var kc1 = p1.DistanceTo(rec.Pointupright);
                var p2 = new XYZ(rec.Pointdowright.X, rec.Pointdowright.Y, recboedge2.GetEndPoint(0).Z);
                var kc2 = p2.DistanceTo(rec.Pointdowright);
                var p3 = new XYZ(rec.Pointupleft.X, rec.Pointupleft.Y, recboedge1.GetEndPoint(0).Z);
                var p4 = new XYZ(rec.Pointdowleft.X, rec.Pointdowleft.Y, recboedge1.GetEndPoint(0).Z);
                if (Up.X > 0 || Up.Y > 0 || Up.Z > 0)
                {
                    var g1 = ElementTransformUtils.CopyElement(doc, canhdaitren.Id, -Up * (kc1 - space));
                    detailLine = doc.GetElement(g1.First()) as DetailLine;
                    var g2 = ElementTransformUtils.CopyElement(doc, canhdaiduoi.Id, Up * (kc2 - space));
                    detailLine1 = doc.GetElement(g2.First()) as DetailLine;
                }
                if (Up.X < 0 || Up.Y < 0 || Up.Z < 0)
                {
                    var g3 = ElementTransformUtils.CopyElement(doc, canhdaitren.Id, Up * (kc1 - space));
                    detailLine = doc.GetElement(g3.First()) as DetailLine;
                    var g4 = ElementTransformUtils.CopyElement(doc, canhdaiduoi.Id, -Up * (kc2 - space));
                    detailLine1 = doc.GetElement(g4.First()) as DetailLine;
                }
                listline1.Add(detailLine);
                listline1.Add(detailLine1);
                var m1 = Line.CreateBound(detailLine1.GeometryCurve.GetEndPoint(0), canhdaiduoi.GeometryCurve.GetEndPoint(0));
                var m2 = Line.CreateBound(detailLine1.GeometryCurve.GetEndPoint(1), canhdaiduoi.GeometryCurve.GetEndPoint(1));
                var m3 = Line.CreateBound(detailLine.GeometryCurve.GetEndPoint(0), canhdaitren.GeometryCurve.GetEndPoint(0));
                var m4 = Line.CreateBound(detailLine.GeometryCurve.GetEndPoint(1), canhdaitren.GeometryCurve.GetEndPoint(1));
                DetailLine v1 = doc.Create.NewDetailCurve(doc.ActiveView, m1) as DetailLine;
                DetailLine v2 = doc.Create.NewDetailCurve(doc.ActiveView, m2) as DetailLine;
                DetailLine v3 = doc.Create.NewDetailCurve(doc.ActiveView, m3) as DetailLine;
                DetailLine v4 = doc.Create.NewDetailCurve(doc.ActiveView, m4) as DetailLine;
                listline3.Add(v1);
                listline3.Add(v2);
                listline2.Add(v3);
                listline2.Add(v4);
                listline2.Add(detailLine);
                listline3.Add(detailLine1);
                Dimentionleftinsulation.Add(detailLine);
                Dimentionleftblockout.Add(detailLine);
                Dimentionleftinsulation.Add(detailLine1);
                Dimentionleftblockout.Add(detailLine1);
                hozi1 = detailLine.GeometryCurve as Line;
                hozi2 = detailLine1.GeometryCurve as Line;
                Line canh1 = Line.CreateBound(detailLine.GeometryCurve.GetEndPoint(0), detailLine1.GeometryCurve.GetEndPoint(0));
                Line canh2 = Line.CreateBound(detailLine.GeometryCurve.GetEndPoint(1), detailLine1.GeometryCurve.GetEndPoint(1));
                DetailLine detailLinecanh1 = doc.Create.NewDetailCurve(doc.ActiveView, canh1) as DetailLine;
                DetailLine detailLinecanh2 = doc.Create.NewDetailCurve(doc.ActiveView, canh2) as DetailLine;
                listline1.Add(detailLinecanh1);
                listline1.Add(detailLinecanh2);
                Dimentioninsulationtop.Add(v1);
                Dimentioninsulationtop.Add(v2);
                DimentionSheargirdtopandBlockout.Add(v1);
                DimentionSheargirdtopandBlockout.Add(v2);
                Dimentiontoptong.Add(v1);
                Dimentiontoptong.Add(v2);
                Dimentionleftinsulation.Add(canhdaitren as DetailLine);
                Dimentionleftinsulation.Add(canhdaiduoi as DetailLine);
                Dimentionleftblockout.Add(canhdaitren as DetailLine);
                Dimentionleftblockout.Add(canhdaiduoi as DetailLine);
                Dimentionlefttong.Add(canhdaitren as DetailLine);
                Dimentionlefttong.Add(canhdaiduoi as DetailLine);
                // tao line doc blockout
                for (int i = 0; i < newlist.Count; i++)
                {
                    var rectangle = newlist[i];
                    XYZ pointcenter = rectangle.Center;
                    XYZ pointrightup = XYZ.Zero;
                    XYZ pointrightdow = XYZ.Zero;
                    XYZ pointleftup = XYZ.Zero;
                    XYZ pointleftdow = XYZ.Zero;
                    var canha = rectangle.a / 2;
                    if (Math.Floor(right.X) == 0 && right.Y != 0)
                    {
                        pointrightup = new XYZ(rectangle.Pointupright.X, rectangle.Pointupright.Y - space, hozi1.GetEndPoint(0).Z);
                        pointrightdow = new XYZ(rectangle.Pointdowright.X, rectangle.Pointdowright.Y - space, hozi2.GetEndPoint(0).Z);
                        XYZ no = (pointrightup + pointrightdow) / 2;
                        var kc = pointcenter.DistanceTo(no);
                        if (kc > canha)
                        {
                            pointrightup = new XYZ(rectangle.Pointupright.X, rectangle.Pointupright.Y - space, hozi1.GetEndPoint(0).Z);
                            pointrightdow = new XYZ(rectangle.Pointdowright.X, rectangle.Pointdowright.Y - space, hozi2.GetEndPoint(0).Z);
                            pointleftup = new XYZ(rectangle.Pointupleft.X, rectangle.Pointupleft.Y + space, hozi1.GetEndPoint(0).Z);
                            pointleftdow = new XYZ(rectangle.Pointdowleft.X, rectangle.Pointdowleft.Y + space, hozi2.GetEndPoint(0).Z);
                        }
                        else
                        {
                            pointrightup = new XYZ(rectangle.Pointupright.X, rectangle.Pointupright.Y + space, hozi1.GetEndPoint(0).Z);
                            pointrightdow = new XYZ(rectangle.Pointdowright.X, rectangle.Pointdowright.Y + space, hozi2.GetEndPoint(0).Z);
                            pointleftup = new XYZ(rectangle.Pointupleft.X, rectangle.Pointupleft.Y - space, hozi1.GetEndPoint(0).Z);
                            pointleftdow = new XYZ(rectangle.Pointdowleft.X, rectangle.Pointdowleft.Y - space, hozi2.GetEndPoint(0).Z);
                        }
                    }
                    if (Math.Floor(right.X) != 0)
                    {
                        pointrightup = new XYZ(rectangle.Pointupright.X - space, rectangle.Pointupright.Y, hozi1.GetEndPoint(0).Z);
                        pointrightdow = new XYZ(rectangle.Pointdowright.X - space, rectangle.Pointdowright.Y, hozi2.GetEndPoint(0).Z);
                        XYZ no = (pointrightup + pointrightdow) / 2;
                        var kc = pointcenter.DistanceTo(no);
                        if (kc > canha)
                        {
                            pointrightup = new XYZ(rectangle.Pointupright.X - space, rectangle.Pointupright.Y, hozi1.GetEndPoint(0).Z);
                            pointrightdow = new XYZ(rectangle.Pointdowright.X - space, rectangle.Pointdowright.Y, hozi2.GetEndPoint(0).Z);
                            pointleftup = new XYZ(rectangle.Pointupleft.X + space, rectangle.Pointupleft.Y, hozi1.GetEndPoint(0).Z);
                            pointleftdow = new XYZ(rectangle.Pointdowleft.X + space, rectangle.Pointdowleft.Y, hozi2.GetEndPoint(0).Z);
                        }
                        else
                        {
                            pointrightup = new XYZ(rectangle.Pointupright.X + space, rectangle.Pointupright.Y, hozi1.GetEndPoint(0).Z);
                            pointrightdow = new XYZ(rectangle.Pointdowright.X + space, rectangle.Pointdowright.Y, hozi2.GetEndPoint(0).Z);
                            pointleftup = new XYZ(rectangle.Pointupleft.X - space, rectangle.Pointupleft.Y, hozi1.GetEndPoint(0).Z);
                            pointleftdow = new XYZ(rectangle.Pointdowleft.X - space, rectangle.Pointdowleft.Y, hozi2.GetEndPoint(0).Z);
                        }
                    }
                    Line vert1 = Line.CreateBound(pointrightup, pointrightdow);
                    Line vert2 = Line.CreateBound(pointleftup, pointleftdow);
                    DetailLine detailLine2 = doc.Create.NewDetailCurve(doc.ActiveView, vert1) as DetailLine;
                    DetailLine detailLine3 = doc.Create.NewDetailCurve(doc.ActiveView, vert2) as DetailLine;
                    listline1.Add(detailLine2);
                    listline1.Add(detailLine3);
                    DimentionSheargirdtopandBlockout.Add(detailLine2);
                    DimentionSheargirdtopandBlockout.Add(detailLine3);
                }
                tran.Commit();
            }
            List<FamilyInstance> sheargrid = GetShearGrid(doc, assemblyInstance);
            List<FamilyInstance> shearline1 = new List<FamilyInstance>();
            List<FamilyInstance> shearline2 = new List<FamilyInstance>();
            List<List<FamilyInstance>> listsheargridvert = new List<List<FamilyInstance>>();
            List<FamilyInstance> shear1 = new List<FamilyInstance>();
            List<FamilyInstance> shear2 = new List<FamilyInstance>();
            List<FamilyInstance> shear3 = new List<FamilyInstance>();
            List<FamilyInstance> shear4 = new List<FamilyInstance>();
            List<FamilyInstance> shear5 = new List<FamilyInstance>();
            List<FamilyInstance> shear6 = new List<FamilyInstance>();
            foreach (var i in sheargrid)
            {
                var XYZgrid = GetCurveSheargrid(i) as Line;
                var pi = Math.Floor(XYZgrid.Direction.DotProduct(Up));
                if (pi == 0)
                {
                    shearline1.Add(i);
                }
                if (pi != 0)
                {
                    shearline2.Add(i);
                }
            }
            shear1.Add(shearline1[0]);
            for (int i = 1; i < shearline1.Count; i++)
            {
                var rec1 = shearline1[0];
                var rec2 = shearline1[i];
                LocationPoint point1 = rec1.Location as LocationPoint;
                LocationPoint point2 = rec2.Location as LocationPoint;
                if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                {
                    shear1.Add(rec2);
                }
            }
            List<FamilyInstance> query1 = shearline1.Except(shear1).ToList();
            if (query1.Count != 0)
            {
                shear2.Add(query1[0]);
                for (int i = 1; i < query1.Count; i++)
                {
                    var rec1 = query1[0];
                    var rec2 = query1[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear2.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query2 = query1.Except(shear2).ToList();
            if (query2.Count != 0)
            {
                shear3.Add(query2[0]);
                for (int i = 1; i < query2.Count; i++)
                {
                    var rec1 = query2[0];
                    var rec2 = query2[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear3.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query3 = query2.Except(shear3).ToList();
            if (query3.Count != 0)
            {
                shear4.Add(query3[0]);
                for (int i = 1; i < query3.Count; i++)
                {
                    var rec1 = query3[0];
                    var rec2 = query3[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear4.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query4 = query3.Except(shear4).ToList();
            if (query4.Count != 0)
            {
                shear5.Add(query4[0]);
                for (int i = 1; i < query4.Count; i++)
                {
                    var rec1 = query4[0];
                    var rec2 = query4[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear5.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query5 = query4.Except(shear5).ToList();
            if (query5.Count != 0)
            {
                shear6.Add(query5[0]);
                for (int i = 1; i < query5.Count; i++)
                {
                    var rec1 = query5[0];
                    var rec2 = query5[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear6.Add(rec2);
                    }
                }
            }
            if (shear1.Count != 0)
            {
                listsheargridvert.Add(shear1);
            }
            if (shear2.Count != 0)
            {
                listsheargridvert.Add(shear2);
            }
            if (shear3.Count != 0)
            {
                listsheargridvert.Add(shear3);
            }
            if (shear4.Count != 0)
            {
                listsheargridvert.Add(shear4);
            }
            if (shear5.Count != 0)
            {
                listsheargridvert.Add(shear5);
            }
            if (shear6.Count != 0)
            {
                listsheargridvert.Add(shear6);
            }
            //tao line sheargrid doc
            foreach (var i in shearline2)
            {
                using (Transaction tr = new Transaction(doc, "Create sheargrid vertical"))
                {
                    tr.Start();
                    LocationPoint point1 = i.Location as LocationPoint;
                    XYZ pointshear1 = new XYZ(point1.Point.X, point1.Point.Y, hozi1.Origin.Z);
                    XYZ pointshear2 = new XYZ(point1.Point.X, point1.Point.Y, hozi2.Origin.Z);
                    Line lineshearhozi = Line.CreateBound(pointshear1, pointshear2);
                    DetailLine lineshearline = doc.Create.NewDetailCurve(doc.ActiveView, lineshearhozi) as DetailLine;
                    listline1.Add(lineshearline);
                    DimentionSheargirdtopandBlockout.Add(lineshearline);
                    tr.Commit();
                }
            }
            //tao line sheargrid ngang
            foreach (var j in listsheargridvert)
            {
                using (Transaction tr = new Transaction(doc, "Create sheargrid holizontal"))
                {
                    tr.Start();
                    var sh1 = j.First();
                    LocationPoint point1 = sh1.Location as LocationPoint;
                    DetailLine lineshearline = null;
                    var bm = recboedge1.Distance(point1.Point);
                    if (Up.X > 0 || Up.Y > 0 || Up.Z > 0)
                    {
                        var f1 = ElementTransformUtils.CopyElement(doc, canhdaitren.Id, -Up * bm);
                        lineshearline = doc.GetElement(f1.First()) as DetailLine;
                    }
                    if (Up.X < 0 || Up.Y < 0 || Up.Z < 0)
                    {
                        var f1 = ElementTransformUtils.CopyElement(doc, canhdaitren.Id, Up * bm);
                        lineshearline = doc.GetElement(f1.First()) as DetailLine;
                    }
                    Dimentionleftinsulation.Add(lineshearline);
                    var lineshearhozi = lineshearline.GeometryCurve as Line;
                    if (lineshearhozi.Origin.Z < hozi2.Origin.Z)
                    {
                        listline3.Add(lineshearline);
                    }
                    else
                    {
                        listline2.Add(lineshearline);
                    }
                    tr.Commit();
                }
            }
            // chia insulation
            double soluonginsu = Math.Ceiling(hozi2.Length / insulationlength);
            XYZ pointfr1x = new XYZ(recboedshort2.GetEndPoint(1).X, recboedshort2.GetEndPoint(1).Y, recboedshort2.GetEndPoint(0).Z);
            XYZ pointfr2x = new XYZ(recboedshort2.GetEndPoint(1).X, recboedshort2.GetEndPoint(1).Y, hozi1.GetEndPoint(0).Z);
            XYZ pointfr3x = new XYZ(recboedshort2.GetEndPoint(0).X, recboedshort2.GetEndPoint(0).Y, recboedshort2.GetEndPoint(1).Z);
            XYZ pointfr4x = new XYZ(recboedshort2.GetEndPoint(0).X, recboedshort2.GetEndPoint(0).Y, hozi2.GetEndPoint(1).Z);
            for (int i = 1; i < soluonginsu; i++)
            {
                using (Transaction newtr = new Transaction(doc, "line"))
                {
                    newtr.Start();
                    XYZ xYZ = right * i * insulationlength;
                    XYZ pointfr1 = null;
                    XYZ pointfr2 = null;
                    XYZ pointfr3 = null;
                    XYZ pointfr4 = null;
                    pointfr1 = pointfr1x + xYZ;
                    pointfr2 = pointfr2x + xYZ;
                    Line line1 = Line.CreateBound(pointfr1, pointfr2);
                    DetailLine detail2 = doc.Create.NewDetailCurve(doc.ActiveView, line1) as DetailLine;
                    listline2.Add(detail2);
                    listline3.Add(detail2);
                    Dimentioninsulationtop.Add(detail2);
                    pointfr3 = pointfr3x + xYZ;
                    pointfr4 = pointfr4x + xYZ;
                    Line line2 = Line.CreateBound(pointfr3, pointfr4);
                    DetailLine detail3 = doc.Create.NewDetailCurve(doc.ActiveView, line2) as DetailLine;
                    listline2.Add(detail3);
                    listline3.Add(detail3);
                    newtr.Commit();
                }
            }
            List<Rectangleslbr> rec11 = new List<Rectangleslbr>();
            List<Rectangleslbr> rec12 = new List<Rectangleslbr>();
            List<Rectangleslbr> rec13 = new List<Rectangleslbr>();
            ICollection<ElementId> ids = new List<ElementId>();
            ICollection<ElementId> ids2 = new List<ElementId>();
            ICollection<ElementId> ids3 = new List<ElementId>();
            List<List<Rectangleslbr>> list1 = new List<List<Rectangleslbr>>();
            List<List<Rectangleslbr>> list2 = new List<List<Rectangleslbr>>();
            List<List<Rectangleslbr>> list3 = new List<List<Rectangleslbr>>();
            foreach (var i in listline1)
            {
                ids.Add(i.Id);
            }
            foreach (var i in listline2)
            {
                ids2.Add(i.Id);
            }
            foreach (var i in listline3)
            {
                ids3.Add(i.Id);
            }
            var points1 = lbr.GetIntersectXYZoncurve(doc, ids);
            var points2 = lbr.GetIntersectXYZoncurve(doc, ids2);
            var points3 = lbr.GetIntersectXYZoncurve(doc, ids3);
            if (Math.Floor(right.X) == 0)
            {
                rec11 = Rectangleslbr.CreateRectangle2(doc, points1);
                rec12 = Rectangleslbr.CreateRectangle2(doc, points2);
                rec13 = Rectangleslbr.CreateRectangle2(doc, points3);

            }
            if (Math.Floor(right.X) != 0)
            {
                rec11 = Rectangleslbr.CreateRectangle(doc, points1);
                rec12 = Rectangleslbr.CreateRectangle(doc, points2);
                rec13 = Rectangleslbr.CreateRectangle(doc, points3);
            }
            foreach (var i in rec11)
            {
                rectangleslbrs.Add(i);
            }
            foreach (var i in rec12)
            {
                rectangleslbrs.Add(i);
            }
            foreach (var i in rec13)
            {
                rectangleslbrs.Add(i);
            }
            List<FamilySymbol> listsym = lbr.GetFamilySymbols(doc);
            lbr.PlaceSymmbolonRectangles(doc, listsym, rectangleslbrs);

            //lbr.PlaceSymmbolonPoint(doc, listsym, points1);
            #region Create dimmention
            Dimension d1 = CreateDimentionHolizontal(doc, Dimentioninsulationtop, recboedge1 as Line, 2);
            Dimension d2 = CreateDimentionHolizontal(doc, DimentionSheargirdtopandBlockout, recboedge1 as Line, 4);
            CreateDimentionHolizontal(doc, Dimentiontoptong, recboedge1 as Line, 6);
            Dimension e1 = CreateDimentionVertical2(doc, Dimentionleftinsulation, recboedshort2 as Line, 4);
            Dimension e2 = CreateDimentionVertical2(doc, Dimentionleftblockout, recboedshort2 as Line, 6);
            CreateDimentionVertical2(doc, Dimentionlefttong, recboedshort2 as Line, 8);
            //CreateLine2Dim(doc, e1, d1);
            //CreateLine2Dim(doc, e2, d2);
            #endregion

            return rectangleslbrs;
        }
        #endregion
        #region Draingblockout1
        public List<Rectangleslbr> DrawBlockOut1(Document doc, AssemblyInstance assemblyInstance, FamilyInstance familyInstance, FamilyInstance WALLREC, Rectangleslbr recbo, double space, double insulationlength)
        {
            View view = doc.ActiveView;
            XYZ right = view.RightDirection;
            XYZ Up = view.UpDirection;
            Insulationlbr lbr = Insulationlbr._instance;
            List<Rectangleslbr> listrec = new List<Rectangleslbr>();
            List<Rectangleslbr> rectangleslbrs = new List<Rectangleslbr>();
            List<DetailLine> listline1 = new List<DetailLine>();
            List<DetailLine> listline2 = new List<DetailLine>();
            List<DetailLine> listline3 = new List<DetailLine>();
            List<DetailLine> Dimentioninsulationtop = new List<DetailLine>();
            List<DetailLine> DimentionSheargirdtopandBlockout = new List<DetailLine>();
            List<DetailLine> Dimentiontoptong = new List<DetailLine>();
            List<DetailLine> Dimentionleftinsulation = new List<DetailLine>();
            List<DetailLine> Dimentionleftblockout = new List<DetailLine>();
            List<DetailLine> Dimentionlefttong = new List<DetailLine>();
            //lay 2 canh dai cua recbo 
            List<Rectangleslbr> newlist = new List<Rectangleslbr>();
            List<CurveLoop> curveloop = new List<CurveLoop>();
            PlanarFace topface = FindTopWallother(familyInstance);
            Transform transform = WALLREC.GetTransform();
            DetailCurve canhdaitren = null;
            DetailCurve canhdaiduoi = null;
            Curve recboedge1 = Line.CreateBound(transform.OfPoint(recbo.Pointupright), transform.OfPoint(recbo.Pointupleft));
            Curve recboedge2 = Line.CreateBound(transform.OfPoint(recbo.Pointdowright), transform.OfPoint(recbo.Pointdowleft));
            XYZ pointsource = (transform.OfPoint(recbo.Pointupright) + transform.OfPoint(recbo.Pointdowright)) / 2;
            using (Transaction tr = new Transaction(doc, "create detail line"))
            {
                tr.Start();
                canhdaitren = doc.Create.NewDetailCurve(doc.ActiveView, recboedge1);
                canhdaiduoi = doc.Create.NewDetailCurve(doc.ActiveView, recboedge2);
                tr.Commit();
            }
            listline2.Add(canhdaitren as DetailLine);
            listline3.Add(canhdaiduoi as DetailLine);
            // lay 1 canh ngan cua recbo 
            Line recboedshort1 = Line.CreateBound(transform.OfPoint(recbo.Pointupright), transform.OfPoint(recbo.Pointdowright));
            Line recboedshort2 = Line.CreateBound(transform.OfPoint(recbo.Pointupleft), transform.OfPoint(recbo.Pointdowleft));
            IList<CurveLoop> loops = topface.GetEdgesAsCurveLoops();
            foreach (var i in loops)
            {
                curveloop.Add(i);
            }
            listrec = Rectangleslbr.GetRectangles(doc, curveloop);
            for (int i = 0; i < listrec.Count; i++)
            {
                var rec = listrec[i];
                if (rec.a > 3)
                {
                    if (rec.Aleft.ApproximateLength < 10 && rec.Bbot.ApproximateLength < 10)
                    {
                        newlist.Add(rec);
                    }
                }
            }
            List<FamilyInstance> sheargrid = GetShearGrid(doc, assemblyInstance);
            List<FamilyInstance> shearline1 = new List<FamilyInstance>();
            List<FamilyInstance> shearline2 = new List<FamilyInstance>();
            List<List<FamilyInstance>> listsheargridvert = new List<List<FamilyInstance>>();
            List<FamilyInstance> shear1 = new List<FamilyInstance>();
            List<FamilyInstance> shear2 = new List<FamilyInstance>();
            List<FamilyInstance> shear3 = new List<FamilyInstance>();
            List<FamilyInstance> shear4 = new List<FamilyInstance>();
            List<FamilyInstance> shear5 = new List<FamilyInstance>();
            List<FamilyInstance> shear6 = new List<FamilyInstance>();
            foreach (var i in sheargrid)
            {
                var XYZgrid = GetCurveSheargrid(i) as Line;
                var pi = Math.Floor(XYZgrid.Direction.DotProduct(Up));
                if (pi == 0)
                {
                    shearline1.Add(i);
                }
                if (pi != 0)
                {
                    shearline2.Add(i);
                }
            }
            shear1.Add(shearline1[0]);
            for (int i = 1; i < shearline1.Count; i++)
            {
                var rec1 = shearline1[0];
                var rec2 = shearline1[i];
                LocationPoint point1 = rec1.Location as LocationPoint;
                LocationPoint point2 = rec2.Location as LocationPoint;
                if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                {
                    shear1.Add(rec2);
                }
            }
            List<FamilyInstance> query1 = shearline1.Except(shear1).ToList();
            if (query1.Count != 0)
            {
                shear2.Add(query1[0]);
                for (int i = 1; i < query1.Count; i++)
                {
                    var rec1 = query1[0];
                    var rec2 = query1[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear2.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query2 = query1.Except(shear2).ToList();
            if (query2.Count != 0)
            {
                shear3.Add(query2[0]);
                for (int i = 1; i < query2.Count; i++)
                {
                    var rec1 = query2[0];
                    var rec2 = query2[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear3.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query3 = query2.Except(shear3).ToList();
            if (query3.Count != 0)
            {
                shear4.Add(query3[0]);
                for (int i = 1; i < query3.Count; i++)
                {
                    var rec1 = query3[0];
                    var rec2 = query3[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear4.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query4 = query3.Except(shear4).ToList();
            if (query4.Count != 0)
            {
                shear5.Add(query4[0]);
                for (int i = 1; i < query4.Count; i++)
                {
                    var rec1 = query4[0];
                    var rec2 = query4[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear5.Add(rec2);
                    }
                }
            }
            List<FamilyInstance> query5 = query4.Except(shear5).ToList();
            if (query5.Count != 0)
            {
                shear6.Add(query5[0]);
                for (int i = 1; i < query5.Count; i++)
                {
                    var rec1 = query5[0];
                    var rec2 = query5[i];
                    LocationPoint point1 = rec1.Location as LocationPoint;
                    LocationPoint point2 = rec2.Location as LocationPoint;
                    if (Math.Round(point2.Point.Z, 4) == Math.Round(point1.Point.Z, 4))
                    {
                        shear6.Add(rec2);
                    }
                }
            }
            if (shear1.Count != 0)
            {
                listsheargridvert.Add(shear1);
            }
            if (shear2.Count != 0)
            {
                listsheargridvert.Add(shear2);
            }
            if (shear3.Count != 0)
            {
                listsheargridvert.Add(shear3);
            }
            if (shear4.Count != 0)
            {
                listsheargridvert.Add(shear4);
            }
            if (shear5.Count != 0)
            {
                listsheargridvert.Add(shear5);
            }
            if (shear6.Count != 0)
            {
                listsheargridvert.Add(shear6);
            }

            //tao line sheargrid ngang
            foreach (var j in listsheargridvert)
            {
                using (Transaction tr = new Transaction(doc, "Create sheargrid holizontal"))
                {
                    tr.Start();
                    var sh1 = j.First();
                    LocationPoint point1 = sh1.Location as LocationPoint;
                    DetailLine lineshearline = null;
                    XYZ newpoint = new XYZ(point1.Point.X, point1.Point.Y, recboedge1.GetEndPoint(0).Z);
                    var bm = point1.Point.DistanceTo(newpoint);
                    if (Up.X > 0 || Up.Y > 0 || Up.Z > 0)
                    {
                        var f1 = ElementTransformUtils.CopyElement(doc, canhdaitren.Id, -Up * bm);
                        lineshearline = doc.GetElement(f1.First()) as DetailLine;
                    }
                    if (Up.X < 0 || Up.Y < 0 || Up.Z < 0)
                    {
                        var f1 = ElementTransformUtils.CopyElement(doc, canhdaitren.Id, Up * bm);
                        lineshearline = doc.GetElement(f1.First()) as DetailLine;
                    }
                    Dimentionleftinsulation.Add(lineshearline);
                    var lineshearhozi = lineshearline.GeometryCurve as Line;
                    if (lineshearhozi.Origin.Z < pointsource.Z)
                    {
                        listline3.Add(lineshearline);
                    }
                    else
                    {
                        listline2.Add(lineshearline);
                    }
                    tr.Commit();
                }
            }
            DetailLine linetop = Findlineneraly(listline2, pointsource);
            listline1.Add(linetop);
            DetailLine linebot = Findlineneraly(listline3, pointsource);
            listline1.Add(linebot);
            // chia insulation
            double soluonginsu = Math.Ceiling(canhdaitren.GeometryCurve.Length / insulationlength);
            XYZ pointfr1x = new XYZ(recboedshort2.GetEndPoint(1).X, recboedshort2.GetEndPoint(1).Y, recboedshort2.GetEndPoint(0).Z);
            XYZ pointfr2x = new XYZ(recboedshort2.GetEndPoint(1).X, recboedshort2.GetEndPoint(1).Y, linetop.GeometryCurve.GetEndPoint(0).Z);
            XYZ pointfr3x = new XYZ(recboedshort2.GetEndPoint(0).X, recboedshort2.GetEndPoint(0).Y, recboedshort2.GetEndPoint(1).Z);
            XYZ pointfr4x = new XYZ(recboedshort2.GetEndPoint(0).X, recboedshort2.GetEndPoint(0).Y, linebot.GeometryCurve.GetEndPoint(1).Z);
            for (int i = 1; i < soluonginsu; i++)
            {
                using (Transaction newtr = new Transaction(doc, "line"))
                {
                    newtr.Start();
                    XYZ xYZ = right * i * insulationlength;
                    XYZ pointfr1 = null;
                    XYZ pointfr2 = null;
                    XYZ pointfr3 = null;
                    XYZ pointfr4 = null;
                    pointfr1 = pointfr1x + xYZ;
                    pointfr2 = pointfr2x + xYZ;
                    Line line1 = Line.CreateBound(pointfr1, pointfr2);
                    DetailLine detail2 = doc.Create.NewDetailCurve(doc.ActiveView, line1) as DetailLine;
                    listline2.Add(detail2);
                    listline3.Add(detail2);
                    Dimentioninsulationtop.Add(detail2);
                    pointfr3 = pointfr3x + xYZ;
                    pointfr4 = pointfr4x + xYZ;
                    Line line2 = Line.CreateBound(pointfr3, pointfr4);
                    DetailLine detail3 = doc.Create.NewDetailCurve(doc.ActiveView, line2) as DetailLine;
                    listline2.Add(detail3);
                    listline3.Add(detail3);
                    newtr.Commit();
                }
            }
            //tao line sheargrid doc
            foreach (var i in shearline2)
            {
                using (Transaction tr = new Transaction(doc, "Create sheargrid vertical"))
                {
                    tr.Start();
                    LocationPoint point1 = i.Location as LocationPoint;
                    XYZ pointshear1 = new XYZ(point1.Point.X, point1.Point.Y, (linetop.GeometryCurve as Line).Origin.Z);
                    XYZ pointshear2 = new XYZ(point1.Point.X, point1.Point.Y, (linebot.GeometryCurve as Line).Origin.Z);
                    Line lineshearhozi = Line.CreateBound(pointshear1, pointshear2);
                    DetailLine lineshearline = doc.Create.NewDetailCurve(doc.ActiveView, lineshearhozi) as DetailLine;
                    listline1.Add(lineshearline);
                    DimentionSheargirdtopandBlockout.Add(lineshearline);
                    tr.Commit();
                }
            }
            //tao line ngan 2 ben 
            using (Transaction t = new Transaction(doc, "create line"))
            {
                t.Start();
                Line line1 = Line.CreateBound(linetop.GeometryCurve.GetEndPoint(0), canhdaitren.GeometryCurve.GetEndPoint(0));
                Line line2 = Line.CreateBound(linetop.GeometryCurve.GetEndPoint(1), canhdaitren.GeometryCurve.GetEndPoint(1));
                Line line3 = Line.CreateBound(linebot.GeometryCurve.GetEndPoint(0), canhdaiduoi.GeometryCurve.GetEndPoint(0));
                Line line4 = Line.CreateBound(linebot.GeometryCurve.GetEndPoint(1), canhdaiduoi.GeometryCurve.GetEndPoint(1));
                Line line5 = Line.CreateBound(linebot.GeometryCurve.GetEndPoint(0), linetop.GeometryCurve.GetEndPoint(0));
                Line line6 = Line.CreateBound(linebot.GeometryCurve.GetEndPoint(1), linetop.GeometryCurve.GetEndPoint(1));
                var dline1 = doc.Create.NewDetailCurve(doc.ActiveView, line1) as DetailLine;
                var dline2 = doc.Create.NewDetailCurve(doc.ActiveView, line2) as DetailLine;
                var dline3 = doc.Create.NewDetailCurve(doc.ActiveView, line3) as DetailLine;
                var dline4 = doc.Create.NewDetailCurve(doc.ActiveView, line4) as DetailLine;
                var dline5 = doc.Create.NewDetailCurve(doc.ActiveView, line5) as DetailLine;
                var dline6 = doc.Create.NewDetailCurve(doc.ActiveView, line6) as DetailLine;
                listline2.Add(dline1);
                listline2.Add(dline2);
                listline3.Add(dline3);
                listline3.Add(dline4);
                listline1.Add(dline5);
                listline1.Add(dline6);
                t.Commit();
            }
            List<Rectangleslbr> rec11 = new List<Rectangleslbr>();
            List<Rectangleslbr> rec12 = new List<Rectangleslbr>();
            List<Rectangleslbr> rec13 = new List<Rectangleslbr>();
            ICollection<ElementId> ids = new List<ElementId>();
            ICollection<ElementId> ids2 = new List<ElementId>();
            ICollection<ElementId> ids3 = new List<ElementId>();
            List<List<Rectangleslbr>> list1 = new List<List<Rectangleslbr>>();
            List<List<Rectangleslbr>> list2 = new List<List<Rectangleslbr>>();
            List<List<Rectangleslbr>> list3 = new List<List<Rectangleslbr>>();
            foreach (var i in listline1)
            {
                ids.Add(i.Id);
            }
            foreach (var i in listline2)
            {
                ids2.Add(i.Id);
            }
            foreach (var i in listline3)
            {
                ids3.Add(i.Id);
            }
            var points1 = lbr.GetIntersectXYZoncurve(doc, ids);
            var points2 = lbr.GetIntersectXYZoncurve(doc, ids2);
            var points3 = lbr.GetIntersectXYZoncurve(doc, ids3);
            if (Math.Floor(right.X) == 0)
            {
                rec11 = Rectangleslbr.CreateRectangle2(doc, points1);
                rec12 = Rectangleslbr.CreateRectangle2(doc, points2);
                rec13 = Rectangleslbr.CreateRectangle2(doc, points3);

            }
            if (Math.Floor(right.X) != 0)
            {
                rec11 = Rectangleslbr.CreateRectangle(doc, points1);
                rec12 = Rectangleslbr.CreateRectangle(doc, points2);
                rec13 = Rectangleslbr.CreateRectangle(doc, points3);
            }
            foreach (var i in rec11)
            {
                rectangleslbrs.Add(i);
            }
            foreach (var i in rec12)
            {
                rectangleslbrs.Add(i);
            }
            foreach (var i in rec13)
            {
                rectangleslbrs.Add(i);
            }
            List<FamilySymbol> listsym = lbr.GetFamilySymbols(doc);
            lbr.PlaceSymmbolonRectangles(doc, listsym, rectangleslbrs);

            lbr.PlaceSymmbolonPoint(doc, listsym, points2);
            #region Create dimmention
            //Dimension d1 = CreateDimentionHolizontal(doc, Dimentioninsulationtop, recboedge1 as Line, 2);
            //Dimension d2 = CreateDimentionHolizontal(doc, DimentionSheargirdtopandBlockout, recboedge1 as Line, 4);
            //CreateDimentionHolizontal(doc, Dimentiontoptong, recboedge1 as Line, 6);
            //Dimension e1 = CreateDimentionVertical2(doc, Dimentionleftinsulation, recboedshort2 as Line, 4);
            //Dimension e2 = CreateDimentionVertical2(doc, Dimentionleftblockout, recboedshort2 as Line, 6);
            //CreateDimentionVertical2(doc, Dimentionlefttong, recboedshort2 as Line, 8);
            //CreateLine2Dim(doc, e1, d1);
            //CreateLine2Dim(doc, e2, d2);
            #endregion

            return rectangleslbrs;
        }
        public DetailLine Findlineneraly(List<DetailLine> list, XYZ xYZ)
        {
            DetailLine mb = null;
            if (list.Count > 1)
            {
                double min;
                min = list.First().GeometryCurve.Distance(xYZ);
                foreach (var i in list)
                {
                    double op = i.GeometryCurve.Distance(xYZ);
                    if (op < min)
                    {
                        min = op;
                        mb = i;
                    }
                }
            }
            else
            {
                mb = list.First();
            }
            return mb;
        }
        #endregion
        public void SortRectangleslist(List<List<Rectangleslbr>> rectangleslbrs)
        {
            for (int i = 0; i < rectangleslbrs.Count; i++)
            {
                for (int j = 0; j < rectangleslbrs.Count; j++)
                {
                    if (rectangleslbrs[i].First().Center.Z < rectangleslbrs[j].First().Center.Z)
                    {
                        var temp = rectangleslbrs[i];
                        rectangleslbrs[i] = rectangleslbrs[j];
                        rectangleslbrs[j] = temp;
                    }
                }
            }
        }
    }
}