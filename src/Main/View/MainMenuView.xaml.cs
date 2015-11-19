using System.Windows;
using SRL.Main.Model;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : Window
    {
        public MainMenuView(Language lang)
        {
            InitializeComponent();
            DataContext = new MainMenuViewModel();

            //TODO do something with the lang setting. Do not pass it to the view-model!
        }

        private void addVehicle_Click(object sender, RoutedEventArgs e)
        {
            Window editorWindow = new VehicleEditorView();
            editorWindow.Show();
        }

        private void addMap_Click(object sender, RoutedEventArgs e)
        {
            Window editorWindow = new MapEditorView();
            editorWindow.Show();
        }
    }
}
