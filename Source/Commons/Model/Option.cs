using System;
using System.Collections.Generic;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using SRL.Commons.Localization;
using SRL.Commons.Model.Base;

namespace SRL.Commons.Model
{
    /// <summary>
    /// Model class that represents a single <see cref="IAlgorithm"/> setting.
    /// </summary>
    public class Option : ObservableObject, IDataErrorInfo, ICloneable
    {
        /// <summary>
        /// Possible option types.
        /// </summary>
        public enum ValueType
        {
            [Description("Integer")]
            Integer,

            [Description("Double")]
            Double,

            [Description("Boolean")]
            Boolean
        }

        /// <summary>
        /// Option key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Type of the option.
        /// </summary>
        public ValueType Type { get; }

        /// <summary>
        /// Current option value. Must adhere to constraints specified by <see cref="MinValue"/> and <see cref="MaxValue"/> properties.
        /// </summary>
        /// <remarks>Boxed type must agree with <see cref="Type"/> property.</remarks>
        public object Value
        {
            get { return _value; }
            set { Set(ref _value, value); }
        }

        /// <summary>
        /// Lower <see cref="Value"/> bound (inclusive).
        /// </summary>
        /// <remarks>Boxed type must agree with <see cref="Type"/> property.</remarks>
        public object MinValue
        {
            get { return _minValue; }
            set { Set(ref _minValue, value); }
        }

        /// <summary>
        /// Upper <see cref="Value"/> bound (inclusive).
        /// </summary>
        /// <remarks>Boxed type must agree with <see cref="Type"/> property.</remarks>
        public object MaxValue
        {
            get { return _maxValue; }
            set { Set(ref _maxValue, value); }
        }

        /// <summary>
        /// Collection of user-friendly names of this option.
        /// </summary>
        public Dictionary<Language, string> Names { get; }
        /// <summary>
        /// Collection of user-friendly descriptions of this option.
        /// </summary>
        public Dictionary<Language, string> Tooltips { get; }

        private object _value;
        private object _minValue;
        private object _maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Option"/> class.
        /// </summary>
        /// <param name="type">Type of the option.</param>
        /// <param name="key">Option key.</param>
        public Option(ValueType type, string key)
        {
            Key = key;
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
                        return OptionErrors.ResourceManager.GetString("valueInvalid");

                    if (Type == ValueType.Integer)
                    {
                        if (MinValue != null && (int)Value < (int)MinValue)
                            return string.Format(OptionErrors.ResourceManager.GetString("valueTooSmall"), MinValue);

                        if (MaxValue != null && (int)Value > (int)MaxValue)
                            return string.Format(OptionErrors.ResourceManager.GetString("valueTooBig"), MaxValue);
                    }
                    if (Type == ValueType.Double)
                    {
                        if (MinValue != null && (double)Value < (double)MinValue)
                            return string.Format(OptionErrors.ResourceManager.GetString("valueTooSmall"), MinValue);

                        if (MaxValue != null && (double)Value > (double)MaxValue)
                            return string.Format(OptionErrors.ResourceManager.GetString("valueTooBig"), MaxValue);
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
                    return OptionErrors.ResourceManager.GetString("valueInvalid");

                return null;
            }
        }

        /// <summary>
        /// Returns a boolean value that indicates whether the current <see cref="Value"/> passes validation.
        /// </summary>
        public bool IsValid => Error == null;

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
