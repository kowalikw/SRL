using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Messages;
using SRL.Main.Tracing;
using SRL.Main.Utilities;
using SRL.Main.View.Dialogs;
using SRL.Main.View.Pages;

namespace SRL.Main.ViewModel
{
    public class TracingViewModel : Base.ViewModel
    {
        public RelayCommand LoadBitmapCommand
        {
            get
            {
                if (_loadBitmapCommand == null)
                {
                    _loadBitmapCommand = new RelayCommand(() => 
                    {
                        var args = new OpenFileDialogArgs();
                        args.Filter = "Raster image files|*.bmp;*.jpg;*.jpeg;*.png";
                        args.CloseCallback = (result, filename) =>
                        {
                            if (!result)
                                return;

                            Bitmap = new BitmapImage(new Uri(filename));
                            _tracer = new BitmapTracer(filename);
                            Polygons.Clear();
                            SelectedPolygonIndices.Clear();
                        };
                    }, () =>
                    {
                        return !OngoingTracing;
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
                        var argMsg = new SetModelMessage<Map>(new Map());
                        argMsg.Model.Obstacles.AddRange(
                            Polygons.Where((polygon, i) => SelectedPolygonIndices.Contains(i)));

                        var gotoMsg = new GoToPageMessage(typeof(MapEditorView));

                        Messenger.Default.Send(argMsg);
                        Messenger.Default.Send(gotoMsg);
                    }, () =>
                    {
                        return !OngoingTracing;
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
                        var argMsg = new SetModelMessage<Vehicle>(new Vehicle());
                        argMsg.Model.Shape = Polygons[SelectedPolygonIndices.GetLast()];

                        var gotoMsg = new GoToPageMessage(typeof(VehicleEditorView));
                        
                        Messenger.Default.Send(argMsg);
                        Messenger.Default.Send(gotoMsg);
                    }, () =>
                    {
                        //TODO allow polygon sums?
                        return SelectedPolygonIndices.Count == 1 && !OngoingTracing;
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
                        var token = _traceCancellationTokenSource.Token;
                        TraceTask = new Task(() =>
                        {
                            var traceResult = _tracer.Trace(_pixelAreaThreshold, _absoluteColorThreshold);

                            if (!token.IsCancellationRequested) //TODO token
                            {
                                Polygons.ReplaceRange(traceResult);
                                TraceTask = null;
                            }
                        }, token);
                        Polygons.Clear();
                        SelectedPolygonIndices.Clear();
                        TraceTask.Start();
                    }, () =>
                    {
                        return _tracer != null && !OngoingTracing;
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
                            if (GeometryHelper.IsEnclosed(point, Polygons[i])
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
                            if (GeometryHelper.IsEnclosed(point, Polygons[i])
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


        public double AreaThreshold
        {
            get { return _areaThreshold; }
            set
            {
                if (_areaThreshold != value)
                {
                    _areaThreshold = value;
                    RaisePropertyChanged();
                }
            }
        }
        public double ColorThreshold
        {
            get { return _colorThreshold; }
            set
            {
                if (_colorThreshold != value)
                {
                    _colorThreshold = value;
                    RaisePropertyChanged();

                }
            }
        }



        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            private set
            {
                if (_bitmap != value)
                {
                    _bitmap = value;
                    RaisePropertyChanged();
                }
            }
        }
        public ObservableCollectionEx<Polygon> Polygons { get; }
        public ObservableCollectionEx<int> SelectedPolygonIndices { get; }


        private CancellationTokenSource _traceCancellationTokenSource;

        public bool OngoingTracing
        {
            get { return _ongoingTracing; }
            private set
            {
                if (_ongoingTracing != value)
                {
                    _ongoingTracing = value;
                    RaisePropertyChanged();
                    RaiseRequerySuggested();
                }
            }
        }
        private Task TraceTask
        {
            get { return _traceTask; }
            set
            {
                _traceTask = value;
                OngoingTracing = _traceTask != null;
            }
        }

        private BitmapTracer _tracer;
        private Task _traceTask;

        private BitmapSource _bitmap;
        private double _areaThreshold;
        private double _colorThreshold;
        private int _pixelAreaThreshold;
        private int _absoluteColorThreshold;
        private bool _ongoingTracing;


        public TracingViewModel()
        {
            // Make sure that default instances of Map/Vehicle editors exist.
            SimpleIoc.Default.GetInstance<MapEditorViewModel>();
            SimpleIoc.Default.GetInstance<VehicleEditorViewModel>();

            Polygons = new ObservableCollectionEx<Polygon>();
            SelectedPolygonIndices = new ObservableCollectionEx<int>();

            PropertyChanged += (o, e) =>
            {
                if (e.PropertyName == nameof(Bitmap))
                {
                    _absoluteColorThreshold = (int)(255 * ColorThreshold);
                    _pixelAreaThreshold = (int)(_bitmap.Height * _bitmap.Width * AreaThreshold);
                }
                else if (e.PropertyName == nameof(ColorThreshold))
                {
                    _absoluteColorThreshold = (int)(255 * ColorThreshold);
                }
                else if (e.PropertyName == nameof(AreaThreshold))
                {
                    if (Bitmap != null)
                        _pixelAreaThreshold = (int)(_bitmap.Height * _bitmap.Width * AreaThreshold);
                }
            };

        }
    }
}
