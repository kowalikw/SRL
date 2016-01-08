using SRL.Main.View.Dialogs;

namespace SRL.Main.Messages
{
    internal class ShowDialogMessage
    {
        public DialogArgs Args { get; }

        public ShowDialogMessage(DialogArgs args)
        {
            Args = args;
        }
    }
    
}
