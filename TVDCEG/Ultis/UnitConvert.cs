using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace TVDCEG.Ultis
{
    public class UnitConvert
    {
        public static double StringToFeetAndInches(string s, out string decimalFeet)
        {
            double num = 0.0;
            decimalFeet = string.Empty;
            bool flag = s == null;
            double result;
            if (flag)
            {
                result = num;
            }
            else
            {
                string text = string.Empty;
                string text2 = string.Empty;
                string text3 = string.Empty;
                string[] array = new string[]
                {
                    "",
                    "",
                    ""
                };
                char[] array2 = s.ToCharArray();
                int num2 = 0;
                for (int i = 0; i < array2.Length; i++)
                {
                    bool flag2 = num2 > 2;
                    if (flag2)
                    {
                        break;
                    }
                    bool flag3 = i == 0 && (array2[i].ToString() == "-" || array2[i].ToString() == "+");
                    if (!flag3)
                    {
                        string text4 = array2[i].ToString();
                        int num3;
                        bool flag4 = int.TryParse(text4, out num3);
                        if (flag4)
                        {
                            array[num2] += array2[i].ToString();
                        }
                        int num4;
                        bool flag5 = i < array2.Length - 1 && !int.TryParse(text4, out num3) && int.TryParse(array2[i + 1].ToString(), out num4);
                        if (flag5)
                        {
                            bool flag6 = array2[i].ToString() == "/";
                            if (flag6)
                            {
                                array[num2] += array2[i].ToString();
                            }
                            else
                            {
                                bool flag7 = i > 0 && array2[i - 1].ToString() != "-";
                                if (flag7)
                                {
                                    num2++;
                                }
                            }
                        }
                        bool flag8 = i != 0 && text4 == "-";
                        if (flag8)
                        {
                            num2++;
                        }
                    }
                }
                bool flag9 = array[0].Length > 0 && array[1].Length == 0 && array[2].Length == 0;
                if (flag9)
                {
                    text = array[0];
                }
                bool flag10 = array[0].Length > 0 && array[1].Length > 0 && array[2].Length == 0;
                if (flag10)
                {
                    bool flag11 = array[1].Contains("/");
                    if (flag11)
                    {
                        text = array[0];
                        text2 = "0";
                        text3 = array[1];
                    }
                    else
                    {
                        text = array[0];
                        text2 = array[1];
                    }
                }
                bool flag12 = array[0].Length > 0 && array[1].Length > 0 && array[2].Length > 0;
                if (flag12)
                {
                    text = array[0];
                    text2 = array[1];
                    text3 = array[2];
                }
                bool flag13 = string.IsNullOrEmpty(text);
                if (flag13)
                {
                    text = "0";
                }
                bool flag14 = string.IsNullOrEmpty(text2);
                if (flag14)
                {
                    text2 = "0";
                }
                bool flag15 = !string.IsNullOrEmpty(text3);
                if (flag15)
                {
                    decimalFeet = string.Concat(new string[]
                    {
                        text,
                        "' ",
                        text2,
                        " ",
                        text3,
                        "\""
                    });
                }
                bool flag16 = string.IsNullOrEmpty(text3);
                if (flag16)
                {
                    decimalFeet = text + "' " + text2 + "\"";
                }
                double num5 = 0.0;
                bool flag17 = double.TryParse(text, out num5);
                if (flag17)
                {
                    num += num5;
                }
                double num6 = 0.0;
                bool flag18 = double.TryParse(text2, out num6);
                if (flag18)
                {
                    double num7 = UnitUtils.Convert(num6, DisplayUnitType.DUT_DECIMAL_INCHES, DisplayUnitType.DUT_DECIMAL_FEET);
                    num += num7;
                }
                bool flag19 = text3.Length > 0 && text3.Contains("/");
                if (flag19)
                {
                    int num8 = text3.IndexOf("/");
                    string s2 = text3.Substring(0, num8);
                    string s3 = text3.Substring(num8 + 1, text3.Length - num8 - 1);
                    bool flag20 = double.TryParse(s2, out num5) && double.TryParse(s3, out num6);
                    if (flag20)
                    {
                        bool flag21 = num6 > 1E-23;
                        if (flag21)
                        {
                            double value = num5 / num6;
                            double num9 = UnitUtils.Convert(value, DisplayUnitType.DUT_DECIMAL_INCHES, DisplayUnitType.DUT_DECIMAL_FEET);
                            num += num9;
                        }
                    }
                }
                bool flag22 = s.StartsWith("-");
                if (flag22)
                {
                    num = -num;
                    decimalFeet = "-" + decimalFeet;
                }
                bool flag23 = Math.Abs(num) < double.Epsilon;
                if (flag23)
                {
                    decimalFeet = "0\"";
                }
                result = num;
            }
            return result;
        }
        public static string StringToFeetAndInchesformattext(string s)
        {
            double num = 0.0;
            string decimalFeet = string.Empty;
            bool flag = s == null;
            double result;
            if (flag)
            {
                result = num;
            }
            else
            {
                string text = string.Empty;
                string text2 = string.Empty;
                string text3 = string.Empty;
                string[] array = new string[]
                {
                    "",
                    "",
                    ""
                };
                char[] array2 = s.ToCharArray();
                int num2 = 0;
                for (int i = 0; i < array2.Length; i++)
                {
                    bool flag2 = num2 > 2;
                    if (flag2)
                    {
                        break;
                    }
                    bool flag3 = i == 0 && (array2[i].ToString() == "-" || array2[i].ToString() == "+");
                    if (!flag3)
                    {
                        string text4 = array2[i].ToString();
                        int num3;
                        bool flag4 = int.TryParse(text4, out num3);
                        if (flag4)
                        {
                            array[num2] += array2[i].ToString();
                        }
                        int num4;
                        bool flag5 = i < array2.Length - 1 && !int.TryParse(text4, out num3) && int.TryParse(array2[i + 1].ToString(), out num4);
                        if (flag5)
                        {
                            bool flag6 = array2[i].ToString() == "/";
                            if (flag6)
                            {
                                array[num2] += array2[i].ToString();
                            }
                            else
                            {
                                bool flag7 = i > 0 && array2[i - 1].ToString() != "-";
                                if (flag7)
                                {
                                    num2++;
                                }
                            }
                        }
                        bool flag8 = i != 0 && text4 == "-";
                        if (flag8)
                        {
                            num2++;
                        }
                    }
                }
                bool flag9 = array[0].Length > 0 && array[1].Length == 0 && array[2].Length == 0;
                if (flag9)
                {
                    text = array[0];
                }
                bool flag10 = array[0].Length > 0 && array[1].Length > 0 && array[2].Length == 0;
                if (flag10)
                {
                    bool flag11 = array[1].Contains("/");
                    if (flag11)
                    {
                        text = array[0];
                        text2 = "0";
                        text3 = array[1];
                    }
                    else
                    {
                        text = array[0];
                        text2 = array[1];
                    }
                }
                bool flag12 = array[0].Length > 0 && array[1].Length > 0 && array[2].Length > 0;
                if (flag12)
                {
                    text = array[0];
                    text2 = array[1];
                    text3 = array[2];
                }
                bool flag13 = string.IsNullOrEmpty(text);
                if (flag13)
                {
                    text = "0";
                }
                bool flag14 = string.IsNullOrEmpty(text2);
                if (flag14)
                {
                    text2 = "0";
                }
                bool flag15 = !string.IsNullOrEmpty(text3);
                if (flag15)
                {
                    decimalFeet = string.Concat(new string[]
                    {
                        text,
                        "' ",
                        "-",
                        " ",
                        text2,
                        " ",
                        text3,
                        "\""
                    });
                }
                bool flag16 = string.IsNullOrEmpty(text3);
                if (flag16)
                {
                    decimalFeet = text + "' " + "-" +" "+ text2 + "\"";
                }
                double num5 = 0.0;
                bool flag17 = double.TryParse(text, out num5);
                if (flag17)
                {
                    num += num5;
                }
                double num6 = 0.0;
                bool flag18 = double.TryParse(text2, out num6);
                if (flag18)
                {
                    double num7 = UnitUtils.Convert(num6, DisplayUnitType.DUT_DECIMAL_INCHES, DisplayUnitType.DUT_DECIMAL_FEET);
                    num += num7;
                }
                bool flag19 = text3.Length > 0 && text3.Contains("/");
                if (flag19)
                {
                    int num8 = text3.IndexOf("/");
                    string s2 = text3.Substring(0, num8);
                    string s3 = text3.Substring(num8 + 1, text3.Length - num8 - 1);
                    bool flag20 = double.TryParse(s2, out num5) && double.TryParse(s3, out num6);
                    if (flag20)
                    {
                        bool flag21 = num6 > 1E-23;
                        if (flag21)
                        {
                            double value = num5 / num6;
                            double num9 = UnitUtils.Convert(value, DisplayUnitType.DUT_DECIMAL_INCHES, DisplayUnitType.DUT_DECIMAL_FEET);
                            num += num9;
                        }
                    }
                }
                bool flag22 = s.StartsWith("-");
                if (flag22)
                {
                    num = -num;
                    decimalFeet = "-" + decimalFeet;
                }
                bool flag23 = Math.Abs(num) < double.Epsilon;
                if (flag23)
                {
                    decimalFeet = "0'";
                }
                result = num;
            }
            return decimalFeet;
        }
        public static string DoubleToImperial(double doubleLength)
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
            return text2 + "' " + str + "\"";
        }
        public static double StringToInch(string s, out string inch)
        {
            double num = 0.0;
            inch = string.Empty;
            bool flag = s == null;
            double result;
            if (flag)
            {
                result = num;
            }
            else
            {
                bool flag2 = s.EndsWith("''");
                if (flag2)
                {
                    s = s.TrimEnd("''".ToCharArray());
                }
                bool flag3 = double.TryParse(s, out num);
                if (flag3)
                {
                    inch = s + "''";
                }
                bool flag4 = s.Contains(" ") && s.Contains("/");
                if (flag4)
                {
                    int num2 = s.IndexOf(" ");
                    int num3 = s.IndexOf("/");
                    string s2 = s.Substring(0, num2);
                    string s3 = s.Substring(num2 + 1, num3 - num2 - 1);
                    string s4 = s.Substring(num3 + 1, s.Length - num3 - 1);
                    double num4;
                    double num5 = double.MinValue;
                    double num6 = double.MinValue;
                    bool flag5 = double.TryParse(s2, out num4) && double.TryParse(s3, out num5) && double.TryParse(s4, out num6);
                    if (flag5)
                    {
                        num = num4 + (num5 / num6);
                    }
                    inch = s + "''";
                }
                bool flag6 = s.Contains("/");
                if (flag6)
                {
                    int num7 = s.IndexOf("/");
                    string s5 = s.Substring(0, num7);
                    string s6 = s.Substring(num7 + 1, s.Length - num7 - 1);
                    double num8;
                    double num9 = double.MinValue;
                    bool flag7 = double.TryParse(s5, out num8) && double.TryParse(s6, out num9);
                    if (flag7)
                    {
                        num = num8 / num9;
                    }
                    inch = s + "''";
                }
                num /= 12.0;
                result = num;
            }
            return result;
        }
        public static string DoubleToFeetsAndInches(double d)
        {
            string empty = string.Empty;
            int num = (int)d;
            double num2 = d - (double)num;
            int num3 = (int)(num2 * 12.0);
            double num4 = d - (double)num - (double)num3 / 12.0;
            int num5 = (int)(num4 * 12.0 * 256.0);
            bool flag = Math.Abs((double)num5 - 12.0 * num4 * 256.0) > 0.5;
            if (flag)
            {
                num5++;
            }
            int num6 = num5;
            int num7 = 256;
            while (num6 % 2 == 0 && num7 % 2 == 0)
            {
                num6 /= 2;
                num7 /= 2;
            }
            string text = string.Empty;
            bool flag2 = num5 != 0;
            if (flag2)
            {
                text = " " + num6.ToString() + "/" + num7.ToString();
            }
            bool flag3 = num6 == num7;
            if (flag3)
            {
                text = string.Empty;
                num3++;
            }
            bool flag4 = num3 == 12;
            if (flag4)
            {
                num++;
                num3 = 0;
            }
            return string.Concat(new string[]
            {
                num.ToString(),
                "' ",
                num3.ToString(),
                text,
                "\""
            });
        }

        public static string DoubleToFeetsAndInches(double d, string roundOff)
        {
            string text;
            double num = StringToInch(roundOff, out text);
            int num2 = (int)(1.0 / (num * 12.0));
            string empty = string.Empty;
            int num3 = (int)d;
            double num4 = d - (double)num3;
            int num5 = (int)(num4 * 12.0);
            double num6 = d - (double)num3 - (double)num5 / 12.0;
            int num7 = (int)(num6 * 12.0 * (double)num2);
            bool flag = Math.Abs((double)num7 - 12.0 * num6 * (double)num2) > 0.5;
            if (flag)
            {
                num7++;
            }
            int num8 = num7;
            int num9 = num2;
            while (num8 % 2 == 0 && num9 % 2 == 0)
            {
                num8 /= 2;
                num9 /= 2;
            }
            string text2 = string.Empty;
            bool flag2 = num7 != 0;
            if (flag2)
            {
                text2 = " " + num8.ToString() + "/" + num9.ToString();
            }
            bool flag3 = num8 == num9;
            if (flag3)
            {
                text2 = string.Empty;
                num5++;
            }
            bool flag4 = num5 == 12;
            if (flag4)
            {
                num3++;
                num5 = 0;
            }
            return string.Concat(new string[]
            {
                num3.ToString(),
                "' - ",
                num5.ToString(),
                text2,
                "''"
            });
        }
        public static string DoubleToInches(double d)
        {
            string empty = string.Empty;
            int num = (int)d;
            double num2 = d - (double)num;
            int num3 = (int)(num2 * 12.0);
            double num4 = d - (double)num - (double)num3 / 12.0;
            int num5 = (int)(num4 * 12.0 * 256.0);
            bool flag = Math.Abs((double)num5 - 12.0 * num4 * 256.0) > 0.5;
            if (flag)
            {
                num5++;
            }
            int num6 = num5;
            int num7 = 256;
            while (num6 % 2 == 0 && num7 % 2 == 0)
            {
                num6 /= 2;
                num7 /= 2;
            }
            string str = string.Empty;
            bool flag2 = num5 != 0;
            if (flag2)
            {
                str = " " + num6.ToString() + "/" + num7.ToString();
            }
            bool flag3 = num6 == num7;
            if (flag3)
            {
                str = string.Empty;
                num3++;
            }
            bool flag4 = num3 == 12;
            if (flag4)
            {
                num++;
                num3 = 0;
            }
            return (num * 12 + num3).ToString() + str + "''";
        }
    }

}
