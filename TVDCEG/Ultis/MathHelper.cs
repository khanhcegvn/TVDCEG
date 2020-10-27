using System;
namespace TVDCEG.LBR
{
    public static class MathHelper
    {
        public const double EPSILON = 1E-06;

        public const double PI = Math.PI;

        public static bool IsEqualTol(this double A, double B, double tolerance)
        {
            return Math.Abs(B - A) < tolerance;
        }

        public static bool IsEqual(this double A, double B)
        {
            return Math.Abs(B - A) < 1E-06;
        }

        public static bool IsZero(this double A)
        {
            return IsEqual(0.0, A);
        }

        public static bool IsZeroTol(this double A, double tolerance)
        {
            return IsEqualTol(0.0, A, tolerance);
        }

        public static bool IsSmallerTol(this double A, double B, double tolerance)
        {
            return A + tolerance < B;
        }

        public static bool IsSmallerEqualTol(this double A, double B, double tolerance)
        {
            if (A + tolerance >= B)
            {
                return Math.Abs(B - A) < tolerance;
            }
            return true;
        }

        public static bool IsSmaller(this double A, double B)
        {
            return A + 1E-06 < B;
        }

        public static bool IsSmallerEqual(this double A, double B)
        {
            if (A + 1E-06 >= B)
            {
                return Math.Abs(B - A) < 1E-06;
            }
            return true;
        }

        public static bool IsGreaterTol(this double A, double B, double tolerance)
        {
            return A > B + tolerance;
        }

        public static bool IsGreaterEqualTol(this double A, double B, double tolerance)
        {
            if (Math.Abs(B - A) >= tolerance)
            {
                return A > B + tolerance;
            }
            return true;
        }

        public static bool IsGreater(this double A, double B)
        {
            return A > B + 1E-06;
        }

        public static bool IsGreaterEqual(this double A, double B)
        {
            if (A <= B + 1E-06)
            {
                return Math.Abs(B - A) < 1E-06;
            }
            return true;
        }

        public static double Min(this double A, double B)
        {
            if (!IsSmaller(A, B))
            {
                return B;
            }
            return A;
        }

        public static double MinTol(this double A, double B, double tolerance)
        {
            if (!IsSmallerTol(A, B, tolerance))
            {
                return B;
            }
            return A;
        }

        public static double Max(this double A, double B)
        {
            if (!IsGreater(A, B))
            {
                return B;
            }
            return A;
        }

        public static double MaxTol(double A, double B, double tolerance)
        {
            if (!IsGreaterTol(A, B, tolerance))
            {
                return B;
            }
            return A;
        }
    }
}
