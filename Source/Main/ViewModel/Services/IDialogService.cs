using System;
using System.Collections.Generic;
using SRL.Commons.Model;

namespace SRL.Main.ViewModel.Services
{
    internal interface IDialogService
    {
        void ShowOpenFileDialog(string filter, Action<bool, string> closeCallback);
        void ShowSaveFileDialog(string filter, Action<bool, string> closeCallback);
        void ShowMessageDialog(string title, string message, Action closeCallback);
        void ShowOptionsDialog(List<Option> options, Action<bool> closeCallback);
    }
}
