#region Namespaces
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Linq;
#endregion

namespace TVDCEG
{
    public class CreateAssembly_sup
    {
        public static CreateAssembly_sup _instance;
        private CreateAssembly_sup()
        {

        }
        public static CreateAssembly_sup Instance => _instance ?? (_instance = new CreateAssembly_sup());
        public void Create(Document doc, FamilyInstance familyInstance)
        {
            var member = familyInstance.GetSubComponentIds();
            member.Add(familyInstance.Id);
            var Frame = doc.GetElement((from x in member where doc.GetElement(x).Name.Contains("Frame") select x).First()) as FamilyInstance;
            string[] parameterNames = new string[]
            {
                "CONTROL_MARK",
                "MARK",
                "Mark#",
                "Mark"
            };
            foreach (var i in Frame.GetSubComponentIds())
            {
                member.Add(i);
            }
            IList<FamilyInstance> lop = FindMemberForAssembly(doc, familyInstance);
            IList<FamilyInstance> bn = FindConn(doc, familyInstance);
            foreach (var m in lop)
            {
                if (!IsAssemblyelement(m))
                {
                    member.Add(m.Id);
                }

            }
            foreach (var g in bn)
            {
                if (!IsAssemblyelement(g))
                {
                    member.Add(g.Id);
                }
            }
            AssemblyInstance assemblyInstance = null;
            using (Transaction tran = new Transaction(doc, "Create Assembly"))
            {
                tran.Start();
                assemblyInstance = AssemblyInstance.Create(doc, member, familyInstance.Category.Id);
                tran.Commit();
            }
            using (Transaction t = new Transaction(doc, "Set name"))
            {
                t.Start();
                if (assemblyInstance != null)
                {
                    Parameter parameter = this.LookupElementParameter(familyInstance, parameterNames);
                    string text = null;
                    bool flag = parameter != null && parameter.StorageType == StorageType.String;
                    if (flag)
                    {
                        text = parameter.AsString();
                        bool flag2 = text != null;
                        if (flag2)
                        {
                            assemblyInstance.AssemblyTypeName = text;
                            var mem = assemblyInstance.GetMemberIds();
                            foreach (var e in mem)
                            {
                                Element element = doc.GetElement(e);
                                Parameter parameter1 = element.LookupParameter("CONSTRUCTION_PRODUCT_HOST");
                                if (parameter1 != null)
                                {
                                    parameter1.Set(text);
                                }
                            }
                        }
                    }
                }
                t.Commit();
            }
        }
        bool IsAssemblyelement(FamilyInstance familyInstance)
        {
            var ass = familyInstance.AssemblyInstanceId;
            if (ass.IntegerValue != -1) return true;
            else return false;
        }
        public IList<FamilyInstance> FindConn(Document doc, FamilyInstance instance)
        {
            IList<FamilyInstance> list = new List<FamilyInstance>();
            var Frame = doc.GetElement((from x in instance.GetSubComponentIds() where doc.GetElement(x).Name.Contains("Frame") select x).First()) as FamilyInstance;
            List<FamilyInstance> source = new List<FamilyInstance>();
            foreach (var i in Frame.GetSubComponentIds())
            {
                FamilyInstance gh = doc.GetElement(i) as FamilyInstance;
                source.Add(gh);
            }
            foreach (FamilyInstance familyInstance in source)
            {
                BoundingBoxXYZ boundingBoxXYZ = familyInstance.get_BoundingBox(doc.ActiveView);
                Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
                BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);
                BoundingBoxIsInsideFilter filter2 = new BoundingBoxIsInsideFilter(outline);
                LogicalOrFilter filter3 = new LogicalOrFilter(filter, filter2);
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc, doc.ActiveView.Id);
                ICollection<ElementId> ids = filteredElementCollector.OfClass(typeof(FamilyInstance)).WherePasses(filter3).ToElementIds();
                IList<Solid> list2 = this.AllSolids(familyInstance);
                List<ElementId> nestedIds = this.GetNestedIds(doc, ids);
                foreach (Solid solid in list2)
                {
                    IList<FamilyInstance> fromList = this.IntersectPart(doc, solid, nestedIds);
                    this.AddListFamilyInstance(fromList, list);
                }
            }
            return list;
        }
        public IList<FamilyInstance> FindMemberForAssembly(Document doc, FamilyInstance familyInstance)
        {
            IList<FamilyInstance> list = new List<FamilyInstance>();
            BoundingBoxXYZ boundingBoxXYZ = familyInstance.get_BoundingBox(doc.ActiveView);
            Outline outline = new Outline(boundingBoxXYZ.Min, boundingBoxXYZ.Max);
            BoundingBoxIntersectsFilter filter = new BoundingBoxIntersectsFilter(outline);
            BoundingBoxIsInsideFilter filter2 = new BoundingBoxIsInsideFilter(outline);
            LogicalOrFilter filter3 = new LogicalOrFilter(filter, filter2);
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc, doc.ActiveView.Id);
            ICollection<ElementId> ids = filteredElementCollector.OfClass(typeof(FamilyInstance)).WherePasses(filter3).ToElementIds();
            IList<Solid> list2 = this.AllSolids(familyInstance);
            List<ElementId> nestedIds = this.GetNestedIds(doc, ids);
            foreach (Solid solid in list2)
            {
                IList<FamilyInstance> fromList = this.IntersectPart(doc, solid, nestedIds);
                this.AddListFamilyInstance(fromList, list);
            }
            return list;
        }
        public void AddListFamilyInstance(IList<FamilyInstance> fromList, IList<FamilyInstance> toList)
        {
            foreach (FamilyInstance item in fromList)
            {
                bool flag = !toList.Contains(item);
                if (flag)
                {
                    toList.Add(item);
                }
            }
        }
        public IList<FamilyInstance> IntersectPart(Document doc, Solid solid, ICollection<ElementId> collectionIds)
        {
            IList<FamilyInstance> list = new List<FamilyInstance>();
            bool flag = collectionIds.Count == 0;
            IList<FamilyInstance> result;
            if (flag)
            {
                result = list;
            }
            else
            {
                FilteredElementCollector filteredElementCollector = new FilteredElementCollector(doc, collectionIds);
                ICollection<ElementId> collection = filteredElementCollector.OfClass(typeof(FamilyInstance)).WherePasses(new ElementIntersectsSolidFilter(solid)).ToElementIds();
                foreach (ElementId id in collection)
                {
                    Element element = doc.GetElement(id);
                    FamilyInstance familyInstance = element as FamilyInstance;
                    bool flag2 = familyInstance == null;
                    if (!flag2)
                    {
                        bool flag3 = familyInstance.Category.Id == Category.GetCategory(doc, BuiltInCategory.OST_StructuralFraming).Id || familyInstance.Category.Id == Category.GetCategory(doc, BuiltInCategory.OST_StructuralFoundation).Id;
                        if (!flag3)
                        {
                            list.Add(familyInstance);
                            //bool flag4 = this.CheckManufactureComponent(familyInstance);
                            //if (flag4)
                            //{
                            //    list.Add(familyInstance);
                            //}
                        }
                    }
                }
                result = list;
            }
            return result;
        }
        public bool CheckManufactureComponent(FamilyInstance instance)
        {
            bool result = true;
            Parameter parameter = instance.LookupParameter("MANUFACTURE_COMPONENT");
            bool flag = parameter != null;
            if (flag)
            {
                string text = parameter.AsString();
                bool flag2 = text != null && text.Contains("ERECTION");
                if (flag2)
                {
                    result = false;
                }
                bool flag3 = text != null && text.Contains("CIP");
                if (flag3)
                {
                    result = false;
                }
            }
            Parameter parameter2 = instance.Symbol.LookupParameter("MANUFACTURE_COMPONENT");
            bool flag4 = parameter2 != null;
            if (flag4)
            {
                string text2 = parameter2.AsString();
                bool flag5 = text2 != null && text2.Contains("ERECTION");
                if (flag5)
                {
                    result = false;
                }
                bool flag6 = text2 != null && text2.Contains("CIP");
                if (flag6)
                {
                    result = false;
                }
            }
            return result;
        }
        public List<ElementId> GetNestedIds(Document doc, ICollection<ElementId> ids)
        {
            List<ElementId> list = new List<ElementId>();
            foreach (ElementId elementId in ids)
            {
                FamilyInstance familyInstance = doc.GetElement(elementId) as FamilyInstance;
                bool flag = familyInstance == null;
                if (!flag)
                {
                    bool flag2 = familyInstance.SuperComponent != null;
                    if (flag2)
                    {
                        list.Add(elementId);
                    }
                    bool flag3 = familyInstance.SuperComponent == null && familyInstance.GetSubComponentIds().Count == 0;
                    if (flag3)
                    {
                        list.Add(elementId);
                    }
                    bool flag4 = familyInstance.Symbol.LookupParameter("BAR_SHAPE") != null;
                    if (flag4)
                    {
                        list.Add(elementId);
                    }
                    Parameter parameter = familyInstance.Symbol.LookupParameter("NOTE");
                    bool flag5 = ((parameter != null) ? parameter.AsString() : null) == "ADD";
                    if (flag5)
                    {
                        list.Add(elementId);
                    }
                    Parameter parameter2 = familyInstance.Symbol.LookupParameter("MANUFACTURE_COMPONENT");
                    bool flag6 = ((parameter2 != null) ? parameter2.AsString() : null) == "LIFTING";
                    if (flag6)
                    {
                        list.Add(elementId);
                    }
                }
            }
            return list;
        }
        public Parameter LookupElementParameter(Element element, string[] parameterNames)
        {
            Parameter parameter = null;
            foreach (string name in parameterNames)
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
        public void AddReforcement(Document doc, FamilyInstance familyInstance)
        {
            List<Solid> solid = AllSolids(familyInstance);
        }
        public void GetSolidsFromSymbol(FamilyInstance familyInstance, ref List<Solid> solids)
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
        public void GetSolidsFromNested(FamilyInstance familyInstance, ref List<Solid> solids)
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
        public void GetSolids(FamilyInstance familyInstance, ref List<Solid> solids)
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
        public List<Solid> AllSolids(FamilyInstance familyInstance)
        {
            List<Solid> allSolids = new List<Solid>();
            GetSolids(familyInstance, ref allSolids);
            GetSolidsFromSymbol(familyInstance, ref allSolids);
            GetSolidsFromNested(familyInstance, ref allSolids);
            return allSolids;
        }
    }
}
