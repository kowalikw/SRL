using System;
using System.Linq;
using System.Windows;

namespace SRL.Commons.Utilities
{
    public static class MathHelper
    {
        public const double DoubleComparisonEpsilon = 1e-15;

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static float Rfpart(float val) //TODO rename
        {
            return 1 - Fpart(val);
        }

        public static float Fpart(float val) //TODO rename
        {
            if (val < 0)
                return 1f - val + (float)Math.Floor(val);
            return val - (float)Math.Floor(val);
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
