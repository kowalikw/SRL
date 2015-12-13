using System.Collections.Generic;
using System.Windows.Input;
using SRL.Main.Utilities;
using SRL.Model.Model;
using SRL.Model;
using System;

namespace SRL.Main.ViewModel
{
    internal sealed class VehicleEditorViewModel : EditorViewModel<Vehicle>
    {
        public enum EditingStage
        {
            NotStarted = 0,
            ShapeStarted,
            ShapeDone,
            OrientationOriginSet,
            OrientationAngleSet,
        }


        public ICommand CloseVehicleShapeCommand { get; }
        public ICommand SetOrientationOriginCommand { get; }
        public ICommand SetOrientationAngleCommand { get; }


        public EditingStage Stage
        {
            get
            {
                return _stage;
            }
            private set
            {
                _stage = value;
                IsCurrentModelValid = _stage == EditingStage.OrientationAngleSet;
            }
        }
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
        protected override string SaveFileExtension => "vvd";
        

        private bool _isCurrentModelValid;
        private EditingStage _stage;


        public VehicleEditorViewModel(object model = null)
        {
            Reset();

            if (model is Vehicle)
            {
                CurrentModel = (Vehicle)model;
                Stage = EditingStage.ShapeDone;
            }

            if(CurrentModel != null)
                VehicleShape = CurrentModel.Shape.Vertices;

            CloseVehicleShapeCommand = new RelayCommand(o =>
            {
                CurrentModel.Shape = new Polygon(VehicleShape);
                Stage = EditingStage.ShapeDone;
            },
            c => Stage == EditingStage.ShapeStarted && VehicleShape.Count >= 3);

            SetOrientationOriginCommand = new RelayCommand(o =>
            {
                Point position = (Point) o;
                CurrentModel.OrientationOrigin = position;
                Stage = EditingStage.OrientationOriginSet;
            },
            c =>
            {
                if (Stage != EditingStage.ShapeDone)
                    return false;

                Point position = (Point) c;

                return GeometryHelper.IsPointInPolygon(position, CurrentModel.Shape);
            });

            SetOrientationAngleCommand = new RelayCommand(o =>
            {
                // TODO: Refactoring
                Point position = (Point)o;

                double angle = GeometryHelper.GetRadAngle(CurrentModel.OrientationOrigin, position);
                if (CurrentModel.OrientationOrigin.X > position.X) angle += Math.PI;
                if (angle < 0) angle += 2 * Math.PI;

                // TODO: Refactoring.
                CurrentModel.OrientationOriginOld = CurrentModel.OrientationOrigin;
                CurrentModel.OrientationAngle = angle;
                CurrentModel.OrientationOriginEnd = position;

                List<Point> lst = new List<Point>();
                for (int i = 0; i < CurrentModel.Shape.VertexCount; i++)
                {
                    lst.Add(GeometryHelper.RotatePoint(CurrentModel.Shape.Vertices[i] - CurrentModel.OrientationOrigin, new Point(0, 0), -CurrentModel.OrientationAngle));
                }
                Vehicle vehicleTemplate = new Vehicle(new Polygon(lst), new Point(0, 0), 0);
                vehicleTemplate.OrientationOriginEnd = CurrentModel.OrientationOriginEnd;
                vehicleTemplate.OrientationOriginOld = CurrentModel.OrientationOriginOld;

                CurrentModel = vehicleTemplate;

                Stage = EditingStage.OrientationAngleSet;
            },
            c => Stage == EditingStage.OrientationOriginSet);
        }

        protected override void Reset()
        {
            CurrentModel = new Vehicle();
            VehicleShape = new List<Point>();
            Stage = EditingStage.NotStarted;
        }

        protected override void LoadModel(Vehicle model)
        {
            Reset();
            CurrentModel = model;

            //TODO check if loaded vehicle has orientation, set Stage accordingly
        }

        protected override bool CanAddVertex(Point newPoint)
        {
            if (Stage >= EditingStage.ShapeDone)
                return false;

            int vCount = VehicleShape.Count;

            if (vCount <= 2)
                return true;

            for (int i = 0; i < vCount - 2; i++)
            {
                if (GeometryHelper.DoSegmentsIntersect(
                    VehicleShape[i], VehicleShape[i + 1],
                    VehicleShape[vCount - 1], newPoint))
                {
                    return false;
                }
            }
            return true;
        }

        protected override void AddVertex(Point newPoint)
        {
            VehicleShape.Add(newPoint);
            Stage = EditingStage.ShapeStarted;
        }
    }
}
