using System.Windows;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for TracingView.xaml
    /// </summary>
    public partial class TracingView : Window
    {
        public TracingView()
        {
            InitializeComponent();

            var viewModel = new TracingViewModel();
            viewModel.ClosingRequest += (sender, e) => this.Close();
            DataContext = viewModel;
        }
    }
}
