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

        private BitmapTracer _tracer;
        private BitmapSource _bitmapToTrace;
        private int _areaThreshold;
        private int _colorThresHold;

        public BitmapSource BitmapToTrace
        {
            get { return _bitmapToTrace; }
            private set
            {
                _bitmapToTrace = value;
                OnPropertyChanged();
            }
        }
        public int AreaThreshold
        {
            get { return _areaThreshold; }
            set
            {
                _areaThreshold = value;

                if (_tracer != null)
                {
                    var traceOutput = _tracer.Trace(_areaThreshold, _colorThresHold);
                    foreach (var polygon in traceOutput)
                        TracedPolygons.Add(polygon);
                }
                OnPropertyChanged();
            }
        }
        public int ColorThreshold
        {
            get { return _colorThresHold; }
            set
            {
                _colorThresHold = value;

                if (_tracer != null)
                {                    
                    var traceOutput = _tracer.Trace(_areaThreshold, _colorThresHold);
                    //if (TracedPolygons.Count > 0)
                    //    TracedPolygons[0].Vertices.Clear();
                    TracedPolygons.Clear();
                    //TracedPolygons = new ObservableCollection<Polygon>();
                    OnPropertyChanged();
                    foreach (var polygon in traceOutput)
                        TracedPolygons.Add(polygon);
                }
                OnPropertyChanged();
            }
        }

        
        public ObservableCollection<Polygon> TracedPolygons { get; private set; }


        public TracingViewModel()
        {
            AreaThreshold = 50;
            ColorThreshold = 50;

            TracedPolygons = new ObservableCollection<Polygon>();

            LoadBitmapCommand = new RelayCommand(o =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "Image files (*.bmp, *.jpg, *.jpeg, *.gif, *.png) | *.bmp; *.jpg; *.jpeg; *.gif; *.png";

                if (dialog.ShowDialog() == true)
                {
                    BitmapToTrace = new BitmapImage(new Uri(dialog.FileName));
                    _tracer = new BitmapTracer(dialog.FileName);

                    

                    

                    //TODO re-trace on each area/color threshold change
                }
            });

            AcceptVectorCommand = new RelayCommand(o =>
            {
                //TODO

                TracedPolygons.Clear();
                OnPropertyChanged();
            },
            c =>
            {
                //TODO
                return true;
            });
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
