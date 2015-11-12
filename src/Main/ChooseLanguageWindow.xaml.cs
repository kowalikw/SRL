using System.Windows;

namespace SRL.Main
{
    /// <summary>
    /// Interaction logic for ChooseLanguageWindow.xaml
    /// </summary>
    public partial class ChooseLanguageWindow : Window
    {
        public ChooseLanguageWindow()
        {
            InitializeComponent();
        }

        private void btnPolish_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainWindow("pl");
            mainWindow.Show();
            this.Close();
        }

        private void btnEnglish_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainWindow("en");
            mainWindow.Show();
            this.Close();
        }
    }
}
