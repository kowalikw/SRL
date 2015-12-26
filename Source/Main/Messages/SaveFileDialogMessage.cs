using System;
using System.Collections.Generic;

namespace SRL.Main.Messages
{
    internal class SaveFileDialogMessage
    {
        /// <summary>
        /// Action perfomed after the user picks a filename and confirms the file save.
        /// </summary>
        /// <remarks>Action's parameter is null when dialog is cancelled.</remarks>
        public Action<string> FilenameCallback { get; set; }

        /// <summary>
        /// Filter string that determines what types of files are displayed.
        /// </summary>
        public string Filter { get; set; }
    }
}
