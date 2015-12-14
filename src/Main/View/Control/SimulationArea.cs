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
        private Point lastMousePosition;

        protected override void Initialize()
        {
            base.Initialize();
            _context = (VisualizationModuleViewModel)DataContext;
        }

        protected override void Render(SpriteBatch spriteBatch, TimeSpan time)
        {
            Console.WriteLine(isMousePressed.ToString());
            spriteBatch.DrawFrame(_context.CurrentFrame, _context.Vehicle, _context.Map, _context.Startpoint, _context.Endpoint);

            if(isMousePressed && isMouseInVehicle)
            {
                Point positionDifference = new Point(0, 0);
                if (MousePosition != lastMousePosition)
                {
                    //_context.CurrentFrame = null;
                    _context.Endpoint = null;
                    positionDifference = MousePosition - lastMousePosition;
                }

                for (int i = 0; i < _context.Vehicle.Shape.VertexCount; i++)
                    _context.Vehicle.Shape.Vertices[i] = _context.Vehicle.Shape.Vertices[i] + positionDifference;
                _context.Vehicle.OrientationOrigin += positionDifference;
                _context.Startpoint += positionDifference;

                lastMousePosition = MousePosition;
            }

            if (_context.orders != null)
            {
                for (int i = 1; i < _context.orders.Count; i++)
                    spriteBatch.DrawLine(_context.orders[i - 1].Destination, _context.orders[i].Destination, Color.Red);
                spriteBatch.DrawLine(_context.Startpoint, _context.orders[0].Destination, Color.Red);
                spriteBatch.DrawLine(_context.Endpoint, _context.orders[_context.orders.Count - 1].Destination, Color.Red);
            }
        }

        protected override void OnMouseUp(Point position)
        {
            isMousePressed = false;

            if (_context.Endpoint == null && _context.Startpoint != null && !GeometryHelper.IsPointOnRectangleOfPolygon(position, _context.Vehicle.Shape))
            {
                _context.Endpoint = position;
                ((RelayCommand)_context.CalculatePathCommand).OnCanExecuteChanged();
            }
            }
        protected override void OnMouseDown(Point position)
        {
            lastMousePosition = position;
            isMousePressed = true;
            isMouseInVehicle = _context.Vehicle == null ? false : GeometryHelper.IsPointInPolygon(MousePosition, _context.Vehicle.Shape);
        }

    }
}
