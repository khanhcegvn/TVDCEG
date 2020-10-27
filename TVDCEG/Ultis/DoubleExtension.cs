using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVDCEG.Ultis
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Rounds the value to the nearest increment. 
        /// Assumes mid-point rounding, value >= 0.5 rounds up, value < 0.5 rounds down.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="increment">Enter the increment value to round toward.</param>
        /// <returns>Returns the value rounded to the nearest increment value.</returns>
        public static double RoundToNearest(this double Value, double increment)
        {
            // Returning the value rounded to the nearest increment value.
            return Math.Round(Value * Math.Pow(increment, -1), 0) * increment;
        }

        /// <summary>
        /// Rounds down the value to the nearest increment. 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="increment">Enter the increment value to round down toward.</param>
        /// <returns>Returns the value rounded down to the nearest increment value.</returns>
        public static double FloorToNearest(this double Value, double increment)
        {
            // Returning the value rounded down to the nearest increment value.
            return Math.Floor(Value * Math.Pow(increment, -1)) * increment;
        }

        /// <summary>
        /// Rounds up the value to the nearest increment. 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="increment">Enter the increment value to round up toward.</param>
        /// <returns>Returns the value rounded up to the nearest increment value.</returns>
        public static double CeilingToNearest(this double Value, double increment)
        {
            // Returning the value rounded up to the nearest increment value.
            return Math.Ceiling(Value * Math.Pow(increment, -1)) * increment;
        }

        /// <summary>
        /// Rounds the value down to the nearest imperial fractional increment
        /// and converts the value into an Inch-Fraction (IF) string. 
        /// Note: Assumes value is in inches and does not convert to Feet-Inch-Fraction (FIF)
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="maxDenominator">Enter the maximum denominator to round toward (i.e. 1/16 --> 16)</param>
        /// <returns>Returns the value rounded down to the nearest increment value based on the maxDenominator.</returns>
        public static string FloorToInchFraction(this double Value, int maxDenominator)
        {
            // Returning the rounded value converted into an Inch-Fraction (IF) string.
            return Value.FloorToNearest(Math.Pow(maxDenominator, -1)).ToInchFraction(maxDenominator);
        }

        /// <summary>
        /// Rounds the value up to the nearest imperial fractional increment
        /// and converts the value into an Inch-Fraction (IF) string. 
        /// Note: Assumes value is in inches and does not convert to Feet-Inch-Fraction (FIF)
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="maxDenominator">Enter the maximum denominator to round toward (i.e. 1/16 --> 16)</param>
        /// <returns>Returns the value rounded up to the nearest increment value based on the maxDenominator.</returns>
        public static string CeilingToInchFraction(this double Value, int maxDenominator)
        {
            // Returning the rounded value converted into a fraction string.
            return Value.CeilingToNearest(Math.Pow(maxDenominator, -1)).ToInchFraction(maxDenominator);
        }

        /// <summary>
        /// Rounds the value to the nearest increment value based on the maximum denominator specified.
        /// Assumes mid-point rounding, value >= 0.5 rounds up, value < 0.5 rounds down.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="maxDenominator">Enter the maximum denominator to round toward (i.e. 1/16 --> 16)</param>
        /// <returns>Returns the value rounded to the nearest increment value based on the maxDenominator.</returns>
        public static string ToInchFraction(this double Value, int maxDenominator)
        {
            // Calculating the nearest increment of the value
            // argument based on the denominator argument.
            double incValue = Value.RoundToNearest(Math.Pow(maxDenominator, -1));

            // Identifying the whole number of the argument value.
            int wholeValue = (int)Math.Truncate(incValue);

            // Calculating the remainder of the argument value and the whole value.
            double remainder = incValue - wholeValue;

            // Checking for the whole number case and returning if found.
            if (remainder == 0.0) { return wholeValue.ToString() + (char)34; }

            // Iterating through the exponents of base 2 values until the
            // maximum denominator value has been reached or until the modulus
            // of the divisor.
            int numbertest = (int)Math.Log(maxDenominator, 2);
            for (int i = 1; i < (int)Math.Log(maxDenominator, 2) + 1; i++)
            {
                // Calculating the denominator of the current iteration
                double denominator = Math.Pow(2, i);

                // Calculating the divisor increment value
                double divisor = Math.Pow(denominator, -1);

                // Checking if the current denominator evenly divides the remainder
                if ((remainder % divisor) == 0.0) // If, yes
                {
                    // Calculating the numerator of the remainder 
                    // given the calculated denominator
                    int numerator = Convert.ToInt32(remainder * denominator);

                    // Returning the resulting string from the conversion.
                    string h = (wholeValue > 0 ? wholeValue.ToString() + "-" : "") + numerator.ToString() + "/" + ((int)denominator).ToString() + (char)34;
                    return (wholeValue > 0 ? wholeValue.ToString() + "-" : "") + numerator.ToString() + "/" + ((int)denominator).ToString() + (char)34;
                }
            }

            // Returns Error if something goes wrong.
            return "Error";
        }

        /// <summary>
        /// Rounds the value down to the nearest imperial fractional increment
        /// and converts the value into an Feet-Inch-Fraction (FIF) string. 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="maxDenominator">Enter the maximum denominator to round toward (i.e. 1/16 --> 16)</param>
        /// <returns>Returns the value rounded down to the nearest increment value based on the maxDenominator.</returns>
        public static string FloorToFeetInchFraction(this double Value, int maxDenominator)
        {
            // Returning the rounded value converted into an Feet-Inch-Fraction (FIF) string.
            return Value.FloorToNearest(Math.Pow(maxDenominator, -1)).ToFeetInchFraction(maxDenominator);
        }

        /// <summary>
        /// Rounds the value up to the nearest imperial fractional increment
        /// and converts the value into an Feet-Inch-Fraction (FIF) string. 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="maxDenominator">Enter the maximum denominator to round toward (i.e. 1/16 --> 16)</param>
        /// <returns>Returns the value rounded up to the nearest increment value based on the maxDenominator.</returns>
        public static string CeilingToFeetInchFraction(this double Value, int maxDenominator)
        {
            // Returning the rounded value converted into a fraction string.
            return Value.CeilingToNearest(Math.Pow(maxDenominator, -1)).ToFeetInchFraction(maxDenominator);
        }

        /// <summary>
        /// Rounds the value to the nearest increment value based on the maximum denominator specified.
        /// Assumes mid-point rounding, value >= 0.5 rounds up, value < 0.5 rounds down.
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="maxDenominator">Enter the maximum denominator to round toward (i.e. 1/16 --> 16)</param>
        /// <returns>Returns the value rounded to the nearest increment value based on the maxDenominator.</returns>
        public static string ToFeetInchFraction(this double Value, int maxDenominator)
        {
            // Calculating the nearest increment of the value
            // argument based on the denominator argument.
            double incValue = Value.RoundToNearest(Math.Pow(maxDenominator, -1));

            // Calculating the remainder of the argument value and the whole value.
            double FeetInch = Math.Truncate(incValue) / 12.0;

            // Calculating the remainder of the argument value and the whole value.
            int Feet = (int)Math.Truncate(FeetInch);

            // Calculating remaining inches.
            incValue -= (double)(Feet * 12.0);

            // Returns the feet plus the remaining amount converted to inch fraction.
            return (Feet > 0 ? Feet.ToString() + (char)39 + " " : "") + incValue.ToInchFraction(maxDenominator);
        }
        public static string DoubleRoundFraction(this double d,int xc)
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
                double v1 = num7 / xc;
                text = " " + Math.Round(num6/v1,0).ToString() + "/" + Math.Round(num7/v1,0).ToString();
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
    }
}
