using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using CsPotrace;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
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

        public List<Polygon> Trace(int areaThreshold, int colorThreshold)
        {
            var output = new List<Polygon>();

            Potrace.alphamax = 0; // Corner threshold.
            Potrace.turdsize = areaThreshold;
            Potrace.curveoptimizing = false; // Bezier curve optimization.

            ArrayList polygons = new ArrayList();
            bool[,] matrix = Potrace.BitMapToBinary(_bitmap, 255 - colorThreshold);
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
            
            RemoveEnclosedPolygons(ref output);
            NormalizeOutput(output);

            return output;
        }

        private void NormalizeOutput(List<Polygon> polygons)
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

        private void RemoveEnclosedPolygons(ref List<Polygon> polygons)
        {
            bool[] enclosed = new bool[polygons.Count];

            for (int i = 0; i < polygons.Count; i++)
            {
                for (int j = i + 1; j < polygons.Count; j++)
                {
                    if (enclosed[j])
                        continue;

                    enclosed[j] = GeometryHelper.IsEnclosed(polygons[j], polygons[i]);
                }
            }
            polygons = new List<Polygon>(polygons.SkipWhile((polygon, i) => enclosed[i]));
        }
    }
}
