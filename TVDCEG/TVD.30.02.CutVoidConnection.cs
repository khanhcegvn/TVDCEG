#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.LBR;
using TVDCEG.Ultis;
#endregion

namespace TVDCEG
{
    public class CutVoidConnection
    {
        private static CutVoidConnection _instance;
        private CutVoidConnection()
        {

        }
        public static CutVoidConnection Instance => _instance ?? (_instance = new CutVoidConnection());
        public Dictionary<string, List<FamilyInstance>> GetAllConnection(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            var Conn = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var i in Conn)
            {
                ElementId faid = i.GetTypeId();
                Element elemtype = doc.GetElement(faid);
                Autodesk.Revit.DB.Parameter pa = elemtype.LookupParameter("MANUFACTURE_COMPONENT");
                if (pa != null)
                {
                    var vla = pa.AsString();
                    if (vla.Contains("CONNECTION"))
                    {
                        if (dic.ContainsKey(i.Name))
                        {
                            dic[i.Name].Add(i);
                        }
                        else
                        {
                            dic.Add(i.Name, new List<FamilyInstance> { i });
                        }
                    }
                }
            }
            return dic;
        }
        public List<FamilyInstance> FindVoidConnection(Document doc, FamilyInstance familyInstance)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            var fg = familyInstance.GetSubComponentIds();
            foreach (var item in fg)
            {
                FamilyInstance element = doc.GetElement(item) as FamilyInstance;
                if (element.Category.IsCuttable)
                {
                    list.Add(element);
                }
            }
            return list;
        }
        public List<FamilyInstance> FindProductVoidcut(Document doc, List<FamilyInstance> Voids)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            foreach (var item in Voids)
            {
                var solids = Solidhelper.AllSolids(item);
                foreach (var solid in solids)
                {
                    ICollection<ElementId> col = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).WherePasses(new ElementIntersectsSolidFilter(solid)).ToElementIds();
                }
            }
            return list;
        }
        public void Cutting(Document doc, List<FamilyInstance> listcut, List<FamilyInstance> listconn)
        {
            ProgressBarform progressBarform = new ProgressBarform(listconn.Count, "Cutting");
            progressBarform.Show();
            using (Transaction tran = new Transaction(doc, "Cut Void"))
            {
                tran.Start();
                foreach (var conn in listconn)
                {
                    progressBarform.giatri();
                    if (progressBarform.iscontinue == false)
                    {
                        break;
                    }
                    //var list = FindElementNearly(doc, conn, listcut);
                    foreach (var framming in listcut)
                    {
                        try
                        {
                            if (InstanceVoidCutUtils.CanBeCutWithVoid(framming))
                            {
                                InstanceVoidCutUtils.AddInstanceVoidCut(doc, framming, conn);
                            }
                        }
                        catch
                        {

                        }
                    }
                }
                tran.Commit();
            }
            progressBarform.Close();
        }
        public List<FamilyInstance> FindElementNearly(Document doc, FamilyInstance conn, List<FamilyInstance> listcut)
        {
            List<FamilyInstance> list = new List<FamilyInstance>();
            List<ElementId> listid = new List<ElementId>();
            listcut.ForEach(x => listid.Add(x.Id));
            var listsolid = Solidhelper.AllSolids(conn);
            foreach (var item in listsolid)
            {
                IList<FamilyInstance> a1 = Checkintersect(doc, item, listid, conn);
                List<FamilyInstance> checksup = GetSuperInstances(a1.ToList());
                checksup.ForEach(y => list.Add(y));
            }
            return Removeduplicatinstance(list);
        }
        static List<FamilyInstance> Removeduplicatinstance(List<FamilyInstance> familyInstances)
        {
            for (int i = 0; i < familyInstances.Count; i++)
            {
                for (int j = i + 1; j < familyInstances.Count; j++)
                {
                    FamilyInstance point1 = familyInstances[i];
                    FamilyInstance point2 = familyInstances[j];
                    if (point1.Id == point2.Id)
                    {
                        familyInstances.RemoveAt(j);
                        j--;
                    }
                }
            }
            return familyInstances;
        }
        public List<FamilyInstance> GetSuperInstances(List<FamilyInstance> familyInstances)
        {
            var superInstances = new List<FamilyInstance>();
            foreach (var instance in familyInstances)
            {
                var f1 = instance.SuperComponent as FamilyInstance;
                if (instance.SuperComponent != null)
                {
                    superInstances.Add(f1);
                }
                if (instance.SuperComponent == null)
                {
                    superInstances.Add(instance);
                }

            }
            return superInstances;
        }
        public List<FamilyInstance> Checkintersect(Document doc, Solid solid, List<ElementId> ColelementIds, FamilyInstance instance)
        {
            List<FamilyInstance> listfam = new List<FamilyInstance>();
            if (ColelementIds.Count == 0)
            {
                return listfam;
            }
            FilteredElementCollector filtercol = new FilteredElementCollector(doc, ColelementIds);
            ICollection<ElementId> col = filtercol.OfClass(typeof(FamilyInstance)).WherePasses(new ElementIntersectsSolidFilter(solid)).ToElementIds();
            foreach (var i in col)
            {
                Element element = doc.GetElement(i);
                FamilyInstance familyInstance = element as FamilyInstance;
                if (familyInstance != null && familyInstance.Name != "CONNECTOR_COMPONENT")
                {
                    var val = GetSupFamilyInstance(familyInstance);
                    Element ele = instance.SuperComponent;
                    if (ele != null)
                    {
                        if (val.Id.IntegerValue != ele.Id.IntegerValue)
                        {
                            //listfam.Add(val);
                            Solid solid1 = Solidhelper.AllSolids(familyInstance).First();
                            if (CheckSolid(solid, solid1))
                            {
                                listfam.Add(val);
                            }
                        }
                    }
                    else
                    {
                        //listfam.Add(val);
                        if (val.Id.IntegerValue != instance.Id.IntegerValue)
                        {
                            Solid solid1 = Solidhelper.AllSolids(familyInstance).First();
                            if (CheckSolid(solid, solid1))
                            {
                                listfam.Add(val);
                            }
                        }
                    }
                }
            }
            return listfam;
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
        public Dictionary<string, List<FamilyInstance>> Getallframing(Document doc)
        {
            Dictionary<string, List<FamilyInstance>> dic = new Dictionary<string, List<FamilyInstance>>();
            List<FamilyInstance> list = new List<FamilyInstance>();
            var structural = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            //foreach (var item in structural)
            //{
            //    if (item.GetSubComponentIds().Count == 1 || item.GetSubComponentIds().Count == 0)
            //    {
            //        list.Add(item);
            //    }
            //}
            foreach (var item in structural)
            {
                if (dic.ContainsKey(item.Symbol.FamilyName))
                {
                    dic[item.Symbol.FamilyName].Add(item);
                }
                else
                {
                    dic.Add(item.Symbol.FamilyName, new List<FamilyInstance> { item });
                }
            }
            return dic;
        }
    }
    public class CegProduct
    {
        public string CONSTRUCTION_PRODUCT { get; set; }
        public CegProduct(Document doc, Element element)
        {
            ElementId faid = element.GetTypeId();
            Element elemtype = doc.GetElement(faid);
            Parameter pa = elemtype.LookupParameter("CONSTRUCTION_PRODUCT");
            if (pa != null)
            {
                CONSTRUCTION_PRODUCT = pa.AsString();
            }
        }
    }
}
