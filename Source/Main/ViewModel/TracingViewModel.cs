using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Messages;
using SRL.Main.Tracing;
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
                        Messenger.Default.Send(new OpenFileDialogMessage
                        {
                            Filter = "Raster image files|*.bmp;*.jpg;*.jpeg;*.png",
                            FilenameCallback = filename =>
                            {
                                if (filename == null)
                                    return;

                                Bitmap = new BitmapImage(new Uri(filename));
                                _tracer = new BitmapTracer(filename);
                            }

                        });
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
                            List<Polygon> output = _tracer.Trace(_pixelAreaThreshold, _absoluteColorThreshold);

                            if (!_traceCancellationTokenSource.Token.IsCancellationRequested)
                            {
                                Polygons.ReplaceRange(output);
                                SelectedPolygonIndices.Clear();
                                _traceTask = null;
                            }
                        });
                        _traceTask.Start();
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
        public bool AntialiasingEnabled { get; set; }

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
        private Task _traceTask;
        private BitmapTracer _tracer;

        private BitmapSource _bitmap;
        private double _areaThreshold;
        private double _colorThreshold;
        private int _pixelAreaThreshold;
        private int _absoluteColorThreshold;

        public TracingViewModel()
        {
            Polygons = new ObservableCollectionEx<Polygon>();
            SelectedPolygonIndices = new ObservableCollectionEx<int>();

            PropertyChanged += (o, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(Bitmap):
                        _absoluteColorThreshold = (int)(255 * ColorThreshold);
                        _pixelAreaThreshold = (int)(_bitmap.Height * _bitmap.Width * AreaThreshold);
                        break;
                    case nameof(ColorThreshold):
                        _absoluteColorThreshold = (int)(255 * ColorThreshold);
                        break;
                    case nameof(AreaThreshold):
                        if (Bitmap != null)
                            _pixelAreaThreshold = (int)(_bitmap.Height * _bitmap.Width * AreaThreshold);
                        break;
                }
            };

        }

    }
}
