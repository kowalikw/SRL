using System.Windows;

namespace SRL.Main.Utilities
{
    /// <summary>
    /// <see cref="Normalization"/> class contains methods to normalize and 
    /// denormalize point coordinates to/from coordination system [-1,1].
    /// </summary>
    public static class Normalization
    {
        /// <summary>
        /// Normalize point coordinates to [-1,1] system.
        /// </summary>
        /// <param name="point"><see cref="Point"/> to normalize.</param>
        /// <param name="renderSize">Size of viewport - <see cref="Size"/> type object.</param>
        /// <returns>Normalized point.</returns>
        public static Point Normalize(this Point point, Size renderSize)
        {
            double widthCenter = renderSize.Width / 2;
            double heightCenter = renderSize.Height / 2;

            return new Point(
                (point.X - widthCenter) / widthCenter,
                (heightCenter - point.Y) / heightCenter);
        }

        /// <summary>
        /// Denormalize point coordinates form [-1,1] system.
        /// </summary>
        /// <param name="point"><see cref="Point"/> to normalize.</param>
        /// <param name="renderSize">Size of viewport - <see cref="Size"/> type object.</param>
        /// <returns>Denormalized point.</returns>
        public static Point Denormalize(this Point point, Size renderSize)
        {
            return new Point(
                (renderSize.Width + point.X * renderSize.Width) / 2,
                (renderSize.Height - point.Y * renderSize.Height) / 2);
        }
    }
}
