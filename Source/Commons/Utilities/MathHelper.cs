using System;
using System.Windows;

namespace SRL.Commons.Utilities
{
    public static class MathHelper
    {
        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }

        public static double Rfpart(double val)
        {
            return 1 - Fpart(val);
        }

        public static double Fpart(double val)
        {
            if (val < 0)
                return (float)(1 - (val - Math.Floor(val)));
            return (float)(val - Math.Floor(val));
        }

        public static double Clamp(this double value, double min, double max)
        {
            return value < min ? min : value > max ? max : value;
        }
    }
}
