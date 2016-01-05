using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using GalaSoft.MvvmLight;
using SRL.Commons.Model;

namespace SRL.Main.ViewModel
{
    public class OptionsDialogViewModel : Base.ViewModel
    {
        // This view-model shan't be managed by the ViewModelLocator, as every OptionsDialogView
        // instantiates a new OptionsDialogViewModel object.


        public ObservableCollection<Option> Options { get; }

        public bool AreValuesValid
        {
            get { return Options.All(option => option.IsValid); }
        }


        public OptionsDialogViewModel(List<Option> options)
        {
            Options = new ObservableCollection<Option>(options);
        }



    }
}
