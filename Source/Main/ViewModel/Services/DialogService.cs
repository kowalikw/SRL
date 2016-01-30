using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using Infralution.Localization.Wpf;
using Microsoft.Win32;
using SRL.Commons.Model;
using SRL.Main.View;

namespace SRL.Main.ViewModel.Services
{
    /// <summary>
    /// Simple implementation of <see cref="IDialogService"/> interface.
    /// </summary>
    internal class DialogService : IDialogService
    {
        private CultureInfo _culture;
        private Window _owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="DialogService"/> class.
        /// </summary>
        /// <param name="window">Dialog owner.</param>
        public DialogService(Window window)
        {
            _owner = window;
            _culture = CultureManager.UICulture;
        }

        public void ShowOpenFileDialog(string filter, Action<bool, string> closeCallback)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = filter;
            bool? dialogResult = null;

            _owner.Dispatcher.Invoke(() => dialogResult = dialog.ShowDialog(_owner));
            var filename = dialogResult == true ? dialog.FileName : null;

            closeCallback?.Invoke(dialogResult == true, filename);
        }

        public void ShowSaveFileDialog(string filter, Action<bool, string> closeCallback)
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = filter;
            bool? dialogResult = null;

            _owner.Dispatcher.Invoke(() => dialogResult = dialog.ShowDialog(_owner));
            var filename = dialogResult == true ? dialog.FileName : null;

            closeCallback?.Invoke(dialogResult == true, filename);
        }

        public void ShowMessageDialog(string title, string message, Action closeCallback)
        {
            _owner.Dispatcher.Invoke(() => ModernDialog.ShowMessage(message, title, MessageBoxButton.OK, _owner));
            closeCallback?.Invoke();
        }

        public void ShowOptionsDialog(List<Option> options, Action<bool> closeCallback)
        {
            var dialog = new OptionsDialogView(options);
            bool? dialogResult = null;

            _owner.Dispatcher.Invoke(() => dialogResult = dialog.ShowDialog());

            closeCallback?.Invoke(dialogResult == true);
        }
    }
}
