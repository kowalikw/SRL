using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Color = Microsoft.Xna.Framework.Color;

namespace SRL.Main.Drawing
{
    public class LockBitmap
    {
        /// <summary>
        /// Number of bytes per pixel.
        /// </summary>
        public const int Depth = 32;

        /// <summary>
        /// Pixel data.
        /// </summary>
        public byte[] Pixels { get; set; }

        /// <summary>
        /// Pixelwise width.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Pixelwise height.
        /// </summary>
        public int Height { get; private set; }


        private readonly Bitmap _source;
        private IntPtr _iptr = IntPtr.Zero;
        private BitmapData _bitmapData;

        /// <summary>
        /// Initializes a new instance of the <see cref="Bitmap"/> class.
        /// </summary>
        /// <param name="source">32bpp ARGB bitmap.</param>
        public LockBitmap(Bitmap source)
        {
            if (source.PixelFormat != PixelFormat.Format32bppArgb)
                throw new ArgumentException("Source bitmap must have 32 bpp depth.");

            _source = source;
        }

        /// <summary>
        /// Lock bitmap data.
        /// </summary>
        public void LockBits()
        {
            // Get width and height of bitmap.
            Width = _source.Width;
            Height = _source.Height;

            // Get total locked pixels count.
            int pixelCount = Width * Height;

            // Create rectangle to lock.
            Rectangle rect = new Rectangle(0, 0, Width, Height);

            // Lock bitmap and return bitmap data.
            _bitmapData = _source.LockBits(rect, ImageLockMode.ReadWrite,
                _source.PixelFormat);

            // Create byte array to copy pixel values.
            const int step = Depth / 8;
            Pixels = new byte[pixelCount * step];
            _iptr = _bitmapData.Scan0;

            // Copy data from pointer to array.
            Marshal.Copy(_iptr, Pixels, 0, Pixels.Length);
        }

        /// <summary>
        /// Unlock bitmap data.
        /// </summary>
        public void UnlockBits()
        {
            // Copy data from byte array to pointer.
            Marshal.Copy(Pixels, 0, _iptr, Pixels.Length);

            // Unlock bitmap data.
            _source.UnlockBits(_bitmapData);
        }

        private static byte BlendChannel(byte src, byte dest, byte srcAlpha)
        {
            return (byte)((src * byte.MaxValue + dest * (byte.MaxValue - srcAlpha)) / byte.MaxValue);
        }

        /// <summary>
        /// Gets <see cref="Color"/> of a pixel specified by its x/y coordinates.
        /// </summary>
        /// <param name="x">Column position.</param>
        /// <param name="y">Row position.</param>
        /// <returns>Color of the pixel.</returns>
        public Color GetPixel(int x, int y)
        {
            // Get color components count.
            int cCount = Depth / 8;

            // Get start index of the specified pixel.
            int i = (y * Width + x) * cCount;

            if (i > Pixels.Length - cCount)
                throw new IndexOutOfRangeException();

            byte b = Pixels[i];
            byte g = Pixels[i + 1];
            byte r = Pixels[i + 2];
            byte a = Pixels[i + 3];

            return new Color(b, g, r, a);
        }

        /// <summary>
        /// Sets pixel in a position specified by x/y coordinates.
        /// </summary>
        /// <param name="x">Column position.</param>
        /// <param name="y">Row position.</param>
        /// <param name="color">Color of the pixel.</param>
        public void SetPixel(int x, int y, Color color)
        {
            if (x < 0 || x >= Width)
                return;

            if (y < 0 || y >= Height)
                return;

            // Get color components count.
            const int cCount = Depth / 8;

            // Get start index of the specified pixel.
            int i = (y * Width + x) * cCount;

            Pixels[i] = BlendChannel(color.R, Pixels[i], color.A);
            Pixels[i + 1] = BlendChannel(color.G, Pixels[i + 1], color.A);
            Pixels[i + 2] = BlendChannel(color.B, Pixels[i + 2], color.A);
            Pixels[i + 3] = BlendChannel(color.A, Pixels[i + 3], color.A);
        }
    }
}
