using System;
using System.Collections.Generic;
using System.Windows;
using FirstFloor.ModernUI.Windows.Controls;
using Infralution.Localization.Wpf;
using Microsoft.Win32;
using SRL.Commons.Model;
using System.Globalization;
using System.Windows.Threading;

namespace SRL.Main.View.Dialogs
{
    internal class DialogService : IDialogService
    {
        private CultureInfo _culture;
        private Window _owner;

        public DialogService(Window window)
        {
            _owner = window;
            _culture = CultureManager.UICulture;
        }

        public void ShowDialog(DialogArgs dialogArgs)
        {
            switch (dialogArgs.DialogType)
            {
                case DialogArgs.Type.OpenFileDialog:
                    {
                        var args = (OpenFileDialogArgs)dialogArgs;
                        
                        var dialog = new OpenFileDialog();
                        dialog.Filter = args.Filter;
                        bool? dialogResult = null;

                        _owner.Dispatcher.Invoke(() => dialogResult = dialog.ShowDialog(_owner));
                        string filename = dialogResult == true ? dialog.FileName : null;

                        args.CloseCallback?.Invoke(dialogResult == true, filename);
                    }
                    break;
                case DialogArgs.Type.SaveFileDialog:
                    {
                        var args = (SaveFileDialogArgs)dialogArgs;

                        var dialog = new SaveFileDialog();
                        dialog.Filter = args.Filter;
                        bool? dialogResult = null;

                        _owner.Dispatcher.Invoke(() => dialogResult = dialog.ShowDialog(_owner));
                        string filename = dialogResult == true ? dialog.FileName : null;

                        args.CloseCallback?.Invoke(dialogResult == true, filename);
                    }
                    break;
                case DialogArgs.Type.OptionsDialog:
                    {
                        var args = (OptionsDialogArgs)dialogArgs;

                        var dialog = new OptionsDialogView(args.Options);
                        bool? dialogResult = null;

                        _owner.Dispatcher.Invoke(() => dialogResult = dialog.ShowDialog());

                        args.CloseCallback?.Invoke(dialogResult == true);
                    }
                    break;
                case DialogArgs.Type.MessageDialog:
                    {
                        var args = (MessageDialogArgs)dialogArgs;
                        _owner.Dispatcher.Invoke(() => ModernDialog.ShowMessage(args.Description, args.Title, MessageBoxButton.OK, _owner));
                        args.CloseCallback?.Invoke();
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
