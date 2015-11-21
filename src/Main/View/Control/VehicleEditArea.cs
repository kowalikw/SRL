using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model;
using SRL.Model.Enum;
using SRL.Model.Model;
using SRL.MonoGameControl;
using Point = SRL.Model.Model.Point;
using SrlPoint = SRL.Model.Model.Point;
using WinPoint = System.Windows.Point;
using XnaPoint = Microsoft.Xna.Framework.Point;

namespace SRL.Main.View.Control
{
    public class VehicleEditArea : EditArea
    {
        private VehicleEditorViewModel _context;


        private SrlPoint LastPolygonVertex => _context.VehicleShape[_context.VehicleShape.Count - 1];
        private SrlPoint FirstPolygonVertex => _context.VehicleShape[0];





        private const int AxisThickness = 2;
        private const int ArrowTopX = -12;
        private const int ArrowTopY = -6;
        private const int ArrowCenterX = 0;
        private const int ArrowCenterY = 0;
        private const int ArrowBottomX = -12;
        private const int ArrowBottomY = 6;


        public Vehicle Vehicle { get; private set; }
        public VehicleEditorMode Mode { get; set; }
        public Point OriginStart { get; set; }
        public Point OriginEnd { get; set; }
        public bool IsAngleSet { get; set; }
        public DrawPolygonState ActualPolygonState { get; private set; }

        protected override void Initialize()
        {
            base.Initialize();
            _context = (VehicleEditorViewModel) DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {

            //Draw shape of the vehicle.
            Color shapeColor = _context.IsShapeDone ? RegularColor : ActiveColor;
            spriteBatch.DrawPath(_context.VehicleShape, _context.IsShapeDone, shapeColor, LineThickness);

            
            if (!_context.IsShapeDone)
            {
                //Draw new potential shape polygon side.
                Color segmentColor = _context.AddVertexCommand.CanExecute(MousePosition) ? ValidColor : InvalidColor;

                //TODO
            }
            else if (!_context.IsOrientationOriginSet)
            {
                //Draw new potential orientation origin.
                Color originColor = _context.SetOrientationOriginCommand.CanExecute(MousePosition) ? ValidColor : InvalidColor;

                //TODO
            }
            else if (!_context.IsOrientationAngleSet)
            {
                //Draw new potential orientation axis.
                Color axisColor = _context.SetOrientationAngleCommand.CanExecute(MousePosition) ? ValidColor : InvalidColor;

                //TODO
            }
        }


        protected override void OnMouseUp(Point position)
        {
            if (!_context.IsShapeDone)
            {
                if (GeometryHelper.GetDistance(position, FirstPolygonVertex) < VertexPullRadius)
                {
                    if (_context.CloseVehicleShapeCommand.CanExecute(null))
                    {
                        _context.CloseVehicleShapeCommand.Execute(null);
                        return;
                    }
                }

                if (_context.AddVertexCommand.CanExecute(position))
                    _context.AddVertexCommand.Execute(position);
            }
            else if (!_context.IsOrientationOriginSet)
            {
                if (_context.SetOrientationOriginCommand.CanExecute(position))
                    _context.SetOrientationOriginCommand.Execute(position);
            }
            else if (!_context.IsOrientationAngleSet)
            {
                if (_context.SetOrientationAngleCommand.CanExecute(position))
                    _context.SetOrientationAngleCommand.Execute(position);
            }
        }

        private void DrawAxis(SpriteBatch spriteBatch, Point axisStart, Point axisEnd, double axisAngle, bool activeDraw)
        {
            //Point arrowCenter = new Point(ArrowCenterX, ArrowCenterY);
            //Point arrowTop = new Point(ArrowTopX, ArrowTopY);
            //Point arrowBottom = new Point(ArrowBottomX, ArrowBottomY);

            //// Mirror of arrow
            //if (axisStart.X > axisEnd.X)
            //{
            //    arrowCenter = new Point(arrowCenter.X, arrowCenter.Y);
            //    arrowTop = new Point(-arrowTop.X, arrowTop.Y);
            //    arrowBottom = new Point(-arrowBottom.X, arrowBottom.Y);
            //}

            //// Rotate and translate of arrow
            //axisAngle = axisAngle * Math.PI / 180;
            //arrowCenter = new Point(axisEnd.X, axisEnd.Y);
            //arrowTop = new Point(((arrowTop.X * Math.Cos(axisAngle) - arrowTop.Y * Math.Sin(axisAngle)) + axisEnd.X),
            //    ((arrowTop.X * Math.Sin(axisAngle) + arrowTop.Y * Math.Cos(axisAngle)) + axisEnd.Y));
            //arrowBottom = new Point(((arrowBottom.X * Math.Cos(axisAngle) - arrowBottom.Y * Math.Sin(axisAngle)) + axisEnd.X),
            //    ((arrowBottom.X * Math.Sin(axisAngle) + arrowBottom.Y * Math.Cos(axisAngle)) + axisEnd.Y));

            //// Draw
            //spriteBatch.DrawLine(arrowTop, arrowCenter, activeDraw ? activeDrawColor : normalDrawColor, AxisThickness);
            //spriteBatch.DrawLine(arrowBottom, arrowCenter, activeDraw ? activeDrawColor : normalDrawColor, AxisThickness);
            //spriteBatch.DrawLine(axisStart, axisEnd, activeDraw ? activeDrawColor : normalDrawColor, AxisThickness);
        }

        private double CalculateAxisAngle(Point start, Point end)
        {
            //return (Math.Atan((end.Y - start.Y) / (end.X - start.X))) * 180 / Math.PI;

            return 0;
        }
    }
}
