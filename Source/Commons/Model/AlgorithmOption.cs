using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SRL.Commons.Model
{
    

    public class AlgorithmOption
    {
        public enum ValueType
        {
            Integer,
            Double,
            Boolean
        }

        public ValueType Type { get; set; }

        public object Value { get; set; }
        public object MinValue { get; set; }
        public object MaxValue { get; set; }

        public Dictionary<Language, string> Names;
        public Dictionary<Language, string> Tooltips;


    }
}
