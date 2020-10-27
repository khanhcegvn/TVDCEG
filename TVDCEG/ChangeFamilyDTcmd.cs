#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.Generic;
using System.Linq;
using TVDCEG.WPF;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class ChangeFamilyDTcmd : IExternalCommand
    {
        public List<FamilySymbol> LtfamilySymbols = new List<FamilySymbol>();
        public List<FamilyInstance> listdt = new List<FamilyInstance>();
        public Selection sel;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            // Access current selection

            sel = uidoc.Selection;
            IList<Reference> references = sel.PickObjects(ObjectType.Element);
            foreach (var i in references)
            {
                FamilyInstance instance = doc.GetElement(i) as FamilyInstance;
                listdt.Add(instance);
            }
            LtfamilySymbols = GetfamilyDtchange(doc);
            var form = new ChangeFamilyDT(this, doc);
            if (form.ShowDialog() == true)
            {
                Changefamily(doc, form._symbaol, listdt);
            }
            return Result.Succeeded;
        }
        public List<FamilySymbol> GetfamilyDtchange(Document doc)
        {
            List<FamilySymbol> familySymbol = new List<FamilySymbol>();
            var col = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).WhereElementIsElementType().OfClass(typeof(FamilySymbol)).ToElements().Cast<FamilySymbol>().ToList();
            foreach (FamilySymbol Instance in col)
            {
                if (Instance.Family.FamilyCategory.Name != null && Instance.Family.Name.Contains("DOUBLE_TEE_CHAMFERED_NOMINAL W WASH"))
                {
                    familySymbol.Add(Instance);
                }
            }
            return familySymbol;
        }
        public void Changefamily(Document doc, FamilySymbol familySymbol, List<FamilyInstance> listinstance)
        {
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(listinstance.Count, "Loading...");
            progressbarWPF.Show();
            foreach (FamilyInstance familyInstance in listinstance)
            {
                progressbarWPF.Giatri();
                if (progressbarWPF.iscontinue == false)
                {
                    break;
                }
                Dictionary<string, string> dicparameter = new Dictionary<string, string>();
                Dictionary<string, double> dicparameter1 = new Dictionary<string, double>();
                using (Transaction tran = new Transaction(doc, "sss"))
                {
                    tran.Start();
                    FailureHandlingOptions options = tran.GetFailureHandlingOptions();
                    MyPreProcessor preproccessor = new MyPreProcessor();
                    options.SetClearAfterRollback(true);
                    options.SetFailuresPreprocessor(preproccessor);
                    List<Parameter> list1 = ParameterofDt.Getpameteryesno(doc, familyInstance);
                    List<Parameter> list2 = ParameterofDt.Getpameterdouble(doc, familyInstance);
                    foreach (var item in list1)
                    {
                        if (item != null)
                        {
                            dicparameter.Add(item.Definition.Name, item.AsValueString());
                        }
                    }
                    foreach (var item in list2)
                    {
                        if (item != null)
                        {
                            dicparameter1.Add(item.Definition.Name, item.AsDouble());
                        }
                    }
                    familyInstance.Symbol = familySymbol;
                    List<Parameter> list3 = ParameterofDt.Getpameteryesno(doc, familyInstance);
                    List<Parameter> list4 = ParameterofDt.Getpameterdouble(doc, familyInstance);
                    for (int i = 0; i < list3.Count; i++)
                    {
                        var g1 = list3[i];
                        if (g1 != null)
                        {
                            var giatri1 = g1.AsString();
                            foreach (var o in dicparameter)
                            {
                                if (o.Key == g1.Definition.Name)
                                {
                                    g1.Set(o.Value);
                                }
                            }
                        }
                    }
                    for (int i = 0; i < list4.Count; i++)
                    {
                        var g1 = list4[i];
                        if (g1 != null)
                        {
                            var giatri1 = g1.AsDouble();
                            foreach (var o in dicparameter1)
                            {
                                if (o.Key == g1.Definition.Name)
                                {
                                    g1.Set(o.Value);
                                }
                            }
                        }
                    }
                    tran.Commit(options);
                }
            }
            progressbarWPF.Close();
        }
    }
    public class ParameterofDt
    {
        public Parameter Bearing_Pad_Visible;
        public Parameter High_Point_Right;
        public Parameter Left_DT_Jt_Included;
        public Parameter Right_DT_Jt_Included;
        public Parameter Mark_Opp_Indicators;
        public Parameter Warp_Mark_End;
        public Parameter Warp_Opp_End;
        public Parameter DIM_LENGTH;
        public Parameter Flange_Edge_Offset_Right;
        public Parameter Flange_Edge_Offset_Left;
        public Parameter Joint;
        public Parameter Length_of_Drop;
        public Parameter High_Point_Elevation;
        public Parameter Low_Point_Elevation;
        public Parameter CL_DT;
        public Parameter Manual_Mark_End_Offset;
        public Parameter Manual_Mark_End_Warp_Angle;
        public Parameter Manual_Opp_End_Offset;
        public Parameter Manual_Opp_End_Warp_Angle;
        public static List<Parameter> Getpameteryesno(Document doc, FamilyInstance familyInstance)
        {
            List<Parameter> list = new List<Parameter>();
            Parameter p1 = familyInstance.LookupParameter("Bearing_Pad_Visible");
            list.Add(p1);
            Parameter p2 = familyInstance.LookupParameter("High_Point_Right");
            list.Add(p2);
            Parameter p3 = familyInstance.LookupParameter("Left_DT_Jt_Included");
            list.Add(p3);
            Parameter p4 = familyInstance.LookupParameter("Right_DT_Jt_Included");
            list.Add(p4);
            Parameter p5 = familyInstance.LookupParameter("Mark_Opp_Indicators");
            list.Add(p5);
            Parameter p6 = familyInstance.LookupParameter("Warp_Mark_End");
            list.Add(p6);
            Parameter p7 = familyInstance.LookupParameter("Warp_Opp_End");
            list.Add(p7);
            return list;
        }
        public static List<Parameter> Getpameterdouble(Document doc, FamilyInstance familyInstance)
        {
            List<Parameter> list = new List<Parameter>();
            Parameter p8 = familyInstance.LookupParameter("DIM_LENGTH");
            list.Add(p8);
            Parameter p9 = familyInstance.LookupParameter("Flange_Edge_Offset_Right");
            list.Add(p9);
            Parameter p10 = familyInstance.LookupParameter("Flange_Edge_Offset_Left");
            list.Add(p10);
            Parameter p11 = familyInstance.LookupParameter("Joint");
            list.Add(p11);
            Parameter p12 = familyInstance.LookupParameter("Length_of_Drop");
            list.Add(p12);
            Parameter p13 = familyInstance.LookupParameter("High_Point_Elevation");
            list.Add(p13);
            Parameter p14 = familyInstance.LookupParameter("Low_Point_Elevation");
            list.Add(p14);
            Parameter p15 = familyInstance.LookupParameter("CL_DT");
            list.Add(p15);
            Parameter p16 = familyInstance.LookupParameter("Manual_Mark_End_Offset");
            list.Add(p16);
            Parameter p17 = familyInstance.LookupParameter("Manual_Mark_End_Warp_Angle");
            list.Add(p17);
            Parameter p18 = familyInstance.LookupParameter("Manual_Opp_End_Offset");
            list.Add(p18);
            Parameter p19 = familyInstance.LookupParameter("Manual_Opp_End_Warp_Angle");
            list.Add(p19);
            return list;
        }
    }
}
