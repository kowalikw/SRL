using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Models;
using SRL.Models.Enum;
using SRL.Models.Model;
using SRL.MonoGameControl;
using Point = SRL.Models.Model.Point;

namespace SRL.Editors
{
    public abstract class Editor : MonoGameControl.MonoGameControl
    {
        protected const int StartCircleRadius = 8;
        protected const int StartCircleThickness = 3;
        protected const int PointRadius = 3;
        protected const int PointThickness = 3;
        protected const int LineThickness = 2;
        protected const int CircleSegments = 100;

        protected Color normalDrawColor = Color.Black;
        protected Color activeDrawColor = Color.Blue;
        protected Color correctActiveDrawColor = Color.Green;
        protected Color incorrectActiveDrawColor = Color.Red;
        protected Color activeStartCircleColor = Color.Orange;

        public DrawLineState CheckLine(Polygon polygon, Point nextVertice)
        {
            foreach(Point p in polygon.Vertices)
                if (p.X == nextVertice.X && p.Y == nextVertice.Y)
                    return DrawLineState.Done;

            for (int i = 0; i < polygon.VertexCount - 2; i++)
                if (GeometryHelper.SegmentIntersection(polygon.Vertices[i], polygon.Vertices[i + 1], polygon.Vertices[polygon.VertexCount - 1], nextVertice))
                    return DrawLineState.Incorrect;

            return DrawLineState.Correct;
        }

        public DrawPolygonState CheckPolygon(Polygon polygon, Point cursorPosition = null, bool activeDraw = false)
        {
            if (!activeDraw)
                return DrawPolygonState.Done;
            else
            {
                if (!polygon.IsEmpty())
                {
                    switch (CheckLine(polygon, cursorPosition))
                    {
                        case DrawLineState.Correct:
                            return DrawPolygonState.Correct;
                        case DrawLineState.Incorrect:
                            return DrawPolygonState.Incorrect;
                        case DrawLineState.Done:
                            return DrawPolygonState.Correct;
                    }
                }

                return DrawPolygonState.Empty;
            }
        }

        
    }
}
