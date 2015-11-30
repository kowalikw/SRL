using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using CsPotrace;
using SRL.Model.Model;
using Point = SRL.Model.Model.Point;

namespace SRL.Model.Tracing
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

            return output;
        }
    }
}
