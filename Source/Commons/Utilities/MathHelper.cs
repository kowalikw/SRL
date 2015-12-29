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

        public static float Rfpart(float val)
        {
            return 1 - Fpart(val);
        }

        public static float Fpart(float val)
        {
            if (val < 0)
                return (float)(1 - (val - Math.Floor(val)));
            return (float)(val - Math.Floor(val));
        }
    }
}
