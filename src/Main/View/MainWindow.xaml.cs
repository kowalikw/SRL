using System.Windows;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string language;

        public MainWindow(string language)
        {
            InitializeComponent();

            this.language = language;
        }

        private void addVehicle_Click(object sender, RoutedEventArgs e)
        {
            Window editorWindow = new VehicleEditorWindow();
            editorWindow.Show();
        }

        private void addMap_Click(object sender, RoutedEventArgs e)
        {
            Window editorWindow = new MapEditorWindow();
            editorWindow.Show();
        }
    }
}
