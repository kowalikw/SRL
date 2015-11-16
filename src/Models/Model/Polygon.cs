using SRL.Models.Enum;
using System;
using System.Collections.Generic;

namespace SRL.Models.Model
{
    public class Polygon
    {
        public const int MinVerticesCount = 3;
        public const int StartPointRadius = 8;

        public List<Point> Vertices { get; }
        public int VertexCount => Vertices.Count;

        public Polygon()
        {
            Vertices = new List<Point>();
        }

        public Polygon(List<Point> vertices)
        {
            if (vertices.Count < MinVerticesCount)
                throw new ArgumentException("Polygon must have minimum 3 vertices.");

            Vertices = vertices;
        }

        public bool IsCorrect()
        {
            if (VertexCount < MinVerticesCount) return false;

            for (int i = 1; i < VertexCount - 2; i++)
                if (GeometryHelper.SegmentIntersection(Vertices[i], Vertices[i + 1], Vertices[VertexCount - 1], Vertices[0]))
                    return false;

            return true;
        }

        public bool IsCorrect(Point nextVertice)
        {
            if (VertexCount < MinVerticesCount) return false;

            for (int i = 1; i < VertexCount - 2; i++)
                if (GeometryHelper.SegmentIntersection(Vertices[i], Vertices[i + 1], nextVertice, Vertices[0]))
                    return false;

            return true;
        }

        public bool IsFinished()
        {
            if (GeometryHelper.DistanceBetweenPoints(Vertices[0], Vertices[VertexCount - 1]) <= StartPointRadius && VertexCount >= MinVerticesCount && IsCorrect())
                return true;

            return false;
        }

        public bool IsFinished(Point nextVertice)
        {
            if (Vertices.Count >= MinVerticesCount && GeometryHelper.DistanceBetweenPoints(Vertices[0], nextVertice) <= StartPointRadius)
                return true;

            return false;
        }

        public bool IsEmpty()
        {
            return VertexCount == 0;
        }


    }
}
