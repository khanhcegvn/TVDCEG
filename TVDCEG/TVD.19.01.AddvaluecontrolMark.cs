#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Linq;
using System.Collections.Generic;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class AddvaluecontrolMarkcmd : IExternalCommand
    {
        public Dictionary<string, List<FamilyInstance>> independentTags = new Dictionary<string, List<FamilyInstance>>();
        public Dictionary<string, Parameter> dicpa = new Dictionary<string, Parameter>();
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            dicpa = GetParameterelements(doc);
            independentTags = Findsymboltag(doc);
            using (var form = new FrmAddValue(this))
            {
                if (form.ShowDialog() != true)
                {
                    if (form.check)
                    {
                        Setvalue(doc, form.listinstance, form.Parametertag);
                    }
                }
            }
            return Result.Succeeded;
        }
        public  Dictionary<string, List<FamilyInstance>> Findsymboltag(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var col = new FilteredElementCollector(doc).WhereElementIsNotElementType().OfClass(typeof(IndependentTag)).Where(x => x.Category.Name == "Multi-Category Tags" || x.Category.Name == "Structural Framing Tags").Cast<IndependentTag>().ToList();
            foreach (var tag in col)
            {
                FamilyInstance instancesource = (doc.GetElement(tag.TaggedLocalElementId) as FamilyInstance).GetSuperInstances();
                //if(!CheckInAssembly(doc,instancesource))
                //{
                //    if (instancesource.Category.Name != null)
                //    {
                //        if (instancesource.Category.Name.Contains("Structural Framing"))
                //        {
                //            if (dic.ContainsKey(tag.Name))
                //            {
                //                dic[tag.Name].Add(instancesource);
                //            }
                //            else
                //            {
                //                dic.Add(tag.Name, new List<FamilyInstance> { instancesource });
                //            }
                //        }
                //    }
                //}
                if (instancesource.Category.Name != null)
                {
                    if (instancesource.Category.Name.Contains("Structural Framing"))
                    {
                        if (dic.ContainsKey(tag.Name))
                        {
                            dic[tag.Name].Add(instancesource);
                        }
                        else
                        {
                            dic.Add(tag.Name, new List<FamilyInstance> { instancesource });
                        }
                    }
                }
            }
            return dic;
        }
        public bool CheckInAssembly(Document doc,FamilyInstance familyInstance)
        {
            bool flag = false;
            ElementtransformToCopy elementtransformToCopy = new ElementtransformToCopy();
            FamilyInstance source = elementtransformToCopy.GetFlat(doc, familyInstance);
            if(source!=null)
            {
               if(source.AssemblyInstanceId.IntegerValue!=-1)
                {
                    flag = true;
                }
            }
            else
            {
                if(familyInstance.AssemblyInstanceId.IntegerValue!=-1)
                {
                    flag = true;
                }
            }
            return flag;
        }
        public Dictionary<string, Parameter> GetParameterelements(Document doc)
        {
            Dictionary<string, Parameter> dic = new Dictionary<string, Parameter>();
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in col)
            {
                var pas = item.ParametersMap;
                foreach (var item2 in pas)
                {
                    var value = item2 as Parameter;
                    if (dic.ContainsKey(value.Definition.Name))
                    {
                        continue;
                    }
                    else
                    {
                        dic.Add(value.Definition.Name, value);
                    }
                }
            }
            return dic;
        }
        public List<FamilyInstance> FindledgeandhaunchesNearly(Document doc, FamilyInstance familyInstance, List<ElementId> ids)
        {
            IList<Solid> solids = Solidhelper.AllSolids(familyInstance);
            List<FamilyInstance> listintersect = new List<FamilyInstance>();
            foreach (var solid in solids)
            {
                var col = Checkintersect(doc, solid, ids, familyInstance);
                col.ForEach(x => listintersect.Add(x));
            }
            return Removeduplicatefamilyinstance(listintersect);
        }
        List<FamilyInstance> Removeduplicatefamilyinstance(List<FamilyInstance> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    FamilyInstance instance1 = list[i];
                    FamilyInstance instance2 = list[j];
                    if (instance1.Id == instance2.Id)
                    {
                        list.RemoveAt(j);
                        j--;
                    }
                }
            }
            return list;
        }
        public List<FamilyInstance> Checkintersect(Document doc, Solid solid, List<ElementId> ColelementIds, FamilyInstance instance)
        {
            List<FamilyInstance> listfam = new List<FamilyInstance>();
            if (ColelementIds.Count == 0)
            {
                return listfam;
            }
            Solid solid2 = solidBoundingBox(solid);
            FilteredElementCollector filtercol = new FilteredElementCollector(doc, ColelementIds);
            ICollection<ElementId> col = filtercol.OfClass(typeof(FamilyInstance)).WherePasses(new ElementIntersectsSolidFilter(solid2, false)).ToElementIds();
            foreach (var i in col)
            {
                Element element = doc.GetElement(i);
                FamilyInstance familyInstance = element as FamilyInstance;
                if (familyInstance.Symbol.Category.ToBuiltinCategory() == BuiltInCategory.OST_GenericModel)
                {
                    //var val = GetSupFamilyInstance(familyInstance);
                    Element ele = instance.SuperComponent;
                    if (ele != null)
                    {
                        if (familyInstance.Id.IntegerValue != ele.Id.IntegerValue)
                        {
                            //listfam.Add(val);
                            Solid solid1 = Solidhelper.AllSolids(familyInstance).First();
                            //if (CheckSolid(solid, solid1))
                            //{
                            //    listfam.Add(val);
                            //}
                            listfam.Add(familyInstance);
                        }
                    }
                    else
                    {
                        //listfam.Add(val);
                        if (familyInstance.Id.IntegerValue != instance.Id.IntegerValue)
                        {
                            Solid solid1 = Solidhelper.AllSolids(familyInstance).First();
                            //if (CheckSolid(solid, solid1))
                            //{
                            //    listfam.Add(val);
                            //}
                            listfam.Add(familyInstance);
                        }
                    }
                }
            }
            return listfam;
        }
        public Solid solidBoundingBox(Solid inputSolid)
        {
            BoundingBoxXYZ bbox = inputSolid.GetBoundingBox();

            // corners in BBox coords
            XYZ pt0 = new XYZ(bbox.Min.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt1 = new XYZ(bbox.Max.X, bbox.Min.Y, bbox.Min.Z);
            XYZ pt2 = new XYZ(bbox.Max.X, bbox.Max.Y, bbox.Min.Z);
            XYZ pt3 = new XYZ(bbox.Min.X, bbox.Max.Y, bbox.Min.Z);
            //edges in BBox coords
            Line edge0 = Line.CreateBound(pt0, pt1);
            Line edge1 = Line.CreateBound(pt1, pt2);
            Line edge2 = Line.CreateBound(pt2, pt3);
            Line edge3 = Line.CreateBound(pt3, pt0);
            //create loop, still in BBox coords
            List<Curve> edges = new List<Curve>();
            edges.Add(edge0);
            edges.Add(edge1);
            edges.Add(edge2);
            edges.Add(edge3);
            Double height = bbox.Max.Z - bbox.Min.Z;
            CurveLoop baseLoop = CurveLoop.Create(edges);
            List<CurveLoop> loopList = new List<CurveLoop>();
            loopList.Add(baseLoop);
            Solid preTransformBox = GeometryCreationUtilities.CreateExtrusionGeometry(loopList, XYZ.BasisZ, height);
            Transform transform = bbox.Transform.ScaleBasis(1.01);
            Solid transformBox = SolidUtils.CreateTransformed(preTransformBox,transform);

            return transformBox;

        }
        public bool CheckSolid(Solid solid1, Solid solid2)
        {
            bool flag = false;
            try
            {
                Solid solid = BooleanOperationsUtils.ExecuteBooleanOperation(solid1, solid2, BooleanOperationsType.Intersect);
                if (solid.Volume > 0.000000001)
                {
                    flag = true;
                }
                return flag;
            }
            catch
            {
                return flag;
            }
        }
        public FamilyInstance GetSupFamilyInstance(FamilyInstance familyInstance)
        {
            var f1 = familyInstance;
            var f2 = familyInstance.SuperComponent as FamilyInstance;
            if (familyInstance.SuperComponent != null)
            {
                familyInstance = f2;
                return familyInstance;
            }
            else
            {
                return familyInstance;
            }
        }
        public List<ElementId> FindAllLedgeHaunches(Document doc)
        {
            List<ElementId> list = new List<ElementId>();
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_GenericModel).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in col)
            {
                if (item.Name.Contains("LEGDE") || item.Name.Contains("CORBEL"))
                {
                    list.Add(item.Id);
                }
            }
            return list;
        }
        public void Setvalue(Document doc, List<FamilyInstance> list, string value)
        {
            ProgressbarWPF progressBarform = new ProgressbarWPF(list.Count, "Loading...");
            progressBarform.Show();
            var ListCorbelHaunch = FindAllLedgeHaunches(doc);
            foreach (var i in list)
            {
                progressBarform.Giatri();
                if (progressBarform.iscontinue == false)
                {
                    break;
                }
                if(!CheckInAssembly(doc,i))
                {
                    var listintersect = FindledgeandhaunchesNearly(doc, i, ListCorbelHaunch);
                    Parameter commentdata = i.LookupParameter(value);
                    string hj = commentdata.AsString();
                    using (Transaction tran = new Transaction(doc, "Set"))
                    {
                        tran.Start();
                        ElementtransformToCopy tr = new ElementtransformToCopy();
                        FamilyInstance flat = tr.GetFlat(doc, i);
                        FamilyInstance wapred = tr.GetWarped(doc, i);
                        if (flat != null)
                        {
                            Parameter FlatVolumnTag = flat.LookupParameter(value);
                            Parameter WarpedVolumnTag = wapred.LookupParameter(value);
                            Parameter volume = flat.LookupParameter("Volume");
                            double Value = volume.AsDouble() + VolumnCorbelHaunch(listintersect);
                            double kl = Value * 150;
                            double k1 = Math.Round(kl / 1000, 1);
                            double k2 = k1 * 1000;
                            commentdata.Set(k2.ToString());
                            FlatVolumnTag.Set(k2.ToString());
                            WarpedVolumnTag.Set(k2.ToString());
                            string gh = commentdata.AsString();
                        }
                        else
                        {
                            Parameter volume = i.LookupParameter("Volume");
                            double Value = volume.AsDouble() + VolumnCorbelHaunch(listintersect);
                            double kl = Value * 150;
                            double k1 = Math.Round(kl / 1000, 1);
                            double k2 = k1 * 1000;
                            commentdata.Set(k2.ToString());
                            string gh = commentdata.AsString();
                        }
                        tran.Commit();
                    }
                }
               else
                {
                    using (Transaction tran = new Transaction(doc, "Set"))
                    {
                        tran.Start();
                        Parameter commentdata = i.LookupParameter(value);
                        if(!string.IsNullOrEmpty(commentdata.AsString()))
                        {
                            ElementtransformToCopy tr = new ElementtransformToCopy();
                            FamilyInstance flat = tr.GetFlat(doc, i);
                            FamilyInstance wapred = tr.GetWarped(doc, i);
                            Parameter paproduct = i.LookupParameter(value);
                            string CVl = paproduct.AsString();
                            if (flat != null)
                            {
                                Parameter FlatVolumnTag = flat.LookupParameter(value);
                                Parameter WarpedVolumnTag = wapred.LookupParameter(value);
                                FlatVolumnTag.Set(CVl);
                                WarpedVolumnTag.Set(CVl);
                            }
                        }
                        else
                        {
                            var listintersect = FindledgeandhaunchesNearly(doc, i, ListCorbelHaunch);
                            ElementtransformToCopy tr = new ElementtransformToCopy();
                            FamilyInstance flat = tr.GetFlat(doc, i);
                            FamilyInstance wapred = tr.GetWarped(doc, i);
                            if (flat != null)
                            {
                                Parameter FlatVolumnTag = flat.LookupParameter(value);
                                Parameter WarpedVolumnTag = wapred.LookupParameter(value);
                                Parameter volume = flat.LookupParameter("Volume");
                                double Value = volume.AsDouble() + VolumnCorbelHaunch(listintersect);
                                double kl = Value * 150;
                                double k1 = Math.Round(kl / 1000, 1);
                                double k2 = k1 * 1000;
                                commentdata.Set(k2.ToString());
                                FlatVolumnTag.Set(k2.ToString());
                                WarpedVolumnTag.Set(k2.ToString());
                                string gh = commentdata.AsString();
                            }
                            else
                            {
                                Parameter volume = i.LookupParameter("Volume");
                                double Value = volume.AsDouble() + VolumnCorbelHaunch(listintersect);
                                double kl = Value * 150;
                                double k1 = Math.Round(kl / 1000, 1);
                                double k2 = k1 * 1000;
                                commentdata.Set(k2.ToString());
                                string gh = commentdata.AsString();
                            }
                        }
                        tran.Commit();
                    }
                }
            }
            progressBarform.Close();
        }
        public double VolumnCorbelHaunch(List<FamilyInstance> list)
        {
            double Volumn = 0;
            foreach (var item in list)
            {
                Parameter volume = item.LookupParameter("Volume");
                double Value = volume.AsDouble();
                Volumn = Volumn + Value;
            }
            return Volumn;
        }
    }
}
