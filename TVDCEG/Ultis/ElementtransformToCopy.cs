using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using TVDCEG.LBR;
using TVDCEG.Ultis;
using Autodesk.Revit.UI.Selection;
using MaterialDesignThemes.Wpf;

namespace TVDCEG
{
    public class ElementtransformToCopy
    {
        public void PlaceViewToSheet(Document doc, Autodesk.Revit.DB.View view, ViewSheet sheet)
        {

        }
        public static Transform TransformToCopy(FamilyInstance instance1, FamilyInstance instance2)
        {
            Transform transform1 = instance1.GetTransform();
            Transform transform2 = instance2.GetTransform();
            return transform1.Multiply(transform2.Inverse);
        }
        public static Transform TransformFlatWapred(Document doc, FamilyInstance instance1, FamilyInstance instance2)
        {
            Transform Newtransform = null;
            BoundingBoxXYZ bb1 = instance1.get_BoundingBox(doc.ActiveView);
            Transform transform1 = bb1.Transform;
            BoundingBoxXYZ bb2 = instance2.get_BoundingBox(doc.ActiveView);
            Transform transform2 = bb2.Transform;
            Newtransform = transform1.Multiply(transform2.Inverse);
            return Newtransform;
        }
        public static FamilyInstance Elementcopy(Document doc, AssemblyInstance assembly)
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
        public FamilyInstance GetFlat(Document doc, FamilyInstance familyInstance)
        {
            FamilyInstance flat = null;
            foreach (var i in familyInstance.GetSubComponentIds())
            {
                Element ele = doc.GetElement(i);
                if (ele.Name.Contains("FLAT"))
                {
                    flat = ele as FamilyInstance;
                }
            }
            return flat;
        }
        public FamilyInstance GetWarped(Document doc, FamilyInstance familyInstance)
        {
            FamilyInstance WARPED = null;
            foreach (var i in familyInstance.GetSubComponentIds())
            {
                Element ele = doc.GetElement(i);
                if (ele.Name.Contains("WARPED"))
                {
                    WARPED = ele as FamilyInstance;
                }
            }
            return WARPED;
        }
        public void CopyElements(Document doc, FamilyInstance familyInstance, List<FamilyInstance> listinstance, ICollection<ElementId> elementIds)
        {
            ICollection<ElementId> newlist = new List<ElementId>();
            CopyPasteOptions option = new CopyPasteOptions();
            ProgressBarform progressBarform = new ProgressBarform(listinstance.Count, "Loading...");
            progressBarform.Show();
            foreach (FamilyInstance source in listinstance)
            {
                progressBarform.giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                Transform transform = TransformToCopy(source, familyInstance);
                Transform transform1 = Transform.CreateTranslation(transform.Origin);
                using (Transaction tran = new Transaction(doc, "copy"))
                {
                    tran.Start();
                    FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                    IgnoreProcess ignoreProcess = new IgnoreProcess();
                    options.SetClearAfterRollback(true);
                    options.SetFailuresPreprocessor(ignoreProcess);
                    tran.SetFailureHandlingOptions(options);
                    try
                    {
                        newlist = ElementTransformUtils.CopyElements(doc, elementIds, doc, transform, option);
                        Remove_product(doc, newlist);
                    }
                    catch (Exception)
                    {

                    }
                    tran.Commit();
                }
            }
            progressBarform.Close();
        }
        public void CopyElementsConnFlangeDtee(Document doc, FamilyInstance familyInstance, List<FamilyInstance> listinstance, ICollection<ElementId> elementIds, bool valuekey)
        {
            ICollection<ElementId> newlist = new List<ElementId>();
            Parameter pa1 = familyInstance.LookupParameter("Flange_Edge_Offset_Right");
            double Flange_Right1 = pa1.AsDouble();
            Parameter pa2 = familyInstance.LookupParameter("Flange_Edge_Offset_Left");
            double Flange_Left1 = pa2.AsDouble();
            Parameter pal = familyInstance.LookupParameter("DIM_LENGTH");
            double dim_length = pal.AsDouble();
            var f1 = elementIds.First();
            double kl = Nut(doc, familyInstance, f1);
            CopyPasteOptions option = new CopyPasteOptions();
            ProgressBarform progressBarform = new ProgressBarform(listinstance.Count, "Loading...");
            progressBarform.Show();
            foreach (FamilyInstance source in listinstance)
            {
                if (source.Id != familyInstance.Id)
                {
                    using (Transaction tran = new Transaction(doc, "Copy"))
                    {
                        tran.Start();
                        Transform transform1 = source.GetTransform();
                        Parameter pa3 = source.LookupParameter("Flange_Edge_Offset_Right");
                        double Flange_Right = pa3.AsDouble();
                        ElementId sourceid = source.GetTypeId();
                        Element sourcetype = doc.GetElement(sourceid);
                        Parameter sourcepa = sourcetype.LookupParameter("DT_Stem_Spacing_Form");
                        Parameter pa4 = source.LookupParameter("Flange_Edge_Offset_Left");
                        double Flange_Left = pa4.AsDouble();
                        double val1 = Flange_Right1 - Flange_Right;
                        double val2 = Flange_Left1 - Flange_Left;
                        progressBarform.giatri();
                        if (progressBarform.iscontinue == false)
                        {
                            break;
                        }
                        Transform transform = TransformToCopy(source, familyInstance);
                        FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                        IgnoreProcess ignoreProcess = new IgnoreProcess();
                        options.SetClearAfterRollback(true);
                        options.SetFailuresPreprocessor(ignoreProcess);
                        tran.SetFailureHandlingOptions(options);
                        try
                        {
                            newlist = ElementTransformUtils.CopyElements(doc, elementIds, doc, transform, option);
                            Remove_product(doc, newlist);
                        }
                        catch (Exception)
                        {

                        }
                        if (valuekey == true)
                        {
                            if (sourcepa == null)
                            {
                                if (val1 != 0 || val2 != 0)
                                {
                                    FamilyInstance flatsource = GetFlat(doc, familyInstance);
                                    FamilyInstance flattarget = GetFlat(doc, source);
                                    List<PlanarFace> planarFacessource = FlFaces(flatsource);
                                    List<PlanarFace> planarFacetarget = FlFaces(flattarget);
                                    Element elesource = doc.GetElement(elementIds.First());
                                    double spatarget = 0;
                                    foreach (ElementId i in newlist)
                                    {
                                        Element eletarget = doc.GetElement(i);
                                        LocationPoint locationPoint2 = eletarget.Location as LocationPoint;
                                        XYZ pointtarget = locationPoint2.Point;
                                        spatarget = DistanceToMin(doc, source, planarFacetarget, pointtarget, kl);
                                        if (spatarget != 0)
                                        {
                                            break;
                                        }
                                    }
                                    if (spatarget != 0)
                                    {
                                        foreach (ElementId i in newlist)
                                        {
                                            XYZ point1 = new XYZ(0, 0, 0);
                                            point1 = point1 + transform1.BasisX * -spatarget;
                                            ElementTransformUtils.MoveElement(doc, i, point1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (val2 > 0)
                                {
                                    foreach (ElementId i in newlist)
                                    {
                                        XYZ point1 = new XYZ(0, 0, 0);
                                        point1 = point1 + transform1.BasisX * -val2;
                                        ElementTransformUtils.MoveElement(doc, i, point1);
                                    }
                                }
                                else
                                {
                                    foreach (ElementId i in newlist)
                                    {
                                        XYZ point1 = new XYZ(0, 0, 0);
                                        point1 = point1 + transform1.BasisX * -val2;
                                        ElementTransformUtils.MoveElement(doc, i, point1);
                                    }
                                }
                            }

                        }
                        if (valuekey == false)
                        {
                            if (sourcepa == null)
                            {
                                if (val1 != 0 || val2 != 0)
                                {
                                    FamilyInstance flatsource = GetFlat(doc, familyInstance);
                                    FamilyInstance flattarget = GetFlat(doc, source);
                                    List<PlanarFace> planarFacessource = FlFaces(flatsource);
                                    List<PlanarFace> planarFacetarget = FlFaces(flattarget);
                                    Element elesource = doc.GetElement(elementIds.First());
                                    double spatarget = 0;
                                    foreach (ElementId i in newlist)
                                    {
                                        Element eletarget = doc.GetElement(i);
                                        LocationPoint locationPoint2 = eletarget.Location as LocationPoint;
                                        XYZ pointtarget = locationPoint2.Point;
                                        spatarget = DistanceToMin(doc, source, planarFacetarget, pointtarget, kl);
                                        if (spatarget != 0)
                                        {
                                            break;
                                        }
                                    }
                                    if (spatarget != 0)
                                    {
                                        foreach (ElementId i in newlist)
                                        {
                                            XYZ point1 = new XYZ(0, 0, 0);
                                            point1 = point1 + transform1.BasisX * spatarget;
                                            ElementTransformUtils.MoveElement(doc, i, point1);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (val1 > 0)
                                {
                                    foreach (ElementId i in newlist)
                                    {
                                        XYZ point1 = new XYZ(0, 0, 0);
                                        point1 = point1 + transform1.BasisX * (val1);
                                        ElementTransformUtils.MoveElement(doc, i, point1);
                                    }
                                }
                                else
                                {
                                    foreach (ElementId i in newlist)
                                    {
                                        XYZ point1 = new XYZ(0, 0, 0);
                                        point1 = point1 + transform1.BasisX * (-val1);
                                        ElementTransformUtils.MoveElement(doc, i, point1);
                                    }
                                }
                            }
                        }
                        tran.Commit();
                    }
                }
            }
            progressBarform.Close();
        }
        public void CopyElementsFlatToWarped(Document doc, FamilyInstance familyInstance, ICollection<ElementId> elementIds, Selection sel)
        {
            ICollection<ElementId> newlist = new List<ElementId>();
            CopyPasteOptions option = new CopyPasteOptions();
            ElementtransformToCopy tr = new ElementtransformToCopy();
            FamilyInstance flat = tr.GetFlat(doc, familyInstance);
            List<PlanarFace> list1 = FlFaces(flat);
            FamilyInstance warped = tr.GetWarped(doc, familyInstance);
            PlanarFace face1 = Facemax(list1);
            Element element1 = doc.GetElement(elementIds.First());
            LocationPoint loc = element1.Location as LocationPoint;
            XYZ pointorg = loc.Point;
            //double minspace = MinSpacePlanarFace(doc, familyInstance, face1, pointorg);
            Transform transform = TransformFlatWapred(doc, flat, warped);
            //Solid solid1 = Solidhelper.AllSolids(flat).First();
            //Solid solid2 = Solidhelper.AllSolids(warped).First();
            //Transform transform1 = solid1.GetBoundingBox().Transform;
            //Transform transform2 = solid2.GetBoundingBox().Transform;
            //Transform transform = transform1.Inverse.Multiply(transform2);
            using (Transaction tran = new Transaction(doc, "copy"))
            {
                tran.Start();
                FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                IgnoreProcess ignoreProcess = new IgnoreProcess();
                options.SetClearAfterRollback(true);
                options.SetFailuresPreprocessor(ignoreProcess);
                tran.SetFailureHandlingOptions(options);
                try
                {
                    newlist = ElementTransformUtils.CopyElements(doc, elementIds, doc, transform, option);
                    Remove_product(doc, newlist);
                }
                catch (Exception)
                {

                }
                foreach (ElementId eleid in newlist)
                {
                    Element ele = doc.GetElement(eleid);
                    LocationPoint locele = ele.Location as LocationPoint;
                    XYZ pointcon = locele.Point;
                    List<HermiteFace> list2 = WarpedFace(warped);
                    if (list2.Count != 0)
                    {
                        HermiteFace face2 = FacemaxWraped(list2);
                        //double distanceloc = kcpointtoHemiteFace(face2, pointcon);

                        double distanceloc = MinSpaceHermiteFace(doc, familyInstance, face2, pointcon);
                        var xv1 = FIndpointonwraped(pointcon, new XYZ(0, 0, 1), distanceloc);
                        XYZ point1 = pointcon - xv1;
                        ElementTransformUtils.MoveElement(doc, eleid, point1);
                    }
                    else
                    {
                        List<RevolvedFace> revolvedFaces = WarpedFaceRevolFace(warped);
                        RevolvedFace face2 = FacemaxWrapedRevolFace(revolvedFaces);
                        //double distanceloc = kcpointtoHemiteFace(face2, pointcon);

                        double distanceloc = MinSpaceRevolFace(doc, familyInstance, face2, pointcon);
                        var xv1 = FIndpointonwraped(pointcon, new XYZ(0, 0, 1), distanceloc);
                        XYZ point1 = pointcon - xv1;
                        ElementTransformUtils.MoveElement(doc, eleid, point1);
                    }
                    //if (minspace > 0)
                    //{
                    //    var xv1 = FIndpointonwraped(pointcon,doc.ActiveView.UpDirection, distanceloc);
                    //    XYZ point1 = pointcon - xv1;
                    //    ElementTransformUtils.MoveElement(doc, eleid, point1);
                    //}
                    //else
                    //{
                    //    var xv1 = FIndpointonwraped(pointcon, doc.ActiveView.UpDirection, distanceloc);
                    //    XYZ point1 = xv1 - pointcon;
                    //    ElementTransformUtils.MoveElement(doc, eleid, point1);
                    //}
                }
                tran.Commit();
            }
        }
        public double MinSpaceHermiteFace(Document doc, FamilyInstance familyInstance, HermiteFace face, XYZ point)
        {
            IList<XYZ> points = face.Points;
            Transform transform = familyInstance.GetTransform();
            XYZ point1 = points.First();
            XYZ point2 = points.Last();
            XYZ point3 = new XYZ();
            points.Remove(point1);
            points.Remove(point2);
            point3 = points.First();
            foreach (var i in points)
            {
                if (point3.Z > i.Z)
                {
                    point3 = i;
                }
            }
            Plane plane = Plane.CreateByThreePoints(transform.OfPoint(point1), transform.OfPoint(point2), transform.OfPoint(point3));
            plane.SignedDistanceTo(point);
            SketchPlane skt = SketchPlane.Create(doc, plane);
            doc.ActiveView.SketchPlane = skt;
            return plane.SignedDistanceTo(point);
        }
        public double MinSpaceRevolFace(Document doc, FamilyInstance familyInstance, RevolvedFace face, XYZ point)
        {
            IList<XYZ> points = new List<XYZ>();
            Transform transform = familyInstance.GetTransform();
            foreach (EdgeArray item in face.EdgeLoops)
            {
                foreach (Edge item3 in item)
                {
                    Curve curve = item3.AsCurve();
                    points.Add(curve.GetEndPoint(0));
                    points.Add(curve.GetEndPoint(1));
                }
            }
            XYZ point1 = points.First();
            XYZ point2 = points.Last();
            XYZ point3 = new XYZ();
            points.Remove(point1);
            points.Remove(point2);
            point3 = points.First();
            foreach (var i in points)
            {
                if (point3.Z > i.Z)
                {
                    point3 = i;
                }
            }
            Plane plane = Plane.CreateByThreePoints(transform.OfPoint(point1), transform.OfPoint(point2), transform.OfPoint(point3));
            plane.SignedDistanceTo(point);
            SketchPlane skt = SketchPlane.Create(doc, plane);
            doc.ActiveView.SketchPlane = skt;
            return plane.SignedDistanceTo(point);
        }
        public double kcpointtoHemiteFace(HermiteFace face, XYZ point)
        {
            double vn = double.MinValue;
            var Interult = face.Project(point);
            if (Interult != null)
            {
                XYZ xYZ = Interult.XYZPoint;
                vn = point.DistanceTo(xYZ);
            }
            return vn;
        }
        public double MinSpacePlanarFace(Document doc, FamilyInstance familyInstance, PlanarFace face, XYZ point)
        {
            double min;
            Transform transform = familyInstance.GetTransform();
            XYZ op = transform.OfPoint(face.Origin);
            double z1 = op.Z;
            double z2 = point.Z;
            Plane plane = Plane.CreateByNormalAndOrigin(transform.BasisZ, op);
            UV uuv = new UV();
            double ii;
            plane.Project(point, out uuv, out ii);
            if (z2 < z1)
            {
                min = ii;
            }
            else
            {
                min = -ii;
            }
            return min;
        }

        public XYZ FIndpointonwraped(XYZ A, XYZ V, double val)
        {
            XYZ diem = null;
            var tong = Math.Pow(V.X, 2) + Math.Pow(V.Y, 2) + Math.Pow(V.Z, 2);
            var x1 = A.X + ((V.X) * val / (tong));
            var y1 = A.Y + ((V.Y) * val / (tong));
            var z1 = A.Z + ((V.Z) * val / (tong));
            var x2 = A.X - ((V.X) * val / (tong));
            var y2 = A.Y - ((V.Y) * val / (tong));
            var z2 = A.Z - ((V.Z) * val / (tong));
            XYZ diem1 = new XYZ(x1, y1, z1);
            XYZ diem2 = new XYZ(x2, y2, z2);
            XYZ Adiem1 = new XYZ(x1 - A.X, y1 - A.Y, z1 - A.Z);
            XYZ Adiem2 = new XYZ(x2 - A.X, y2 - A.Y, z2 - A.Z);
            if (!Util.Nguochuong(Adiem1, V))
            {
                diem = diem1;
            }
            if (!Util.Nguochuong(Adiem2, V))
            {
                diem = diem2;
            }
            return diem;
        }
        public void Remove_product(Document doc, ICollection<ElementId> elementIds)
        {
            foreach (ElementId i in elementIds)
            {
                Element ele = doc.GetElement(i);
                Parameter pa1 = ele.LookupParameter("CONSTRUCTION_PRODUCT_HOST");
                Parameter pa2 = ele.LookupParameter("BOM_PRODUCT_HOST");
                if (pa1 != null)
                {
                    pa1.Set("");
                }
                if (pa2 != null)
                {
                    pa2.Set("");
                }
            }
        }
        public void CopyElementsConnDtee(Document doc, FamilyInstance familyInstance, List<FamilyInstance> listinstance, ICollection<ElementId> elementIds)
        {
            ICollection<ElementId> newlist = new List<ElementId>();
            CopyPasteOptions option = new CopyPasteOptions();
            ProgressBarform progressBarform = new ProgressBarform(listinstance.Count, "Loading...");
            progressBarform.Show();
            foreach (FamilyInstance source in listinstance)
            {
                using (Transaction tran = new Transaction(doc, "Copy"))
                {
                    tran.Start();
                    progressBarform.giatri();
                    if (progressBarform.iscontinue == false)
                    {
                        break;
                    }
                    Transform transform = TransformToCopy(source, familyInstance);
                    FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                    IgnoreProcess ignoreProcess = new IgnoreProcess();
                    options.SetClearAfterRollback(true);
                    options.SetFailuresPreprocessor(ignoreProcess);
                    tran.SetFailureHandlingOptions(options);
                    try
                    {
                        newlist = ElementTransformUtils.CopyElements(doc, elementIds, doc, transform, option);
                        Remove_product(doc, newlist);
                    }
                    catch (Exception)
                    {

                    }
                    tran.Commit();
                }
            }
            progressBarform.Close();
        }
        public ICollection<ElementId> GetPartOfConn(Document doc, IList<Reference> references)
        {
            ICollection<ElementId> listconnection = new List<ElementId>();
            foreach (var j in references)
            {
                Element ee = doc.GetElement(j);
                FamilyInstance instance = ee as FamilyInstance;
                ICollection<ElementId> elementIds = instance.GetSubComponentIds();
                foreach (ElementId i in elementIds)
                {
                    listconnection.Add(i);
                }
            }
            return listconnection;
        }
        public PlanarFace PlanarFaceDistanceToMin(List<PlanarFace> listPlanarFaces, XYZ point)
        {
            PlanarFace face = null;
            double min = 100;
            foreach (PlanarFace planar in listPlanarFaces)
            {
                var fg = planar.Project(point);
                if (fg == null) continue;
                else
                {
                    XYZ Tert = fg.XYZPoint;
                    if (Tert != null)
                    {
                        double spa = point.DistanceTo(planar.Project(point).XYZPoint);
                        if (spa < min)
                        {
                            min = spa;
                            face = planar;
                        }
                    }
                }
            }
            return face;
        }
        public double DistanceToMin(Document doc, FamilyInstance familyInstance, List<PlanarFace> listPlanarFaces, XYZ point, double kl)
        {
            XYZ nor = new XYZ(1, 0, 0);
            double min = 0;
            var t = familyInstance.Id;
            Transform transform = familyInstance.GetTransform();
            Parameter pa2 = familyInstance.LookupParameter("Flange_Edge_Offset_Right");
            double pal2 = pa2.AsDouble();
            Parameter pa3 = familyInstance.LookupParameter("Flange_Edge_Offset_Left");
            double pal3 = pa3.AsDouble();
            Parameter pa4 = familyInstance.LookupParameter("Joint");
            double pal4 = pa4.AsDouble();
            double sum1 = pal2 - pal4 / 2;
            double sum2 = pal3 - pal4 / 2;
            LocationPoint loc = familyInstance.Location as LocationPoint;
            XYZ pointloc = loc.Point;
            XYZ point1 = pointloc - transform.BasisX * sum1;
            XYZ point2 = pointloc + transform.BasisX * sum2;
            double space1 = point.DistanceTo(point1);
            double space2 = point.DistanceTo(point2);
            if (space1 < space2)
            {
                Plane plane = Plane.CreateByNormalAndOrigin(transform.BasisX, point1);
                UV uuv = new UV();
                double ii;
                plane.Project(point, out uuv, out ii);
                min = ii - kl;
            }
            else
            {
                Plane plane = Plane.CreateByNormalAndOrigin(transform.BasisX, point2);
                UV uuv = new UV();
                double ii;
                plane.Project(point, out uuv, out ii);
                min = ii - kl;
            }
            return min;
        }
        public double Nut(Document doc, FamilyInstance familyInstance, ElementId elementId)
        {
            double min;
            Element ele = doc.GetElement(elementId);
            LocationPoint locele = ele.Location as LocationPoint;
            XYZ pointele = locele.Point;
            Transform transform = familyInstance.GetTransform();
            LocationPoint loc = familyInstance.Location as LocationPoint;
            XYZ pointloc = loc.Point;
            ElementId rt = familyInstance.GetTypeId();
            Element po = doc.GetElement(rt);
            Parameter pa1 = familyInstance.LookupParameter("Flange_Edge_Offset_Right");
            double pal1 = pa1.AsDouble();
            Parameter pa2 = familyInstance.LookupParameter("Flange_Edge_Offset_Left");
            double pal2 = pa2.AsDouble();
            Parameter pa3 = familyInstance.LookupParameter("Joint");
            double pal3 = pa3.AsDouble();
            Parameter pa4 = po.LookupParameter("DT_Stem_Spacing_Form");
            double pal4 = pa4.AsDouble();
            double h1 = (pal4 / 2 + pal1) - pal3 / 2;
            double h2 = (pal4 / 2 + pal2) - pal3 / 2;
            XYZ point1 = pointloc - transform.BasisX * h1;
            XYZ point2 = pointloc + transform.BasisX * h2;
            double space1=double.MinValue;
            double space2=double.MinValue;
            PLane3D pLane3D = new PLane3D(point1, transform.BasisX);
            var p1 = pLane3D.ProjectPointOnPlane(pointele);
            space1 = point1.DistanceTo(p1);
            PLane3D pLane3D2 = new PLane3D(point2, transform.BasisX);
            var p2 = pLane3D2.ProjectPointOnPlane(pointele);
            space2 = point2.DistanceTo(p2);
            //Plane plane = Plane.CreateByNormalAndOrigin(transform.BasisX, point1);
            //UV uv1 = new UV();
            //plane.Project(pointele, out uv1, out space1);
            //Plane plane1 = Plane.CreateByNormalAndOrigin(transform.BasisX, point2);
            //UV uv2 = new UV();
            //plane1.Project(pointele, out uv2, out space2);
            Transaction tran = new Transaction(doc, "ss");
            tran.Start();
            if (space1 < space2)
            {
                min = space1;
            }
            else
            {
                min = space2;
            }
            tran.Commit();
            return min;
        }
        public PlanarFace Facemax(List<PlanarFace> planarFaces)
        {
            PlanarFace face = planarFaces.First();
            foreach (PlanarFace i in planarFaces)
            {
                if (i.Area > face.Area)
                {
                    face = i;
                }
            }
            return face;
        }
        public HermiteFace FacemaxWraped(List<HermiteFace> hermiteFaces)
        {
            HermiteFace face = hermiteFaces.First();
            foreach (HermiteFace i in hermiteFaces)
            {
                if (i.Area > face.Area)
                {
                    face = i;
                }
            }
            return face;
        }
        public RevolvedFace FacemaxWrapedRevolFace(List<RevolvedFace> hermiteFaces)
        {
            RevolvedFace face = hermiteFaces.First();
            foreach (RevolvedFace i in hermiteFaces)
            {
                if (i.Area > face.Area)
                {
                    face = i;
                }
            }
            return face;
        }
        public List<PlanarFace> FlFaces(FamilyInstance familyInstance)
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
                                foreach (var geoFace in solid.Faces)
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
                }
            }
            return face;
        }
        public EdgeArray FlEdgeArray(FamilyInstance familyInstance)
        {
            EdgeArray edge = new EdgeArray();
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
                                foreach (var geoFace in solid.Edges)
                                {
                                    edge.Append(geoFace as Edge);
                                }
                            }
                        }
                    }
                }
            }
            return edge;
        }
        public List<HermiteFace> WarpedFace(FamilyInstance familyInstance)
        {
            List<HermiteFace> face = new List<HermiteFace>();
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
                                foreach (var geoFace in solid.Faces)
                                {
                                    var g = geoFace.GetType();
                                    if (g.Name.Equals("HermiteFace"))
                                    {
                                        face.Add(geoFace as HermiteFace);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return face;
        }
        public List<RevolvedFace> WarpedFaceRevolFace(FamilyInstance familyInstance)
        {
            List<RevolvedFace> face = new List<RevolvedFace>();
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
                                foreach (var geoFace in solid.Faces)
                                {
                                    var g = geoFace.GetType();
                                    if (g.Name.Equals("RevolvedFace"))
                                    {
                                        face.Add(geoFace as RevolvedFace);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return face;
        }
    }
    public class IgnoreProcess : IFailuresPreprocessor
    {
        FailureProcessingResult IFailuresPreprocessor.PreprocessFailures(FailuresAccessor failuresAccessor)
        {
            string transactionName = failuresAccessor.GetTransactionName();
            IList<FailureMessageAccessor> po = failuresAccessor.GetFailureMessages();
            if (po.Count == 0)
                return FailureProcessingResult.Continue;
            if (transactionName.Equals("EXEMPLE"))
            {
                foreach (FailureMessageAccessor p1 in po)
                {
                    if (p1.GetSeverity() == FailureSeverity.Error)
                    {
                        failuresAccessor.DeleteAllWarnings();
                        return FailureProcessingResult.ProceedWithRollBack;
                    }
                    else
                    {
                        failuresAccessor.DeleteWarning(p1);
                    }
                }
            }
            else
            {
                foreach (FailureMessageAccessor p1 in po)
                {
                    failuresAccessor.DeleteAllWarnings();
                }
            }
            return FailureProcessingResult.Continue;
        }
    }

}
