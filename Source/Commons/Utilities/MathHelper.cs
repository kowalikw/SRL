﻿using System;
using System.Linq;
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
                return 1f - val + (float)Math.Floor(val);
            return val - (float)Math.Floor(val);
        }

        public static double Clamp(this double value, double min, double max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        public static double Max(params double[] values)
        {
            return values.Max();
        }
    }
}
