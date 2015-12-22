using System.Windows;

namespace SRL.Commons.Utilities
{
    public static class MathHelper
    {
        //public static Point Normalize(Point point, Size renderSize)
        //{
        //    double widthCenter = renderSize.Width / 2;
        //    double heightCenter = renderSize.Height / 2;

        //    return new Point(
        //        (point.X - widthCenter) / widthCenter,
        //        (heightCenter - point.Y) / heightCenter);
        //}

        //public static Point Denormalize(Point point, Size renderSize)
        //{
        //    return new Point(
        //        (renderSize.Width + point.X * renderSize.Width) / 2,
        //        (renderSize.Height - point.Y * renderSize.Height) / 2);
        //}

        public static void Swap<T>(ref T lhs, ref T rhs)
        {
            T temp = lhs;
            lhs = rhs;
            rhs = temp;
        }
    }
}
