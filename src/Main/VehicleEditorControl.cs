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
        private Color normalDrawColor = Color.Black;
        private Color activeDrawColor = Color.Blue;
        private Color correctActiveDrawColor = Color.Green;
        private Color incorrectActiveDrawColor = Color.Red;
        private Color activeStartCircleColor = Color.Orange;

        public Vehicle Vehicle { get; private set; }
        public VehicleEditorMode Mode { get; set; }
        public Point OriginStart { get; set; }
        public Point OriginEnd { get; set; }
        public Point CursorPosition { get; set; }
        public bool IsAngleSet { get; set; }
        public bool IsSegmentIntersection { get; set; }
        public bool IsSegmentIntersectionUnchecked { get; private set; }
        
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Mode = VehicleEditorMode.Empty;
            Vehicle = new Vehicle();
            CursorPosition = new Point(0, 0);
            IsSegmentIntersection = false;
            IsSegmentIntersectionUnchecked = false;
        }

        protected override void Unitialize()
        {
            spriteBatch.Dispose();
        }

        protected override void Render(TimeSpan time)
        {
            GraphicsDevice.Clear(Color.LightSkyBlue);
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            spriteBatch.BeginDraw();

            switch(Mode)
            {
                case VehicleEditorMode.Empty:
                    Vehicle.Shape = new Polygon();
                    IsSegmentIntersection = false;
                    IsSegmentIntersectionUnchecked = false;
                    break;
                case VehicleEditorMode.DrawPolygon:
                    for (int i = 0; i < Vehicle.Shape.VertexCount; i++)
                    { 
                        if (i == 0)
                            spriteBatch.DrawCircle(Vehicle.Shape.Vertices[0], int.Parse(Number.PolygonStartCircleRadius), int.Parse(Number.CircleSegments), activeDrawColor, int.Parse(Number.PolygonStartCircleThickness));
                        else
                            spriteBatch.DrawLine(new Point(Vehicle.Shape.Vertices[i - 1].X, Vehicle.Shape.Vertices[i - 1].Y), new Point(Vehicle.Shape.Vertices[i].X, Vehicle.Shape.Vertices[i].Y), activeDrawColor, int.Parse(Number.PolygonLineThickness));

                        spriteBatch.DrawCircle(Vehicle.Shape.Vertices[0], int.Parse(Number.PolygonPointRadius), int.Parse(Number.CircleSegments), activeDrawColor, int.Parse(Number.PolygonPointThickness));
                    }

                    if (Vehicle.Shape.VertexCount > 0)
                    {
                        if (GeometryHelper.DistanceBetweenPoints(Vehicle.Shape.Vertices[0], CursorPosition) <= int.Parse(Number.PolygonStartCircleRadius) && Vehicle.Shape.VertexCount >= int.Parse(Number.MinimumPolygonVertices) && !IsSegmentIntersection)
                        {
                            spriteBatch.DrawLine(new Point(Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1].X, Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1].Y), new Point(Vehicle.Shape.Vertices[0].X, Vehicle.Shape.Vertices[0].Y), Color.Blue, 2);
                            spriteBatch.DrawCircle(new Point(Vehicle.Shape.Vertices[0].X, Vehicle.Shape.Vertices[0].Y), 8, int.Parse(Number.CircleSegments), Color.Yellow, 3);
                        }
                        else
                        {
                            IsSegmentIntersection = false;

                            for (int i = 0; i < Vehicle.Shape.VertexCount - 2; i++)
                            {
                                if (GeometryHelper.SegmentIntersection(Vehicle.Shape.Vertices[i], Vehicle.Shape.Vertices[i + 1], Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1], CursorPosition))
                                    IsSegmentIntersection = true;
                            }

                            if(IsSegmentIntersection)
                                spriteBatch.DrawLine(new Point(Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1].X, Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1].Y), new Point(CursorPosition.X, CursorPosition.Y), Color.Red, 2);
                            else
                                spriteBatch.DrawLine(new Point(Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1].X, Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1].Y), new Point(CursorPosition.X, CursorPosition.Y), Color.Green, 2);
                        }
                    }

                    if(Vehicle.Shape.VertexCount > 3)
                    {
                        if(GeometryHelper.DistanceBetweenPoints(Vehicle.Shape.Vertices[0], Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1]) <= 8)
                        {
                            Vehicle.Shape.Vertices.RemoveAt(Vehicle.Shape.VertexCount - 1);
                            Mode = VehicleEditorMode.DrawDone;
                        }
                    }

                    for (int i = 1; i < Vehicle.Shape.VertexCount - 2; i++)
                        if (GeometryHelper.SegmentIntersection(Vehicle.Shape.Vertices[i], Vehicle.Shape.Vertices[i + 1], Vehicle.Shape.Vertices[Vehicle.Shape.VertexCount - 1], Vehicle.Shape.Vertices[0]))
                            IsSegmentIntersectionUnchecked = true;

                    break;
                case VehicleEditorMode.DrawDone:
                    DrawVehicle(Vehicle);
                    IsSegmentIntersection = false;
                    IsSegmentIntersectionUnchecked = false;
                    break;
                case VehicleEditorMode.SetAxis:
                    DrawVehicle(Vehicle);

                    if (OriginStart != null)
                    {
                        double axisAngle = (Math.Atan((CursorPosition.Y - OriginStart.Y) / (CursorPosition.X - OriginStart.X))) * 180 / Math.PI;

                        DrawAxis(OriginStart, CursorPosition, axisAngle, true);
                    }
                    
                    if(OriginEnd != null)
                    {
                        if(!IsAngleSet)
                        {
                            Vehicle.FrontAngle = (Math.Atan((OriginEnd.Y - OriginStart.Y) / (OriginEnd.X - OriginStart.X))) * 180 / Math.PI;
                            Mode = VehicleEditorMode.Idle;
                            IsAngleSet = true;
                        }
                    }

                    break;
                case VehicleEditorMode.Idle:
                    DrawVehicle(Vehicle);
                    DrawAxis(OriginStart, OriginEnd, Vehicle.FrontAngle, false);
                    break;
            }

            spriteBatch.End();
        }

        private void DrawVehicle(Vehicle vehicle)
        {
            for (int i = 0; i < Vehicle.Shape.VertexCount; i++)
                spriteBatch.DrawLine(vehicle.Shape.Vertices[i], vehicle.Shape.Vertices[(i + 1) % Vehicle.Shape.VertexCount], normalDrawColor, int.Parse(Number.PolygonLineThickness));
        }

        private void DrawAxis(Point axisStart, Point axisEnd, double axisAngle, bool activeDraw)
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
            spriteBatch.DrawLine(arrowTop, arrowCenter, activeDraw ? activeDrawColor : normalDrawColor, int.Parse(Number.AxisThickness));
            spriteBatch.DrawLine(arrowBottom, arrowCenter, activeDraw ? activeDrawColor : normalDrawColor, int.Parse(Number.AxisThickness));
            spriteBatch.DrawLine(axisStart, axisEnd, activeDraw ? activeDrawColor : normalDrawColor, int.Parse(Number.AxisThickness));
        }

    }
}
