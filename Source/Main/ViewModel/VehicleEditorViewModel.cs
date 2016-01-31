using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;
using System;
using SRL.Main.ViewModel.Base;

namespace SRL.Main.ViewModel
{
    /// <summary>
    /// View-model class that contains non-UI logic for the vehicle editor.
    /// </summary>
    public class VehicleEditorViewModel : EditorViewModel<Vehicle>
    {
        /// <inheritdoc />
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

        /// <summary>
        /// Reverts last edit action.
        /// </summary>
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

        /// <summary>
        /// Adds new vertex to the vehicle shape polygon.
        /// </summary>
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
        /// <summary>
        /// Closes the polygon that makes up the vehicle. 
        /// </summary>
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
        /// <summary>
        /// Sets vehicle turning point.
        /// </summary>
        public RelayCommand<Point> SetPivotCommand
        {
            get
            {
                if (_setPivotCommand == null)
                {
                    _setPivotCommand = new RelayCommand<Point>(
                        point => Pivot = point,
                        point => GeometryHelper.IsEnclosed(point, new Polygon(VehicleShape)));
                }
                return _setPivotCommand;
            }
        }
        /// <summary>
        /// Sets the direction in which the vehicle moves forward.
        /// </summary>
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

        /// <summary>
        /// Points that make up the vehicle's shape.
        /// </summary>
        public ObservableCollectionEx<Point> VehicleShape { get; }

        /// <summary>
        /// Indicates whether the vehicle polygon has been closed.
        /// </summary>
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

        /// <summary>
        /// Turning point of the vehicle.
        /// </summary>
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

        /// <summary>
        /// Direction in which the vehicle moves forward.
        /// </summary>
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

        /// <inheritdoc />
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

        /// <inheritdoc />
        public override Vehicle GetEditedModel()
        {
            if (!IsEditedModelValid)
                return null;

            Vehicle vehicle = new Vehicle();

            Polygon shape = new Polygon(VehicleShape);
            Polygon rotatedShape = GeometryHelper.Rotate(shape, Pivot.Value , - Direction.Value);

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
        /// <inheritdoc />
        public override void SetEditedModel(Vehicle model)
        {
            VehicleShape.ReplaceRange(model.Shape.Vertices);
            if (VehicleShape.Count >= 3)
            {
                ShapeDone = true;
                if (GeometryHelper.IsEnclosed(new Point(0, 0), model.Shape))
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
