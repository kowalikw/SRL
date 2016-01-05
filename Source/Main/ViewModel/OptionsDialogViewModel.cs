using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using SRL.Commons.Model;
using SRL.Main.View;

namespace SRL.Main.ViewModel
{
    public class OptionsDialogViewModel : ViewModelBase
    {


        // This view-model shan't be managed by the ViewModelLocator, as every OptionsDialogView
        // instantiates a new OptionsDialogViewModel object.


        public ObservableCollection<AlgorithmOption> Options { get; }

        public bool AreValuesValid
        {
            get { return Options.All(option => option.IsValid); }
        }


        public OptionsDialogViewModel(List<AlgorithmOption> options)
        {
            Options = new ObservableCollection<AlgorithmOption>(options);
        }



    }
}
