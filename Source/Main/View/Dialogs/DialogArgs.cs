using System;
using System.Collections.Generic;
using SRL.Commons.Model;

namespace SRL.Main.View.Dialogs
{
    internal abstract class DialogArgs
    {
        public enum Type
        {
            OpenFileDialog,
            SaveFileDialog,
            OptionsDialog,
            MessageDialog
        }

        public abstract Type DialogType { get; }
    }

    internal class OpenFileDialogArgs : DialogArgs
    {
        public override Type DialogType => Type.OpenFileDialog;

        public string Filter;
        public Action<bool, string> CloseCallback;
    }


    internal class SaveFileDialogArgs : DialogArgs
    {
        public override Type DialogType => Type.SaveFileDialog;

        public string Filter;
        public Action<bool, string> CloseCallback;
    }

    internal class OptionsDialogArgs : DialogArgs
    {
        public override Type DialogType => Type.OptionsDialog;

        public List<Option> Options;
        public Action<bool> CloseCallback;
    }

    internal class MessageDialogArgs : DialogArgs
    {
        public override Type DialogType => Type.MessageDialog;

        public string Title;
        public string Description;
        public Action CloseCallback;
    }
}
