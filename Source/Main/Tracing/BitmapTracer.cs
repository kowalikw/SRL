using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using CsPotrace;
using SRL.Commons.Model;
using SRL.Main.Utilities;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace SRL.Main.Tracing
{
    public class BitmapTracer
    {
        private readonly Bitmap _bitmap;

        public BitmapTracer(string path) :
            this(new Bitmap(path))
        { }

        public BitmapTracer(Bitmap bitmap)
        {
            _bitmap = bitmap;
        }

        public List<Polygon> Trace(double areaThreshold, double colorThreshold)
        {
            int pixelThreshold = (int)(_bitmap.Height * _bitmap.Width * areaThreshold);
            int absoluteColorThreshold = (int)(254 * colorThreshold) + 1;

            var output = new List<Polygon>(TraceBitmap(_bitmap, pixelThreshold, absoluteColorThreshold));
            NormalizeOutput(output);
            
            return output;
        }

        private IEnumerable<Polygon> TraceBitmap(Bitmap bitmap, int pixelThreshold, int maxRgb)
        {
            var output = new List<Polygon>();

            Potrace.alphamax = 0; // Corner threshold.
            Potrace.turdsize = pixelThreshold;
            Potrace.curveoptimizing = false; // Bezier curve optimization.

            ArrayList polygons = new ArrayList();
            bool[,] matrix = Potrace.BitMapToBinary(bitmap, maxRgb);
            Potrace.potrace_trace(matrix, polygons);

            for (int i = 0; i < polygons.Count; i++)
            {
                var polygon = (ArrayList)polygons[i];
                // polygon[0] is the contour and polygon[>0] are its holes - we can ignore them.
                var lines = (Potrace.Curve[])polygon[0];

                var polygonPoints = new Point[lines.Length];
                for (int j = 0; j < lines.Length; j++)
                    polygonPoints[j] = new Point(lines[j].A.x, lines[j].A.y);

                output.Add(new Polygon(polygonPoints));
            }
            return output;
        }

        private void NormalizeOutput(IEnumerable<Polygon> polygons)
        {
            bool heightShrink = _bitmap.Width > _bitmap.Height;
            double shrinkFactor = heightShrink
                ? (double)_bitmap.Height / _bitmap.Width
                : (double)_bitmap.Width / _bitmap.Height;

            Size bitmapSize = new Size(_bitmap.Width, _bitmap.Height);
            foreach (var polygon in polygons)
            {
                for (int i = 0; i < polygon.Vertices.Count; i++)
                {
                    Point normalized = polygon.Vertices[i].Normalize(bitmapSize);
                    Point shrinked;

                    if (heightShrink)
                        shrinked = new Point(normalized.X, normalized.Y * shrinkFactor);
                    else
                        shrinked = new Point(normalized.X * shrinkFactor, normalized.Y);

                    polygon.Vertices[i] = shrinked;
                }
            }
        }

        private Bitmap GetInvertedBitmap(Bitmap bitmap)
        {
            Bitmap inverted = new Bitmap(bitmap);
            var data = inverted.LockBits(new Rectangle(0, 0, inverted.Width, inverted.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppRgb);
            var length = data.Stride * data.Height;
            var newData = new byte[length];
            Marshal.Copy(data.Scan0, newData, 0, length);
            inverted.UnlockBits(data);

            for (int i = 0; i < length; i += 4)
            {
                newData[i] = (byte)(255 - newData[i]);
                newData[i + 1] = (byte)(255 - newData[i + 1]);
                newData[i + 2] = (byte)(255 - newData[i + 2]);
            }

            var bitmapWrite = inverted.LockBits(new Rectangle(0, 0, inverted.Width, inverted.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppPArgb);
            Marshal.Copy(newData, 0, bitmapWrite.Scan0, length);
            inverted.UnlockBits(bitmapWrite);
            return inverted;
        }


    }
}
