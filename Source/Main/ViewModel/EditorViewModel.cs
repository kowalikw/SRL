using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;

namespace SRL.Main.ViewModel
{
    public abstract class EditorViewModel<T> : ViewModelBase 
        where T : IXmlSerializable
    {
        public RelayCommand SaveCommand { get; }
        public RelayCommand LoadCommand { get; }

        public abstract RelayCommand ResetCommand { get; }
        public abstract RelayCommand BackCommand { get; }

        public abstract bool IsModelValid { get; }
        /// <summary>
        /// Returns created model.
        /// </summary>
        /// <returns>Model or null depending on whether IsModelValid property returns true.</returns>
        public abstract T GetModel();
    }
}
