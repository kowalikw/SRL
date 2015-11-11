﻿using System.Collections;
using System.Collections.Generic;
using CsPotrace;
using SRL.Models.Model;
using Bitmap = System.Drawing.Bitmap;

namespace SRL.Models
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
            bool[,] matrix = Potrace.BitMapToBinary(_bitmap, colorThreshold);
            Potrace.potrace_trace(matrix, polygons);

            for (int i = 0; i < polygons.Count; i++)
            {
                var polygon = (ArrayList)polygons[i];
                // polygon[0] is the contour and polygon[>0] are its holes - we can ignore them.
                var lines = (Potrace.Curve[])polygon[0];

                var polygonPoints = new List<Point>();
                for (int j = 0; j < lines.Length; j++)
                    polygonPoints[j] = new Point(lines[j].A.x, lines[j].A.y);

                output.Add(new Polygon(polygonPoints));
            }

            return output;
        }
    }
}
