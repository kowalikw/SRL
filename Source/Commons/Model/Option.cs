﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using GalaSoft.MvvmLight;

namespace SRL.Commons.Model
{
    public class Option : ObservableObject, IDataErrorInfo, ICloneable
    {
        public enum ValueType
        {
            Integer,
            Double,
            Boolean
        }

        public ValueType Type { get; }

        public object Value
        {
            get { return _value; }
            set
            {
                Set(ref _value, value);
            }
        }

        public object MinValue
        {
            get { return _minValue; }
            set
            {
                Set(ref _minValue, value);
            }
        }

        public object MaxValue
        {
            get { return _maxValue; }
            set
            {
                Set(ref _maxValue, value);
            }
        }

        public Dictionary<Language, string> Names { get; }
        public Dictionary<Language, string> Tooltips { get; }

        private object _value;
        private object _minValue;
        private object _maxValue;


        public Option(ValueType type)
        {
            Type = type;

            Names = new Dictionary<Language, string>();
            Tooltips = new Dictionary<Language, string>();
        }


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

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
