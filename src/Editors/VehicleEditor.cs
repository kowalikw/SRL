using System;
using SRL.MonoGameControl;
using SRL.Models.Model;
using SRL.Models.Enum;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Point = SRL.Models.Model.Point;
using SRL.Models;

namespace SRL.Editors
{
    public class VehicleEditor : Editor
    {
        private const int AxisThickness = 2;
        private const int ArrowTopX = -12;
        private const int ArrowTopY = -6;
        private const int ArrowCenterX = 0;
        private const int ArrowCenterY = 0;
        private const int ArrowBottomX = -12;
        private const int ArrowBottomY = 6;

        private SpriteBatch spriteBatch;

        public Vehicle Vehicle { get; private set; }
        public VehicleEditorMode Mode { get; set; }
        public Point OriginStart { get; set; }
        public Point OriginEnd { get; set; }
        public Point CursorPosition { get; set; }
        public bool IsAngleSet { get; set; }
        public DrawPolygonState ActualPolygonState { get; private set; }

        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Mode = VehicleEditorMode.Empty;
            Vehicle = new Vehicle();
            CursorPosition = new Point(0, 0);
            ActualPolygonState = DrawPolygonState.Empty;
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

            DrawVehicle();

            switch(Mode)
            {
                case VehicleEditorMode.Empty:
                    Vehicle.Shape = new Polygon();
                    ActualPolygonState = DrawPolygonState.Empty;
                    break;
                case VehicleEditorMode.DrawPolygon:
                    //if (DrawPolygon(spriteBatch, Vehicle.Shape, CursorPosition, true) == DrawPolygonState.Incorrect)
                    //    IsSegmentIntersection = true;
                    ActualPolygonState = CheckPolygon(Vehicle.Shape, CursorPosition, true);

                    break;
                case VehicleEditorMode.DrawDone:
                    ActualPolygonState = CheckPolygon(Vehicle.Shape, CursorPosition, false);
                    if (!Vehicle.Shape.IsCorrect())
                        Vehicle.Shape.Vertices.Clear();
                    break;
                case VehicleEditorMode.SetAxis:
                    ActualPolygonState = CheckPolygon(Vehicle.Shape);
                    //DrawVehicle();

                    if (OriginStart != null)
                        DrawAxis(OriginStart, CursorPosition, CalculateAxisAngle(OriginStart, CursorPosition), true);
                    
                    if(OriginEnd != null)
                    {
                        if(!IsAngleSet)
                        {
                            Vehicle.DirectionAngle = CalculateAxisAngle(OriginStart, OriginEnd);
                            Mode = VehicleEditorMode.Idle;
                            IsAngleSet = true;
                        }
                    }

                    break;
                case VehicleEditorMode.Idle:
                    ActualPolygonState = CheckPolygon(Vehicle.Shape);
                    //DrawVehicle();
                    DrawAxis(OriginStart, OriginEnd, Vehicle.DirectionAngle, false);
                    break;
            }

            spriteBatch.End();
        }

        public void Reset()
        {
            Vehicle.Shape.Vertices.Clear();
            Vehicle.Origin = null;
            OriginStart = null;
            OriginEnd = null;
            IsAngleSet = false;
            Mode = VehicleEditorMode.Empty;
        }

        private void DrawVehicle()
        {
            //spriteBatch.DrawPolygon(Vehicle.Shape)
            spriteBatch.DrawPolygon(Vehicle.Shape, ActualPolygonState, CursorPosition);
        }

        private void DrawAxis(Point axisStart, Point axisEnd, double axisAngle, bool activeDraw)
        {
            Point arrowCenter = new Point(ArrowCenterX, ArrowCenterY);
            Point arrowTop = new Point(ArrowTopX, ArrowTopY);
            Point arrowBottom = new Point(ArrowBottomX, ArrowBottomY);

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
            spriteBatch.DrawLine(arrowTop, arrowCenter, activeDraw ? activeDrawColor : normalDrawColor, AxisThickness);
            spriteBatch.DrawLine(arrowBottom, arrowCenter, activeDraw ? activeDrawColor : normalDrawColor, AxisThickness);
            spriteBatch.DrawLine(axisStart, axisEnd, activeDraw ? activeDrawColor : normalDrawColor, AxisThickness);
        }

        private double CalculateAxisAngle(Point start, Point end)
        {
            return (Math.Atan((end.Y - start.Y) / (end.X - start.X))) * 180 / Math.PI;
        }
    }
}
