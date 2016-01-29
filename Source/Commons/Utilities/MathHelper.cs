using System;
using System.Linq;

namespace SRL.Commons.Utilities
{
    /// <summary>
    /// <see cref="MathHelper"/> class contains helper math methods.
    /// </summary>
    public static class MathHelper
    {
        /// <summary>
        /// Value of real number comparison epsilon.
        /// </summary>
        public const double DoubleComparisonEpsilon = 1e-14;

        /// <summary>
        /// Swaps two parameters value.
        /// </summary>
        /// <typeparam name="T">Type of parameters to swap.</typeparam>
        /// <param name="a">Parameter 1.</param>
        /// <param name="b">Parameter 2.</param>
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        /// <summary>
        /// Gets a value of 1-fraction of the number.
        /// </summary>
        /// <param name="val">Float number.</param>
        /// <returns>1-fraction of the number.</returns>
        public static float Rfrac(float val)
        {
            return 1 - Frac(val);
        }

        /// <summary>
        /// Gets a fraction of the number.
        /// </summary>
        /// <param name="val">Float number.</param>
        /// <returns>Fraction of the number.</returns>
        public static float Frac(float val)
        {
            return val - (float) Math.Truncate(val);
        }

        /// <summary>
        /// Determines whether value is between min and max. If it is, returns
        /// value. If it is greater than max, returns max. If it is lower than
        /// min, returns min.
        /// </summary>
        /// <typeparam name="T">Type of parameters must implement <see cref="IComparable"/> interface.</typeparam>
        /// <param name="value">Value parameter.</param>
        /// <param name="min">Min value of parameter.</param>
        /// <param name="max">Max value of parameter.</param>
        /// <returns>Returns value, min or max depending on value.</returns>
        public static T Clamp<T>(this T value, T min, T max)
            where T: IComparable
        {
            if (value.CompareTo(min) < 0)
                return min;
            if (value.CompareTo(max) > 0)
                return max;
            return value;
        }

        /// <summary>
        /// Returns max value of all parameters.
        /// </summary>
        /// <typeparam name="T">Type of parameters must implement <see cref="IComparable"/> interface.</typeparam>
        /// <param name="values">Set of parameters.</param>
        /// <returns>Max value of parameters.</returns>
        public static T Max<T>(params T[] values)
            where T: IComparable
        {
            return values.Max();
        }

        /// <summary>
        /// Determines whether two double numbers are equal with an accuracy of epsilon.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool EpsilonEquals(this double a, double b)
        {
            return Math.Abs(a - b) <= DoubleComparisonEpsilon;
        }
    }
}
