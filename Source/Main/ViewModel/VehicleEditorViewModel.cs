using System.Windows;
using FirstFloor.ModernUI.Presentation;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;
using RelayCommand = GalaSoft.MvvmLight.CommandWpf.RelayCommand;

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

        protected override bool IsModelValid
        {
            get
            {
                return ShapeDone
                    && Pivot.HasValue
                    && Direction.HasValue;
            }
        }
        public bool AntialiasingEnabled { get; set; }


        public VehicleEditorViewModel()
        {
            VehicleShape = new ObservableCollectionEx<Point>();
        }

        protected override Vehicle GetModel()
        {
            if (!IsModelValid)
                return null;

            Vehicle vehicle = new Vehicle();

            //TODO resize & rotate vehicle

            return vehicle;
        }
        protected override bool SetModel(Vehicle model)
        {
            throw new System.NotImplementedException();//TODO
        }
    }
}
