using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRL.Commons
{
    interface ISaveable
    {
        int Width { get; set; }
        int Height { get; set; }
        string Type { get; set; }
    }
}
