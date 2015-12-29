using System;
using System.Collections.Generic;
using System.Windows;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;

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
                        
                    }, () =>
                    {
                        return false;
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
                        //TODO
                        SimulationRunning = true;
                    }, () =>
                    {
                        return true;
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
                        //TODO
                        SimulationRunning = false;
                    }, () =>
                    {
                        return true;
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
                        //TODO
                        SimulationRunning = false;
                    }, () =>
                    {
                        return true;
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

                }
                return _loadVehicleCommand;
            }
        }
        public RelayCommand<Point> SetStartPoint
        {
            get
            {
                if (_setStartPoint == null)
                {

                }
                return _setStartPoint;
            }
        }
        public RelayCommand<Point> SetEndPoint
        {
            get
            {
                if (_setEndPoint == null)
                {

                }
                return _setEndPoint;
            }
        }
        public RelayCommand<double> SetInitialVehicleRotation
        {
            get
            {
                if (_setInitialVehicleRotation == null)
                {

                }
                return _setInitialVehicleRotation;
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
        private RelayCommand<Point> _setStartPoint;
        private RelayCommand<Point> _setEndPoint;
        private RelayCommand<double> _setInitialVehicleRotation;

        #endregion

        public bool SimulationRunning
        {
            get { return _simulationRunning; }
            private set
            {
                if (_simulationRunning != value)
                {
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
        

        public Map Map { get; private set; }
        public Vehicle Vehicle { get; private set; }
        public double? VehicleSize { get; private set; }
        public double? InitialVehicleRotation { get; private set; }
        public Point? StartPoint { get; private set; }
        public Point? EndPoint { get; private set; }

        public Frame CurrentFrame => _frames[CurrentFrameIdx];
        public int CurrentFrameIdx { get; private set; }
        public int MaxFrameIdx => _frames.Count - 1;


        private List<Frame> _frames;
        private List<Order> _orders;
        private bool _simulationRunning;


        public SimulationViewModel()
        {

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
            throw new NotImplementedException(); //TODO
        }
    }
}
