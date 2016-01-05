using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;

namespace SRL.Main.ViewModel
{
    public class OptionsDialogViewModel : ViewModelBase
    {

        // This view-model shan't be managed by the ViewModelLocator, as every OptionsDialogView
        // instantiates a new OptionsDialogViewModel object.

        public List<AlgorithmOption> Options { get; }

        public bool AreValuesValid
        {
            get { return _areValuesValid; }
            set { Set(ref _areValuesValid, value); }
        }

        //   public bool AreValuesValid => Options.All(option => option.Error == string.Empty);

        private bool _areValuesValid;

        public OptionsDialogViewModel(List<AlgorithmOption> options)
        {
            Options = options;
        }

        public void RaiseOptionValuesChanged()
        {
            AreValuesValid = Options.All(option => option.Error == string.Empty);
        }


    }
}
