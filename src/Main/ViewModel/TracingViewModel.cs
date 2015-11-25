using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SRL.Main.Annotations;
using SRL.Main.Utilities;
using SRL.Model.Model;
using SRL.Model.Tracing;
using System.Windows;
using SRL.Main.View;

namespace SRL.Main.ViewModel
{
    internal class TracingViewModel : CloseableViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand LoadBitmapCommand { get; }
        public ICommand AcceptVectorCommand { get; }

        public bool IsCorrect { get; private set; }
        public int AreaThreshold
        {
            get { return _areaThreshold; }
            set
            {
                _areaThreshold = value;

                if (BitmapToTrace != null)
                    Trace();

                // TODO - retrace in timer?

                OnPropertyChanged();
            }
        }
        public int ColorThreshold
        {
            get { return _colorThreshold; }
            set
            {
                _colorThreshold = value;

                if (BitmapToTrace != null)
                    Trace();

                // TODO - retrace in timer?

                OnPropertyChanged();
            }
        }


        public BitmapSource BitmapToTrace
        {
            get { return _bitmapToTrace;}
            private set
            {
                _bitmapToTrace = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<Polygon> TracedPolygons { get; private set; }


        private BitmapSource _bitmapToTrace;
        private int _areaThreshold;
        private int _colorThreshold;

        private BitmapTracer _tracer;


        public TracingViewModel()
        {
            AreaThreshold = 50;
            ColorThreshold = 50;

            TracedPolygons = new ObservableCollection<Polygon>();

            LoadBitmapCommand = new RelayCommand(o =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "BMP files (*.bmp, *.png, *.jpg, *.jpeg)|*.bmp; *.png; *.jpg; *.jpeg;";

                if (dialog.ShowDialog() == true)
                {
                    BitmapToTrace = new BitmapImage(new Uri(dialog.FileName));
                    _tracer = new BitmapTracer(dialog.FileName);

                    Trace();
                }
            });

            AcceptVectorCommand = new RelayCommand(o =>
            {
                //TODO

                // MAP
                /*List<Polygon> obstacles = new List<Polygon>();
                foreach (var polygon in TracedPolygons)
                    obstacles.Add(polygon);
                var map = new Map(512, 512, obstacles);
                Window window = new MapEditorView(map);
                window.Show();*/

                // VEHICLE
                var vehicle = new Vehicle(TracedPolygons[0], null, 0);
                Window window = new VehicleEditorView(vehicle);
                window.Show();

                OnClosingRequest();
            },
            c =>
            {
                return true;
                /*if (TracedPolygons.Count == 0)
                    return false;

                foreach (var polygon in TracedPolygons)
                    if (!polygon.IsCorrect())
                        return false;

                return true;*/
            });
        }

        private void Trace()
        {
            TracedPolygons.Clear();
            var traceOutput = _tracer.Trace(AreaThreshold, ColorThreshold);
            foreach (var polygon in traceOutput)
                TracedPolygons.Add(polygon);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
