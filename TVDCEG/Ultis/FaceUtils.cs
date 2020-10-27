using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.LBR
{
    public static class FaceUtils
    {
        public static List<PlanarFace> FacesBySymbol(this FamilyInstance familyInstance)
        {
            var faces = new List<PlanarFace>();
            var op = new Options();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;
            op.DetailLevel = ViewDetailLevel.Undefined;
            var geoE = familyInstance.get_Geometry(op);
            if (geoE == null) return faces;
            foreach (var geoO in geoE)
            {
                var geoI = geoO as GeometryInstance;
                if (geoI == null) continue;
                var instanceGeoE = geoI.GetSymbolGeometry();
                foreach (var instanceGeoObj in instanceGeoE)
                {
                    var solid = instanceGeoObj as Solid;
                    if (solid == null || solid.Faces.Size == 0) continue;
                    foreach (Face face in solid.Faces)
                    {
                        var planarFace = face as PlanarFace;
                        if (planarFace != null)
                        {
                            faces.Add(planarFace);
                        }
                    }
                }
            }

            return faces;
        }

        public static List<PlanarFace> Faces(this FamilyInstance fi)
        {
            List<PlanarFace> faces = new List<PlanarFace>();
            // Option :
            var op = new Options();
            op.ComputeReferences = true;
            op.IncludeNonVisibleObjects = true;
            op.DetailLevel = ViewDetailLevel.Undefined;
            var geoE = fi.get_Geometry(op);
            if (geoE == null) return faces;
            foreach (var geoO in geoE)
            {
                var solid = geoO as Solid;
                if (solid == null || solid.Faces.Size == 0 || solid.Edges.Size == 0) continue;
                foreach (var f in solid.Faces)
                {
                    var planarFace = f as PlanarFace;
                    if (planarFace != null)
                    {
                        faces.Add(planarFace);
                    }
                }
            }
            return faces;
        }

        public static IList<Edge> AllEdges(this FamilyInstance familyInstance)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            string name = familyInstance.Name;
            IList<Edge> edgeList = new List<Edge>();
            foreach (GeometryObject geometryObject in familyInstance.get_Geometry(options))
            {
                Solid solid = geometryObject as Solid;
                if (!(null == solid) && solid.Faces.Size != 0 && solid.Edges.Size != 0)
                {
                    foreach (Edge edge in solid.Edges)
                        edgeList.Add(edge);
                }
            }
            return edgeList;
        }

        public static IList<Edge> AllEdgesBySymbol(this FamilyInstance familyInstance)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            string name = familyInstance.Name;
            IList<Edge> edgeList = new List<Edge>();
            foreach (GeometryObject geometryObject1 in familyInstance.get_Geometry(options))
            {
                GeometryInstance geometryInstance = geometryObject1 as GeometryInstance;
                if (null != geometryInstance)
                {
                    foreach (GeometryObject geometryObject2 in geometryInstance.GetSymbolGeometry())
                    {
                        Solid solid = geometryObject2 as Solid;
                        if (!(null == solid) && solid.Faces.Size != 0 && solid.Edges.Size != 0)
                        {
                            foreach (Edge edge in solid.Edges)
                            {
                                if (edge != null)
                                    edgeList.Add(edge);
                            }
                        }
                    }
                }
            }
            return edgeList;
        }
        public static Plane Face2Plane(this Face face)
        {
            Plane plane = null;
            var planarFace = face as PlanarFace;
            if (planarFace == null)
            {
                return plane;
            }
            plane = Plane.CreateByNormalAndOrigin(planarFace.FaceNormal, planarFace.Origin);
            return plane;
        }
        public static Plane Faceby3pointPlane(this Face face)
        {
            Plane plane = null;
            var planarFace = face as PlanarFace;
            if (planarFace == null)
            {
                return plane;
            }
            List<XYZ> Points = new List<XYZ>();
            EdgeArrayArray edgeArrayArray = planarFace.EdgeLoops;
            foreach (EdgeArray item in edgeArrayArray)
            {
                foreach (Edge item2 in item)
                {
                    Curve curve = item2.AsCurve();
                    XYZ startpoint = curve.GetEndPoint(0);
                    XYZ endpoint = curve.GetEndPoint(1);
                    Points.Add(startpoint);
                    Points.Add(endpoint);
                }
            }
            var newlist = Points.OrderBy(p => p.X).ThenBy(p => p.Y).ThenBy(p => p.Z).ToList();
            Removeduplicatepoint(newlist);
            plane = Plane.CreateByThreePoints(newlist[0], newlist[1], newlist[3]);
            return plane;
        }
        static List<XYZ> Removeduplicatepoint(List<XYZ> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    XYZ point1 = list[i];
                    XYZ point2 = list[j];
                    if (point1.Equalpoint(point2))
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
            return list;
        }
        static List<FamilyInstance> Removeduplicatefamilyinstance(List<FamilyInstance> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    FamilyInstance instance1 = list[i];
                    FamilyInstance instance2 = list[j];
                    if (instance1.Id==instance2.Id)
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
            return list;
        }
    }
}
