#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Resources;
using System.Text;
using TVDCEG.Ultis;
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    public class AutodimElement
    {
        private static AutodimElement _instance;
        private AutodimElement()
        {

        }
        public static AutodimElement Instance => _instance ?? (_instance = new AutodimElement());
        public IList<PlanarFace> FaceofWall(FamilyInstance familyInstance)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            string name = familyInstance.Name;
            IList<PlanarFace> list = new List<PlanarFace>();
            GeometryElement geometryElement = familyInstance.get_Geometry(options);
            bool flag = geometryElement == null;
            IList<PlanarFace> result;
            if (flag)
            {
                result = list;
            }
            else
            {
                foreach (GeometryObject item in geometryElement)
                {
                    Solid solid = item as Solid;
                    bool flag3 = null == solid || solid.Faces.Size == 0 || solid.Edges.Size == 0;
                    if (!flag3)
                    {
                        foreach (object obj in solid.Faces)
                        {
                            Face face = (Face)obj;
                            PlanarFace planarFace = face as PlanarFace;
                            bool flag4 = planarFace != null;
                            if (flag4)
                            {
                                list.Add(planarFace);
                            }
                        }
                    }
                    else
                    {
                        GeometryInstance geometryInstance = item as GeometryInstance;
                        bool flag2 = null != geometryInstance;
                        if (flag2)
                        {
                            GeometryElement symbolGeometry = geometryInstance.GetSymbolGeometry();
                            foreach (GeometryObject geometryObject2 in symbolGeometry)
                            {
                                Solid solid1 = geometryObject2 as Solid;
                                bool flag31 = null == solid1 || solid1.Faces.Size == 0 || solid1.Edges.Size == 0;
                                if (!flag31)
                                {
                                    foreach (object obj in solid1.Faces)
                                    {
                                        Face face = (Face)obj;
                                        PlanarFace planarFace = face as PlanarFace;
                                        bool flag4 = planarFace != null;
                                        if (flag4)
                                        {
                                            list.Add(planarFace);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                result = list;
            }
            return result;
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
        public PlanarFace Facegannhat(XYZ point, XYZ direction, IList<PlanarFace> listFaces, Transform transform)
        {
            PlanarFace planarFace = null;
            double num = double.MinValue;
            PLane3D plane3D = new PLane3D(point, direction);
            foreach (PlanarFace i in listFaces)
            {
                XYZ xyz = transform.OfVector(i.FaceNormal);
                XYZ xyz2 = direction.CrossProduct(xyz);
                bool check = xyz2.GetLength() > 0.001 || xyz.DotProduct(direction) < 0.0;
                if (!check)
                {
                    double num2 = plane3D.DistancepointtoPlane(transform.OfPoint(i.Origin));
                    bool flag2 = num2 < 0.001;
                    if (!flag2)
                    {
                        bool flag3 = num2 > num;
                        if (flag3)
                        {
                            planarFace = i;
                            num = num2;
                        }
                    }
                }
            }
            return planarFace;
        }
        public PlanarFace Facexanhat(XYZ point, XYZ direction, IList<PlanarFace> listFaces, Transform transform)
        {
            PlanarFace planarFace = null;
            double num = double.MinValue;
            PLane3D plane3D = new PLane3D(point, direction);
            foreach (PlanarFace i in listFaces)
            {
                XYZ xyz = transform.OfVector(i.FaceNormal);
                XYZ xyz2 = direction.CrossProduct(xyz);
                bool check = xyz2.GetLength() > 0.001 || xyz.DotProduct(direction) < 0.0;
                if (!check)
                {
                    double num2 = plane3D.DistancepointtoPlane(transform.OfPoint(i.Origin));
                    bool flag2 = num2 < 0.001;
                    if (!flag2)
                    {
                        bool flag3 = num2 > num;
                        if (flag3)
                        {
                            planarFace = i;
                            num = num2;
                        }
                    }
                }
            }
            return planarFace;
        }
        public void GetReferenceBrick(Document doc, XYZ direction, FamilyInstance wall, out PlanarFace rightFace, out PlanarFace leftFace)
        {
            XYZ location = ((LocationPoint)wall.Location).Point;
            FamilyInstance flat = GetFlatInstance(doc, wall);
            IList<PlanarFace> Listfaces = FaceofWall(flat);
            if (Listfaces.Count != 0)
            {
                Transform transform = wall.GetTransform();
                rightFace = Facexanhat(location, direction, Listfaces, transform);
                leftFace = Facexanhat(location, -direction, Listfaces, transform);
            }
            else
            {
                IList<PlanarFace> planarFaces = FlFaces(wall);
                Transform transform = wall.GetTransform();
                rightFace = Facexanhat(location, direction, planarFaces, transform);
                leftFace = Facexanhat(location, -direction, planarFaces, transform);
            }
        }
        public void GetReferenceWall(Document doc, FamilyInstance wall, XYZ direction, ref PlanarFace rightFace, ref PlanarFace leftFace)
        {
            XYZ location = ((LocationPoint)wall.Location).Point;
            FamilyInstance flat = GetFlatInstance(doc, wall);
            Transform transform = flat.GetTransform();
            IList<PlanarFace> Listfaces = FaceofWall(flat);
            var listsau = (from x in Listfaces where x.Reference != null select x).ToList();
            XYZ mn = transform.OfPoint(location).Normalize();
            var leftf1 = listsau.FirstOrDefault(x => x.ComputeNormal(UV.Zero).IsAlmostEqualTo(-1 * direction));
            var rightf2 = listsau.FirstOrDefault(x => x.ComputeNormal(UV.Zero).IsAlmostEqualTo(1 * direction));
            var leftd1 = listsau.FirstOrDefault(x => x.CheckComputeNormal(transform).IsAlmostEqualTo(-1 * direction));
            var rightd2 = listsau.FirstOrDefault(x => x.CheckComputeNormal(transform).IsAlmostEqualTo(1 * direction));
            if (leftf1.Area != leftd1.Area || rightf2.Area != rightd2.Area)
            {
                if (Math.Round(leftf1.Area,3) < Math.Round(leftd1.Area,3))
                {
                    leftFace = leftf1;
                }
                else
                {
                    leftFace = leftd1;
                }
                if (Math.Round(rightf2.Area,3) < Math.Round(rightd2.Area,3))
                {
                    rightFace = rightf2;
                }
                else
                {
                    rightFace = rightd2;
                }
            }
            else
            {
                leftFace = leftf1;
                rightFace = rightf2;
            }
        }
        public void GetReferenceWall2(Document doc, FamilyInstance wall, XYZ direction,ref PlanarFace rightFace, ref PlanarFace leftFace)
        {
            XYZ location = ((LocationPoint)wall.Location).Point;
            FamilyInstance flat = GetFlatInstance(doc, wall);
            Transform transform = flat.GetTransform();
            IList<PlanarFace> Listfaces = FaceofWall(flat);
            var listsau = (from x in Listfaces where x.Reference != null select x).ToList();
            leftFace = listsau.FirstOrDefault(x => x.CheckComputeNormal(transform).IsAlmostEqualTo(-1 * direction));
            rightFace = listsau.FirstOrDefault(x => x.CheckComputeNormal(transform).IsAlmostEqualTo(1 * direction));
        }
        public PlanarFace Getcrossection(View view, List<PlanarFace> list)
        {
            PlanarFace crossSection = null;
            XYZ director = view.ViewDirection;
            foreach (PlanarFace f in list)
            {
                if (Util.IsParallel(director,
                    (f as PlanarFace).FaceNormal))
                {
                    crossSection = f as PlanarFace;
                    break;
                }
            }
            return crossSection;
        }
        public void Dimwall(Document doc, Selection sel, DimensionType dimensionType)
        {
            List<FamilyInstance> listwall = new List<FamilyInstance>();
            FamilyInstance wall1 = (FamilyInstance)doc.GetElement(sel.PickObject(ObjectType.Element, new InstanceFilter(), "Select Element First"));
            FamilyInstance wall2 = (FamilyInstance)doc.GetElement(sel.PickObject(ObjectType.Element, new InstanceFilter(), "Select Element Second"));
            LocationPoint locationPoint = (LocationPoint)wall1.Location;
            XYZ xyz = (locationPoint != null) ? locationPoint.Point : null;
            LocationPoint locationPoint2 = (LocationPoint)wall2.Location;
            XYZ xyz2 = (locationPoint2 != null) ? locationPoint2.Point : null;
            //XYZ npoint = tranlatorxyz1(doc.ActiveView, xyz2, xyz);
            //XYZ direction = (xyz - npoint).Normalize();
            XYZ direction = (xyz - xyz2).Normalize();
            listwall = Getwallinrow(doc, wall1, wall2, direction);
            ReferenceArray referenceArray = new ReferenceArray();
            if (listwall.Count == 1)
            {
                listwall.Clear();
                XYZ npoint2 = tranlatorxyz2(doc.ActiveView, xyz2, xyz);
                XYZ direction2 = (xyz - npoint2).Normalize().Negate();
                listwall = Getwallinrow(doc, wall1, wall2, direction2);
                foreach (var item in listwall)
                {
                    PlanarFace rigthface = null;
                    PlanarFace leftface = null;
                    GetReferenceWall2(doc, item, direction2, ref rigthface, ref leftface);
                    referenceArray.Append(rigthface.Reference);
                    referenceArray.Append(leftface.Reference);
                }
                using (Transaction tran = new Transaction(doc, "Create dimension"))
                {
                    tran.Start();
                    XYZ p1 = sel.PickPoint();
                    XYZ p2 = FindTaghead(p1, direction2, 10);
                    Line line = Line.CreateBound(p1, p2);
                    var dim = doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType);
                    tran.Commit();
                }
            }
            else
            {
                foreach (var item in listwall)
                {
                    PlanarFace rigthface = null;
                    PlanarFace leftface = null;
                    GetReferenceWall2(doc, item, direction, ref rigthface, ref leftface);
                    referenceArray.Append(rigthface.Reference);
                    referenceArray.Append(leftface.Reference);
                }


                using (Transaction tran = new Transaction(doc, "Create dimension"))
                {
                    tran.Start();
                    FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                    MyPreProcessor preproccessor = new MyPreProcessor();
                    options.SetClearAfterRollback(false);
                    options.SetFailuresPreprocessor(preproccessor);
                    tran.SetFailureHandlingOptions(options);
                    Plane plane = Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin);
                    SketchPlane skt = SketchPlane.Create(doc, plane);
                    doc.ActiveView.SketchPlane = skt;
                    XYZ p1 = sel.PickPoint();
                    XYZ p2 = FindTaghead(p1, direction, 10);
                    Line line = Line.CreateBound(p1, p2);
                    var dim = doc.Create.NewDimension(doc.ActiveView, line, referenceArray, dimensionType);
                    tran.Commit(options);
                }
            }
        }
        public XYZ FindTaghead(XYZ A, XYZ V, double vm)
        {
            XYZ diem = null;
            var val = vm /*/ Convert.ToDouble(view.Scale)*/;
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
        // chuyen location point ele2 sang ele2
        public XYZ tranlatorxyz1(View view, XYZ p1, XYZ p2)
        {
            XYZ doc = view.UpDirection;
            XYZ ngang = view.RightDirection;
            double t = double.MinValue;
            if (Math.Round(p2.X - p1.X, 0) != 0 && Math.Round(doc.X - ngang.X, 0) != 0)
            {
                t = (p2.X - p1.X) / (doc.X - ngang.X);
                var x = p2.X + ngang.X * t;
                var y = p2.Y + ngang.Y * t;
                var z = p2.Z + ngang.Z * t;
                XYZ point = new XYZ(x, y, z);
                return point;
            }
            else
            {
                t = 1;
                var x = p2.X + ngang.X * t;
                var y = p2.Y + ngang.Y * t;
                var z = p2.Z + ngang.Z * t;
                XYZ point = new XYZ(x, y, z);
                return point;
            }
        }
        public XYZ tranlatorxyz2(View view, XYZ p1, XYZ p2)
        {
            XYZ doc = view.UpDirection;
            XYZ ngang = view.RightDirection;
            double t = double.MinValue;
            if (Math.Round(p2.X - p1.X, 0) != 0 && Math.Round(doc.X - ngang.X, 0) != 0)
            {
                t = (p2.X - p1.X) / (doc.X - ngang.X);
                var x = p2.X + doc.X * t;
                var y = p2.Y + doc.Y * t;
                var z = p2.Z + doc.Z * t;
                XYZ point = new XYZ(x, y, z);
                return point;
            }
            else
            {
                t = 1;
                var x = p2.X + doc.X * t;
                var y = p2.Y + doc.Y * t;
                var z = p2.Z + doc.Z * t;
                XYZ point = new XYZ(x, y, z);
                return point;
            }
        }
        public List<DimensionType> GetDimensions(Document doc)
        {
            var col = new FilteredElementCollector(doc).OfClass(typeof(DimensionType)).Cast<DimensionType>().ToList();
            var list = (from x in col where x.StyleType == DimensionStyleType.Linear orderby x.Name select x).ToList();
            return list;
        }
        public List<FamilyInstance> Getwallinrow(Document doc, FamilyInstance firstinstance, FamilyInstance secondinstance, XYZ direction)
        {
            //List<FamilyInstance> familyInstances = this.GetFamilyInstances(doc, doc.ActiveView, firstinstance, secondinstance, direction);
            List<FamilyInstance> familyInstances = this.GetFamilyInstances2(doc, doc.ActiveView, firstinstance, secondinstance, direction);
            return familyInstances;
        }
        public FamilyInstance GetFlatInstance(Document doc, FamilyInstance instance)
        {
            FamilyInstance familyInstance = null;
            foreach (ElementId id in instance.GetSubComponentIds())
            {
                FamilyInstance familyInstance2 = doc.GetElement(id) as FamilyInstance;
                bool flag = familyInstance2 == null;
                if (!flag)
                {
                    bool flag2 = familyInstance2.Symbol.FamilyName.Contains("_FLAT");
                    if (flag2)
                    {
                        familyInstance = familyInstance2;
                    }
                }
            }
            bool flag3 = familyInstance == null;
            if (flag3)
            {
                familyInstance = instance;
            }
            return familyInstance;
        }
        public List<FamilyInstance> GetFamilyInstances(Document doc, View view, FamilyInstance firstInstance, FamilyInstance secondInstance, XYZ dimDirection)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            bool flag = firstInstance.Id == secondInstance.Id;
            List<FamilyInstance> result;
            if (flag)
            {
                list.Add(firstInstance);
                result = list;
            }
            else
            {
                LocationPoint locationPoint = firstInstance.Location as LocationPoint;
                LocationPoint locationPoint2 = secondInstance.Location as LocationPoint;
                bool flag2 = locationPoint == null || locationPoint2 == null;
                if (flag2)
                {
                    result = list;
                }
                else
                {
                    XYZ xyz = locationPoint.Point;
                    XYZ point = locationPoint2.Point;
                    XYZ right = xyz;
                    PLane3D plane3DLib = new PLane3D(xyz, dimDirection);
                    PLane3D plane3DLib2 = new PLane3D(view.Origin, view.ViewDirection);
                    XYZ xyz2 = point - xyz;
                    xyz2 = plane3DLib2.ProjectVectorOnPlane(xyz2);
                    BoundingBoxXYZ boundingBoxXYZ = firstInstance.get_BoundingBox(view);
                    XYZ xyz3 = boundingBoxXYZ.Max;
                    XYZ xyz4 = boundingBoxXYZ.Min;
                    double num = Math.Abs((xyz4 - xyz3).DotProduct(XYZ.BasisZ));
                    xyz3 = plane3DLib.ProjectPointOnPlane(xyz3);
                    xyz3 = plane3DLib2.ProjectPointOnPlane(xyz3);
                    xyz4 = plane3DLib.ProjectPointOnPlane(xyz4);
                    xyz4 = plane3DLib2.ProjectPointOnPlane(xyz4);
                    double num2 = xyz3.DistanceTo(xyz4);
                    FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc, view.Id);
                    IList<FamilyInstance> list2 = (from x in filteredElementCollector.OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_StructuralFraming).Cast<FamilyInstance>() where x.Symbol.Name.Equals(firstInstance.Symbol.Name) select x).ToList();
                    xyz = plane3DLib2.ProjectPointOnPlane(xyz);
                    foreach (FamilyInstance familyInstance in list2)
                    {
                        bool flag4 = familyInstance.Symbol.FamilyName != firstInstance.Symbol.FamilyName;
                        if (!flag4)
                        {
                            LocationPoint locationPoint3 = familyInstance.Location as LocationPoint;
                            bool flag5 = locationPoint3 == null;
                            if (!flag5)
                            {
                                XYZ xyz5 = locationPoint3.Point;
                                double num3 = Math.Abs((xyz5 - right).DotProduct(XYZ.BasisZ));
                                bool flag6 = num3 > num;
                                if (!flag6)
                                {
                                    XYZ xyz6 = plane3DLib2.ProjectVectorOnPlane(xyz5 - xyz);
                                    bool flag7 = xyz6.DotProduct(xyz2) < 0.0 || xyz6.GetLength() > xyz2.GetLength();
                                    if (!flag7)
                                    {
                                        xyz5 = plane3DLib.ProjectPointOnPlane(xyz5);
                                        xyz5 = plane3DLib2.ProjectPointOnPlane(xyz5);
                                        double num4 = xyz.DistanceTo(xyz5);
                                        bool flag8 = num4 < num2 / 2.0;
                                        if (flag8)
                                        {
                                            list.Add(familyInstance);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    //list.Add(secondInstance);
                    result = list;
                }
            }
            return result;
        }
        public List<FamilyInstance> GetFamilyInstances2(Document doc, View view, FamilyInstance firstInstance, FamilyInstance secondInstance, XYZ dimDirection)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            bool flag = firstInstance.Id == secondInstance.Id;
            List<FamilyInstance> result;
            if (flag)
            {
                list.Add(firstInstance);
                result = list;
            }
            else
            {
                LocationPoint locationPoint = firstInstance.Location as LocationPoint;
                LocationPoint locationPoint2 = secondInstance.Location as LocationPoint;
                bool flag2 = locationPoint == null || locationPoint2 == null;
                if (flag2)
                {
                    result = list;
                }
                else
                {
                    list.Add(firstInstance);
                    list.Add(secondInstance);
                    PLane3D pLane3D = new PLane3D(doc.ActiveView.Origin, doc.ActiveView.ViewDirection);
                    XYZ point1 = pLane3D.ProjectPointOnPlane(locationPoint.Point);
                    XYZ point2 = pLane3D.ProjectPointOnPlane(locationPoint2.Point);
                    //XYZ tranlator = Timdiemthu3(point1,point2);
                    //XYZ direction = (tranlator - point2);
                    XYZ direction = (point1 - point2);
                    FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc, view.Id);
                    IList<FamilyInstance> list2 = (from x in filteredElementCollector.OfClass(typeof(FamilyInstance)).OfCategory(firstInstance.Category.ToBuiltinCategory()).Cast<FamilyInstance>() where x.Symbol.Name.Contains(firstInstance.Symbol.Name) select x).ToList();
                    foreach (var item in list2)
                    {
                        if (item.Id != firstInstance.Id && item.Id != secondInstance.Id)
                        {
                            XYZ locationinstance = item.Getlocationofinstacne();

                            //XYZ tranlatoritem = Timdiemthu3(locationinstance, locationPoint2.Point);
                            //XYZ vector = (pLane3D.ProjectPointOnPlane(tranlatoritem) - point2);

                            XYZ vector = (pLane3D.ProjectPointOnPlane(locationinstance) - point2);
                            if (vector.IsParallel(direction))
                            {
                                if (vector.GetLength() <= direction.GetLength())
                                {
                                    bool flag3 = vector.DotProduct(direction) != 0;
                                    if (flag3)
                                    {
                                        list.Add(item);
                                    }
                                }
                            }
                        }
                    }
                    result = list;
                }
            }
            return result;
        }
        public double Gochopboi2vector(XYZ u1, XYZ u2)
        {
            double a = u1.DotProduct(u2);
            double y1 = Math.Sqrt(Math.Pow(u1.X, 2) + Math.Pow(u1.Y, 2) + Math.Pow(u1.Z, 2));
            double y2 = Math.Sqrt(Math.Pow(u2.X, 2) + Math.Pow(u2.Y, 2) + Math.Pow(u2.Z, 2));
            return Math.Abs(a / (y1 * y2));
        }
        //tim diem thu 3 tu 2 diem cho truoc de tao thanh tam giac vuong
        public XYZ Timdiemthu3(XYZ p1,XYZ p2)
        {
            XYZ px = XYZ.Zero;
            XYZ r1 = new XYZ(p1.X, p2.Y, p2.Z);
            XYZ vector1 = p1 - r1;
            XYZ vector2 = p2 - r1;
            if (vector1.IsPerpendicular(vector2))
            {
                px = r1;
            }
            return px;
        }
        public View3D FindView3D(Document doc)
        {
            View3D result = null;
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc);
            IList<Element> list = filteredElementCollector.WhereElementIsNotElementType().OfClass(typeof(View3D)).ToElements();
            foreach (Element element in list)
            {
                View3D view3D = element as View3D;
                bool flag = view3D == null || view3D.IsTemplate;
                if (!flag)
                {
                    string viewName = view3D.Name;
                    bool flag2 = viewName == "toanvd";
                    if (flag2)
                    {
                        result = view3D;
                        break;
                    }
                }
            }
            return result;
        }
        public List<View3D> Get3Dview(Document doc)
        {
            var col = (from View3D x in new FilteredElementCollector(doc).OfClass(typeof(View3D)).Cast<View3D>() where !x.IsAssemblyView select x).ToList();
            var col2 = (from View3D y in col where !y.IsTemplate select y).Cast<View3D>().ToList();
            return col2;
        }
    }
    public class SettingAdutodimelement
    {
        private static SettingAdutodimelement _instance;
        private SettingAdutodimelement()
        {

        }
        public static SettingAdutodimelement Instance => _instance ?? (_instance = new SettingAdutodimelement());
        public string DimensionType { get; set; }
        public string GetFolderPath()
        {
            string folderPath = SettingExtension.GetSettingPath() + "\\TVD.22.SettingAutodimelement";
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            return folderPath;
        }

        public string GetFileName()
        {
            return "SettingAutodimelement.json";
        }

        public string GetFullFileName()
        {
            return GetFolderPath() + "\\" + GetFileName();
        }

        public void SaveSetting()
        {
            var gh = GetFullFileName();
            SettingExtension.SaveSetting(this, GetFullFileName());
        }

        public SettingAdutodimelement GetSetting()
        {
            SettingAdutodimelement setting = SettingExtension.GetSetting<SettingAdutodimelement>(GetFullFileName());
            if (setting == null) setting = new SettingAdutodimelement();
            return setting;
        }
    }

}
