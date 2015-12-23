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
    }
}
