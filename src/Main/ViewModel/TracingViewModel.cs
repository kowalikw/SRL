using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Win32;
using SRL.Main.Annotations;
using SRL.Main.Utilities;
using SRL.Model.Model;
using SRL.Model.Tracing;

namespace SRL.Main.ViewModel
{
    internal class TracingViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand LoadBitmapCommand { get; }
        public ICommand AcceptVectorCommand { get; }


        public int AreaThreshold
        {
            get { return _areaThreshold; }
            set
            {
                _areaThreshold = value;
                OnPropertyChanged();
            }
        }
        public int ColorThreshold
        {
            get { return _colorThreshold; }
            set
            {
                _colorThreshold = value;
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
               // dialog.Filter = "BMP files (*.bmp)|*.bmp";

                if (dialog.ShowDialog() == true)
                {
                    BitmapToTrace = new BitmapImage(new Uri(dialog.FileName));
                    _tracer = new BitmapTracer(dialog.FileName);

                    TracedPolygons.Clear();
                    var traceOutput = _tracer.Trace(AreaThreshold, ColorThreshold);
                    foreach (var polygon in traceOutput)
                        TracedPolygons.Add(polygon);

                    //TODO re-trace on each area/color threshold change
                }
            });

            AcceptVectorCommand = new RelayCommand(o =>
            {
                //TODO
            },
            c =>
            {
                //TODO
                return false;
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
