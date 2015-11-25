using System.Windows;
using SRL.Main.ViewModel;

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

            var viewModel = new LanguagePickerViewModel();
            viewModel.ClosingRequest += (sender, e) => this.Close();
            DataContext = viewModel;
        }
    }
}
