using System;
using System.Collections.Generic;
using SRL.Commons.Model;

namespace SRL.Main.View.Dialogs
{
    internal interface IDialogService
    {

        void ShowMessageDialog(string title, string message, Action closeCallback);
        void ShowOptionsDialog(List<Option> options, Action<bool> closeCallback);
        void ShowOpenFileDialog(string filter, out string filename, Action<bool> closeCallback);
        void ShowSaveFileDialog(string filter, out string filename, Action<bool> closeCallback);
    }
}
