using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using SRL.Main.Messages;

namespace SRL.Main.ViewModel
{
    public abstract class EditorViewModel<T> : ViewModelBase 
        where T : IXmlSerializable
    {
        public RelayCommand SaveCommand
        {
            get
            {
                if (_saveCommand == null)
                {
                    _saveCommand = new RelayCommand(() =>
                    {
                        var msg = new SaveFileDialogMessage();
                        //TODO
                        Messenger.Default.Send(msg);
                        //TODO
                    }, 
                    () => IsModelValid);
                }
                return _saveCommand;
            }
        }
        public RelayCommand LoadCommand
        {
            get
            {
                if (_loadCommand == null)
                {
                    _loadCommand = new RelayCommand(() =>
                    {
                        var msg = new OpenFileDialogMessage();
                        //TODO
                        Messenger.Default.Send(msg);
                        //TODO
                    });
                }
                return _loadCommand;
            }
        }

        public abstract RelayCommand ResetCommand { get; }

        private RelayCommand _saveCommand;
        private RelayCommand _loadCommand;

        
        protected abstract bool IsModelValid { get; }
        /// <summary>
        /// Returns created model.
        /// </summary>
        /// <returns>Model or null depending on whether IsModelValid property returns true.</returns>
        protected abstract T GetModel();
        /// <summary>
        /// Sets model in the editor.
        /// </summary>
        /// <param name="model">Model to set; can be incomplete or even erroneous.</param>
        /// <returns>True if it's possible to set <paramref name="model"/>; false otherwise.</returns>
        protected abstract bool SetModel(T model);
    }
}
