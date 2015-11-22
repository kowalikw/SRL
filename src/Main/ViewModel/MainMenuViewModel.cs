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
        }
    }
}
