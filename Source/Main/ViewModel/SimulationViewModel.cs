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
        public override RelayCommand ResetCommand
        {
            get
            {
                if (_resetCommand == null)
                {
                    _resetCommand = new RelayCommand(() =>
                    {
                        Map = null;
                        Vehicle = null;
                        VehicleSize = null;
                        InitialVehicleRotation = null;
                        StartPoint = null;
                        EndPoint = null;
                        _orders = null;
                        _frames = null;
                    });
                }
                return _resetCommand;
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
                        CalculateFrames(_orders);
                    }, () =>
                    {
                        return _orders != null;
                    });
                }
                return _calculatePathCommand;
            }
        }

        public RelayCommand StartPlaybackCommand
        {
            get
            {
                if (_startPlaybackCommand == null)
                {
                    _startPlaybackCommand = new RelayCommand(() =>
                    {
                        SimulationRunning = true;
                    }, () =>
                    {
                        return _frames != null;
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
                        SimulationRunning = false;
                        CurrentFrameIdx = 0;
                    }, () =>
                    {
                        return SimulationRunning;
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
                        SimulationRunning = false;
                    }, () =>
                    {
                        return SimulationRunning;
                    });
                }
                return _pausePlaybackCommand;
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
                        StartPoint = null;
                        EndPoint = null;
                        VehicleSize = null;
                        InitialVehicleRotation = null;
                        _orders = null;
                        _frames = null;

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
                        VehicleSize = null;
                        InitialVehicleRotation = null;
                        _orders = null;
                        _frames = null;

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
                        VehicleSize = null;
                        InitialVehicleRotation = null;
                        _orders = null;
                        _frames = null;

                        StartPoint = point;
                    }, point =>
                    {
                        if (Map == null || Vehicle == null)
                            return false;

                        foreach (var obstacle in Map.Obstacles)
                        {
                            if (GeometryHelper.IsInsidePolygon(point, obstacle))
                                return false;
                        }
                        return true;
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
                        _orders = null;
                        _frames = null;

                        EndPoint = point;
                    }, point =>
                    {
                        if (Map == null || Vehicle == null)
                            return false;

                        foreach (var obstacle in Map.Obstacles)
                        {
                            if (GeometryHelper.IsInsidePolygon(point, obstacle))
                                return false;
                        }
                        return true;
                    });
                }
                return _setEndPointCommand;
            }
        }
        public RelayCommand<double> SetInitialVehicleRotationCommand
        {
            get
            {
                if (_setInitialVehicleRotationCommand == null)
                {
                    _setInitialVehicleRotationCommand = new RelayCommand<double>(angle =>
                    {
                        _orders = null;
                        _frames = null;

                        InitialVehicleRotation = angle;
                    }, angle =>
                    {
                        if (Map == null 
                        || Vehicle == null 
                        || StartPoint == null)
                            return false;

                        return true;
                    });
                }
                return _setInitialVehicleRotationCommand;
            }
        }
        public RelayCommand<double> SetVehicleSizeCommand
        {
            get
            {
                if (_setVehicleSizeCommand == null)
                {
                    _setVehicleSizeCommand = new RelayCommand<double>(sizeFactor =>
                    {
                        _orders = null;
                        _frames = null;

                        VehicleSize = sizeFactor;
                    }, sizeFactor =>
                    {
                        if (Map == null 
                        || Vehicle == null 
                        || StartPoint == null 
                        || InitialVehicleRotation == null)
                            return false;

                        return false; //TODO check if vehicle overlays any obstacle
                    });
                }
                return _setVehicleSizeCommand;
            }
        }

        #region Command backing fields

        private RelayCommand _resetCommand;
        private RelayCommand _calculatePathCommand;

        private RelayCommand _startPlaybackCommand;
        private RelayCommand _stopPlaybackCommand;
        private RelayCommand _pausePlaybackCommand;

        private RelayCommand _loadMapCommand;
        private RelayCommand _loadVehicleCommand;
        private RelayCommand<Point> _setStartPointCommand;
        private RelayCommand<Point> _setEndPointCommand;
        private RelayCommand<double> _setInitialVehicleRotationCommand;
        private RelayCommand<double> _setVehicleSizeCommand;

        #endregion

        public bool SimulationRunning
        {
            get { return _simulationRunning; }
            private set
            {
                if (_simulationRunning != value)
                {
                    if (value == true)
                    {
                        if (CurrentFrameIdx == MaxFrameIdx)
                            CurrentFrameIdx = 0;
                        _simulationTimer.Start();
                    }
                    else
                        _simulationTimer.Stop();

                    _simulationRunning = value;
                    RaisePropertyChanged();
                }
            }
        }

        protected override bool IsModelValid
        {
            get
            {
                return Map != null
                       && Vehicle != null
                       && VehicleSize.HasValue
                       && InitialVehicleRotation.HasValue
                       && StartPoint.HasValue
                       && EndPoint.HasValue
                       && _orders != null;
            }
        }


        public Map Map
        {
            get { return _map; }
            private set
            {
                if (_map != value)
                {
                    _map = value;
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
                    ActualVehicle
                }
            }
        }

        public double? VehicleSize
        {
            get { return _vehicleSize; }
            private set
            {
                _vehicleSize = value;
            }
        }

        /// <summary>
        /// <see cref="Vehicle"/> resized by <see cref="VehicleSize"/> factor.
        /// </summary>
        public Vehicle ActualVehicle { get; private set; }
        public double? InitialVehicleRotation { get; private set; }

        public Point? StartPoint
        {
            get { return _startPoint; }
            private set
            {
                if (_startPoint != null)
                {
                    _startPoint = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Point? EndPoint
        {
            get { return _endPoint; }
            private set
            {
                if (_endPoint != null)
                {
                    _endPoint = value;
                    RaisePropertyChanged();
                }
            }
        }

        public Frame CurrentFrame => _frames?[CurrentFrameIdx];
        public int CurrentFrameIdx { get; private set; }
        public int MaxFrameIdx => _frames.Count - 1;


        private readonly DispatcherTimer _simulationTimer;
        private List<Frame> _frames;
        private List<Order> _orders;
        private bool _simulationRunning;
        private IAlgorithm _algorithm;




        private Map _map;
        private Point? _startPoint;
        private Point? _endPoint;
        private Vehicle _vehicle;
        private double? _vehicleSize;


        public SimulationViewModel()
        {
            _algorithm = new MockAlgorithm(); //TODO change to an actual implementation

            _simulationTimer = new DispatcherTimer();
            _simulationTimer.Interval = new TimeSpan(0,0,0,1);
            _simulationTimer.Tick += (o, e) =>
            {
                if (CurrentFrameIdx == MaxFrameIdx)
                    SimulationRunning = false;
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
                Orders = _orders
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
            _orders = model.Orders;

            if (_orders != null)
                CalculateFrames(_orders);
        }

        private void CalculateFrames(List<Order> orders) //TODO refactor
        {
            if (!IsModelValid)
                return;

            const int partsPerRadian = 128;
            double radiansPerPart = 1 / (double)partsPerRadian;

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

                int partCount = (int)(Math.Abs(relativeRotation) * partsPerRadian);

                double currentAngle = frames.Peek().Rotation;
                Point currentPosition = frames.Peek().Position;
                double angleChange = relativeRotation > 0 ? radiansPerPart : -radiansPerPart;

                for (int p = 0; p < partCount - 1; p++)
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

                double xStep = dx > 0 ? 1 : -1;
                double yStep = dy > 0 ? 1 : -1;

                currentAngle = frames.Peek().Rotation;
                currentPosition = frames.Peek().Position;
                if (Math.Abs(dx) < 0.5)
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
                else if (Math.Abs(dy) < 0.5)
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
            _frames = frames.Reverse().ToList();
        }
    }
}
