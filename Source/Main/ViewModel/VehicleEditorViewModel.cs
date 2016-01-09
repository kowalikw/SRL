using System.Windows;
using FirstFloor.ModernUI.Presentation;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using SRL.Main.ViewModel.Base;

namespace SRL.Main.ViewModel
{
    public class VehicleEditorViewModel : EditorViewModel<Vehicle>
    {
        public override RelayCommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(() =>
                    {
                        ShapeDone = false;
                        VehicleShape.Clear();
                        Pivot = null;
                        Direction = null;
                    }, () =>
                    {
                        return VehicleShape.Count > 0
                            || ShapeDone
                            || Pivot.HasValue
                            || Direction.HasValue;
                    });
                }
                return _resetCommand;
            }
        }
        public RelayCommand BackCommand
        {
            get
            {
                if (_backCommand == null)
                {
                    _backCommand = new RelayCommand(() =>
                    {
                        if (Direction.HasValue)
                            Direction = null;
                        else if (Pivot.HasValue)
                            Pivot = null;
                        else if (ShapeDone)
                            ShapeDone = false;
                        else
                            VehicleShape.RemoveLast();
                    }, () =>
                    {
                        return VehicleShape.Count > 0
                        || ShapeDone
                        || Pivot.HasValue
                        || Direction.HasValue;
                    });
                }
                return _backCommand;
            }
        }
        public RelayCommand<Point> AddShapeVertexCommand
        {
            get
            {
                if (_addShapeVertexCommand == null)
                {
                    _addShapeVertexCommand = new RelayCommand<Point>(vertex =>
                    {
                        VehicleShape.Add(vertex);
                        ShapeDone = false;
                    }, vertex =>
                    {
                        if (VehicleShape.Count > 0 && VehicleShape.GetLast() == vertex)
                            return false;

                        if (VehicleShape.Count <= 2)
                            return true;

                        for (int i = 0; i < VehicleShape.Count - 2; i++)
                        {
                            if (GeometryHelper.DoSegmentsIntersect(
                                VehicleShape[i], VehicleShape[i + 1],
                                VehicleShape.GetLast(), vertex))
                            {
                                return false;
                            }
                        }
                        return true;
                    });
                }
                return _addShapeVertexCommand;
            }
        }
        public RelayCommand FinishShapeCommand
        {
            get
            {
                if (_finishShapeCommand == null)
                {
                    _finishShapeCommand = new RelayCommand(
                        () => ShapeDone = true,
                        () => VehicleShape.Count >= 3);
                }
                return _finishShapeCommand;
            }
        }
        public RelayCommand<Point> SetPivotCommand
        {
            get
            {
                if (_setPivotCommand == null)
                {
                    _setPivotCommand = new RelayCommand<Point>(
                        point => Pivot = point,
                        point => GeometryHelper.IsInsidePolygon(point, new Polygon(VehicleShape)));
                }
                return _setPivotCommand;
            }
        }
        public RelayCommand<double> SetDirectionCommand
        {
            get
            {
                if (_setDirectionCommand == null)
                {
                    _setDirectionCommand = new RelayCommand<double>(
                        angle => Direction = angle,
                        angle => Pivot.HasValue);
                }
                return _setDirectionCommand;
            }
        }

        private RelayCommand _resetCommand;
        private RelayCommand _backCommand;
        private RelayCommand<Point> _addShapeVertexCommand;
        private RelayCommand _finishShapeCommand;
        private RelayCommand<Point> _setPivotCommand;
        private RelayCommand<double> _setDirectionCommand;


        public ObservableCollectionEx<Point> VehicleShape { get; }

        public bool ShapeDone
        {
            get { return _shapeDone; }
            set
            {
                if (value != _shapeDone)
                {
                    _shapeDone = value;
                    RaisePropertyChanged();
                }
            }
        }
        public Point? Pivot
        {
            get { return _pivot; }
            private set
            {
                if (value != _pivot)
                {
                    _pivot = value;
                    RaisePropertyChanged();
                }
            }
        }
        public double? Direction
        {
            get { return _direction; }
            private set
            {
                if (value != _direction)
                {
                    _direction = value;
                    RaisePropertyChanged();
                }
            }
        }

        private Point? _pivot;
        private bool _shapeDone;
        private double? _direction;

        protected override bool IsEditedModelValid
        {
            get
            {
                return ShapeDone
                    && Pivot.HasValue
                    && Direction.HasValue;
            }
        }


        public VehicleEditorViewModel()
        {
            VehicleShape = new ObservableCollectionEx<Point>();
        }

        public override Vehicle GetEditedModel()
        {
            if (!IsEditedModelValid)
                return null;

            Vehicle vehicle = new Vehicle();

            Polygon shape = new Polygon(VehicleShape);
            Polygon rotatedShape = GeometryHelper.Rotate(Pivot.Value, shape, -Direction.Value);

            double minX = double.MaxValue, minY = double.MaxValue;
            double maxX = double.MinValue, maxY = double.MinValue;

            for (int i = 0; i < rotatedShape.Vertices.Count; i++)
            {
                Point vertex = new Point(
                    rotatedShape.Vertices[i].X - Pivot.Value.X,
                    rotatedShape.Vertices[i].Y - Pivot.Value.Y);

                minX = vertex.X < minX ? vertex.X : minX;
                minY = vertex.Y < minY ? vertex.Y : minY;
                maxX = vertex.X > maxX ? vertex.X : maxX;
                maxY = vertex.Y > maxY ? vertex.Y : maxY;

                rotatedShape.Vertices[i] = vertex;
            }

            double shrinkFactor = MathHelper.Max(
                Math.Abs(minX),
                Math.Abs(maxX),
                Math.Abs(minY),
                Math.Abs(maxY));

            if (shrinkFactor > 1)
            {
                for (int i = 0; i < rotatedShape.Vertices.Count; i++)
                {
                    Point vertex = new Point(
                        rotatedShape.Vertices[i].X * 0.9 / shrinkFactor,
                        rotatedShape.Vertices[i].Y * 0.9 / shrinkFactor);

                    rotatedShape.Vertices[i] = vertex;
                }
            }

            vehicle.Shape = rotatedShape;
            return vehicle;
        }
        public override void SetEditedModel(Vehicle model)
        {
            VehicleShape.ReplaceRange(model.Shape.Vertices);
            if (VehicleShape.Count >= 3) // TODO instead of checking count only, make sure at least 3 vertices are non collinear
            {
                ShapeDone = true;
                if (GeometryHelper.IsInsidePolygon(new Point(0, 0), model.Shape))
                {
                    Pivot = new Point(0, 0);
                    Direction = 0;
                }
                else
                {
                    Pivot = null;
                    Direction = null;
                }
            }
            else
            {
                ShapeDone = false;
                Pivot = null;
                Direction = null;
            }
        }
    }
}
