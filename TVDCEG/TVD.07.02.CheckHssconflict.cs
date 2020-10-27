#region Namespaces
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    public class CheckHssconflict
    {
        private static CheckHssconflict _instance;
        private CheckHssconflict()
        {

        }
        public static CheckHssconflict Instance => _instance ?? (_instance = new CheckHssconflict());
    }
    public class Hssinfo
    {
        public XYZ StartPoint { get; set; }
        public XYZ EndPoint { get; set; }
        public Transform Transform { get; set; }
        public Hssinfo(FamilyInstance familyInstance)
        {
            LineMax(familyInstance);
            Transform = familyInstance.GetTransform();
        }
        PlanarFace FindPlanarFaces(FamilyInstance familyInstance)
        {
            List<PlanarFace> list = new List<PlanarFace>();
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
                            Solid solid = o as Solid;
                            if (solid != null)
                            {
                                FaceArray kl = solid.Faces;
                                foreach (var i in kl)
                                {
                                    if (i.GetType().Name.Contains("Planar"))
                                    {
                                        list.Add(i as PlanarFace);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            SortPlanarFace(list);
            PlanarFace face = list.Last();
            return face;
        }
        void LineMax(FamilyInstance familyInstance)
        {
            PlanarFace planarFace = FindPlanarFaces(familyInstance);
            List<Line> list1 = new List<Line>();
            var list = planarFace.GetEdgesAsCurveLoops();
            foreach (var i in list)
            {
                foreach (var j in i)
                {
                    list1.Add(j as Line);
                }
            }
            SortLine(list1);
            Line line2 = list1[3];
            StartPoint = line2.GetEndPoint(1);
            EndPoint = line2.GetEndPoint(0);
        }
        void SortPlanarFace(List<PlanarFace> planarFaces)
        {
            for (int i = 0; i < planarFaces.Count; i++)
            {
                for (int j = 0; j < planarFaces.Count; j++)
                {
                    if (planarFaces[i].Area < planarFaces[j].Area)
                    {
                        var temp = planarFaces[i];
                        planarFaces[i] = planarFaces[j];
                        planarFaces[j] = temp;
                    }
                }
            }
        }
        void SortLine(List<Line> Lines)
        {
            for (int i = 0; i < Lines.Count; i++)
            {
                for (int j = 0; j < Lines.Count; j++)
                {
                    if (Lines[i].Length < Lines[j].Length)
                    {
                        var temp = Lines[i];
                        Lines[i] = Lines[j];
                        Lines[j] = temp;
                    }
                }
            }
        }
    }
}
