using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace TVDCEG.LBR
{
    public class MathLib
    {
        private static double _eps = 1E-14;

        private static double _scale = 1.0;

        public static string StripUnits(string input)
        {
            return Regex.Replace(input, "[A-Za-z/¹²³'\"°()·\\[\\]%$€£¥₩₪฿₫]", "").Trim();
        }

        public static double Eps
        {
            get
            {
                return _eps;
            }
            set
            {
                _eps = value;
            }
        }

        public static double Scale => _scale;

        public static void SetScaleFactor(double newScale)
        {
            double num = newScale / _scale;
            _scale = newScale;
            _eps *= num;
        }

        public static int CMP(double d1, double d2)
        {
            if (EQ(d1, d2))
            {
                return 0;
            }
            if (!(d1 > d2))
            {
                return -1;
            }
            return 1;
        }

        public static bool LT(double d1, double d2)
        {
            int digits = Eps.ToString().Length - 2;
            return CMP(Math.Round(d1, digits), Math.Round(d2, digits)) == -1;
        }
        public static bool IsEqual(double d1, double d2, double epsilon)
        {
            bool result = false;
            double num = Math.Abs(d1 - d2);
            bool flag = num < epsilon;
            if (flag)
            {
                result = true;
            }
            return result;
        }
        public static bool GT(double d1, double d2)
        {
            int digits = Eps.ToString().Length - 2;
            return CMP(Math.Round(d1, digits), Math.Round(d2, digits)) == 1;
        }

        public static bool LTE(double d1, double d2)
        {
            int exponent = GetExponent(Eps);
            return CMP(Math.Round(d1, exponent), Math.Round(d2, exponent)) <= 0;
        }

        public static bool GTE(double d1, double d2)
        {
            int exponent = GetExponent(Eps);
            return CMP(Math.Round(d1, exponent), Math.Round(d2, exponent)) >= 0;
        }

        public static int GetExponent(double d)
        {
            string[] array = d.ToString("e").Split('e');
            if (array != null && array.Length > 1)
            {
                try
                {
                    return Math.Abs(int.Parse(array[1]));
                }
                catch
                {
                    return 1;
                }
            }
            return 1;
        }

        public static bool EQ(double d1, double d2)
        {
            return EQ(d1, d2, Eps);
        }

        public static bool EQ(double d1, double d2, double eps)
        {
            if (d1 == 0.0)
            {
                return Math.Abs(d2) <= eps;
            }
            if (d2 == 0.0)
            {
                return Math.Abs(d1) <= eps;
            }
            return Math.Abs(d1 - d2) / Math.Max(Math.Abs(d1), Math.Abs(d2)) <= eps;
        }

        public static double RadToDeg(double ang)
        {
            return 57.295779513082323 * ang;
        }

        public static double DegToRad(double ang)
        {
            return 3.1415926535897931 * ang / 180.0;
        }

        public static int double_det2x2(double a, double b, double c, double d)
        {
            if (!(a * d < b * c))
            {
                return 1;
            }
            return -1;
        }

        public static int leda_integer_det2x2(double a, double b, double c, double d)
        {
            return Math.Sign((int)a * (int)d - (int)b * (int)c);
        }

        public static bool IsInfinity(double d)
        {
            if (!double.IsInfinity(d))
            {
                return Math.Abs(d) > 10000.0 / Scale;
            }
            return true;
        }

        public static bool IsNumeric(object ObjectToTest)
        {
            return IsNumeric(ObjectToTest, CultureInfo.CurrentCulture);
        }

        public static bool IsNumeric(object ObjectToTest, IFormatProvider provider)
        {
            if (ObjectToTest == null)
            {
                return false;
            }
            double result;
            return double.TryParse(ObjectToTest.ToString().Trim(), NumberStyles.Any, provider, out result);
        }

        public static bool IsInteger(object ObjectToTest)
        {
            if (ObjectToTest == null)
            {
                return false;
            }
            int result;
            return int.TryParse(ObjectToTest.ToString().Trim(), NumberStyles.Any, CultureInfo.CurrentCulture, out result);
        }

        public static bool ToDouble(string input, out double outDbl)
        {
            string numberDecimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;
            outDbl = 0.0;
            if (input.Contains(numberDecimalSeparator))
            {
                if (double.TryParse(input, out outDbl))
                {
                    return true;
                }
            }
            else if (numberDecimalSeparator == ".")
            {
                if (double.TryParse(input.Replace(",", "."), out outDbl))
                {
                    return true;
                }
                if (numberDecimalSeparator == ",")
                {
                    if (double.TryParse(input.Replace(".", ","), out outDbl))
                    {
                        return true;
                    }
                }
                else if (double.TryParse(input, out outDbl))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
