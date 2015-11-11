using System;
using System.Collections.Generic;
using SRL.Main.Resources;
using SRL.MonoGameControl;
using SRL.Models.Model;
using SRL.Models.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = SRL.Models.Model.Point;

namespace SRL.Main
{
    class VehicleEditorControl : D3D11Host
    {
        private SpriteBatch spriteBatch;
        private Polygon shape;
        private List<Point> vertices;
        private Point origin;
        public double? angle = null;

        public Vehicle Vehicle { get; private set; }
        public VehicleEditorMode Mode { get; set; }
        public List<Point> Vertices
        {
            get
            {
                return vertices;
            }

            private set
            {
                vertices = value;
            }
        }
        public Point OriginStart
        {
            get
            {
                return origin;
            }

            set
            {
                origin = value;
            }
        }
        public Point OriginEnd { get; set; }
        public double Angle
        {
            get
            {
                return angle.Value;
            }
        }
        public Point CursorPosition { get; set; }
        
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Mode = VehicleEditorMode.Empty;
            Vertices = new List<Point>();
            CursorPosition = new Point(0, 0);
        }


        protected override void Unitialize()
        {
            spriteBatch.Dispose();
        }


        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            spriteBatch.Begin();

            switch(Mode)
            {
                case VehicleEditorMode.DrawPolygon:
                    for (int i = 0; i < Vertices.Count; i++)
                    { 
                        if (i == 0)
                            spriteBatch.DrawCircle(Vertices[0], 8, 100, Color.Blue, 3);
                        else
                        {
                            spriteBatch.DrawCircle(Vertices[0], 3, 100, Color.Blue, 3);
                            spriteBatch.DrawLine(new Point(Vertices[i - 1].X, Vertices[i - 1].Y), new Point(Vertices[i].X, Vertices[i].Y), Color.Blue, 2);
                        }
                    }

                    if (Vertices.Count > 0)
                    {
                        if (GeometryHelper.DistanceBetweenPoints(Vertices[0], CursorPosition) <= 8 && Vertices.Count >= 3)
                        {
                            spriteBatch.DrawLine(new Point(Vertices[Vertices.Count - 1].X, Vertices[Vertices.Count - 1].Y), new Point(Vertices[0].X, Vertices[0].Y), Color.Blue, 2);
                            spriteBatch.DrawCircle(new Point(Vertices[0].X, Vertices[0].Y), 8, 100, Color.Yellow, 3);
                        }
                        else
                        {
                            bool isSegmentIntersection = false;

                            for (int i = 0; i < Vertices.Count - 2; i++)
                            {
                                if (GeometryHelper.SegmentIntersection(Vertices[i], Vertices[i + 1], Vertices[Vertices.Count - 1], CursorPosition))
                                    isSegmentIntersection = true;
                            }

                            if(isSegmentIntersection)
                                spriteBatch.DrawLine(new Point(Vertices[Vertices.Count - 1].X, Vertices[Vertices.Count - 1].Y), new Point(CursorPosition.X, CursorPosition.Y), Color.Red, 2);
                            else
                                spriteBatch.DrawLine(new Point(Vertices[Vertices.Count - 1].X, Vertices[Vertices.Count - 1].Y), new Point(CursorPosition.X, CursorPosition.Y), Color.Green, 2);
                        }
                    }

                    if(Vertices.Count >= 3)
                    {
                        if(GeometryHelper.DistanceBetweenPoints(Vertices[0], Vertices[Vertices.Count - 1]) <= 8)
                        {
                            Vertices.RemoveAt(Vertices.Count - 1);
                            Mode = VehicleEditorMode.DrawDone;
                        }
                    }

                    break;
                case VehicleEditorMode.DrawDone:
                    DrawVehicle(Vertices);
                    break;
                case VehicleEditorMode.SetAxis:
                    DrawVehicle(Vertices);

                    if (OriginStart != null)
                    {
                        double axisAngle = (Math.Atan((CursorPosition.Y - OriginStart.Y) / (CursorPosition.X - OriginStart.X))) * 180 / Math.PI;

                        DrawAxis(OriginStart, CursorPosition, axisAngle);
                    }
                    
                    if(OriginEnd != null)
                    {
                        if(!angle.HasValue)
                        {
                            angle = (Math.Atan((OriginEnd.Y - OriginStart.Y) / (OriginEnd.X - OriginStart.X))) * 180 / Math.PI;
                            Mode = VehicleEditorMode.Idle;
                        }
                    }

                    break;
                case VehicleEditorMode.Idle:
                    DrawVehicle(Vertices);
                    DrawAxis(OriginStart, OriginEnd, Angle);

                    if(Vehicle == null)
                        Vehicle = new Vehicle(new Polygon(vertices), origin, angle.Value);

                    break;
            }

            spriteBatch.End();
        }

        private void DrawVehicle(List<Point> vertices)
        {
            for (int i = 0; i < vertices.Count; i++)
                spriteBatch.DrawLine(vertices[i], vertices[(i + 1) % vertices.Count], Color.Blue, 2);
        }

        private void DrawAxis(Point axisStart, Point axisEnd, double axisAngle)
        {
            Point arrowCenter = new Point(double.Parse(Number.ArrowCenterX), double.Parse(Number.ArrowCenterY));
            Point arrowTop = new Point(double.Parse(Number.ArrowTopX), double.Parse(Number.ArrowTopY));
            Point arrowBottom = new Point(double.Parse(Number.ArrowBottomX), double.Parse(Number.ArrowBottomY));

            // Mirror of arrow
            if (axisStart.X > axisEnd.X)
            {
                arrowCenter = new Point(arrowCenter.X, arrowCenter.Y);
                arrowTop = new Point(-arrowTop.X, arrowTop.Y);
                arrowBottom = new Point(-arrowBottom.X, arrowBottom.Y);
            }

            // Rotate and translate of arrow
            axisAngle = axisAngle * Math.PI / 180;
            arrowCenter = new Point(axisEnd.X, axisEnd.Y);
            arrowTop = new Point(((arrowTop.X * Math.Cos(axisAngle) - arrowTop.Y * Math.Sin(axisAngle)) + axisEnd.X),
                ((arrowTop.X * Math.Sin(axisAngle) + arrowTop.Y * Math.Cos(axisAngle)) + axisEnd.Y));
            arrowBottom = new Point(((arrowBottom.X * Math.Cos(axisAngle) - arrowBottom.Y * Math.Sin(axisAngle)) + axisEnd.X),
                ((arrowBottom.X * Math.Sin(axisAngle) + arrowBottom.Y * Math.Cos(axisAngle)) + axisEnd.Y));

            // Draw
            spriteBatch.DrawLine(arrowTop, arrowCenter, Color.Purple, int.Parse(Number.AxisThickness));
            spriteBatch.DrawLine(arrowBottom, arrowCenter, Color.Purple, int.Parse(Number.AxisThickness));
            spriteBatch.DrawLine(axisStart, axisEnd, Color.Purple, int.Parse(Number.AxisThickness));
        }

    }
}
