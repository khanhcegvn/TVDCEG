#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TVDCEG.LBR;
#endregion

namespace TVDCEG
{
    public class MathWarpedDoubletee
    {
        private static MathWarpedDoubletee _instance;
        private MathWarpedDoubletee()
        {

        }
        public static MathWarpedDoubletee Instance => _instance ?? (_instance = new MathWarpedDoubletee());
        public Dictionary<string, List<DoubleTee>> GetDoubleTee(Document doc)
        {
            Dictionary<string, List<DoubleTee>> dic = new Dictionary<string, List<DoubleTee>>();
            List<FamilyInstance> Col1 = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>().ToList();
            foreach (var item in Col1)
            {
                var ele = item.SuperComponent;
                if (item.Symbol.FamilyName.Contains("DOUBLE_TEE") || item.Symbol.FamilyName.Contains("SINGLE_TEE"))
                {
                    Parameter pa = item.LookupParameter("Work Plane");
                    if (pa != null)
                    {
                        var value = pa.AsString();
                        if (dic.ContainsKey(value))
                        {
                            dic[value].Add(new DoubleTee(item));
                        }
                        else
                        {
                            dic.Add(value, new List<DoubleTee> { new DoubleTee(item) });
                        }
                    }
                }
            }
            return dic;
        }
        public DoubleTee Getdtnearly(DoubleTee familyInstance, List<DoubleTee> listinstance)
        {
            DoubleTee flag = null;
            foreach (var item in listinstance)
            {
                if (Math.Round(item.Location.X, 0) == Math.Round(familyInstance.Location.X, 0) && Math.Round(item.Location.Y, 0) == Math.Round(familyInstance.Location.Y, 0))
                {
                    flag = item;
                }
            }
            return flag;
        }
        public DoubleTee Resource(Document doc, Selection sel)
        {
            Reference reference = sel.PickObject(ObjectType.Element,new Filterstructuralframing(),"Select double tee");
            FamilyInstance familyInstance = doc.GetElement(reference) as FamilyInstance;
            DoubleTee doubleTee = new DoubleTee(familyInstance);
            return doubleTee;
        }
        public void Excute(Document doc, Selection sel)
        {
            DoubleTee doubleTee = Resource(doc, sel);
            var list = ListMath(doc, sel);
            Mathvalue2(doc, doubleTee, list);
        }
        public List<DoubleTee> ListMath(Document doc, Selection sel)
        {
            List<DoubleTee> list = new List<DoubleTee>();
            IList<Reference> references = sel.PickObjects(ObjectType.Element, new FilterDoubletee(), "Select Double Tee");
            foreach (var item in references)
            {
                FamilyInstance familyInstance = doc.GetElement(item) as FamilyInstance;
                DoubleTee doubleTee = new DoubleTee(familyInstance);
                list.Add(doubleTee);
            }
            return list;
        }
        public void Mathvalue2(Document doc, DoubleTee doubleTee, List<DoubleTee> listdt)
        {
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(listdt.Count, "Loading...");
            progressbarWPF.Show();
            foreach (var item in listdt)
            {
                progressbarWPF.Giatri();
                if (progressbarWPF.iscontinue == false)
                {
                    break;
                }
                using (Transaction tran = new Transaction(doc, "Math Value"))
                {
                    tran.Start();
                    item.Length_of_Drop.Set(doubleTee.Length_of_Drop.AsDouble());
                    item.High_Point_Elevation.Set(doubleTee.High_Point_Elevation.AsDouble());
                    item.Low_Point_Elevation.Set(doubleTee.Low_Point_Elevation.AsDouble());
                    item.CL_DT.Set(doubleTee.CL_DT.AsDouble());
                    item.Manual_Mark_End_Offset.Set(doubleTee.Manual_Mark_End_Offset.AsDouble());
                    item.Manual_Mark_End_Warp_Angle.Set(doubleTee.Manual_Mark_End_Warp_Angle.AsDouble());
                    item.Manual_Opp_End_Offset.Set(doubleTee.Manual_Opp_End_Offset.AsDouble());
                    item.Manual_Opp_End_Warp_Angle.Set(doubleTee.Manual_Opp_End_Warp_Angle.AsDouble());
                    tran.Commit();
                }
            }
            progressbarWPF.Close();
        }
        public void MathValue(Document doc, List<DoubleTee> list1, Dictionary<string, List<DoubleTee>> dic)
        {
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(list1.Count, "Loading...");
            progressbarWPF.Show();
            foreach (var item in list1)
            {
                progressbarWPF.Giatri();
                if (progressbarWPF.iscontinue == false)
                {
                    break;
                }
                using (Transaction tran = new Transaction(doc, "Math Dt"))
                {
                    tran.Start();
                    foreach (var item2 in dic.Keys.ToList())
                    {
                        var list2 = dic[item2].ToList();
                        DoubleTee flag = Getdtnearly(item, list2);
                        if(flag!=null)
                        {
                            flag.Length_of_Drop.Set(item.Length_of_Drop.AsDouble());
                            flag.High_Point_Elevation.Set(item.High_Point_Elevation.AsDouble());
                            flag.Low_Point_Elevation.Set(item.Low_Point_Elevation.AsDouble());
                            flag.CL_DT.Set(item.CL_DT.AsDouble());
                            flag.Manual_Mark_End_Offset.Set(item.Manual_Mark_End_Offset.AsDouble());
                            flag.Manual_Mark_End_Warp_Angle.Set(item.Manual_Mark_End_Warp_Angle.AsDouble());
                            flag.Manual_Opp_End_Offset.Set(item.Manual_Opp_End_Offset.AsDouble());
                            flag.Manual_Opp_End_Warp_Angle.Set(item.Manual_Opp_End_Warp_Angle.AsDouble());
                        }
                        else
                        {
                            continue;
                        }
                    }
                    tran.Commit();
                }
            }
            progressbarWPF.Close();
        }
    }
    public class DoubleTee
    {
        public Parameter Length_of_Drop { get; set; }
        public Parameter High_Point_Elevation { get; set; }
        public Parameter Low_Point_Elevation { get; set; }
        public Parameter CL_DT { get; set; }
        public Parameter Manual_Mark_End_Offset { get; set; }
        public Parameter Manual_Mark_End_Warp_Angle { get; set; }
        public Parameter Manual_Opp_End_Offset { get; set; }
        public Parameter Manual_Opp_End_Warp_Angle { get; set; }
        public XYZ Location { get; set; }
        public DoubleTee(FamilyInstance familyInstance)
        {
            Length_of_Drop = familyInstance.LookupParameter("Length_of_Drop");
            High_Point_Elevation = familyInstance.LookupParameter("High_Point_Elevation");
            Low_Point_Elevation = familyInstance.LookupParameter("Low_Point_Elevation");
            CL_DT = familyInstance.LookupParameter("CL_DT");
            Manual_Mark_End_Offset = familyInstance.LookupParameter("Manual_Mark_End_Offset");
            Manual_Mark_End_Warp_Angle = familyInstance.LookupParameter("Manual_Mark_End_Warp_Angle");
            Manual_Opp_End_Offset = familyInstance.LookupParameter("Manual_Opp_End_Offset");
            Manual_Opp_End_Warp_Angle = familyInstance.LookupParameter("Manual_Opp_End_Warp_Angle");
            Location = (familyInstance.Location as LocationPoint).Point;
        }
    }
}
