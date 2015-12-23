using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Win32;
using SRL.Commons.Model;
using SRL.Main.Utilities;

namespace SRL.Main.ViewModel
{
    public class TracingViewModel
    {
        public RelayCommand LoadBitmapCommand
        {
            get
            {
                if (_loadBitmapCommand == null)
                {
                    _loadBitmapCommand = new RelayCommand(() =>
                    {
                        //TODO
                    });
                }
                return _loadBitmapCommand;
            }
        }
        public RelayCommand MakeMapCommand
        {
            get
            {
                if (_makeMapCommand == null)
                {
                    _makeMapCommand = new RelayCommand(() =>
                    {
                        //TODO
                    }, () =>
                    {
                        return _traceTask == null;
                    });
                }
                return _makeMapCommand;
            }
        }
        public RelayCommand MakeVehicleCommand
        {
            get
            {
                if (_makeVehicleCommand == null)
                {
                    _makeVehicleCommand = new RelayCommand(() =>
                    {
                        //TODO
                    }, () =>
                    {
                        return SelectedPolygonIndices.Count == 1 && _traceTask == null;
                    });
                }
                return _makeVehicleCommand;
            }
        }
        public RelayCommand TraceCommand
        {
            get
            {
                if (_traceCommand == null)
                {
                    _traceCommand = new RelayCommand(() =>
                    {
                        _traceCancellationTokenSource?.Cancel();
                        _traceCancellationTokenSource = new CancellationTokenSource();
                        _traceTask = new Task(() =>
                        {
                            var output = _tracer.Trace(AreaThreshold, ColorThreshold);
                            if (!_traceCancellationTokenSource.Token.IsCancellationRequested)
                            {
                                Polygons.ReplaceRange(output);
                                SelectedPolygonIndices.Clear();
                                _traceTask = null;
                            }
                        });
                    }, () =>
                    {
                        return _tracer != null;
                    });
                }
                return _traceCommand;
            }
        }
        public RelayCommand<Point> SelectPolygonCommand
        {
            get
            {
                if (_selectPolygonCommand == null)
                {
                    //TODO
                }
                return _selectPolygonCommand;;
            }
        }

        private RelayCommand _loadBitmapCommand;
        private RelayCommand _makeMapCommand;
        private RelayCommand _makeVehicleCommand;
        private RelayCommand _traceCommand;
        private RelayCommand<Point> _selectPolygonCommand;
        

        public int AreaThreshold { get; set; }
        public int ColorThreshold { get; set; }


        public BitmapSource Bitmap { get; private set; }
        public ObservableCollectionEx<Polygon> Polygons { get; }
        public ObservableCollectionEx<int> SelectedPolygonIndices { get; }


        private CancellationTokenSource _traceCancellationTokenSource;
        private Task _traceTask; 
        private BitmapTracer _tracer;


        public TracingViewModel()
        {
            Polygons = new ObservableCollectionEx<Polygon>();
            SelectedPolygonIndices = new ObservableCollectionEx<int>();
        }
    }
}
