using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Utilities;

namespace SRL.Main.ViewModel
{
    public class TracingViewModel : ViewModelBase
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
                        //TODO allow polygon sums?
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
                    _selectPolygonCommand = new RelayCommand<Point>(point =>
                    {
                        for (int i = Polygons.Count - 1; i >= 0; i--)
                        {
                            if (GeometryHelper.IsInsidePolygon(point, Polygons[i])
                               && !SelectedPolygonIndices.Contains(i))
                            {
                                SelectedPolygonIndices.Add(i);
                                return;
                            }
                        }
                    });
                }
                return _selectPolygonCommand; ;
            }
        }
        public RelayCommand<Point> DeselectPolygonCommand
        {
            get
            {
                if (_deselectPolygonCommand == null)
                {
                    _deselectPolygonCommand = new RelayCommand<Point>(point =>
                    {
                        for (int i = Polygons.Count - 1; i >= 0; i--)
                        {
                            if (GeometryHelper.IsInsidePolygon(point, Polygons[i])
                               && SelectedPolygonIndices.Contains(i))
                            {
                                SelectedPolygonIndices.Remove(i);
                                return;
                            }
                        }
                    });
                }
                return _deselectPolygonCommand; ;
            }
        }

        private RelayCommand _loadBitmapCommand;
        private RelayCommand _makeMapCommand;
        private RelayCommand _makeVehicleCommand;
        private RelayCommand _traceCommand;
        private RelayCommand<Point> _selectPolygonCommand;
        private RelayCommand<Point> _deselectPolygonCommand;


        public int AreaThreshold { get; set; }
        public int ColorThreshold { get; set; }
        public bool AntialiasingEnabled { get; set; }


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
