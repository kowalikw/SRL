using FirstFloor.ModernUI.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls;
using SRL.Commons.Model;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for OptionsDialogViewModel.xaml
    /// </summary>
    public partial class OptionsDialogView : ModernDialog
    {



        public List<Option> Result => new List<Option>(_viewModel.Options);

        private readonly OptionsDialogViewModel _viewModel;



        public OptionsDialogView(List<Option> options)
        {
            InitializeComponent();
            _viewModel = new OptionsDialogViewModel(options);
            DataContext = _viewModel;

            Buttons = new[] {OkButton, CancelButton};
        }

        private void OnError(object sender, ValidationErrorEventArgs e)
        {
            OkButton.IsEnabled = _viewModel.AreValuesValid;
        }
    }
}
