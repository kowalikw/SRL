using System;
using System.Drawing.Printing;
using System.Linq;
using System.Windows;

namespace SRL.Commons.Utilities
{
    public static class MathHelper
    {
        public const double DoubleComparisonEpsilon = 1e-15;

        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static float Rfrac(float val)
        {
            return 1 - Frac(val);
        }

        public static float Frac(float val)
        {
            return val - (float) Math.Truncate(val);
        }

        public static T Clamp<T>(this T value, T min, T max)
            where T: IComparable
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        public static T Max<T>(params T[] values)
            where T: IComparable
        {
            return values.Max();
        }

        public static bool EpsilonEquals(this double a, double b)
        {
            return Math.Abs(a - b) <= DoubleComparisonEpsilon;
        }
    }
}
