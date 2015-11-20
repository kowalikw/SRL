using System.Windows;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : Window
    {
        public MainMenuView(Language.Language lang)
        {
            InitializeComponent();
            DataContext = new MainMenuViewModel();

            //TODO do something with the lang setting. Do not pass it to the view-model!
        }
    }
}
