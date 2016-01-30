using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SRL.Commons.Model;

namespace SRL.Main.ViewModel
{
    /// <summary>
    /// View-model class that contains non-UI logic for the options dialog.
    /// </summary>
    /// <remarks>This view-model shan't be managed by the ViewModelLocator, as every OptionsDialogView instantiates a new OptionsDialogViewModel object.</remarks>
    public class OptionsDialogViewModel : Base.ViewModel
    {
        /// <summary>
        /// Options managed by the dialog.
        /// </summary>
        public ObservableCollection<Option> Options { get; }

        /// <summary>
        /// Checks validity of all values entered via the dialog. 
        /// </summary>
        public bool AreValuesValid
        {
            get { return Options.All(option => option.IsValid); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsDialogViewModel"/> class.
        /// </summary>
        /// <param name="options">Options set by this dialog.</param>
        public OptionsDialogViewModel(List<Option> options)
        {
            Options = new ObservableCollection<Option>(options);
        }



    }
}
