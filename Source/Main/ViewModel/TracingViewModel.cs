using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using GalaSoft.MvvmLight.CommandWpf;
using Microsoft.Practices.ServiceLocation;
using SRL.Commons.Model;
using SRL.Commons.Utilities;
using SRL.Main.Messages;
using SRL.Main.Tracing;
using SRL.Main.Utilities;
using SRL.Main.View.Localization;
using SRL.Main.ViewModel.Services;
using MapEditorView = SRL.Main.View.Pages.MapEditorView;
using VehicleEditorView = SRL.Main.View.Pages.VehicleEditorView;

namespace SRL.Main.ViewModel
{
    /// <summary>
    /// View-model class that contains non-UI logic for the tracing module.
    /// </summary>
    public class TracingViewModel : Base.ViewModel
    {
        #region Commands

        /// <summary>
        /// Opens up a dialog and loads selected bitmap.
        /// </summary>
        public RelayCommand LoadBitmapCommand
        {
            get
            {
                if (_loadBitmapCommand == null)
                {
                    _loadBitmapCommand = new RelayCommand(() =>
                    {
                        ServiceLocator.Current.GetInstance<IDialogService>().ShowOpenFileDialog(
                            Dialogs.ResourceManager.GetString("rasterImageFilter"),
                            (result, filename) =>
                            {
                                if (!result)
                                    return;

                                Bitmap = new BitmapImage(new Uri(filename));
                                _tracer = new BitmapTracer(filename);
                                Polygons.Clear();
                                SelectedPolygonIndices.Clear();
                            });
                    }, () =>
                    {
                        return !TracingOngoing;
                    });
                }
                return _loadBitmapCommand;
            }
        }
        /// <summary>
        /// Creates a <see cref="Map"/> out of selected polygons and passes it to the map editor.
        /// </summary>
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

                        MessengerInstance.Send(argMsg);

                        ServiceLocator.Current.GetInstance<INavigationService>().GoToPage(nameof(MapEditorView));
                    }, () =>
                    {
                        return !TracingOngoing;
                    });
                }
                return _makeMapCommand;
            }
        }
        /// <summary>
        /// Creates a <see cref="Vehicle"/> out of selected polygon and passes it to the vehicle editor.
        /// </summary>
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

                        MessengerInstance.Send(argMsg);
                        ServiceLocator.Current.GetInstance<INavigationService>().GoToPage(nameof(VehicleEditorView));
                    }, () =>
                    {
                        return SelectedPolygonIndices.Count == 1 && !TracingOngoing;
                    });
                }
                return _makeVehicleCommand;
            }
        }
        /// <summary>
        /// Starts trace task.
        /// </summary>
        public RelayCommand TraceCommand
        {
            get
            {
                if (_traceCommand == null)
                {
                    _traceCommand = new RelayCommand(StartNewTraceTask,
                        () => _tracer != null && !TracingOngoing);
                }
                return _traceCommand;
            }
        }
        /// <summary>
        /// Marks polygon that encloses passed <see cref="Point"/>.
        /// </summary>
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
                            if (!SelectedPolygonIndices.Contains(i) &&
                                GeometryHelper.IsEnclosed(point, Polygons[i]))
                            {
                                SelectedPolygonIndices.Add(i);
                                return;
                            }
                        }
                    });
                }
                return _selectPolygonCommand;
            }
        }
        /// <summary>
        /// Unmarks polygon that encloses passed <see cref="Point"/>.
        /// </summary>
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
                            if (SelectedPolygonIndices.Contains(i) &&
                                GeometryHelper.IsEnclosed(point, Polygons[i]))
                            {
                                SelectedPolygonIndices.Remove(i);
                                return;
                            }
                        }
                    });
                }
                return _deselectPolygonCommand;
            }
        }

        private RelayCommand _loadBitmapCommand;
        private RelayCommand _makeMapCommand;
        private RelayCommand _makeVehicleCommand;
        private RelayCommand _traceCommand;
        private RelayCommand<Point> _selectPolygonCommand;
        private RelayCommand<Point> _deselectPolygonCommand;

        #endregion

        /// <summary>
        /// Currently loaded raster image.
        /// </summary>
        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            private set { Set(ref _bitmap, value); }
        }

        /// <summary>
        /// Current area threshold setting (see the documentation).
        /// </summary>
        public double AreaThreshold
        {
            get { return _areaThreshold; }
            set { Set(ref _areaThreshold, value); }
        }

        /// <summary>
        /// Current color threshold setting (see the documentation).
        /// </summary>
        public double ColorThreshold
        {
            get { return _colorThreshold; }
            set { Set(ref _colorThreshold, value); }
        }

        /// <summary>
        /// Boolean value that indicates whether the bitmap is being traced.
        /// </summary>
        public bool TracingOngoing
        {
            get { return _tracingOngoing; }
            private set
            {
                if (Set(ref _tracingOngoing, value))
                    RaiseRequerySuggested();
            }
        }

        /// <summary>
        /// All traced polygons.
        /// </summary>
        public ObservableCollectionEx<Polygon> Polygons { get; }
        /// <summary>
        /// Subset of <see cref="Polygons"/> whose elements were marked with <see cref="SelectPolygonCommand"/> command.
        /// </summary>
        public ObservableCollectionEx<int> SelectedPolygonIndices { get; }


        private BitmapSource _bitmap;
        private double _areaThreshold;
        private double _colorThreshold;
        private bool _tracingOngoing;

        private BitmapTracer _tracer;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly object _cancellationLock = new object();


        public TracingViewModel()
        {
            // Make sure that default instances of Map/Vehicle editors exist.
            ServiceLocator.Current.GetInstance<MapEditorViewModel>();
            ServiceLocator.Current.GetInstance<VehicleEditorViewModel>();

            Polygons = new ObservableCollectionEx<Polygon>();
            SelectedPolygonIndices = new ObservableCollectionEx<int>();
        }

        private void StartNewTraceTask()
        {
            Monitor.Enter(_cancellationLock);
            _cancellationTokenSource?.Cancel();

            _cancellationTokenSource = new CancellationTokenSource();
            var token = _cancellationTokenSource.Token;

            TracingOngoing = true;
            new Task(() =>
            {
                var traceResult = _tracer.Trace(AreaThreshold, ColorThreshold);

                if (Monitor.TryEnter(_cancellationLock) && !token.IsCancellationRequested)
                {
                    TracingOngoing = false;
                    Polygons.ReplaceRange(traceResult);
                    Monitor.Exit(_cancellationLock);
                }
            }, token).Start();

            Monitor.Exit(_cancellationLock);
        }

        private void CancelTraceTask()
        {
            Monitor.Enter(_cancellationLock);
            _cancellationTokenSource?.Cancel();
            TracingOngoing = false;
            Monitor.Exit(_cancellationLock);
        }
    }
}
