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

        public string Filter { get; set; }
        public Action<bool, string> CloseCallback { get; set; }
    }


    internal class SaveFileDialogArgs : DialogArgs
    {
        public override Type DialogType => Type.SaveFileDialog;

        public string Filter { get; set; }
        public Action<bool, string> CloseCallback { get; set; }
    }

    internal class OptionsDialogArgs : DialogArgs
    {
        public override Type DialogType => Type.OptionsDialog;

        public List<Option> Options { get; set; }
        public Action<bool> CloseCallback { get; set; }
    }

    internal class MessageDialogArgs : DialogArgs
    {
        public override Type DialogType => Type.MessageDialog;

        public string Title { get; set; }
        public string Description { get; set; }
        public Action CloseCallback { get; set; }
    }
}
