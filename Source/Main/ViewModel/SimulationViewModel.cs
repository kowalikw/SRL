using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Algorithm;
using SRL.Commons.Model;
using SRL.Commons.Model.Base;
using SRL.Commons.Utilities;

namespace SRL.Main.ViewModel
{
    public class SimulationViewModel : EditorViewModel<Simulation>
    {
        private const int FrameChangeInterval = 3;
        private const int FramesPerRadian = 128;
        private const double MovePerFrame = 0.002;

        #region LeftMenuCommands 

        public RelayCommand<Mode> EnterModeCommand
        {
            get
            {
                if (_enterModeCommand == null)
                {
                    _enterModeCommand = new RelayCommand<Mode>(mode =>
                    {
                        switch (mode)
                        {
                            case Mode.StartPointSetup:
                                StartPoint = null;
                                break;
                            case Mode.EndPointSetup:
                                EndPoint = null;
                                break;
                        }
                        EditorMode = mode;
                    }, mode =>
                    {
                        switch (mode)
                        {
                            case Mode.StartPointSetup:
                                return Map != null;
                            case Mode.EndPointSetup:
                                return Map != null;
                            case Mode.VehicleSetup:
                                return StartPoint != null;
                            default:
                                throw new ArgumentException(null, nameof(mode));
                        }
                    });
                }
                return _enterModeCommand;
            }
        }
        public override RelayCommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(() =>
                    {
                        EditorMode = Mode.Normal;
                        Map = null;
                        Vehicle = null;
                        Orders = null;
                    });
                }
                return _resetCommand;
            }
        }
        public RelayCommand LoadMapCommand
        {
            get
            {
                if (_loadMapCommand == null)
                {
                    _loadMapCommand = new RelayCommand(() =>
                    {
                        EditorMode = Mode.Normal;
                        Map = LoadModel<Map>();
                    });
                }
                return _loadMapCommand;
            }
        }
        public RelayCommand LoadVehicleCommand
        {
            get
            {
                if (_loadVehicleCommand == null)
                {
                    _loadVehicleCommand = new RelayCommand(() =>
                    {
                        EditorMode = Mode.Normal;
                        Vehicle = LoadModel<Vehicle>();
                    });
                }
                return _loadVehicleCommand;
            }
        }
        public RelayCommand<Point> SetStartPointCommand
        {
            get
            {
                if (_setStartPointCommand == null)
                {
                    _setStartPointCommand = new RelayCommand<Point>(point =>
                    {
                        EditorMode = Mode.Normal;
                        StartPoint = point;
                        EnterModeCommand.Execute(Mode.VehicleSetup);
                    }, point =>
                    {
                        if (EditorMode != Mode.StartPointSetup || Map == null)
                            return false;

                        return !IsInsideAnyObstacle(point);
                    });
                }
                return _setStartPointCommand;
            }
        }
        public RelayCommand<Point> SetEndPointCommand
        {
            get
            {
                if (_setEndPointCommand == null)
                {
                    _setEndPointCommand = new RelayCommand<Point>(point =>
                    {
                        EditorMode = Mode.Normal;
                        EndPoint = point;
                    }, point =>
                    {
                        if (EditorMode != Mode.EndPointSetup || Map == null)
                            return false;

                        return !IsInsideAnyObstacle(point);
                    });
                }
                return _setEndPointCommand;
            }
        }
        public RelayCommand<VehicleSetup> SetInitialVehicleSetup
        {
            get
            {
                if (_setInitialVehicleSetup == null)
                {
                    _setInitialVehicleSetup = new RelayCommand<VehicleSetup>(setup =>
                    {
                        EditorMode = Mode.Normal;
                        VehicleSize = setup.RelativeSize;
                        InitialVehicleRotation = setup.Rotation;

                    }, setup =>
                    {
                        if (EditorMode != Mode.VehicleSetup ||
                            Map == null ||
                            Vehicle == null ||
                            StartPoint == null)
                            return false;

                        return true; //TODO check if vehicle overlays with any obstacle
                    });
                }
                return _setInitialVehicleSetup;
            }
        }
        public RelayCommand CalculatePathCommand
        {
            get
            {
                if (_calculatePathCommand == null)
                {
                    _calculatePathCommand = new RelayCommand(() =>
                    {
                        EditorMode = Mode.Normal;

                        Orders = _algorithm.GetPath(Map, Vehicle, StartPoint.Value, EndPoint.Value, VehicleSize.Value,
                            InitialVehicleRotation.Value, 360); // TODO angle density as parameter
                    },
                        () =>
                        {
                            return EditorMode == Mode.Normal && Map != null && Vehicle != null && VehicleSize != null &&
                                   InitialVehicleRotation != null && StartPoint != null && EndPoint != null &&
                                   Orders == null;
                        });
                }
                return _calculatePathCommand;
            }
        }

        
        private RelayCommand<Mode> _enterModeCommand;
        private RelayCommand _resetCommand;
        private RelayCommand _loadMapCommand;
        private RelayCommand _loadVehicleCommand;
        private RelayCommand<Point> _setStartPointCommand;
        private RelayCommand<Point> _setEndPointCommand;
        private RelayCommand<VehicleSetup> _setInitialVehicleSetup;
        private RelayCommand _calculatePathCommand;

        #endregion


        #region Playback control commands

        public RelayCommand StartPlaybackCommand
        {
            get
            {
                if (_startPlaybackCommand == null)
                {
                    _startPlaybackCommand = new RelayCommand(() =>
                    {
                        if (CurrentFrameIdx == MaxFrameIdx)
                            CurrentFrameIdx = 0;
                        EditorMode = Mode.SimulationRunning;
                        _simulationTimer.Start();
                    }, () => { return EditorMode == Mode.Normal && Orders != null; });
                }
                return _startPlaybackCommand;
            }
        }
        public RelayCommand StopPlaybackCommand
        {
            get
            {
                if (_stopPlaybackCommand == null)
                {
                    _stopPlaybackCommand = new RelayCommand(() =>
                    {
                        EditorMode = Mode.Normal;
                        _simulationTimer.Stop();
                        CurrentFrameIdx = 0;
                    }, () =>
                    {
                        return CurrentFrameIdx != 0 || EditorMode == Mode.SimulationRunning;
                    });
                }
                return _stopPlaybackCommand;
            }
        }
        public RelayCommand PausePlaybackCommand
        {
            get
            {
                if (_pausePlaybackCommand == null)
                {
                    _pausePlaybackCommand = new RelayCommand(() =>
                    {
                        EditorMode = Mode.Normal;
                        _simulationTimer.Stop();
                    }, () => { return EditorMode == Mode.SimulationRunning; });
                }
                return _pausePlaybackCommand;
            }
        }

        private RelayCommand _startPlaybackCommand;
        private RelayCommand _stopPlaybackCommand;
        private RelayCommand _pausePlaybackCommand;

        #endregion


        #region Map & vehicle properties

        public Map Map
        {
            get { return _map; }
            private set
            {
                if (_map != value)
                {
                    _map = value;

                    StartPoint = null;
                    EndPoint = null;
                    Orders = null;

                    RaisePropertyChanged();
                }
            }
        }
        public Point? StartPoint
        {
            get { return _startPoint; }
            private set
            {
                if (_startPoint != value)
                {
                    _startPoint = value;

                    InitialVehicleRotation = null;
                    VehicleSize = null;
                    Orders = null;

                    RaisePropertyChanged();
                }
            }
        }
        public Point? EndPoint
        {
            get { return _endPoint; }
            private set
            {
                if (_endPoint != value)
                {
                    _endPoint = value;

                    Orders = null;

                    RaisePropertyChanged();
                }
            }
        }
        public Vehicle Vehicle
        {
            get { return _vehicle; }
            private set
            {
                if (_vehicle != value)
                {
                    _vehicle = value;

                    Orders = null;

                    RaisePropertyChanged();
                }
            }
        }
        public double? VehicleSize
        {
            get { return _vehicleSize; }
            private set
            {
                if (_vehicleSize != value)
                {
                    _vehicleSize = value;

                    Orders = null;

                    RaisePropertyChanged();
                }
            }
        }
        public double? InitialVehicleRotation
        {
            get { return _initialVehicleRotation; }
            private set
            {
                _initialVehicleRotation = value;

                Orders = null;
            }
        }

        private Map _map;
        private Point? _startPoint;
        private Point? _endPoint;
        private Vehicle _vehicle;
        private double? _vehicleSize;
        private double? _initialVehicleRotation;

        #endregion


        #region Orders & frames properties

        public List<Order> Orders
        {
            get { return _orders; }
            set
            {
                if (_orders != value)
                {
                    _orders = value;

                    if (value == null)
                    {
                        Path = null;

                        Frames = null;
                        MaxFrameIdx = -1;
                        CurrentFrameIdx = 0;
                    }
                    else
                    {
                        var pathVertices = new List<Point>();
                        pathVertices.Add(StartPoint.Value);
                        pathVertices.AddRange(Orders.Select(order => order.Destination));
                        Path = new Path(pathVertices);

                        CalculateFrames(value);
                        MaxFrameIdx = Frames.Count - 1;
                        CurrentFrameIdx = 0;
                    }

                    RaisePropertyChanged();
                }
            }
        }
        private List<Frame> Frames
        {
            get; set;
        }
        public Frame CurrentFrame => Frames?[CurrentFrameIdx];
        public int CurrentFrameIdx
        {
            get { return _currentFrameIdx; }
            set
            {
                if (_currentFrameIdx != value)
                {
                    _currentFrameIdx = value;
                    RaisePropertyChanged();
                }
            }
        }
        public int MaxFrameIdx
        {
            get { return _maxFrameIdx; }
            set
            {
                if (_maxFrameIdx != value)
                {
                    _maxFrameIdx = value;
                    RaisePropertyChanged();
                }
            }
        }
        public Path Path
        {
            get { return _path; }
            private set
            {
                if (_path != value)
                {
                    _path = value;
                    RaisePropertyChanged();
                }
            }
        }


        private List<Order> _orders;
        private int _currentFrameIdx;
        private int _maxFrameIdx;
        private Path _path;

        #endregion


        public Mode EditorMode
        {
            get { return _editorMode; }
            private set
            {
                if (_editorMode != value)
                {
                    _editorMode = value;
                    RaisePropertyChanged();
                }
            }
        }
        protected override bool IsModelValid
        {
            get
            {
                return Map != null && Vehicle != null && VehicleSize.HasValue && InitialVehicleRotation.HasValue &&
                       StartPoint.HasValue && EndPoint.HasValue && Orders != null;
            }
        }



        private IAlgorithm _algorithm;



        private readonly DispatcherTimer _simulationTimer;



        private Mode _editorMode;
        


        public SimulationViewModel()
        {
            EditorMode = Mode.Normal;

            _algorithm = new Algorithm.Algorithm(); //TODO change to an actual implementation

            _simulationTimer = new DispatcherTimer();
            _simulationTimer.Interval = new TimeSpan(0, 0, 0, 0, FrameChangeInterval);
            _simulationTimer.Tick += (o, e) =>
            {
                if (CurrentFrameIdx == MaxFrameIdx)
                    EditorMode = Mode.Normal;
                else
                    CurrentFrameIdx++;
            };
        }

        protected override Simulation GetModel()
        {
            if (!IsModelValid)
                return null;

            Simulation simulation = new Simulation()
            {
                Map = Map,
                Vehicle = Vehicle,
                VehicleSize = VehicleSize.Value,
                InitialVehicleRotation = InitialVehicleRotation.Value,
                StartPoint = StartPoint.Value,
                EndPoint = EndPoint.Value,
                Orders = Orders
            };
            return simulation;
        }

        protected override void SetModel(Simulation model)
        {
            Map = model.Map;
            Vehicle = model.Vehicle;
            VehicleSize = model.VehicleSize;
            InitialVehicleRotation = model.InitialVehicleRotation;
            StartPoint = model.StartPoint;
            EndPoint = model.EndPoint;
            Orders = model.Orders;

            if (Orders != null)
                CalculateFrames(Orders);
        }

        private void CalculateFrames(List<Order> orders) //TODO refactor
        {
            double radiansPerPart = 1 / (double)FramesPerRadian;

            Stack<Frame> frames = new Stack<Frame>();
            frames.Push(new Frame
            {
                Position = StartPoint.Value,
                Rotation = InitialVehicleRotation.Value
            });

            for (int o = 0; o < orders.Count; o++)
            {
                // Rotation frames.
                double relativeRotation;

                if (o == 0)
                    relativeRotation = orders[o].Rotation - InitialVehicleRotation.Value;
                else
                    relativeRotation = orders[o].Rotation - orders[o - 1].Rotation;

                int rotationFrameCount = (int)(Math.Abs(relativeRotation) * FramesPerRadian);

                double currentAngle = frames.Peek().Rotation;
                Point currentPosition = frames.Peek().Position;

                double angleChange = relativeRotation > 0 ? radiansPerPart : -radiansPerPart;

                for (int p = 0; p < rotationFrameCount - 1; p++)
                {
                    currentAngle += angleChange;

                    frames.Push(new Frame
                    {
                        Position = currentPosition,
                        Rotation = currentAngle < 0 ? Math.PI * 2 + currentAngle : currentAngle
                    });
                }

                frames.Push(new Frame
                {
                    Position = currentPosition,
                    Rotation = orders[o].Rotation
                });

                // Move frames.
                Point start = o == 0 ? StartPoint.Value : orders[o - 1].Destination;
                Point end = orders[o].Destination;

                double dx = end.X - start.X;
                double dy = end.Y - start.Y;

                double xStep = dx > 0 ? MovePerFrame : -MovePerFrame;
                double yStep = dy > 0 ? MovePerFrame : -MovePerFrame;

                currentAngle = frames.Peek().Rotation;
                currentPosition = frames.Peek().Position;

                if (Math.Abs(dx) < Math.Abs(xStep) / 2)
                {
                    for (double y = yStep; Math.Abs(y) <= Math.Abs(dy); y += yStep)
                    {
                        frames.Push(new Frame
                        {
                            Position = new Point(currentPosition.X, currentPosition.Y + y),
                            Rotation = currentAngle,
                        });
                    }
                }
                else if (Math.Abs(dy) < Math.Abs(yStep) / 2)
                {
                    for (double x = xStep; Math.Abs(x) <= Math.Abs(dx); x += xStep)
                    {
                        frames.Push(new Frame
                        {
                            Position = new Point(currentPosition.X + x, currentPosition.Y),
                            Rotation = currentAngle,
                        });
                    }
                }
                else
                {
                    // Based on Bresenham's line algorithm.
                    if (Math.Abs(dx) > Math.Abs(dy))
                    {
                        double error = Math.Abs(dx / 2) - Math.Abs(dy);
                        for (double x = xStep, y = 0; Math.Abs(x) <= Math.Abs(dx); x += xStep)
                        {
                            if (error < 0)
                            {
                                y += yStep;
                                error += Math.Abs(dx);
                            }
                            error -= Math.Abs(dy);

                            frames.Push(new Frame
                            {
                                Position = new Point(currentPosition.X + x, currentPosition.Y + y),
                                Rotation = currentAngle,
                            });
                        }
                    }
                    else
                    {
                        double error = Math.Abs(dy / 2) - Math.Abs(dx);
                        for (double y = yStep, x = 0; Math.Abs(y) <= Math.Abs(dy); y += yStep)
                        {
                            if (error < 0)
                            {
                                x += xStep;
                                error += Math.Abs(dy);
                            }
                            error -= Math.Abs(dx);

                            frames.Push(new Frame
                            {
                                Position = new Point(currentPosition.X + x, currentPosition.Y + y),
                                Rotation = currentAngle,
                            });
                        }
                    }
                }
            }
            Frames = frames.Reverse().ToList();
        }

        public bool IsInsideAnyObstacle(Point point)
        {
            foreach (var obstacle in Map.Obstacles)
            {
                if (GeometryHelper.IsInsidePolygon(point, obstacle))
                    return true;
            }
            return false;
        }

        public enum Mode
        {
            Normal,
            StartPointSetup,
            EndPointSetup,
            VehicleSetup,
            SimulationRunning,
        }
    }
}