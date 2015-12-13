using System.Windows;
using System.Windows.Input;
using SRL.Main.Utilities;
using SRL.Main.View;

namespace SRL.Main.ViewModel
{
    internal class MainMenuViewModel
    {
        public ICommand VehicleEditorCommand { get; }
        public ICommand MapEditorCommand { get; }
        public ICommand TracerCommand { get; }
        public ICommand VizualizationCommand { get; }

        public MainMenuViewModel()
        {
            VehicleEditorCommand = new RelayCommand(o =>
            {
                Window window = new VehicleEditorView();
                window.Show();
            });

            MapEditorCommand = new RelayCommand(o =>
            {
                Window window = new MapEditorView();
                window.Show();
            });

            TracerCommand = new RelayCommand(o =>
            {
                Window window = new TracingView();
                window.Show();
            });

            VizualizationCommand = new RelayCommand(o =>
            {
                Window window = new VisualizationModuleView();
                window.Show();
            });
        }
    }
}
