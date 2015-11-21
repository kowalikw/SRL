using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Model;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace SRL.Main.View.Control
{
    public abstract class EditArea : MonoGameControl.MonoGameControl
    {
        protected const double VertexPullRadius = 8;

        protected const int VertexThickness = 2;
        protected const int LineThickness = 2;

        protected static readonly Color RegularColor = Color.Black;
        protected static readonly Color ActiveColor = Color.Blue;
        protected static readonly Color InvalidColor = Color.Red;
        protected static readonly Color ValidColor = Color.Green;


        protected SrlPoint MousePosition { get; private set; }


        protected SpriteBatch SpriteBatch;


        protected override void Initialize()
        {
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            MouseUp += (o, e) =>
            {
                WinPoint position = e.GetPosition((UIElement) o);
                OnMouseUp((SrlPoint)position);
            };
            MouseMove += (o, e) =>
            {
                WinPoint position = e.GetPosition((UIElement)o);
                MousePosition = ((SrlPoint)position);
            };
        }

        protected override void Unitialize()
        {
            SpriteBatch.Dispose();
        }

        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            SpriteBatch.BeginDraw();
            Render(SpriteBatch, time);
            SpriteBatch.EndDraw();
        }

        protected abstract void Render(SpriteBatch spriteBatch, TimeSpan time);
        
        protected abstract void OnMouseUp(SrlPoint position);

        protected bool IsMousePulledByPoint(SrlPoint point)
        {
            return GeometryHelper.GetDistance(MousePosition, point) <= VertexPullRadius;
        }














        /*
        public DrawLineState CheckLine(Polygon polygon, Model.Model.Point nextVertice)
        {
            foreach (Model.Model.Point p in polygon.Vertices)
                if (p.X == nextVertice.X && p.Y == nextVertice.Y)
                    return DrawLineState.Done;

            for (int i = 0; i < polygon.VertexCount - 2; i++)
                if (GeometryHelper.DoSegmentsIntersect(polygon.Vertices[i], polygon.Vertices[i + 1], polygon.Vertices[polygon.VertexCount - 1], nextVertice))
                    return DrawLineState.Incorrect;

            return DrawLineState.Correct;
        }

        public DrawPolygonState CheckPolygon(Polygon polygon, Model.Model.Point cursorPosition = null, bool activeDraw = false)
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

    */

    }
}
