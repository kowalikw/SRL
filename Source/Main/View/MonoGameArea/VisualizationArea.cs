using System;
using System.ComponentModel;
using System.Windows.Input;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Drawing;
using SRL.Main.Utilities;
using SRL.Main.ViewModel;
using Point = System.Windows.Point;
using Mode = SRL.Main.ViewModel.SimulationViewModel.Mode;

namespace SRL.Main.View.MonoGameArea
{
    internal class VisualizationArea : AreaBase
    {
        protected static readonly Color StartPointColor = new Color(200, 0, 200);
        protected static readonly Color EndPointColor = StartPointColor;
        protected static readonly Color PathColor = new Color(174, 221, 247);
        protected static readonly Color VehicleColor = ActiveColor;

        private readonly SimulationViewModel _context = ServiceLocator.Current.GetInstance<SimulationViewModel>();

        protected bool ShowPath => Settings.Default.ShowPath;


        private PropertyChangedEventHandler _propertyChangedHandler;


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

            if (_context.EditorMode == Mode.StartPointSetup)
            {
                if (IsMouseOver)
                {
                    Color color = _context.SetStartPointCommand.CanExecute(normalizedMousePos)
                        ? ValidColor
                        : InvalidColor;

                    spriteBatch.DrawVertex(normalizedMousePos, RenderSize, color, AntialiasingEnabled);
                }
            }
            else if (_context.EditorMode == Mode.EndPointSetup)
            {
                if (IsMouseOver)
                {
                    Color color = _context.SetEndPointCommand.CanExecute(normalizedMousePos)
                        ? ValidColor
                        : InvalidColor;

                    spriteBatch.DrawVertex(normalizedMousePos, RenderSize, color, AntialiasingEnabled);
                }
            }
            else if (_context.EditorMode == Mode.VehicleSetup)
            {
                if (IsMouseOver)
                {
                    var origin = _context.StartPoint.Value;
                    var angle = GeometryHelper.GetAngle(origin, normalizedMousePos);
                    var size = GeometryHelper.GetDistance(origin, normalizedMousePos);

                    var setup = new VehicleSetup
                    {
                        RelativeSize = size,
                        Rotation = angle
                    };

                    Color color;
                    if (_context.SetInitialVehicleSetupCommandCommand.CanExecute(setup))
                        color = ValidColor;
                    else
                        color = InvalidColor;

                    Polygon shape = _context.Vehicle.Shape.Transform(size, angle, origin);
                    spriteBatch.DrawArrow(origin, normalizedMousePos, RenderSize, ActiveColor, AntialiasingEnabled);
                    spriteBatch.DrawPolygon(shape, RenderSize, color, AntialiasingEnabled);
                }
            }

            if (_context.Vehicle != null &&
                _context.StartPoint != null &&
                _context.InitialVehicleRotation != null &&
                _context.VehicleSize != null)
            {
                var origin = _context.StartPoint.Value;
                var angle = _context.InitialVehicleRotation.Value;
                var size = _context.VehicleSize.Value;

                Polygon shape = _context.Vehicle.Shape.Transform(size, angle, origin);
                spriteBatch.DrawPolygon(shape, RenderSize, VehicleColor, AntialiasingEnabled);
            }
        }

        private void RenderFrame(SpriteBatch spriteBatch, TimeSpan time)
        {
            var size = _context.VehicleSize.Value;
            var angle = _context.CurrentFrame.Rotation;
            var position = _context.CurrentFrame.Position;

            Polygon shape = _context.Vehicle.Shape.Transform(size, angle, position);
            spriteBatch.DrawPolygon(shape, RenderSize, VehicleColor, AntialiasingEnabled);
        }

        protected override void RedrawStaticObjects(LockBitmap lockBitmap)
        {
            if (_context.Map != null)
                lockBitmap.DrawMap(_context.Map, RenderSize, RegularColor, AntialiasingEnabled);
            if (ShowPath && _context.Path != null)
                lockBitmap.DrawPath(_context.Path, RenderSize, PathColor, AntialiasingEnabled);
            if (_context.StartPoint != null)
                lockBitmap.DrawVertex(_context.StartPoint.Value, RenderSize, StartPointColor, AntialiasingEnabled);
            if (_context.EndPoint != null)
                lockBitmap.DrawVertex(_context.EndPoint.Value, RenderSize, EndPointColor, AntialiasingEnabled);
        }

        protected override void OnMouseUp(MouseButton button)
        {
            Point normalizedMousePos = MousePosition.Normalize(RenderSize);

            if (button == MouseButton.Left)
            {
                if (_context.EditorMode == Mode.StartPointSetup)
                    _context.SetStartPointCommand.Execute(normalizedMousePos);
                else if (_context.EditorMode == Mode.EndPointSetup)
                    _context.SetEndPointCommand.Execute(normalizedMousePos);
                else if (_context.EditorMode == Mode.VehicleSetup)
                {
                    Point origin = _context.StartPoint.Value;
                    VehicleSetup setup = new VehicleSetup()
                    {
                        RelativeSize = GeometryHelper.GetDistance(origin, normalizedMousePos),
                        Rotation = GeometryHelper.GetAngle(origin, normalizedMousePos)
                    };

                    _context.SetInitialVehicleSetupCommandCommand.Execute(setup);
                }
            }
            else if (button == MouseButton.Right)
            {
                if (_context.EditorMode == Mode.StartPointSetup)
                    _context.EnterModeCommand.Execute(Mode.Normal);
                else if (_context.EditorMode == Mode.VehicleSetup)
                    _context.EnterModeCommand.Execute(Mode.StartPointSetup);
                else if (_context.EditorMode == Mode.EndPointSetup)
                    _context.EnterModeCommand.Execute(Mode.Normal);
                else if (_context.EditorMode == Mode.Normal)
                {
                    if (_context.EndPoint != null)
                    {
                        var denormalizedEndPoint = _context.EndPoint.Value.Denormalize(RenderSize);
                        if (IsMousePulledByPoint(denormalizedEndPoint))
                        {
                            _context.EnterModeCommand.Execute(Mode.EndPointSetup);
                            return;
                        }
                    }
                    if (_context.StartPoint != null)
                    {
                        var size = _context.VehicleSize.Value;
                        var angle = _context.InitialVehicleRotation.Value;
                        var position = _context.StartPoint.Value;

                        Polygon shape = _context.Vehicle.Shape.Transform(size, angle, position);
                        if (GeometryHelper.IsEnclosed(normalizedMousePos, shape))
                        {
                            _context.EnterModeCommand.Execute(Mode.StartPointSetup);
                            return;
                        }
                    }
                }
                else
                    throw new ArgumentOutOfRangeException();
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
        }
    }
}
