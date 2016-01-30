using System;
using System.Collections.Generic;
using SRL.Commons.Model;

namespace SRL.Main.ViewModel.Services
{
    /// <summary>
    /// Contains methods, properties, and events to support dialog creation.
    /// </summary>
    internal interface IDialogService
    {
        /// <summary>
        /// Opens up a dialog that lets the user specify a filename to save a file as.
        /// </summary>
        /// <param name="filter">Filter string that determines what types of files are displayed in the dialog.</param>
        /// <param name="closeCallback">Action called on dialog closing.</param>
        void ShowOpenFileDialog(string filter, Action<bool, string> closeCallback);
        /// <summary>
        /// Opens up a dialog that lets the user select a file to open.
        /// </summary>
        /// <param name="filter">Filter string that determines what types of files are displayed in the dialog.</param>
        /// <param name="closeCallback">Action called on dialog closing.</param>
        void ShowSaveFileDialog(string filter, Action<bool, string> closeCallback);
        /// <summary>
        /// Opens up a dialog that contains a message.
        /// </summary>
        /// <param name="title">Dialog window title.</param>
        /// <param name="message">Dialog description.</param>
        /// <param name="closeCallback">Action called on dialog closing.</param>
        void ShowMessageDialog(string title, string message, Action closeCallback);
        /// <summary>
        /// Opens up a dialog with a list of modifiable settings.
        /// </summary>
        /// <param name="options">Settings to display.</param>
        /// <param name="closeCallback">Action called on dialog closing.</param>
        void ShowOptionsDialog(List<Option> options, Action<bool> closeCallback);
    }
}
