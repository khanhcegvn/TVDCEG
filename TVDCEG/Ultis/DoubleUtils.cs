using Autodesk.Revit.DB;
using System;
using System.Runtime.CompilerServices;

namespace TVDCEG.LBR
{
    public static class DoubleUtils
    {
        private const double METER_TO_FEET = 3.2808399;
        public static double NormalizeAngle(this double value)
        {
            double num = value;
            while (LT_EQ(num, -6.2831853071795862) || GT_EQ(num, 6.2831853071795862))
            {
                num = (!EQ(num, 6.2831853071795862) && !EQ(num, -6.2831853071795862)) ? (num - Math.Floor(num / 6.2831853071795862) * 3.1415926535897931 * 2.0) : 0.0;
            }
            return num;
        }

        public static bool TrySetValue(this double value, string input)
        {
            return double.TryParse(input, out value);
        }
        public static double Getlength(this FamilyInstance familyInstance)
        {
            Parameter pa = familyInstance.LookupParameter("DIM_LENGTH");
            if(pa!=null)
            {
                return pa.AsDouble();
            }
            else
            {
                return 0;
            }
        }
        public static double GetWidthStemOfDoubletee(this FamilyInstance familyInstance,Document doc)
        {
            FamilyInstance familyInstance1 = null;
            Element p = familyInstance.SuperComponent;
            if (p!=null)
            {
                familyInstance1 = p as FamilyInstance;
            }
            else
            {
                familyInstance1 = familyInstance;
            }
            ElementId type = familyInstance1.GetTypeId();
            Element ele = doc.GetElement(type);
            Parameter pa = ele.LookupParameter("DT_Stem_Spacing_Form");
            if (pa != null)
            {
                return pa.AsDouble();
            }
            else
            {
                return 0;
            }
        }
        public static FamilySymbol GetSymbol(this FamilyInstance familyInstance, Document doc)
        {
            FamilyInstance familyInstance1 = null;
            Element p = familyInstance.SuperComponent;
            if (p != null)
            {
                familyInstance1 = p as FamilyInstance;
            }
            else
            {
                familyInstance1 = familyInstance;
            }
            ElementId type = familyInstance1.GetTypeId();
            Element ele = doc.GetElement(type);
            return ele as FamilySymbol;
        }
        public static double R2D(this double value)
        {
            return MathLib.RadToDeg(value);
        }

        public static double D2R(this double value)
        {
            return MathLib.DegToRad(value);
        }

        public static bool EQ(this double value, double compareTo, double eps)
        {
            return MathLib.EQ(value, compareTo, eps);
        }

        public static bool EQ(this double value, double compareTo)
        {
            return MathLib.EQ(value, compareTo);
        }

        public static bool GT_EQ(this double value, double compareTo)
        {
            return MathLib.GTE(value, compareTo);
        }

        public static bool LT_EQ(this double value, double compareTo)
        {
            return MathLib.LTE(value, compareTo);
        }

        public static bool GT(this double value, double compareTo)
        {
            return value > compareTo;
        }

        public static bool LT(this double value, double compareTo)
        {
            return value < compareTo;
        }

        public static double M2F(this double value)
        {
            return value * 3.2808399;
        }

        public static double F2M(this double value)
        {
            return value / 3.2808399;
        }

        public static int ToInt(this decimal value)
        {
            return Convert.ToInt32(value);
        }

        public static double ToDouble(this decimal value)
        {
            return Convert.ToDouble(value);
        }

        public static int ToInt(this double value)
        {
            return Convert.ToInt32(value);
        }

        public static double RandomDouble(this Random r, double minimum, double maximum)
        {
            return r.NextDouble() * (maximum - minimum) + minimum;
        }
    }
}
