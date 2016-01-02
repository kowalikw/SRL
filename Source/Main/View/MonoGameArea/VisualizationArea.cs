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


        private readonly SimulationViewModel _context = SimpleIoc.Default.GetInstance<SimulationViewModel>();
        private PropertyChangedEventHandler _propertyChangedHandler;

        private Polygon _resizedVehicleShape;

        private const double MinSizeFactor = 0.05;
        private const double InitSizeFactor = 0.2;
        private const double MaxSizeFactor = 0.4;


        protected override void Initialize()
        {
            base.Initialize();

            _propertyChangedHandler = (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(_context.Map):
                    case nameof(_context.StartPoint):
                    case nameof(_context.EndPoint):
                        RedrawStaticObjectsTexture();
                        break;

                    case nameof(_context.Vehicle):
                    case nameof(_context.InitialVehicleRotation):
                    case nameof(_context.VehicleSize):
                        if (_context.InitialVehicleRotation != null &&
                            _context.VehicleSize != null &&
                            _context.Vehicle != null)
                            _resizedVehicleShape = GeometryHelper.Resize(_context.Vehicle.Shape,
                                _context.VehicleSize.Value);
                        break;
                }
            };

            _context.PropertyChanged += _propertyChangedHandler;
        }

        protected override void Unitialize()
        {
            base.Unitialize();

            _context.PropertyChanged -= _propertyChangedHandler;
        }

        protected override void RenderDynamicObjects(SpriteBatch spriteBatch, TimeSpan time)
        {
            if (_context.CurrentFrame != null)
            {
                RenderFrame(spriteBatch, time);
                return;
            }

            // We assume that simulation is not ready.

            Point normalizedMousePos = MousePosition.Normalize(RenderSize);

            if (_context.EditorMode == SimulationViewModel.Mode.StartPointSetup)
            {
                Point position;
                RgbColor color = _context.SetStartPointCommand.CanExecute(normalizedMousePos)
                    ? ValidColor : InvalidColor;

                if (IsMouseOver)
                    position = normalizedMousePos;
                else
                {
                    position = new Point(0, 0);
                    color = ActiveColor;
                }

                if (AntialiasingEnabled)
                    spriteBatch.DrawVertexAA(position, RenderSize, color);
                else
                    spriteBatch.DrawVertex(position, RenderSize, color);
            }
            else if (_context.EditorMode == SimulationViewModel.Mode.EndPointSetup)
            {
                Point position;
                RgbColor color = _context.SetEndPointCommand.CanExecute(normalizedMousePos)
                    ? ValidColor : InvalidColor;

                if (IsMouseOver)
                    position = normalizedMousePos;
                else
                {
                    position = new Point(0, 0);
                    color = ActiveColor;
                }

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
                    double sizeFactor = GeometryHelper.GetDistance(origin, normalizedMousePos); //TODO

                    if (_context.SetVehicleSizeCommand.CanExecute(sizeFactor) &&
                        _context.SetInitialVehicleRotationCommand.CanExecute(angle))
                        color = ValidColor;
                    else
                        color = InvalidColor;
                    
                    if (AntialiasingEnabled)
                        spriteBatch.DrawArrowAA(origin, normalizedMousePos, RenderSize, ActiveColor);
                    else
                        spriteBatch.DrawArrow(origin, normalizedMousePos, RenderSize, ActiveColor);

                    shape = _context.Vehicle.Shape;
                    shape = GeometryHelper.Resize(shape, sizeFactor);

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
        }


        private void RenderFrame(SpriteBatch spriteBatch, TimeSpan time)
        {
            //TODO
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
            if (_context.StartPoint != null)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawVertexAA(_context.StartPoint.Value, RenderSize, InvalidColor); // TODO Some other color
                else
                    lockBitmap.DrawVertex(_context.StartPoint.Value, RenderSize, InvalidColor); // TODO Some other color
            }
            if (_context.EndPoint != null)
            {
                if (AntialiasingEnabled)
                    lockBitmap.DrawVertexAA(_context.EndPoint.Value, RenderSize, InvalidColor); // TODO Some other color
                else
                    lockBitmap.DrawVertex(_context.EndPoint.Value, RenderSize, InvalidColor); // TODO Some other color
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
                    double angle = GeometryHelper.GetAngle(origin, normalizedMousePos);
                    double sizeFactor = GeometryHelper.GetDistance(origin, normalizedMousePos);

                    if (_context.SetInitialVehicleRotationCommand.CanExecute(angle) && 
                        _context.SetVehicleSizeCommand.CanExecute(sizeFactor))
                    {
                        _context.SetInitialVehicleRotationCommand.Execute(angle);
                        _context.SetVehicleSizeCommand.Execute(sizeFactor);
                    }
                }
            }
        }
    }
}
