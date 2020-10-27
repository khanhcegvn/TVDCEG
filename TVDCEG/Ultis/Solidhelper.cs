using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;

namespace TVDCEG.Ultis
{
    public static class Solidhelper
    {
        public static void GetSolidsFromSymbol(FamilyInstance familyInstance, ref List<Solid> solids)
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
                            // Try to find solid
                            Solid solid = instObj as Solid;
                            if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;
                            if (!solids.Contains(solid)) solids.Add(solid);
                        }
                    }
                }
            }
        }
        public static void GetSolidsOfwall(FamilyInstance familyInstance, ref List<Solid> solids)
        {
            Options options = new Options();
            options.ComputeReferences = true;
            options.IncludeNonVisibleObjects = true;
            options.DetailLevel = ViewDetailLevel.Undefined;
            string name = familyInstance.Name;
            GeometryElement geometryElement = familyInstance.get_Geometry(options);
            bool flag = geometryElement == null;
            foreach (GeometryObject geometryObject in geometryElement)
            {
                GeometryInstance geometryInstance = geometryObject as GeometryInstance;
                bool flag2 = null != geometryInstance;
                if (flag2)
                {
                    GeometryElement symbolGeometry = geometryInstance.GetSymbolGeometry();
                    foreach (GeometryObject geometryObject2 in symbolGeometry)
                    {
                        Solid solid = geometryObject2 as Solid;
                        bool flag3 = null == solid || solid.Faces.Size == 0 || solid.Edges.Size == 0;
                        if (!flag3)
                        {
                            solids.Add(solid);
                        }
                    }
                }
            }
        }
        public static void GetSolids(FamilyInstance familyInstance, ref List<Solid> solids)
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
                    // Try to find solid
                    Solid solid = geoObject as Solid;
                    if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;
                    if (!solids.Contains(solid)) solids.Add(solid);
                }
            }
        }
        public static void GetSolidSY(FamilyInstance familyInstance, ref List<Solid> solids)
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
                    // Try to find solid
                    Solid solid = geoObject as Solid;
                    if (null == solid || 0 == solid.Faces.Size || 0 == solid.Edges.Size) continue;
                    if (!solids.Contains(solid)) solids.Add(solid);
                }
            }
        }
        public static void GetSolidsFromNested(FamilyInstance familyInstance, ref List<Solid> solids)
        {
            Document doc = familyInstance.Document;
            foreach (ElementId id in familyInstance.GetSubComponentIds())
            {
                FamilyInstance instance = doc.GetElement(id) as FamilyInstance;
                if (instance == null) continue;
                GetSolids(instance, ref solids);
                GetSolidsFromSymbol(instance, ref solids);
            }
        }
        public static List<Solid> AllSolids(FamilyInstance familyInstance)
        {
            List<Solid> allSolids = new List<Solid>();
            GetSolids(familyInstance, ref allSolids);
            GetSolidsFromSymbol(familyInstance, ref allSolids);
            GetSolidsFromNested(familyInstance, ref allSolids);
            return allSolids;
        }
    }
}
