using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using SRL.Commons;
using SRL.Commons.Model;
using SRL.Commons.Model.Base;
using SRL.Commons.Utilities;
using SRL.Main.Messages;
using SRL.Main.View;
using SRL.Main.View.Dialogs;
using SRL.Main.View.Localization;
using SRL.Main.ViewModel.Base;
using Frame = SRL.Commons.Model.Frame;

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
                        EditorMode = mode;
                    }, mode =>
                    {
                        if (CalculatingPath)
                            return false;

                        switch (mode)
                        {
                            case Mode.Normal:
                                return true;
                            case Mode.StartPointSetup:
                            case Mode.EndPointSetup:
                                return Map != null && Vehicle != null;
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
                        CancelPathCalculation();
                        SimulationRunning = false;
                        EditorMode = Mode.Normal;
                        Map = null;
                        Vehicle = null;
                    }, () =>
                    {
                        return Map != null || Vehicle != null;
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
                        var newMap = LoadModelViaDialog<Map>();
                        if (newMap != null)
                            Map = newMap;

                    }, () => !CalculatingPath);
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
                        var newVehicle = LoadModelViaDialog<Vehicle>();
                        if (newVehicle != null)
                            Vehicle = newVehicle;

                    }, () => !CalculatingPath);
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
                        if (EditorMode != Mode.StartPointSetup || CalculatingPath || Map == null)
                            return false;

                        return !GeometryHelper.IsEnclosed(point, Map.Obstacles);
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
                        if (EditorMode != Mode.EndPointSetup || CalculatingPath || Map == null)
                            return false;

                        return !GeometryHelper.IsEnclosed(point, Map.Obstacles);
                    });
                }
                return _setEndPointCommand;
            }
        }
        public RelayCommand<VehicleSetup> SetInitialVehicleSetupCommandCommand
        {
            get
            {
                if (_setInitialVehicleSetupCommand == null)
                {
                    _setInitialVehicleSetupCommand = new RelayCommand<VehicleSetup>(setup =>
                    {
                        VehicleSize = setup.RelativeSize;
                        InitialVehicleRotation = setup.Rotation;
                        EditorMode = Mode.Normal;
                    }, setup =>
                    {
                        if (EditorMode != Mode.VehicleSetup ||
                            CalculatingPath ||
                            Map == null ||
                            Vehicle == null ||
                            StartPoint == null)
                            return false;

                        Polygon shape = GeometryHelper.Resize(Vehicle.Shape, setup.RelativeSize);
                        shape = GeometryHelper.Rotate(shape, setup.Rotation);
                        shape = GeometryHelper.Move(shape, StartPoint.Value);

                        if (shape.Vertices.Any(GeometryHelper.IsOutOfBounds))
                            return false;

                        return !GeometryHelper.IsIntersected(shape, Map.Obstacles);
                    });
                }
                return _setInitialVehicleSetupCommand;
            }
        }
        public RelayCommand StartPathCalculationCommand
        {
            get
            {
                if (_startPathCalculationCommand == null)
                {
                    _startPathCalculationCommand = new RelayCommand(() =>
                    {
                        StopPlaybackCommand.Execute(null);
                        EditorMode = Mode.Normal;

                        List<Option> options = _algorithm.GetOptions();
                        if (GetAlgorithmOptions(options))
                        {
                            _algorithm.SetOptions(options);
                            StartNewPathCalculation();
                        }
                    }, () =>
                    {
                            return !CalculatingPath && Map != null && Vehicle != null && VehicleSize != null &&
                                   InitialVehicleRotation != null && StartPoint != null && EndPoint != null;
                    });
                }
                return _startPathCalculationCommand;
            }
        }
        public RelayCommand CancelPathCalculationCommand
        {
            get
            {
                if (_cancelPathCalculationCommand == null)
                {
                    _cancelPathCalculationCommand = new RelayCommand(CancelPathCalculation, () => CalculatingPath);
                }
                return _cancelPathCalculationCommand;
            }
        }

        private RelayCommand<Mode> _enterModeCommand;
        private RelayCommand _resetCommand;
        private RelayCommand _loadMapCommand;
        private RelayCommand _loadVehicleCommand;
        private RelayCommand<Point> _setStartPointCommand;
        private RelayCommand<Point> _setEndPointCommand;
        private RelayCommand<VehicleSetup> _setInitialVehicleSetupCommand;
        private RelayCommand _startPathCalculationCommand;
        private RelayCommand _cancelPathCalculationCommand;

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
                        SimulationRunning = true;
                    }, () =>
                    {
                        return EditorMode == Mode.Normal && 
                            !CalculatingPath && 
                            !SimulationRunning && 
                            _frames != null;
                    });
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
                        SimulationRunning = false;
                        CurrentFrameIdx = 0;
                    }, () =>
                    {
                        return CurrentFrameIdx != 0 || SimulationRunning;
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
                        SimulationRunning = false;
                    }, () => { return SimulationRunning; });
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
                if (Set(ref _map, value))
                {
                    StartPoint = null;
                    EndPoint = null;
                }
            }
        }
        public Point? StartPoint
        {
            get { return _startPoint; }
            private set
            {
                if (Set(ref _startPoint, value))
                {
                    InitialVehicleRotation = null;
                    VehicleSize = null;
                    Orders = null;
                }
            }
        }
        public Point? EndPoint
        {
            get { return _endPoint; }
            private set
            {
                if (Set(ref _endPoint, value))
                {
                    Orders = null;
                }
            }
        }
        public Vehicle Vehicle
        {
            get { return _vehicle; }
            private set
            {
                if (Set(ref _vehicle, value))
                {
                    StartPoint = null;
                    EndPoint = null;
                }
            }
        }
        public double? VehicleSize
        {
            get { return _vehicleSize; }
            private set
            {
                if (Set(ref _vehicleSize, value))
                {
                    Orders = null;
                }
            }
        }
        public double? InitialVehicleRotation
        {
            get { return _initialVehicleRotation; }
            private set
            {
                if (Set(ref _initialVehicleRotation, value))
                {
                    Orders = null;
                }
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
                if (Set(ref _orders, value))
                {
                    if (value == null)
                    {
                        Path = null;

                        _frames = null;
                        MaxFrameIdx = -1;
                        CurrentFrameIdx = 0;
                    }
                    else
                    {
                        var pathVertices = new List<Point>();
                        pathVertices.Add(StartPoint.Value);
                        pathVertices.AddRange(Orders.Select(order => order.Destination));
                        Path = pathVertices;

                        CalculateFrames(value);
                        MaxFrameIdx = _frames.Count - 1;
                        StopPlaybackCommand.Execute(null);
                    }
                }
            }
        }
        public Frame CurrentFrame => _frames?[CurrentFrameIdx];
        public int CurrentFrameIdx
        {
            get { return _currentFrameIdx; }
            set { Set(ref _currentFrameIdx, value); }
        }
        public int MaxFrameIdx
        {
            get { return _maxFrameIdx; }
            set { Set(ref _maxFrameIdx, value); }
        }
        public List<Point> Path
        {
            get { return _path; }
            private set { Set(ref _path, value); }
        }

        private List<Order> _orders;
        private List<Frame> _frames;
        private int _currentFrameIdx;
        private int _maxFrameIdx;
        private List<Point> _path;

        #endregion


        #region Path calculation properties

        public bool CalculatingPath
        {
            get { return _calculatingPath; }
            set
            {
                Set(ref _calculatingPath, value);
                RaiseRequerySuggested();
            }
        }

        private bool _calculatingPath;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _cancellationLock = new object();

        #endregion


        public Mode EditorMode
        {
            get { return _editorMode; }
            private set
            {
                if (Set(ref _editorMode, value))
                {
                    if (_editorMode == Mode.StartPointSetup)
                        StartPoint = null;
                    else if (_editorMode == Mode.EndPointSetup)
                        EndPoint = null;

                    if (_editorMode != Mode.VehicleSetup &&
                        StartPoint != null &&
                        (InitialVehicleRotation == null || VehicleSize == null))
                        StartPoint = null;
                }
            }
        }

        private Mode _editorMode;

        public bool SimulationRunning
        {
            get { return _simulationRunning; }
            set
            {
                Set(ref _simulationRunning, value);

                if (value)
                    _simulationTimer.Start();
                else
                    _simulationTimer.Stop();
            }
        }


        protected override bool IsEditedModelValid
        {
            get
            {
                return Map != null &&
                    Vehicle != null &&
                    StartPoint != null &&
                    EndPoint != null &&
                    VehicleSize != null &&
                    InitialVehicleRotation != null &&
                    Orders != null;
            }
        }

        private readonly DispatcherTimer _simulationTimer;
        private IAlgorithm _algorithm;


        private bool _simulationRunning;


        public SimulationViewModel()
        {
            EditorMode = Mode.Normal;

            _algorithm = new Algorithm.Algorithm();

            _simulationTimer = new DispatcherTimer();
            _simulationTimer.Interval = new TimeSpan(0, 0, 0, 0, FrameChangeInterval);
            _simulationTimer.Tick += (o, e) =>
            {
                if (CurrentFrameIdx == MaxFrameIdx)
                    SimulationRunning = false;
                else
                    CurrentFrameIdx++;
            };
        }

        public override Simulation GetEditedModel()
        {
            if (!IsEditedModelValid)
                return null;

            Simulation simulation = new Simulation()
            {
                Map = Map,
                Vehicle = Vehicle,
                StartPoint = StartPoint.Value,
                EndPoint = EndPoint.Value,
                VehicleSize = VehicleSize.Value,
                InitialVehicleRotation = InitialVehicleRotation.Value,
                Orders = Orders
            };
            return simulation;
        }

        public override void SetEditedModel(Simulation model)
        {
            Map = model.Map;
            Vehicle = model.Vehicle;
            StartPoint = model.StartPoint;
            EndPoint = model.EndPoint;
            VehicleSize = model.VehicleSize;
            InitialVehicleRotation = model.InitialVehicleRotation;
            Orders = model.Orders;

            if (Orders != null)
                CalculateFrames(Orders);
        }

        private void CalculateFrames(List<Order> orders)
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
                Point originPosition = frames.Peek().Position;

                double originAngle = frames.Peek().Rotation;

                if (Math.Abs(originAngle) > 2 * Math.PI)
                    originAngle %= 2 * Math.PI;
                if (originAngle < 0)
                    originAngle += 2 * Math.PI;

                double targetAngle = orders[o].Rotation;

                if (Math.Abs(targetAngle) > 2 * Math.PI)
                    targetAngle %= 2 * Math.PI;
                if (targetAngle < 0)
                    targetAngle += 2 * Math.PI;

                if (originAngle != targetAngle) // Purposeful comparison of floating point numbers without epsilon.
                {
                    if (orders[o].Rotation >= 0) // CCW turn
                    {
                        if (targetAngle > originAngle)
                            relativeRotation = targetAngle - originAngle;
                        else
                            relativeRotation = 2 * Math.PI - originAngle + targetAngle;
                    }
                    else // CW turn
                    {
                        if (targetAngle > originAngle)
                            relativeRotation = targetAngle - originAngle - 2 * Math.PI;
                        else
                            relativeRotation = targetAngle - originAngle;
                    }

                    int rotationFrameCount = (int)(Math.Abs(relativeRotation) * FramesPerRadian);
                    double frameAngleChange = relativeRotation >= 0 ? radiansPerPart : -radiansPerPart;

                    for (int p = 0; p < rotationFrameCount - 1; p++)
                    {
                        originAngle += frameAngleChange;

                        if (Math.Abs(originAngle) > 2 * Math.PI)
                            originAngle %= 2 * Math.PI;
                        if (originAngle < 0)
                            originAngle += 2 * Math.PI;

                        frames.Push(new Frame
                        {
                            Position = originPosition,
                            Rotation = originAngle,
                        });
                    }

                    frames.Push(new Frame
                    {
                        Position = originPosition,
                        Rotation = targetAngle,
                    });
                }

                // Move frames.
                Point targetPosition = orders[o].Destination;

                double dx = targetPosition.X - originPosition.X;
                double dy = targetPosition.Y - originPosition.Y;

                double xStep = dx > 0 ? MovePerFrame : -MovePerFrame;
                double yStep = dy > 0 ? MovePerFrame : -MovePerFrame;

                originPosition = frames.Peek().Position;

                if (Math.Abs(dx) < Math.Abs(xStep) / 2)
                {
                    for (double y = yStep; Math.Abs(y) <= Math.Abs(dy); y += yStep)
                    {
                        frames.Push(new Frame
                        {
                            Position = new Point(originPosition.X, originPosition.Y + y),
                            Rotation = targetAngle
                        });
                    }
                }
                else if (Math.Abs(dy) < Math.Abs(yStep) / 2)
                {
                    for (double x = xStep; Math.Abs(x) <= Math.Abs(dx); x += xStep)
                    {
                        frames.Push(new Frame
                        {
                            Position = new Point(originPosition.X + x, originPosition.Y),
                            Rotation = targetAngle,
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
                                Position = new Point(originPosition.X + x, originPosition.Y + y),
                                Rotation = targetAngle,
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
                                Position = new Point(originPosition.X + x, originPosition.Y + y),
                                Rotation = targetAngle,
                            });
                        }
                    }
                }

                frames.Push(new Frame
                {
                    Position = targetPosition,
                    Rotation = targetAngle,
                });

            }
            _frames = frames.Reverse().ToList();
        }

        private void StartNewPathCalculation()
        {
            Monitor.Enter(_cancellationLock);
            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();
            CancellationToken token = _cancellationTokenSource.Token;

            CalculatingPath = true;
            Task.Run(() =>
            {
                List<Order> orders = null;
                try
                {
                    orders = _algorithm.GetPath(Map, Vehicle, StartPoint.Value, EndPoint.Value, VehicleSize.Value,
                        InitialVehicleRotation.Value, token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
                catch (NonexistentPathException)
                {
                    var rm = new ResourceManager(typeof(Dialogs).FullName, Assembly.GetExecutingAssembly());

                    var args = new MessageDialogArgs();
                    args.Title = rm.GetString("pathNotFoundTitle");
                    args.Description = rm.GetString("pathNotFoundMsg");
                    Messenger.Default.Send(new ShowDialogMessage(args));
                }

                if (Monitor.TryEnter(_cancellationLock))
                {
                    if (!token.IsCancellationRequested)
                    {
                        CalculatingPath = false;
                        Orders = orders;
                    }
                    Monitor.Exit(_cancellationLock);
                }

            }, token);

            Monitor.Exit(_cancellationLock);
        }

        private void CancelPathCalculation()
        {
            Monitor.Enter(_cancellationLock);
            _cancellationTokenSource?.Cancel();
            CalculatingPath = false;
            Monitor.Exit(_cancellationLock);
        }

        private bool GetAlgorithmOptions(List<Option> options)
        {
            OptionsDialogView dialog = new OptionsDialogView(options);

            if (dialog.ShowDialog() == true)
            {
                options.Clear();
                options.AddRange(dialog.Result);
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
        }
    }
}