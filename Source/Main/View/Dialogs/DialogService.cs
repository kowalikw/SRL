using System;
using System.Collections.Generic;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using Infralution.Localization.Wpf;
using Microsoft.Win32;
using SRL.Commons.Model;
using System.Globalization;

namespace SRL.Main.View.Dialogs
{
    internal class DialogService : IDialogService
    {
        private CultureInfo _culture;

        public DialogService()
        {
            _culture = CultureManager.UICulture;
        }

        public void ShowMessageDialog(string title, string message, Action closeCallback)
        {
            ModernDialog.ShowMessage(message, title, MessageBoxButton.OK);
        }

        public void ShowOptionsDialog(List<Option> options, Action<bool> closeCallback)
        {
            var dialog = new OptionsDialogView(options);
            closeCallback(dialog.ShowDialog() == true);
        }

        public void ShowOpenFileDialog(string filter, out string filename, Action<bool> closeCallback)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = filter;

            bool dialogResult = dialog.ShowDialog() == true;
            filename = dialogResult ? dialog.FileName : null;

            closeCallback(dialogResult);
        }

        public void ShowSaveFileDialog(string filter, out string filename, Action<bool> closeCallback)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = filter;
            
            bool dialogResult = dialog.ShowDialog() == true;
            filename = dialogResult ? dialog.FileName : null;

            closeCallback(dialogResult);
        }


    }
}
