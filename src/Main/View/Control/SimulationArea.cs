using System;
using Microsoft.Xna.Framework.Graphics;
using SRL.Main.ViewModel;
using SRL.Model.Model;
using SRL.Main.Utilities;
using Microsoft.Xna.Framework;
using Point = SRL.Model.Model.Point;
using System.Windows;
using System.Windows.Input;
using SRL.Model;

namespace SRL.Main.View.Control
{
    public class SimulationArea : EditArea
    {
        private VisualizationModuleViewModel _context;

        private bool isMousePressed = false;
        private bool isMouseInVehicle = true;
        private bool isVehicleSelected = false;
        private bool isMouseOnCornerOfVehicle = false;
        private Point lastMousePosition;
        private double lastDistance = 0;

        private const int lineLength = 6;

        protected override void Initialize()
        {
            base.Initialize();
            _context = (VisualizationModuleViewModel)DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            if(isMousePressed && isMouseOnCornerOfVehicle)
            {
                double distance = GeometryHelper.GetDistance(MousePosition, _context.Vehicle.OrientationOrigin);

                if (distance != lastDistance)
                {
                    double ratio = distance / lastDistance;

                    for (int i = 0; i < _context.Vehicle.Shape.VertexCount; i++)
                    {
                        _context.Vehicle.Shape.Vertices[i] = _context.Vehicle.Shape.Vertices[i] - _context.Vehicle.OrientationOrigin;
                        _context.Vehicle.Shape.Vertices[i] = new Point(_context.Vehicle.Shape.Vertices[i].X * ratio, _context.Vehicle.Shape.Vertices[i].Y * ratio);
                        _context.Vehicle.Shape.Vertices[i] = _context.Vehicle.Shape.Vertices[i] + _context.Vehicle.OrientationOrigin;
                    }

                    _context.Endpoint = null;
                    _context.orders = null;
                    _context._frames = null;
                }

                lastDistance = distance;

                ((RelayCommand)_context.CalculatePathCommand).OnCanExecuteChanged();
            }
            else if(isMousePressed && isMouseInVehicle && !isMouseOnCornerOfVehicle)
            {
                Point positionDifference = new Point(0, 0);
                if (MousePosition != lastMousePosition)
                {
                    positionDifference = MousePosition - lastMousePosition;

                    for (int i = 0; i < _context.Vehicle.Shape.VertexCount; i++)
                        _context.Vehicle.Shape.Vertices[i] = _context.Vehicle.Shape.Vertices[i] + positionDifference;
                    _context.Vehicle.OrientationOrigin += positionDifference;
                    _context.Startpoint += positionDifference;

                    _context.Endpoint = null;
                    _context.orders = null;
                    _context._frames = null;
                }

                lastMousePosition = MousePosition;

                ((RelayCommand)_context.CalculatePathCommand).OnCanExecuteChanged();
            }

            // Rysowanie ścieżki
            if (_context.orders != null && _context.Startpoint != null && _context.Endpoint != null)
            {
                for (int i = 1; i < _context.orders.Count; i++)
                    spriteBatch.DrawLine(_context.orders[i - 1].Destination, _context.orders[i].Destination, Color.Red);
                spriteBatch.DrawLine(_context.Startpoint, _context.orders[0].Destination, Color.Red);
                spriteBatch.DrawLine(_context.Endpoint, _context.orders[_context.orders.Count - 1].Destination, Color.Red);
            }

            // TODO: MinkowskiSum
            if (_context.MinkowskiSum != null)
                foreach (var polygon in _context.MinkowskiSum)
                    spriteBatch.DrawPolygon(polygon, Color.Black);

            spriteBatch.DrawFrame(_context.CurrentFrame, _context.Vehicle, _context.Map, _context.Startpoint, _context.Endpoint, isVehicleSelected, _context.Vehicle != null ? GeometryHelper.IsPointOnCornerOfRectangleOfPolygon(MousePosition, _context.Vehicle.Shape) : Corner.None);
        }

        protected override void OnMouseUp(Point position)
        {
            isMousePressed = false;

            isVehicleSelected = _context.Vehicle == null ? false : GeometryHelper.IsPointOnRectangleOfPolygon(MousePosition, _context.Vehicle.Shape) || isMouseOnCornerOfVehicle;

            if (_context.Endpoint == null && _context.Startpoint != null && !isVehicleSelected)
            {
                _context.Endpoint = position;
                ((RelayCommand)_context.CalculatePathCommand).OnCanExecuteChanged();
            }
        }
        protected override void OnMouseDown(Point position)
        {
            lastMousePosition = position;
            lastDistance = _context.Vehicle == null ? 0 : GeometryHelper.GetDistance(MousePosition, _context.Vehicle.OrientationOrigin);

            isMousePressed = true;
            isVehicleSelected = _context.Vehicle == null ? false : isVehicleSelected;
            isMouseInVehicle = _context.Vehicle == null ? false : GeometryHelper.IsPointOnRectangleOfPolygon(MousePosition, _context.Vehicle.Shape);
            isMouseOnCornerOfVehicle = _context.Vehicle == null ? false : GeometryHelper.IsPointOnCornerOfRectangleOfPolygon(MousePosition, _context.Vehicle.Shape) != Corner.None;
        }

    }
}
