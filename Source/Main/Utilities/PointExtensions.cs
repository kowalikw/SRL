using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SRL.Main.Utilities
{
    public static class PointExtensions
    {
        public static Point Normalize(this Point point, Size renderSize)
        {
            double widthCenter = renderSize.Width / 2;
            double heightCenter = renderSize.Height / 2;

            return new Point(
                (point.X - widthCenter) / widthCenter,
                (heightCenter - point.Y) / heightCenter);
        }

        public static Point Denormalize(this Point point, Size renderSize)
        {
            return new Point(
                (renderSize.Width + point.X * renderSize.Width) / 2,
                (renderSize.Height - point.Y * renderSize.Height) / 2);
        }
    }
}
