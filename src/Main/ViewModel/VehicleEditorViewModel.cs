using System.Collections.Generic;
using System.Windows.Input;
using SRL.Model.Model;

namespace SRL.Main.ViewModel
{
    internal sealed class VehicleEditorViewModel : EditorViewModel<Vehicle>
    {
        public ICommand CloseVehicleShapeCommand { get; }
        public ICommand SetOrientationOriginCommand { get; }
        public ICommand SetOrientationAngleCommand { get; }


        public override Vehicle CurrentModel { get; protected set; }
        public List<Point> VehicleShape { get; private set; }
        public override bool IsCurrentModelValid
        {
            get
            {
                return _isCurrentModelValid;
            }
            protected set
            {
                if (_isCurrentModelValid != value)
                {
                    _isCurrentModelValid = value;
                    OnPropertyChanged();
                    ((RelayCommand)SaveModelCommand).OnCanExecuteChanged();
                }

            }
        }
        public bool IsShapeDone { get; private set; }
        public bool IsOrientationOriginSet { get; }
        public bool IsOrientationAngleSet { get; private set; }
        protected override string SaveFileExtension => "vvd";
        

        private bool _isCurrentModelValid;


        public VehicleEditorViewModel()
        {
            Reset();

            CloseVehicleShapeCommand = new RelayCommand(o =>
            {
                IsShapeDone = true;
            },
            c => !IsShapeDone && VehicleShape.Count >= 3);

            SetOrientationOriginCommand = new RelayCommand(o =>
            {
                Point position = (Point) o;
                CurrentModel.Origin = position;
            },
            c =>
            {
                Point position = (Point) c;
                //TODO check whether origin position is inside the shape polygon's bounds.
                return true;
            });

            SetOrientationAngleCommand = new RelayCommand(o =>
            {
                double angle = (double) o;
                CurrentModel.DirectionAngle = angle;
                IsCurrentModelValid = true;
            },
            c => true);
        }

        protected override void Reset()
        {
            CurrentModel = new Vehicle();
            VehicleShape = new List<Point>();
            IsShapeDone = false;

            IsCurrentModelValid = false;
        }

        protected override void LoadModel(Vehicle model)
        {
            Reset();

            CurrentModel = model;
            IsShapeDone = true;

            //TODO check if loaded vehicle has orientation, set IsCurrentModelValid accordingly
        }

        protected override bool CanAddVertex(Point point)
        {
            if (IsShapeDone)
                return false;

            //TODO check if placing new vertex creates an intersection with other segments of the polygon

            return true;
        }

        protected override void AddVertex(Point newPoint)
        {
            VehicleShape.Add(newPoint);
        }
    }
}
