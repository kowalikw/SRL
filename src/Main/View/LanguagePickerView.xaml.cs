using System.Windows;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for LanguagePickerView.xaml
    /// </summary>
    public partial class LanguagePickerView : Window
    {
        public LanguagePickerView()
        {
            InitializeComponent();
        }

        private void btnPolish_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainMenuView("pl");
            mainWindow.Show();
            this.Close();
        }

        private void btnEnglish_Click(object sender, RoutedEventArgs e)
        {
            Window mainWindow = new MainMenuView("en");
            mainWindow.Show();
            this.Close();
        }
    }
}
