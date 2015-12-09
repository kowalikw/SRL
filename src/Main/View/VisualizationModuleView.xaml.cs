using System.Windows;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for VisualizationModuleView.xaml
    /// </summary>
    public partial class VisualizationModuleView : Window
    {
        public VisualizationModuleView()
        {
            InitializeComponent();
            DataContext = new VisualizationModuleViewModel();
        }
    }
}
