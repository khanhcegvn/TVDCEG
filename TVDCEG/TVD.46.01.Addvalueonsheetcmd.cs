#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using TVDCEG.LBR;
using TVDCEG.Ultis;
using TVDCEG.Extension;
#endregion

namespace TVDCEG
{
    [Transaction(TransactionMode.Manual)]
    public class Addvalueonsheetcmd : IExternalCommand
    {
        public Document doc;
        public UIDocument uidoc;
        public Selection sel;
        public Dictionary<string, List<ViewSheet>> AssemblySheetDict;
        public Dictionary<string, FamilyInstance> SheetTitleBlockDict;
        public Dictionary<string, CegParameterSet> SheetParameterDict;
        public Dictionary<string, CegParameterSet> TitleBlockParameterDict;
        public Dictionary<string, ViewSheet> ViewSheetDict;
        public Dictionary<string, List<FamilyInstance>> Products;
        public List<string> SheetList;
        public string DESIGNSHT_ENGDSN;
        public string HANDLINGSHT_HDLDSN;
        public string PRODWT_TKT_WEIGHT;
        public string PRODTYPE_PRDTCODE;
        public string RELEASESTR_STRIPPING_STRENGTH;
        public string DAYSTR_28_DAY_STRENGTH;
        public string BACKUPC_TKT_CUYDS;
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            doc = uidoc.Document;
            sel = uidoc.Selection;
            ViewSheet viewSheet = doc.ActiveView as ViewSheet;
            string sheetname = CegTitleBlockParameter.Instance.GetSheetName(viewSheet);
            if (viewSheet != null)
            {
                if (viewSheet.IsAssemblyView)
                {
                    try
                    {
                        AssemblyInstance assemblyInstance = doc.GetElement(viewSheet.AssociatedAssemblyInstanceId) as AssemblyInstance;
                        FamilyInstance familyInstance = Getstructulral(assemblyInstance);
                        Caculator(familyInstance);
                        this.Products = GetProduct.GetAllProduct(doc);
                        this.AssemblySheetDict = CegTitleBlockParameter.Instance.AssemblySheetDictionary(doc);
                        this.SheetTitleBlockDict = CegTitleBlockParameter.Instance.GetSheetTitleBlock(doc, out this.SheetParameterDict, out this.TitleBlockParameterDict, out this.ViewSheetDict, out this.SheetList);
                        using (Transaction tran = new Transaction(doc, "Add value"))
                        {
                            tran.Start();
                            foreach (KeyValuePair<string, CegParameterSet> item in SheetParameterDict)
                            {
                                if (item.Key.Equals(sheetname))
                                {
                                    foreach (var item2 in item.Value.Parameters.Values)
                                    {
                                        switch (item2.Name)
                                        {
                                            case "ENGDSN":
                                                Parameter parameter1 = viewSheet.LookupParameter(item2.Name);
                                                bool flag1 = parameter1 == null || parameter1.IsReadOnly;
                                                if (!flag1)
                                                {
                                                    SetParameter(parameter1, DESIGNSHT_ENGDSN);
                                                }
                                                break;
                                            case "HDLDSN":
                                                Parameter parameter2 = viewSheet.LookupParameter(item2.Name);
                                                bool flag2 = parameter2 == null || parameter2.IsReadOnly;
                                                if (!flag2)
                                                {
                                                    SetParameter(parameter2, HANDLINGSHT_HDLDSN);
                                                }
                                                break;
                                            case "TKT_WEIGHT":
                                                Parameter parameter3 = viewSheet.LookupParameter(item2.Name);
                                                bool flag3 = parameter3 == null || parameter3.IsReadOnly;
                                                if (!flag3)
                                                {
                                                    SetParameter(parameter3, PRODWT_TKT_WEIGHT);
                                                }
                                                break;
                                            case "PRDTCODE":
                                                Parameter parameter4 = viewSheet.LookupParameter(item2.Name);
                                                bool flag4 = parameter4 == null || parameter4.IsReadOnly;
                                                if (!flag4)
                                                {
                                                    SetParameter(parameter4, PRODTYPE_PRDTCODE);
                                                }
                                                break;
                                            case "STRIPPING_STRENGTH":
                                                Parameter parameter5 = viewSheet.LookupParameter(item2.Name);
                                                bool flag5 = parameter5 == null || parameter5.IsReadOnly;
                                                if (!flag5)
                                                {
                                                    SetParameter(parameter5, RELEASESTR_STRIPPING_STRENGTH);
                                                }
                                                break;
                                            case "28_DAY_STRENGTH":
                                                Parameter parameter6 = viewSheet.LookupParameter(item2.Name);
                                                bool flag6 = parameter6 == null || parameter6.IsReadOnly;
                                                if (!flag6)
                                                {
                                                    SetParameter(parameter6, DAYSTR_28_DAY_STRENGTH);
                                                }
                                                break;
                                            case "TKT_CUYDS":
                                                Parameter parameter7 = viewSheet.LookupParameter(item2.Name);
                                                bool flag7 = parameter7 == null || parameter7.IsReadOnly;
                                                if (!flag7)
                                                {
                                                    SetParameter(parameter7, BACKUPC_TKT_CUYDS);
                                                }
                                                break;
                                            default:
                                                break;
                                        }
                                    }
                                }
                            }
                            tran.Commit();
                        }
                    }
                    catch
                    {

                    }
                }
                return Result.Succeeded;
            }
            else
            {
                return Result.Cancelled;
            }
        }

        public void SetParameter(Parameter p, CegParameterInfo oldInfo, CegParameterInfo newInfo)
        {
            switch (oldInfo.Type)
            {
                case StorageType.Integer:
                    {
                        bool flag = oldInfo.AsInteger != newInfo.AsInteger;
                        if (flag)
                        {
                            p.Set(newInfo.AsInteger);
                        }
                        break;
                    }
                case StorageType.Double:
                    {
                        bool flag2 = Math.Abs(oldInfo.AsDouble - newInfo.AsDouble) > 0.001;
                        if (flag2)
                        {
                            p.Set(newInfo.AsDouble);
                        }
                        break;
                    }
                case StorageType.String:
                    {
                        bool flag3 = oldInfo.AsString != newInfo.AsString;
                        if (flag3)
                        {
                            p.Set(newInfo.AsString);
                        }
                        break;
                    }
            }
        }
        public void SetParameter(Parameter p, string value)
        {
            p.Set(value);
        }
        public FamilyInstance Getstructulral(AssemblyInstance assemblyInstance)
        {
            FamilyInstance familyInstance = null;
            var list = assemblyInstance.GetMemberIds();
            foreach (var item in list)
            {
                Element ele = doc.GetElement(item);
                if (ele.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
                {
                    FamilyInstance g1 = ele as FamilyInstance;
                    if (g1.SuperComponent != null)
                    {
                        FamilyInstance result = g1.SuperComponent as FamilyInstance;
                        Parameter pa = result.LookupParameter("CONTROL_MARK");
                        if (pa != null)
                        {
                            string value = pa.AsString();
                            if (value.Equals(assemblyInstance.Name))
                            {
                                familyInstance = result;
                            }
                        }
                    }
                    else
                    {
                        Parameter pa = g1.LookupParameter("CONTROL_MARK");
                        if (pa != null)
                        {
                            string value = pa.AsString();
                            if (value.Equals(assemblyInstance.Name))
                            {
                                familyInstance = g1;
                            }
                        }
                    }
                }
            }
            return familyInstance;
        }
        public void Caculator(FamilyInstance familyInstance)
        {
            ElementtransformToCopy tr = new ElementtransformToCopy();
            FamilyInstance flat = tr.GetFlat(doc, familyInstance);
            if (flat != null)
            {
                Parameter volume = flat.LookupParameter("Volume");
                double Value = volume.AsDouble();
                double kl = Value * 150;
                double k1 = Math.Round(kl / 1000, 1);
                double k2 = k1 * 1000;
                PRODWT_TKT_WEIGHT = k2.ToString();
            }
            else
            {
                Parameter volume = familyInstance.LookupParameter("Volume");
                double Value = volume.AsDouble();
                double kl = Value * 150;
                double k1 = Math.Round(kl / 1000, 1);
                double k2 = k1 * 1000;
                PRODWT_TKT_WEIGHT = k2.ToString();
            }
            Parameter pa = familyInstance.LookupParameter("ENGDSN");
            if (pa != null)
            {
                DESIGNSHT_ENGDSN = pa.AsString();
            }
            else
            {
                DESIGNSHT_ENGDSN = "";
            }
            Parameter pa2 = familyInstance.LookupParameter("HDLDSN");
            if (pa2 != null)
            {
                HANDLINGSHT_HDLDSN = pa2.AsString();
            }
            else
            {
                HANDLINGSHT_HDLDSN = "";
            }
            Parameter pa3 = familyInstance.LookupParameter("PRDTCODE");
            if (pa3 != null)
            {
                PRODTYPE_PRDTCODE = pa3.AsString();
            }
            else
            {
                PRODTYPE_PRDTCODE = "";
            }
            switch (DESIGNSHT_ENGDSN)
            {
                case "2.1.0":
                    RELEASESTR_STRIPPING_STRENGTH = "4000";
                    DAYSTR_28_DAY_STRENGTH = "6500";
                    break;
                case "2.2.0":
                    RELEASESTR_STRIPPING_STRENGTH = "4000";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "2.3.0":
                    RELEASESTR_STRIPPING_STRENGTH = "4000";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "3.1.0":
                    RELEASESTR_STRIPPING_STRENGTH = "4000";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "3.2.0":
                    RELEASESTR_STRIPPING_STRENGTH = "4000";
                    DAYSTR_28_DAY_STRENGTH = "7500";
                    break;
                case "4.1.0":
                    RELEASESTR_STRIPPING_STRENGTH = "3500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "4.2.0":
                    RELEASESTR_STRIPPING_STRENGTH = "3500";
                    DAYSTR_28_DAY_STRENGTH = "7500";
                    break;
                case "4.3.0":
                    RELEASESTR_STRIPPING_STRENGTH = "3500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "4.4.0":
                    RELEASESTR_STRIPPING_STRENGTH = "3500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "4.5.0":
                    RELEASESTR_STRIPPING_STRENGTH = "3500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "5.1.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "5.2.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "5.3.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "7000";
                    break;
                case "5.4.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "5.6.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "6.1.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "6.2.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "7.1.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "7.2.0":
                    RELEASESTR_STRIPPING_STRENGTH = "2500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                case "7.3.0":
                    RELEASESTR_STRIPPING_STRENGTH = "3500";
                    DAYSTR_28_DAY_STRENGTH = "6000";
                    break;
                default:
                    break;
            }
            BACKUPC_TKT_CUYDS = Math.Round(Convert.ToDouble(PRODWT_TKT_WEIGHT) / (150 * 27), 2).ToString();
        }
    }
}
