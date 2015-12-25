using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRL.Main.Messages
{
    internal class GoToPageMessage
    {
        public Type ViewType { get; set; }

        public GoToPageMessage(Type viewType)
        {
            ViewType = viewType;
        }
    }
}
