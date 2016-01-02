using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;

namespace SRL.Main.View.MonoGameArea
{
    internal class VisualizationArea : AreaBase
    {
        protected static readonly RgbColor StartPointColor = new RgbColor(20, 20, 255);
        protected static readonly RgbColor EndPointColor = StartPointColor;
        protected static readonly RgbColor PathColor = new RgbColor(20, 20, 255);

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
                RgbColor color = _context.SetStartPointCommand.CanExecute(normalizedMousePos)
                    ? ValidColor : InvalidColor;

                if (AntialiasingEnabled)
                    spriteBatch.DrawVertexAA(position, RenderSize, color);
                else
                    spriteBatch.DrawVertex(position, RenderSize, color);
            }
            else if (_context.Vehicle != null && _context.StartPoint != null)
            {
                Polygon shape;
                Point origin = _context.StartPoint.Value;
                RgbColor color;
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

                    if (AntialiasingEnabled)
                        spriteBatch.DrawArrowAA(origin, normalizedMousePos, RenderSize, ActiveColor);
                    else
                        spriteBatch.DrawArrow(origin, normalizedMousePos, RenderSize, ActiveColor);

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

                if (AntialiasingEnabled)
                    spriteBatch.DrawPolygonAA(shape, RenderSize, color);
                else
                    spriteBatch.DrawPolygon(shape, RenderSize, color);
            }

            if (_context.EditorMode == SimulationViewModel.Mode.EndPointSetup)
            {
                if (!IsMouseOver)
                    return;

                Point position = normalizedMousePos;
                RgbColor color = _context.SetEndPointCommand.CanExecute(normalizedMousePos)
                    ? ValidColor : InvalidColor;

                if (AntialiasingEnabled)
                    spriteBatch.DrawVertexAA(position, RenderSize, color);
                else
                    spriteBatch.DrawVertex(position, RenderSize, color);
            }
        }

        private void RenderFrame(SpriteBatch spriteBatch, TimeSpan time)
        {
            double rotation = _context.CurrentFrame.Rotation;
            Point position = _context.CurrentFrame.Position;

            Polygon shape = GeometryHelper.Rotate(_resizedVehicleShape, rotation);
            shape = GeometryHelper.Move(shape, position.X, position.Y);

            if (AntialiasingEnabled)
                spriteBatch.DrawPolygonAA(shape, RenderSize, ActiveColor);
            else
                spriteBatch.DrawPolygon(shape, RenderSize, ActiveColor);
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (_context.Map != null)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawMapAA(_context.Map, RenderSize, RegularColor);
                else
                    lockBitmap.DrawMap(_context.Map, RenderSize, RegularColor);
            }
            if (ShowPath && _context.Path != null)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawPathAA(_context.Path, RenderSize, PathColor);
                else
                    lockBitmap.DrawPath(_context.Path, RenderSize, PathColor);
            }
            if (_context.StartPoint != null)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawVertexAA(_context.StartPoint.Value, RenderSize, StartPointColor);
                else
                    lockBitmap.DrawVertex(_context.StartPoint.Value, RenderSize, StartPointColor);
            }
            if (_context.EndPoint != null)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawVertexAA(_context.EndPoint.Value, RenderSize, EndPointColor);
                else
                    lockBitmap.DrawVertex(_context.EndPoint.Value, RenderSize, EndPointColor);
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
