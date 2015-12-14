using System;
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
using System.Collections.Generic;

namespace SRL.Main.ViewModel
{
    internal class TracingViewModel : CloseableViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand LoadBitmapCommand { get; }
        public ICommand TraceCommand { get; }
        public ICommand CreateMapCommand { get; }
        public ICommand CreateVehicleCommand { get; }

        public int AreaThreshold { get; set; }
        public int ColorThreshold { get; set; }


        public BitmapSource BitmapToTrace
        {
            get { return _bitmapToTrace; }
            private set
            {
                _bitmapToTrace = value;
                OnPropertyChanged();
            }
        }
        public Polygon CurrentShape
        {
            get { return _currentShape; }
            set
            {
                _currentShape = value;

                if (CreateVehicleCommand != null)
                    ((RelayCommand)CreateVehicleCommand).OnCanExecuteChanged();
            }
        }
        public List<Polygon> CurrentModel { get; private set; }

        private BitmapSource _bitmapToTrace;
        private Polygon _currentShape;

        private BitmapTracer _tracer;


        public TracingViewModel()
        {
            AreaThreshold = 50;
            ColorThreshold = 50;

            CurrentModel = new List<Polygon>();
            CurrentShape = new Polygon();

            LoadBitmapCommand = new RelayCommand(o =>
            {
                var dialog = new OpenFileDialog();
                dialog.Filter = "BMP files (*.bmp, *.png, *.jpg, *.jpeg)|*.bmp; *.png; *.jpg; *.jpeg;";

                if (dialog.ShowDialog() == true)
                {
                    BitmapToTrace = new BitmapImage(new Uri(dialog.FileName));
                    _tracer = new BitmapTracer(dialog.FileName);

                    ((RelayCommand)TraceCommand).OnCanExecuteChanged();
                }
            });

            TraceCommand = new RelayCommand(o => Trace(),
            c =>
            {
                return _bitmapToTrace != null;
            });

            CreateMapCommand = new RelayCommand(o =>
            {
                List<Polygon> obstacles = new List<Polygon>();
                foreach (var polygon in CurrentModel)
                    obstacles.Add(polygon);
                var map = new Map(512, 512, obstacles);
                Window window = new MapEditorView(map);
                window.Show();

                OnClosingRequest();
            },
            c =>
            {
                if (CurrentModel.Count == 0)
                    return false;

                foreach (var polygon in CurrentModel)
                    if (!polygon.IsCorrect())
                        return false;

                return true;
            });

            CreateVehicleCommand = new RelayCommand(o =>
            {
                var vehicle = new Vehicle(CurrentShape, null, 0);
                Window window = new VehicleEditorView(vehicle);
                window.Show();

                OnClosingRequest();
            },
            c =>
            {
                return CurrentShape.IsCorrect();
            });
        }

        private void Trace()
        {
            CurrentModel.Clear();
            BackgroundWorker bw = new BackgroundWorker();
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
            /*var traceOutput = _tracer.Trace(AreaThreshold, ColorThreshold);
            foreach (var polygon in traceOutput)
                CurrentModel.Add(polygon);*/

            ((RelayCommand)CreateMapCommand).OnCanExecuteChanged();
            ((RelayCommand)CreateVehicleCommand).OnCanExecuteChanged();
        }

       
        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            var traceOutput = _tracer.Trace(AreaThreshold, ColorThreshold);
            foreach (var polygon in traceOutput)
                CurrentModel.Add(polygon);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
