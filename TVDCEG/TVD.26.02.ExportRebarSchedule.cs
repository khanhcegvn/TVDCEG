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
using System.Collections;
using TVDCEG.LBR;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using Microsoft.Office.Interop.Excel;
#endregion

namespace TVDCEG
{
    public class ExportRebarSchedule
    {
        private static ExportRebarSchedule _instance;
        private ExportRebarSchedule()
        {

        }
        public static ExportRebarSchedule Instance => _instance ?? (_instance = new ExportRebarSchedule());
        public Dictionary<string, List<CEGRebarInfo>> GetAllRebar(Document doc)
        {
            Dictionary<string, List<CEGRebarInfo>> dic = new Dictionary<string, List<CEGRebarInfo>>();
            Dictionary<string, List<CEGRebarInfo>> dic2 = new Dictionary<string, List<CEGRebarInfo>>();
            List<FamilyInstance> col = (from FamilyInstance x in new FilteredElementCollector(doc).WhereElementIsNotElementType().OfCategory(BuiltInCategory.OST_SpecialityEquipment).OfClass(typeof(FamilyInstance)) where x.Symbol.LookupParameter("BAR_SHAPE") != null select x).Cast<FamilyInstance>().ToList();
            ProgressbarWPF progressbarWPF = new ProgressbarWPF(col.Count, "Export Schedule Rebar") { topmessage = "Collecting rebar..." };
            progressbarWPF.Show();
            foreach (var item in col)
            {
                progressbarWPF.Giatri();
                if (!progressbarWPF.iscontinue)
                {
                    break;
                }
                Autodesk.Revit.DB.Parameter parameter = (item != null) ? item.Symbol.LookupParameter("BAR_SHAPE") : null;
                bool fg = string.IsNullOrEmpty((parameter != null) ? parameter.AsString() : null);
                if (!fg)
                {
                    CEGRebarInfo rebar = new CEGRebarInfo(item);
                    if (rebar.UnistImperial != null && rebar.UnistImperial != "")
                    {
                        if (dic.ContainsKey(rebar.UnistImperial))
                        {
                            dic[rebar.UnistImperial].Add(rebar);
                        }
                        else
                        {
                            dic.Add(rebar.UnistImperial, new List<CEGRebarInfo> { rebar });
                        }
                    }
                }
            }
            var keys = dic.Keys.ToList();
            Caculator(keys);
            foreach (var item in keys)
            {
                foreach (var item2 in dic[item])
                {
                    if (dic2.ContainsKey(item2.UnistImperial))
                    {
                        dic2[item2.UnistImperial].Add(item2);
                    }
                    else
                    {
                        dic2.Add(item2.UnistImperial, new List<CEGRebarInfo> { item2 });
                    }
                }
            }
            progressbarWPF.Close();
            return dic2;
        }
        private void Caculator(List<string> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = 0; j < list.Count; j++)
                {
                    var val1 = Convert.ToDouble(list[i]);
                    var val2 = Convert.ToDouble(list[j]);
                    if (val1 < val2)
                    {
                        var temp = list[i];
                        list[i] = list[j];
                        list[j] = temp;
                    }
                }
            }
        }
        public void Export(ObservableCollection<List<CEGRebarInfo>> list)
        {
            _Application application = (Microsoft.Office.Interop.Excel.Application)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("00024500-0000-0000-C000-000000000046")));
            _Workbook workbook = application.Workbooks.Add(Type.Missing);
            int count = workbook.Sheets.Count;
            _Worksheet worksheet = (_Worksheet)workbook.Worksheets.Add((dynamic)workbook.Sheets[count], Type.Missing, Type.Missing, Type.Missing);
            worksheet.Name = "Rebar Schedule";
            dynamic val = worksheet.Range[(dynamic)worksheet.Cells[1, 1], (dynamic)worksheet.Cells[1, 14]];
            val.Font.Bold = true;
            //val.Merge();
            val = worksheet.Range[(dynamic)worksheet.Cells[1, 1], (dynamic)worksheet.Cells[1, 14]];
            val.Font.Bold = true;
            //val.Merge();
            worksheet.Cells[1, 1] = "Control Mark";
            worksheet.Cells[1, 2] = "Bar Size";
            worksheet.Cells[1, 3] = "Overall Length";
            worksheet.Cells[1, 4] = "Type NO.";
            worksheet.Cells[1, 5] = "Bend Dia.";
            worksheet.Cells[1, 6] = "A";
            worksheet.Cells[1, 7] = "B";
            worksheet.Cells[1, 8] = "C";
            worksheet.Cells[1, 9] = "D";
            worksheet.Cells[1, 10] = "E";
            worksheet.Cells[1, 11] = "F";
            worksheet.Cells[1, 12] = "G";
            worksheet.Cells[1, 13] = "H";
            worksheet.Cells[1, 14] = "K";
            val = worksheet.Range[(dynamic)worksheet.Cells[1, 1], (dynamic)worksheet.Cells[1, 14]];
            val.Font.Bold = true;
            val.Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            int num = 2;
            foreach (var item in list)
            {
                worksheet.Cells[num, 1] = item.First().ControlMark;
                worksheet.Cells[num, 2] = item.First().UnistImperial;
                worksheet.Cells[num, 3] = item.First().DimLength;
                worksheet.Cells[num, 4] = item.First().TypeNote;
                worksheet.Cells[num, 5] = item.First().BenDia;
                worksheet.Cells[num, 6] = item.First().BarLengthA;
                worksheet.Cells[num, 7] = item.First().BarLengthB;
                worksheet.Cells[num, 8] = item.First().BarLengthC;
                worksheet.Cells[num, 9] = item.First().BarLengthD;
                worksheet.Cells[num, 10] = item.First().BarLengthE;
                worksheet.Cells[num, 11] = item.First().BarLengthF;
                worksheet.Cells[num, 12] = item.First().BarLengthG;
                worksheet.Cells[num, 13] = item.First().BarLengthH;
                worksheet.Cells[num, 14] = item.First().BarLengthK;
                num++;
            }
            application.Visible = true;
            application.WindowState = XlWindowState.xlMaximized;
        }
    }
    public class CEGRebarInfo
    {
        public string UnistImperial { set; get; }
        public string UnistMetric { get; set; }
        public string TypeNote { get; set; }
        public string BenDia { get; set; }
        public string BarPrefix { get; set; }
        public string BarShape { get; set; }
        public string DimLength { get; set; }
        public string BarLengthA { get; set; }
        public string BarLengthB { get; set; }
        public string BarLengthC { get; set; }
        public string BarLengthD { get; set; }
        public string BarLengthE { get; set; }
        public string BarLengthF { get; set; }
        public string BarLengthG { get; set; }
        public string BarLengthH { get; set; }
        public string BarLengthK { get; set; }
        public string ImperialSize { get; set; }
        public string MetricSize { get; set; }
        public string ControlMark { get; set; }
        public CEGRebarInfo(FamilyInstance familyInstance)
        {
            string imperialSize;
            string metricSize;
            GetBarNumber(familyInstance, out imperialSize, out metricSize);
            this.UnistImperial = imperialSize;
            this.UnistMetric = UnistMetric;
            Autodesk.Revit.DB.Parameter parameter = familyInstance.LookupParameter("BAR_PREFIX");
            bool flag = parameter != null && parameter.StorageType == StorageType.String;
            if (flag)
            {
                this.BarPrefix = parameter.AsString();
            }
            Autodesk.Revit.DB.Parameter parameter2 = familyInstance.LookupParameter("CONTROL_MARK");
            bool flag2 = parameter2 != null && parameter2.StorageType == StorageType.String;
            if (flag2)
            {
                this.ControlMark = parameter2.AsString();
            }
            Autodesk.Revit.DB.Parameter parameter3 = familyInstance.Symbol.LookupParameter("BAR_SHAPE");
            bool flag3 = parameter3 != null && parameter3.StorageType == StorageType.String;
            if (flag3)
            {
                this.BarShape = parameter3.AsString();
            }
            Autodesk.Revit.DB.Parameter parameter4 = familyInstance.LookupParameter("DIM_LENGTH");
            bool flag4 = parameter4 != null && parameter4.StorageType == StorageType.Double;
            if (flag4)
            {
                this.DimLength = ConVertDoubleToImperial(parameter4.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter5 = familyInstance.LookupParameter("BAR_LENGTH_A");
            bool flag5 = parameter5 != null && parameter5.StorageType == StorageType.Double;
            if (flag5)
            {
                this.BarLengthA = ConVertDoubleToImperial(parameter5.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter6 = familyInstance.LookupParameter("BAR_LENGTH_B");
            bool flag6 = parameter6 != null && parameter6.StorageType == StorageType.Double;
            if (flag6)
            {
                this.BarLengthB = ConVertDoubleToImperial(parameter6.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter7 = familyInstance.LookupParameter("BAR_LENGTH_C");
            bool flag7 = parameter7 != null && parameter7.StorageType == StorageType.Double;
            if (flag7)
            {
                this.BarLengthC = ConVertDoubleToImperial(parameter7.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter8 = familyInstance.LookupParameter("BAR_LENGTH_D");
            bool flag8 = parameter8 != null && parameter8.StorageType == StorageType.Double;
            if (flag8)
            {
                this.BarLengthD = ConVertDoubleToImperial(parameter8.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter9 = familyInstance.LookupParameter("BAR_LENGTH_E");
            bool flag9 = parameter9 != null && parameter9.StorageType == StorageType.Double;
            if (flag9)
            {
                this.BarLengthE = ConVertDoubleToImperial(parameter9.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter10 = familyInstance.LookupParameter("BAR_LENGTH_F");
            bool flag10 = parameter10 != null && parameter10.StorageType == StorageType.Double;
            if (flag10)
            {
                this.BarLengthF = ConVertDoubleToImperial(parameter10.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter11 = familyInstance.LookupParameter("BAR_LENGTH_G");
            bool flag11 = parameter11 != null && parameter11.StorageType == StorageType.Double;
            if (flag11)
            {
                this.BarLengthG = ConVertDoubleToImperial(parameter11.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter12 = familyInstance.LookupParameter("BAR_LENGTH_H");
            bool flag12 = parameter12 != null && parameter12.StorageType == StorageType.Double;
            if (flag12)
            {
                this.BarLengthH = ConVertDoubleToImperial(parameter12.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter13 = familyInstance.LookupParameter("BAR_LENGTH_K");
            bool flag13 = parameter13 != null && parameter13.StorageType == StorageType.Double;
            if (flag13)
            {
                this.BarLengthK = ConVertDoubleToImperial(parameter13.AsDouble());
            }
            Autodesk.Revit.DB.Parameter parameter14 = familyInstance.LookupParameter("TypeNote");
            bool flag14 = parameter14 != null && parameter14.StorageType == StorageType.String;
            if (flag14)
            {
                this.TypeNote = parameter14.AsString();
            }
            Autodesk.Revit.DB.Parameter parameter15 = familyInstance.LookupParameter("BenDia");
            bool flag15 = parameter15 != null && parameter15.StorageType == StorageType.Double;
            if (flag15)
            {
                this.BenDia = ConVertDouble(parameter15.AsDouble());
            }
        }
        public static string ConVertDoubleToImperial(double doubleLength)
        {
            string empty = string.Empty;
            double num = UnitUtils.Convert(doubleLength, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES);
            Units units = new Units(UnitSystem.Imperial);
            UnitType unitType = UnitType.UT_Length;
            string text = UnitFormatUtils.Format(units, unitType, doubleLength, true, true);
            int num2 = text.IndexOf("'");
            string text2 = text;
            text2 = text2.Remove(num2, text.Length - num2);
            text = text.Remove(0, num2 + 1).Trim();
            num2 = text.IndexOf("\"");
            string str = text.Remove(num2, text.Length - num2);
            return text2 + "'" + "-" + str + "\"";
        }
        public static string ConVertDouble(double doubleLength)
        {
            string empty = string.Empty;
            double num = UnitUtils.Convert(doubleLength, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_FEET_FRACTIONAL_INCHES);
            Units units = new Units(UnitSystem.Imperial);
            UnitType unitType = UnitType.UT_Length;
            string text = UnitFormatUtils.Format(units, unitType, doubleLength, true, true);
            int num2 = text.IndexOf("'");
            string text2 = text;
            text2 = text2.Remove(num2, text.Length - num2);
            text = text.Remove(0, num2 + 1).Trim();
            num2 = text.IndexOf("\"");
            string str = text.Remove(num2, text.Length - num2);
            //return text2 + "' " + str + "\"";
            return str;
        }
        public void GetBarNumber(double d, out string imperial, out string metric)
        {
            imperial = "";
            metric = "";
            bool flag = MathLib.IsEqual(d, 0.03125, 0.0001);
            if (flag)
            {
                imperial = "3";
                metric = "10";
            }
            else
            {
                bool flag2 = MathLib.IsEqual(d, 0.04167, 0.001);
                if (flag2)
                {
                    imperial = "4";
                    metric = "13";
                }
                else
                {
                    bool flag3 = MathLib.IsEqual(d, 0.05208, 0.001);
                    if (flag3)
                    {
                        imperial = "5";
                        metric = "16";
                    }
                    else
                    {
                        bool flag4 = MathLib.IsEqual(d, 0.0625, 0.001);
                        if (flag4)
                        {
                            imperial = "6";
                            metric = "19";
                        }
                        else
                        {
                            bool flag5 = MathLib.IsEqual(d, 0.07292, 0.001);
                            if (flag5)
                            {
                                imperial = "7";
                                metric = "22";
                            }
                            else
                            {
                                bool flag6 = MathLib.IsEqual(d, 0.08333, 0.001);
                                if (flag6)
                                {
                                    imperial = "8";
                                    metric = "25";
                                }
                                else
                                {
                                    bool flag7 = MathLib.IsEqual(d, 0.09408, 0.001);
                                    if (flag7)
                                    {
                                        imperial = "9";
                                        metric = "29";
                                    }
                                    else
                                    {
                                        bool flag8 = MathLib.IsEqual(d, 0.10579, 0.001);
                                        if (flag8)
                                        {
                                            imperial = "10";
                                            metric = "32";
                                        }
                                        else
                                        {
                                            bool flag9 = MathLib.IsEqual(d, 0.11751, 0.001);
                                            if (flag9)
                                            {
                                                imperial = "11";
                                                metric = "36";
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public void GetBarNumber(FamilyInstance rebarInstance, out string imperial, out string metric)
        {
            imperial = "";
            metric = "";
            Autodesk.Revit.DB.Parameter parameter = rebarInstance.Symbol.LookupParameter("BAR_DIAMETER");
            bool flag = parameter == null || parameter.StorageType != StorageType.Double;
            if (!flag)
            {
                double num = parameter.AsDouble();
                this.GetBarNumber(num, out imperial, out metric);
                bool flag2 = MathLib.IsEqual(num, 0.03125, 0.0001);
                if (flag2)
                {
                    imperial = "3";
                    metric = "10";
                }
                else
                {
                    bool flag3 = MathLib.IsEqual(num, 0.04167, 0.001);
                    if (flag3)
                    {
                        imperial = "4";
                        metric = "13";
                    }
                    else
                    {
                        bool flag4 = MathLib.IsEqual(num, 0.05208, 0.001);
                        if (flag4)
                        {
                            imperial = "5";
                            metric = "16";
                        }
                        else
                        {
                            bool flag5 = MathLib.IsEqual(num, 0.0625, 0.001);
                            if (flag5)
                            {
                                imperial = "6";
                                metric = "19";
                            }
                            else
                            {
                                bool flag6 = MathLib.IsEqual(num, 0.07292, 0.001);
                                if (flag6)
                                {
                                    imperial = "7";
                                    metric = "22";
                                }
                                else
                                {
                                    bool flag7 = MathLib.IsEqual(num, 0.08333, 0.001);
                                    if (flag7)
                                    {
                                        imperial = "8";
                                        metric = "25";
                                    }
                                    else
                                    {
                                        bool flag8 = MathLib.IsEqual(num, 0.09408, 0.001);
                                        if (flag8)
                                        {
                                            imperial = "9";
                                            metric = "29";
                                        }
                                        else
                                        {
                                            bool flag9 = MathLib.IsEqual(num, 0.10579, 0.001);
                                            if (flag9)
                                            {
                                                imperial = "10";
                                                metric = "32";
                                            }
                                            else
                                            {
                                                bool flag10 = MathLib.IsEqual(num, 0.11751, 0.001);
                                                if (flag10)
                                                {
                                                    imperial = "11";
                                                    metric = "36";
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
