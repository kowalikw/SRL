using FirstFloor.ModernUI.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Data;
using SRL.Commons.Model;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for OptionsDialogViewModel.xaml
    /// </summary>
    public partial class OptionsDialogView : ModernDialog
    {



        public List<AlgorithmOption> Result => new List<AlgorithmOption>(_viewModel.Options);

        private OptionsDialogViewModel _viewModel;



        public OptionsDialogView(List<AlgorithmOption> options)
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
