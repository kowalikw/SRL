using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SRL.Main
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
