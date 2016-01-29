using System.Drawing;
using System.Runtime.InteropServices;

namespace SRL.Main.Utilities
{
    internal static class BitmapExtensions
    {
        public static byte[] GetBytes(this Bitmap bitmap)
        {
            var data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                System.Drawing.Imaging.ImageLockMode.ReadOnly, bitmap.PixelFormat);

            // calculate the byte size: for PixelFormat.Format32bppArgb (standard for GDI bitmaps) it's the hight * stride
            int bufferSize = data.Height * data.Stride; // stride already incorporates 4 bytes per pixel

            // create buffer
            byte[] bytes = new byte[bufferSize];

            // copy bitmap data into buffer
            Marshal.Copy(data.Scan0, bytes, 0, bytes.Length);

            // unlock the bitmap data
            bitmap.UnlockBits(data);

            return bytes;

        }
    }
}
