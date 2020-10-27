using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using TVDCEG.LBR;

namespace TVDCEG
{
    public class lbr_
    {
        public static lbr_ _instance;
        private lbr_()
        {

        }
        public static lbr_ Instance => _instance ?? (_instance = new lbr_());
        public static IList<Solid> CreateSolid(Document doc, FamilyInstance familyInstance, Transform transform)
        {
            IList<Solid> solids = new List<Solid>();
            try
            {
                Curve curve = GetCurvemaxfamily(familyInstance);
                Element ele = doc.GetElement(familyInstance.Id);
                string[] parameters = new string[]
                {
                "DIM_WWF_YY",
                "DIM_LENGTH"
                };
                Parameter pa = LookupElementParameter(ele, parameters);
                double lengthcurve = pa.AsDouble();
                XYZ starpointfa = new XYZ(lengthcurve / 2, 0, 0);
                XYZ endpointfa = new XYZ(-lengthcurve / 2, 0, 0);
                XYZ startpoint = transform.OfPoint(starpointfa);
                XYZ endpoint = transform.OfPoint(endpointfa);
                XYZ norm = new XYZ((startpoint.X - endpoint.X), (startpoint.Y - endpoint.Y), (startpoint.Z - endpoint.Z));
                Plane plane = Plane.CreateByNormalAndOrigin(norm, startpoint);
                Transaction newtran = new Transaction(doc, "ss");
                newtran.Start();
                SketchPlane stk = SketchPlane.Create(doc, plane);
                XYZ pt1 = startpoint.Add(0.01 * plane.XVec).Add(0.01 * plane.YVec);
                XYZ pt2 = startpoint.Add(0.01 * plane.YVec);
                XYZ pt3 = startpoint.Add(-0.01 * plane.YVec);
                XYZ pt4 = startpoint.Add(0.01 * plane.XVec).Add(-0.01 * plane.YVec);
                XYZ pt5 = (-1) * norm;
                Line lineleft = Line.CreateBound(pt1, pt2);
                Line linetop = Line.CreateBound(pt2, pt3);
                Line lineright = Line.CreateBound(pt3, pt4);
                Line linebot = Line.CreateBound(pt4, pt1);
                CurveLoop profile = new CurveLoop();
                profile.Append(lineleft);
                profile.Append(linetop);
                profile.Append(lineright);
                profile.Append(linebot);
                IList<CurveLoop> listloop1 = new List<CurveLoop>();
                listloop1.Add(profile);
                Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(listloop1, pt5, lengthcurve);
                newtran.Commit();
                solids.Add(solid);
            }
            catch
            {
                solids = null;
            }
            return solids;
        }
        public static Parameter LookupElementParameter(Element element, string[] parameters)
        {
            Parameter parameter = null;
            foreach (string name in parameters)
            {
                parameter = element.LookupParameter(name);
                bool flag = parameter != null;
                if (flag)
                {
                    break;
                }
            }
            return parameter;
        }
        public static SketchPlane NewSketchPlanePassLine(Document doc, Line line, Transform transform)
        {
            XYZ p = line.GetEndPoint(0);
            XYZ q = line.GetEndPoint(1);
            XYZ endpoint1 = transform.OfPoint(p);
            XYZ starpoint1 = transform.OfPoint(q);
            XYZ norm;
            if (endpoint1.X == starpoint1.X)
            {
                norm = XYZ.BasisX;
            }
            else if (endpoint1.Y == starpoint1.Y)
            {
                norm = XYZ.BasisY;
            }
            else
            {
                norm = XYZ.BasisZ;
            }
            Plane plane = Plane.CreateByNormalAndOrigin(norm, starpoint1);
            SketchPlane skt = SketchPlane.Create(doc, plane);
            doc.ActiveView.SketchPlane = skt;
            return skt;
        }
        public static Curve GetCurvemaxfamily(FamilyInstance familyInstance)
        {
            List<Curve> alllines = new List<Curve>();
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
                        GeometryElement instanceGeometryElement = instance.GetSymbolGeometry();
                        foreach (GeometryObject o in instanceGeometryElement)
                        {
                            try
                            {
                                Solid solid = o as Solid;
                                if (solid != null)
                                {
                                    foreach (Edge item in solid.Edges)
                                    {
                                        alllines.Add(item.AsCurve());
                                    }
                                }
                            }
                            catch
                            {
                                Line line = o as Line;
                                alllines.Add(line as Curve);
                            }
                        }
                        curvesmax = Findcurvemax(alllines);
                    }
                }
            }
            return curvesmax;
        }
        static Curve Findcurvemax(List<Curve> curves)
        {
            Curve max = curves.First();
            foreach (var item in curves)
            {
                if (max.ApproximateLength < item.ApproximateLength)
                {
                    max = item;
                }
            }
            return max;
        }
        public static Curve GetCurveminfamily(FamilyInstance familyInstance)
        {
            IList<Curve> alllines = new List<Curve>();
            Curve curvesmin = null;
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
                        GeometryElement instanceGeometryElement = instance.GetSymbolGeometry();
                        foreach (GeometryObject o in instanceGeometryElement)
                        {
                            Line curve = o as Line;
                            if (curve != null)
                            {
                                alllines.Add(curve);
                            }
                        }
                        var membercurvmax = alllines.OrderByDescending(x => x.Length).LastOrDefault();
                        curvesmin = membercurvmax;
                    }
                }
            }
            return curvesmin;
        }

        public List<ReferencePlane> GetSketchPlanes(Document doc)
        {
            List<ReferencePlane> listskp = new List<ReferencePlane>();
            FilteredElementCollector colec = new FilteredElementCollector(doc).OfClass(typeof(ReferencePlane));
            var lop = colec.ToElements();
            foreach (var i in lop)
            {
                var i2 = i as ReferencePlane;
                listskp.Add(i2);
            }
            listskp.OrderBy(x => x.Name).ToList();
            return listskp;
        }
        public void FindSketchPlanesnearly(FamilyInstance familyInstance, Document doc)
        {
            using (Transaction tran = new Transaction(doc, "Unpin"))
            {
                tran.Start();
                if (familyInstance.Pinned == true)
                {
                    familyInstance.Pinned = false;
                }
                tran.Commit();
            }
            LocationPoint locationPoint = familyInstance.Location as LocationPoint;
            XYZ poi = locationPoint.Point;
            XYZ locsau = XYZ.Zero;
            XYZ dri2 = familyInstance.HandOrientation;
            XYZ lk = familyInstance.FacingOrientation;
            Transform transformsource = familyInstance.GetTransform();
            FamilyInstance a = null;
            var levelnear = GetLevelnearly(doc, familyInstance);
            var Girdnear = GetGridnearly(doc, familyInstance);
            var referenealy = GetReferencenearly(doc, familyInstance);
            if (levelnear.Count != 0)
            {
                if (referenealy.Count != 0)
                {
                    if (levelnear.Keys.First() < referenealy.Keys.First())
                    {
                        using (Transaction tran = new Transaction(doc, "cres"))
                        {
                            tran.Start();
                            FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                            MyPreProcessor preproccessor = new MyPreProcessor();
                            options.SetClearAfterRollback(true);
                            options.SetFailuresPreprocessor(preproccessor);
                            tran.SetFailureHandlingOptions(options);
                            FamilySymbol symbol = familyInstance.Symbol;
                            var LV = levelnear.Values.First().GetPlaneReference();
                            a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                            Mathparameter(doc, familyInstance, a);
                            tran.Commit(options);
                        }
                    }
                    else
                    {
                        using (Transaction tran = new Transaction(doc, "cres"))
                        {
                            tran.Start();
                            FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                            MyPreProcessor preproccessor = new MyPreProcessor();
                            options.SetClearAfterRollback(true);
                            options.SetFailuresPreprocessor(preproccessor);
                            tran.SetFailureHandlingOptions(options);
                            FamilySymbol symbol = familyInstance.Symbol;
                            a = doc.Create.NewFamilyInstance(GetReferencenearly(doc, familyInstance).Values.First().GetReference(), poi, dri2, symbol);
                            Mathparameter(doc, familyInstance, a);
                            tran.Commit(options);
                        }
                    }
                }
                else
                {
                    using (Transaction tran = new Transaction(doc, "cres"))
                    {
                        tran.Start();
                        FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                        MyPreProcessor preproccessor = new MyPreProcessor();
                        options.SetClearAfterRollback(true);
                        options.SetFailuresPreprocessor(preproccessor);
                        tran.SetFailureHandlingOptions(options);
                        FamilySymbol symbol = familyInstance.Symbol;
                        var LV = levelnear.Values.First().GetPlaneReference();
                        a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                        Mathparameter(doc, familyInstance, a);
                        tran.Commit(options);
                    }
                }

            }
            else
            {
                if (referenealy.Count != 0)
                {
                    if (Girdnear.Count != 0)
                    {
                        if (Girdnear.Keys.First() < referenealy.Keys.First())
                        {
                            using (Transaction tran = new Transaction(doc, "cres"))
                            {
                                tran.Start();
                                FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                                MyPreProcessor preproccessor = new MyPreProcessor();
                                options.SetClearAfterRollback(true);
                                options.SetFailuresPreprocessor(preproccessor);
                                tran.SetFailureHandlingOptions(options);
                                FamilySymbol symbol = familyInstance.Symbol;
                                var LV = Girdnear.Values.First().Curve.Reference;
                                a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                                Mathparameter(doc, familyInstance, a);
                                tran.Commit(options);
                            }
                        }
                        else
                        {
                            using (Transaction tran = new Transaction(doc, "cres"))
                            {
                                tran.Start();
                                FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                                MyPreProcessor preproccessor = new MyPreProcessor();
                                options.SetClearAfterRollback(true);
                                options.SetFailuresPreprocessor(preproccessor);
                                tran.SetFailureHandlingOptions(options);
                                FamilySymbol symbol = familyInstance.Symbol;
                                a = doc.Create.NewFamilyInstance(referenealy.First().Value.GetReference(), poi, dri2, symbol);
                                Mathparameter(doc, familyInstance, a);
                                tran.Commit(options);
                            }
                        }
                    }
                    else
                    {
                        using (Transaction tran = new Transaction(doc, "cres"))
                        {
                            tran.Start();
                            FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                            MyPreProcessor preproccessor = new MyPreProcessor();
                            options.SetClearAfterRollback(true);
                            options.SetFailuresPreprocessor(preproccessor);
                            tran.SetFailureHandlingOptions(options);
                            FamilySymbol symbol = familyInstance.Symbol;
                            a = doc.Create.NewFamilyInstance(GetReferencenearly(doc, familyInstance).Values.First().GetReference(), poi, dri2, symbol);
                            Mathparameter(doc, familyInstance, a);
                            tran.Commit(options);
                        }
                    }

                }
                else
                {
                    using (Transaction tran = new Transaction(doc, "cres"))
                    {
                        tran.Start();
                        FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                        MyPreProcessor preproccessor = new MyPreProcessor();
                        options.SetClearAfterRollback(true);
                        options.SetFailuresPreprocessor(preproccessor);
                        tran.SetFailureHandlingOptions(options);
                        FamilySymbol symbol = familyInstance.Symbol;
                        var LV = Girdnear.Values.First().Curve.Reference;
                        a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                        Mathparameter(doc, familyInstance, a);
                        tran.Commit(options);
                    }
                }

            }
            #region Modifyelement
            Transform transformtarget = a.GetTransform();
            if (transformsource != transformtarget)
            {
                using (Transaction tr = new Transaction(doc, "flip workplane"))
                {
                    tr.Start();
                    XYZ facing = a.FacingOrientation;
                    if (facing.Equalpoint(familyInstance.FacingOrientation) == false)
                    {
                        a.FlipWorkPlane();
                    }
                    tr.Commit();
                }
            }
            using (Transaction t = new Transaction(doc, "Move element"))
            {
                t.Start();
                FailureHandlingOptions options = t.GetFailureHandlingOptions();
                MyPreProcessor preproccessor = new MyPreProcessor();
                options.SetClearAfterRollback(true);
                options.SetFailuresPreprocessor(preproccessor);
                t.SetFailureHandlingOptions(options);
                doc.Delete(familyInstance.Id);
                LocationPoint locp = a.Location as LocationPoint;
                XYZ poik = locp.Point;
                if (poik.Equalpoint(poi) == false)
                {
                    XYZ vectorx = new XYZ(poik.X - poi.X, 0, 0);
                    if (vectorx.Equalpoint(XYZ.Zero) == false)
                    {
                        ElementTransformUtils.MoveElement(doc, a.Id, -vectorx);
                        XYZ pom = (a.Location as LocationPoint).Point;
                        double kc = Math.Round(poi.DistanceTo(pom), 0);
                        if (kc != 0)
                        {
                            ElementTransformUtils.MoveElement(doc, a.Id, vectorx * 2);
                        }
                    }
                    XYZ vectory = new XYZ(0, poik.Y - poi.Y, 0);
                    if (vectory.Equalpoint(XYZ.Zero) == false)
                    {
                        ElementTransformUtils.MoveElement(doc, a.Id, -vectory);
                        XYZ pom = (a.Location as LocationPoint).Point;
                        double kc = Math.Round(poi.DistanceTo(pom), 0);
                        if (kc != 0)
                        {
                            ElementTransformUtils.MoveElement(doc, a.Id, vectory * 2);
                        }
                    }
                    XYZ vectorz = new XYZ(0, 0, poik.Z - poi.Z);
                    if (vectorz.Equalpoint(XYZ.Zero) == false)
                    {
                        ElementTransformUtils.MoveElement(doc, a.Id, -vectorz);
                        XYZ pom = (a.Location as LocationPoint).Point;
                        double kc = Math.Round(poi.DistanceTo(pom), 0);
                        if (kc != 0)
                        {
                            ElementTransformUtils.MoveElement(doc, a.Id, vectorz * 2);
                        }
                    }
                }
                t.Commit(options);
            }
            #endregion
        }
        public void SetNewHost(FamilyInstance familyInstance, Document doc)
        {
            using (Transaction tran = new Transaction(doc, "Unpin"))
            {
                tran.Start();
                if (familyInstance.Pinned == true)
                {
                    familyInstance.Pinned = false;
                }
                tran.Commit();
            }
            LocationPoint locationPoint = familyInstance.Location as LocationPoint;
            XYZ poi = locationPoint.Point;
            XYZ locsau = XYZ.Zero;
            XYZ dri2 = familyInstance.HandOrientation;
            XYZ lk = familyInstance.FacingOrientation;
            var H = familyInstance.GetReferences(FamilyInstanceReferenceType.StrongReference);
            Transform transformsource = familyInstance.GetTransform();
            FamilyInstance a = null;
            var levelnear = GetLevelnearly(doc, familyInstance);
            var Girdnear = GetGridnearly(doc, familyInstance);
            var referenealy = GetReferencenearly(doc, familyInstance);
            if (levelnear.Count != 0)
            {
                if (referenealy.Count != 0)
                {
                    if (levelnear.Keys.First() < referenealy.Keys.First())
                    {
                        using (Transaction tran = new Transaction(doc, "cres"))
                        {
                            tran.Start();
                            FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                            MyPreProcessor preproccessor = new MyPreProcessor();
                            options.SetClearAfterRollback(true);
                            options.SetFailuresPreprocessor(preproccessor);
                            tran.SetFailureHandlingOptions(options);
                            FamilySymbol symbol = familyInstance.Symbol;
                            var LV = levelnear.Values.First().GetPlaneReference();
                            a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                            Mathparameter(doc, familyInstance, a);
                            tran.Commit(options);
                        }
                    }
                    else
                    {
                        using (Transaction tran = new Transaction(doc, "cres"))
                        {
                            tran.Start();
                            FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                            MyPreProcessor preproccessor = new MyPreProcessor();
                            options.SetClearAfterRollback(true);
                            options.SetFailuresPreprocessor(preproccessor);
                            tran.SetFailureHandlingOptions(options);
                            FamilySymbol symbol = familyInstance.Symbol;
                            a = doc.Create.NewFamilyInstance(GetReferencenearly(doc, familyInstance).Values.First().GetReference(), poi, dri2, symbol);
                            Mathparameter(doc, familyInstance, a);
                            tran.Commit(options);
                        }
                    }
                }
                else
                {
                    using (Transaction tran = new Transaction(doc, "cres"))
                    {
                        tran.Start();
                        FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                        MyPreProcessor preproccessor = new MyPreProcessor();
                        options.SetClearAfterRollback(true);
                        options.SetFailuresPreprocessor(preproccessor);
                        tran.SetFailureHandlingOptions(options);
                        FamilySymbol symbol = familyInstance.Symbol;
                        var LV = levelnear.Values.First().GetPlaneReference();
                        a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                        Mathparameter(doc, familyInstance, a);
                        tran.Commit(options);
                    }
                }

            }
            else
            {
                if (referenealy.Count != 0)
                {
                    if (Girdnear.Count != 0)
                    {
                        if (Girdnear.Keys.First() < referenealy.Keys.First())
                        {
                            using (Transaction tran = new Transaction(doc, "cres"))
                            {
                                tran.Start();
                                FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                                MyPreProcessor preproccessor = new MyPreProcessor();
                                options.SetClearAfterRollback(true);
                                options.SetFailuresPreprocessor(preproccessor);
                                tran.SetFailureHandlingOptions(options);
                                FamilySymbol symbol = familyInstance.Symbol;
                                var LV = Girdnear.Values.First().Curve.Reference;
                                a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                                Mathparameter(doc, familyInstance, a);
                                tran.Commit(options);
                            }
                        }
                        else
                        {
                            using (Transaction tran = new Transaction(doc, "cres"))
                            {
                                tran.Start();
                                FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                                MyPreProcessor preproccessor = new MyPreProcessor();
                                options.SetClearAfterRollback(true);
                                options.SetFailuresPreprocessor(preproccessor);
                                tran.SetFailureHandlingOptions(options);
                                FamilySymbol symbol = familyInstance.Symbol;
                                a = doc.Create.NewFamilyInstance(referenealy.First().Value.GetReference(), poi, dri2, symbol);
                                Mathparameter(doc, familyInstance, a);
                                tran.Commit(options);
                            }
                        }
                    }
                    else
                    {
                        using (Transaction tran = new Transaction(doc, "cres"))
                        {
                            tran.Start();
                            FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                            MyPreProcessor preproccessor = new MyPreProcessor();
                            options.SetClearAfterRollback(true);
                            options.SetFailuresPreprocessor(preproccessor);
                            tran.SetFailureHandlingOptions(options);
                            FamilySymbol symbol = familyInstance.Symbol;
                            a = doc.Create.NewFamilyInstance(GetReferencenearly(doc, familyInstance).Values.First().GetReference(), poi, dri2, symbol);
                            Mathparameter(doc, familyInstance, a);
                            tran.Commit(options);
                        }
                    }

                }
                else
                {
                    using (Transaction tran = new Transaction(doc, "cres"))
                    {
                        tran.Start();
                        FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                        MyPreProcessor preproccessor = new MyPreProcessor();
                        options.SetClearAfterRollback(true);
                        options.SetFailuresPreprocessor(preproccessor);
                        tran.SetFailureHandlingOptions(options);
                        FamilySymbol symbol = familyInstance.Symbol;
                        var LV = Girdnear.Values.First().Curve.Reference;
                        a = doc.Create.NewFamilyInstance(LV, poi, dri2, symbol);
                        Mathparameter(doc, familyInstance, a);
                        tran.Commit(options);
                    }
                }

            }
            #region Modifyelement
            Transform transformtarget = a.GetTransform();
            if (transformsource != transformtarget)
            {
                using (Transaction tr = new Transaction(doc, "flip workplane"))
                {
                    tr.Start();
                    XYZ facing = a.FacingOrientation;
                    if (facing.Equalpoint(familyInstance.FacingOrientation) == false)
                    {
                        a.FlipWorkPlane();
                    }
                    tr.Commit();
                }
            }
            using (Transaction t = new Transaction(doc, "Move element"))
            {
                t.Start();
                FailureHandlingOptions options = t.GetFailureHandlingOptions();
                MyPreProcessor preproccessor = new MyPreProcessor();
                options.SetClearAfterRollback(true);
                options.SetFailuresPreprocessor(preproccessor);
                t.SetFailureHandlingOptions(options);
                doc.Delete(familyInstance.Id);
                LocationPoint locp = a.Location as LocationPoint;
                XYZ poik = locp.Point;
                if (poik.Equalpoint(poi) == false)
                {
                    XYZ vectorx = new XYZ(poik.X - poi.X, 0, 0);
                    if (vectorx.Equalpoint(XYZ.Zero) == false)
                    {
                        ElementTransformUtils.MoveElement(doc, a.Id, -vectorx);
                        XYZ pom = (a.Location as LocationPoint).Point;
                        double kc = Math.Round(poi.DistanceTo(pom), 0);
                        if (kc != 0)
                        {
                            ElementTransformUtils.MoveElement(doc, a.Id, vectorx * 2);
                        }
                    }
                    XYZ vectory = new XYZ(0, poik.Y - poi.Y, 0);
                    if (vectory.Equalpoint(XYZ.Zero) == false)
                    {
                        ElementTransformUtils.MoveElement(doc, a.Id, -vectory);
                        XYZ pom = (a.Location as LocationPoint).Point;
                        double kc = Math.Round(poi.DistanceTo(pom), 0);
                        if (kc != 0)
                        {
                            ElementTransformUtils.MoveElement(doc, a.Id, vectory * 2);
                        }
                    }
                    XYZ vectorz = new XYZ(0, 0, poik.Z - poi.Z);
                    if (vectorz.Equalpoint(XYZ.Zero) == false)
                    {
                        ElementTransformUtils.MoveElement(doc, a.Id, -vectorz);
                        XYZ pom = (a.Location as LocationPoint).Point;
                        double kc = Math.Round(poi.DistanceTo(pom), 0);
                        if (kc != 0)
                        {
                            ElementTransformUtils.MoveElement(doc, a.Id, vectorz * 2);
                        }
                    }
                }
                t.Commit(options);
            }
            #endregion
        }
        public void Mathparameter(Document doc, FamilyInstance familyInstance1, FamilyInstance familyInstance2)
        {
            ParameterSet parameterSet1 = familyInstance1.Parameters;
            foreach (var i in parameterSet1)
            {
                var i1 = i as Parameter;
                if (i1.IsReadOnly == true) continue;
                else
                {
                    if (i1.HasValue == true)
                    {
                        var giatri = i1.AsDouble();
                        var pataget = familyInstance2.LookupParameter(i1.Definition.Name);
                        if (pataget.Definition.Name.Equals("Offset")) continue;
                        else
                        {
                            pataget.Set(giatri);
                        }
                    }
                }
            }
        }
        public Dictionary<double, ReferencePlane> GetReferencenearly(Document doc, FamilyInstance familyInstance)
        {
            Dictionary<double, ReferencePlane> dicreferenceplane = new Dictionary<double, ReferencePlane>();
            ReferencePlane reference = null;
            double min = 300;
            List<ReferencePlane> referencePlanelist = new List<ReferencePlane>();
            Transform transform = familyInstance.GetTransform();
            XYZ xYZ1 = transform.BasisZ;
            var referencePlanes = new FilteredElementCollector(doc).OfClass(typeof(ReferencePlane)).Cast<ReferencePlane>();
            foreach (var item in referencePlanes)
            {
                if (item.Normal.IsParallel(xYZ1))
                {
                    referencePlanelist.Add(item);
                }
            }
            LocationPoint locationPoint = familyInstance.Location as LocationPoint;
            XYZ point = locationPoint.Point;
            foreach (var i in referencePlanelist)
            {
                Plane plane = i.GetPlane();
                UV uuv = new UV();
                double ii;
                plane.Project(point, out uuv, out ii);
                if (ii < min)
                {
                    min = ii;
                    reference = i;
                }
                //SketchPlane skt = SketchPlane.Create(doc, plane);
                //doc.ActiveView.SketchPlane = skt;
            }
            if (reference != null)
            {
                dicreferenceplane.Add(min, reference);
            }
            return dicreferenceplane;
        }
        public Dictionary<double, Level> GetLevelnearly(Document doc, FamilyInstance familyInstance)
        {
            Dictionary<double, Level> diclevel = new Dictionary<double, Level>();
            Level level = null;
            double min = 300;
            var listlevels = new FilteredElementCollector(doc).OfClass(typeof(Level)).Cast<Level>();
            Transform transform = familyInstance.GetTransform();
            XYZ xYZ1 = transform.BasisZ;
            LocationPoint locationPoint = familyInstance.Location as LocationPoint;
            BoundingBoxXYZ boundingBoxXYZ = familyInstance.get_BoundingBox(doc.ActiveView);
            XYZ poitbou = boundingBoxXYZ.Min;
            if (xYZ1.Z != 0)
            {
                foreach (var i in listlevels)
                {
                    var Elevationlevel = i.Elevation;
                    double space = Math.Abs(poitbou.Z) - Math.Abs(Elevationlevel);
                    if (space > 0 && space < min)
                    {
                        min = space;
                        level = i;
                    }
                }
                if (level != null)
                {
                    diclevel.Add(min, level);
                }
            }
            return diclevel;
        }
        public Dictionary<double, Grid> GetGridnearly(Document doc, FamilyInstance familyInstance)
        {
            Dictionary<double, Grid> dicgrid = new Dictionary<double, Grid>();
            Grid grid = null;
            double min = 300;
            var listgrids = new FilteredElementCollector(doc).OfClass(typeof(Grid)).Cast<Grid>();
            Transform transform = familyInstance.GetTransform();
            XYZ xYZ1 = transform.BasisZ;
            LocationPoint locationPoint = familyInstance.Location as LocationPoint;
            BoundingBoxXYZ boundingBoxXYZ = familyInstance.get_BoundingBox(doc.ActiveView);
            XYZ poitbou = boundingBoxXYZ.Min;
            if (xYZ1.Z == 0)
            {
                foreach (var i in listgrids)
                {
                    var gridcurve = i.Curve;
                    var kc = gridcurve.Distance(locationPoint.Point);
                    if (kc < min)
                    {
                        min = kc;
                        grid = i;
                    }
                }
                if (grid != null)
                {
                    dicgrid.Add(min, grid);
                }
            }
            return dicgrid;
        }
        public List<FamilyInstance> Getallconnection(Document doc)
        {
            List<FamilyInstance> listconns = new List<FamilyInstance>();
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var vla = pa.AsString();
                    if (vla.Contains("CONNECTION"))
                    {
                        listconns.Add(i);
                    }
                }
            }
            return listconns;
        }
        public List<FamilyInstance> GetallconnectionActiveview(Document doc, View view)
        {
            List<FamilyInstance> listconns = new List<FamilyInstance>();
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var vla = pa.AsString();
                    if (vla.Contains("CONNECTION"))
                    {
                        listconns.Add(i);
                    }
                }
            }
            return listconns;
        }
        public List<PlanarFace> GetFacefamily(Document doc, FamilyInstance familyInstance)
        {
            List<PlanarFace> face = new List<PlanarFace>();
            Options option = new Options();
            option.ComputeReferences = true;
            option.IncludeNonVisibleObjects = true;
            option.DetailLevel = ViewDetailLevel.Undefined;
            if (familyInstance != null)
            {
                GeometryElement geoElement = familyInstance.get_Geometry(option);
                foreach (GeometryObject geoObject in geoElement)
                {
                    GeometryInstance instance = geoObject as GeometryInstance;
                    if (instance != null)
                    {
                        FamilySymbol familySymbol = instance.Symbol as FamilySymbol;
                        GeometryElement instancegeotryElement = familySymbol.get_Geometry(option);
                        foreach (GeometryObject insto in instancegeotryElement)
                        {
                            Solid solid = insto as Solid;
                            if (solid != null)
                            {
                                foreach (PlanarFace geoFace in solid.Faces)
                                {
                                    face.Add(geoFace);
                                }
                            }
                        }
                    }
                }
            }
            return face;
        }
        public PlanarFace Getbottom(List<PlanarFace> planarFaces)
        {
            PlanarFace Bottom = null;
            PlanarFace planar = planarFaces.First();
            foreach (PlanarFace i in planarFaces)
            {
                Bottom = planar;
                if (i.Origin.Z < Bottom.Origin.Z)
                {
                    Bottom = i;
                }
            }
            return Bottom;
        }
        public static List<FamilySymbol> StrandsPo(Document doc, List<Family> families)
        {
            List<FamilySymbol> Conns = new List<FamilySymbol>();
            foreach (Family Instance in families)
            {
                ElementId eleid = Instance.GetFamilySymbolIds().ElementAt(0);
                Element elemtype = doc.GetElement(eleid);
                FamilySymbol fasy = elemtype as FamilySymbol;
                Conns.Add(fasy);
            }
            return Conns;
        }
    }
    public class MyPreProcessor : IFailuresPreprocessor
    {
        public string ErrorMessage { set; get; }
        public string ErrorSeverity { set; get; }

        public MyPreProcessor()
        {
            ErrorMessage = "";
            ErrorSeverity = "";
        }

        public FailureProcessingResult PreprocessFailures(
          FailuresAccessor failuresAccessor)
        {
            IList<FailureMessageAccessor> failureMessages
              = failuresAccessor.GetFailureMessages();

            foreach (FailureMessageAccessor
              failureMessageAccessor in failureMessages)
            {
                // We're just deleting all of the warning level 
                // failures and rolling back any others

                FailureDefinitionId id = failureMessageAccessor
                  .GetFailureDefinitionId();

                try
                {
                    ErrorMessage = failureMessageAccessor
                      .GetDescriptionText();
                }
                catch
                {
                    ErrorMessage = "Unknown Error";
                }

                try
                {
                    FailureSeverity failureSeverity
                      = failureMessageAccessor.GetSeverity();

                    ErrorSeverity = failureSeverity.ToString();

                    if (failureSeverity == FailureSeverity.Warning)
                    {
                        failuresAccessor.DeleteWarning(
                          failureMessageAccessor);
                    }
                    else
                    {
                        return FailureProcessingResult
                          .ProceedWithRollBack;
                    }
                }
                catch
                {
                }
            }
            return FailureProcessingResult.Continue;
        }
        //FailureProcessingResult IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
        //{
        //    try
        //    {
        //        String transactionName = failuresAccessor.GetTransactionName();
        //        IList<FailureMessageAccessor> fmas = failuresAccessor.GetFailureMessages();
        //        if (fmas.Count == 0)
        //            return FailureProcessingResult.Continue;
        //        if (transactionName.Equals("EXEMPLE"))
        //        {
        //            foreach (FailureMessageAccessor fma in fmas)
        //            {
        //                if (fma.GetSeverity() == FailureSeverity.Warning)
        //                {
        //                    failuresAccessor.DeleteAllWarnings();
        //                    return FailureProcessingResult.ProceedWithRollBack;
        //                }
        //                else
        //                {
        //                    failuresAccessor.DeleteWarning(fma);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            foreach (FailureMessageAccessor fma in fmas)
        //            {
        //                failuresAccessor.DeleteAllWarnings();
        //            }
        //        }
        //    }
        //    catch
        //    {

        //    }
        //    return FailureProcessingResult.Continue;
        //}
    }
}
