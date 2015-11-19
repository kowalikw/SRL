using System.Windows;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : Window
    {
        private string language;

        public MainMenuView(string language)
        {
            InitializeComponent();

            this.language = language;
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
