using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;

namespace SRL.Commons.Model
{
    public class AlgorithmOption : ObservableObject, IDataErrorInfo
    {
        public enum ValueType
        {
            Integer,
            Double,
            Boolean
        }

        public ValueType Type { get; set; }

        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        public object MinValue { get; set; }
        public object MaxValue { get; set; }

        public Dictionary<Language, string> Names { get; set; }
        public Dictionary<Language, string> Tooltips { get; set; }

        private object _value;


        public string this[string columnName]
        {
            get
            {
                if (columnName == nameof(Value))
                {
                    if (Value == null)
                        return $"A value must be entered.";

                    if (Type == ValueType.Integer)
                    {
                        if (MinValue != null && (int)Value < (int)MinValue)
                            return $"The minimum value is {MinValue}."; //TODO localization

                        if (MaxValue != null && (int)Value > (int)MaxValue)
                            return $"The maximum value is {MaxValue}."; //TODO localization
                    }
                    if (Type == ValueType.Double)
                    {
                        if (MinValue != null && (double)Value < (double)MinValue)
                            return $"The minimum value is {MinValue}."; //TODO localization

                        if (MaxValue != null && (double)Value > (double)MaxValue)
                            return $"The maximum value is {MaxValue}."; //TODO localization
                    }
                }
                return null;
            }
        }

        public string Error
        {
            get
            {
                if (this[nameof(Value)] != null)
                    return "Invalid value."; //TODO localization

                return null;
            }
        }

        public bool IsValid => Error == null;
    }
}
