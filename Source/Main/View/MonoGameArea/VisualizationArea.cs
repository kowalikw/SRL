using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;
using Point = System.Windows.Point;

namespace SRL.Main.View.MonoGameArea
{
    internal class VisualizationArea : AreaBase
    {
        protected static readonly Color StartPointColor = new Color(20, 20, 255);
        protected static readonly Color EndPointColor = StartPointColor;
        protected static readonly Color PathColor = new Color(20, 20, 255);

        private readonly SimulationViewModel _context = SimpleIoc.Default.GetInstance<SimulationViewModel>();

        protected bool ShowPath => Settings.Default.ShowPath;


        private PropertyChangedEventHandler _propertyChangedHandler;
        private Polygon _resizedVehicleShape;


        protected override void Initialize()
        {
            base.Initialize();

            _propertyChangedHandler = (o, e) => HandlePropertyChange(e.PropertyName);

            _context.PropertyChanged += _propertyChangedHandler;
            Settings.Default.PropertyChanged += _propertyChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.PropertyChanged -= _propertyChangedHandler;
            Settings.Default.PropertyChanged -= _propertyChangedHandler;
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            if (_context.CurrentFrame != null)
            {
                RenderFrame(spriteBatch, time);
                return;
            }

            Point normalizedMousePos = MousePosition.Normalize(RenderSize);

            if (_context.EditorMode == SimulationViewModel.Mode.StartPointSetup)
            {
                if (!IsMouseOver)
                    return;

                Point position = normalizedMousePos;
                Color color = _context.SetStartPointCommand.CanExecute(normalizedMousePos)
                    ? ValidColor : InvalidColor;

                spriteBatch.DrawVertex(position, RenderSize, color, AntialiasingEnabled);
            }
            else if (_context.Vehicle != null && _context.StartPoint != null)
            {
                Polygon shape;
                Point origin = _context.StartPoint.Value;
                Color color;
                double angle;

                if (_context.EditorMode == SimulationViewModel.Mode.VehicleSetup)
                {
                    if (!IsMouseOver)
                        return;

                    angle = GeometryHelper.GetAngle(origin, normalizedMousePos);
                    VehicleSetup setup = new VehicleSetup()
                    {
                        RelativeSize = GeometryHelper.GetDistance(origin, normalizedMousePos),
                        Rotation = angle
                    };

                    if (_context.SetInitialVehicleSetup.CanExecute(setup))
                        color = ValidColor;
                    else
                        color = InvalidColor;

                    spriteBatch.DrawArrow(origin, normalizedMousePos, RenderSize, ActiveColor, AntialiasingEnabled);

                    shape = _context.Vehicle.Shape;
                    shape = GeometryHelper.Resize(shape, setup.RelativeSize);

                }
                else if (_context.InitialVehicleRotation != null && _context.VehicleSize != null)
                {
                    angle = _context.InitialVehicleRotation.Value;
                    color = RegularColor;
                    shape = _resizedVehicleShape;
                }
                else
                    throw new MissingMemberException();

                shape = GeometryHelper.Rotate(shape, angle);
                shape = GeometryHelper.Move(shape, origin.X, origin.Y);

                spriteBatch.DrawPolygon(shape, RenderSize, color, AntialiasingEnabled);
            }

            if (_context.EditorMode == SimulationViewModel.Mode.EndPointSetup)
            {
                if (!IsMouseOver)
                    return;

                Point position = normalizedMousePos;
                Color color = _context.SetEndPointCommand.CanExecute(normalizedMousePos)
                    ? ValidColor : InvalidColor;

                spriteBatch.DrawVertex(position, RenderSize, color, AntialiasingEnabled);
            }
        }

        private void RenderFrame(SpriteBatch spriteBatch, TimeSpan time)
        {
            double rotation = _context.CurrentFrame.Rotation;
            Point position = _context.CurrentFrame.Position;

            Polygon shape = GeometryHelper.Rotate(_resizedVehicleShape, rotation);
            shape = GeometryHelper.Move(shape, position.X, position.Y);

            spriteBatch.DrawPolygon(shape, RenderSize, ActiveColor, AntialiasingEnabled);
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (_context.Map != null)
            {
                lockBitmap.DrawMap(_context.Map, RenderSize, RegularColor, AntialiasingEnabled);
            }
            if (ShowPath && _context.Path != null)
            {
                lockBitmap.DrawPath(_context.Path, RenderSize, PathColor, AntialiasingEnabled);
            }
            if (_context.StartPoint != null)
            {
                lockBitmap.DrawVertex(_context.StartPoint.Value, RenderSize, StartPointColor, AntialiasingEnabled);
            }
            if (_context.EndPoint != null)
            {
                lockBitmap.DrawVertex(_context.EndPoint.Value, RenderSize, EndPointColor, AntialiasingEnabled);
            }
        }

        protected override void OnMouseUp(MouseButton button)
        {
            Point normalizedMousePos = MousePosition.Normalize(RenderSize);

            if (button == MouseButton.Left)
            {
                if (_context.EditorMode == SimulationViewModel.Mode.StartPointSetup)
                {
                    if (_context.SetStartPointCommand.CanExecute(normalizedMousePos))
                        _context.SetStartPointCommand.Execute(normalizedMousePos);
                }
                else if (_context.EditorMode == SimulationViewModel.Mode.EndPointSetup)
                {
                    if (_context.SetEndPointCommand.CanExecute(normalizedMousePos))
                        _context.SetEndPointCommand.Execute(normalizedMousePos);
                }
                else if (_context.EditorMode == SimulationViewModel.Mode.VehicleSetup)
                {
                    Point origin = _context.StartPoint.Value;
                    VehicleSetup setup = new VehicleSetup()
                    {
                        RelativeSize = GeometryHelper.GetDistance(origin, normalizedMousePos),
                        Rotation = GeometryHelper.GetAngle(origin, normalizedMousePos)
                    };

                    if (_context.SetInitialVehicleSetup.CanExecute(setup))
                        _context.SetInitialVehicleSetup.Execute(setup);
                }
            }
        }

        private void HandlePropertyChange(string propertyName)
        {
            if (propertyName == nameof(_context.Map) ||
                propertyName == nameof(_context.StartPoint) ||
                propertyName == nameof(_context.EndPoint) ||
                propertyName == nameof(_context.Path) ||
                propertyName == nameof(Settings.Default.ShowPath))
            {
                RedrawStaticObjectsTexture();
            }
            else if (propertyName == nameof(_context.Vehicle) ||
                     propertyName == nameof(_context.VehicleSize) ||
                     _context.Vehicle != null)
            {
                if (_context.VehicleSize == null)
                    _resizedVehicleShape = _context.Vehicle.Shape;
                else
                    _resizedVehicleShape = GeometryHelper.Resize(_context.Vehicle.Shape, _context.VehicleSize.Value);
            }
        }
    }
}
