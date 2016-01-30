using System;
using System.Globalization;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    /// <summary>
    /// Converter <see cref="BoolNegationConverter"/> converts value to its negation
    /// if value type is <see cref="bool"/>. Otherwise converter returns value.
    /// <see cref="BoolNegationConverter"/> implements <see cref="IValueConverter"/>.
    /// </summary>
    internal class BoolNegationConverter : IValueConverter
    {
        /// <summary>
        /// Converts value to its negation if value type is <see cref="bool"/>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter of conversion.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>Negation of value if its type is <see cref="bool"/>. Value otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

        /// <summary>
        /// /// Converts back negated value to non negated value if value type is <see cref="bool"/>.
        /// </summary>
        /// <param name="value">Value to convert back.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter of back conversion.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>Non negated value if its type is <see cref="bool"/>. Value otherwise.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool)
            {
                return !(bool)value;
            }
            return value;
        }

    }
}
