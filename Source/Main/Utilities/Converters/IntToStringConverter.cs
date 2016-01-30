using System;
using System.Globalization;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    /// <summary>
    /// Converter converts type of value from <see cref="int"/> to <see cref="string"/> if value is not null.
    /// Otherwise it returns empty string.
    /// <see cref="IntToStringConverter"/> implements <see cref="IValueConverter"/>.
    /// </summary>
    internal class IntToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts value from <see cref="int"/> to <see cref="string"/>.
        /// If value is null, converter returns empty string.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter of conversion.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>
        /// Converted <see cref="int"/> value to <see cref="string"/> if value is not null. 
        /// Empty string otherwise.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            return ((int)value).ToString(culture);
        }

        /// <summary>
        /// Converts back <see cref="string"/> value to <see cref="int"/> if it is possible
        /// to parse <see cref="string"/>. Returns null otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter of conversion.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>
        /// Converted <see cref="string"/> to <see cref="int"/> if it is possible
        /// to parse value. Otherwise returns null.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int output;

            if (int.TryParse((string)value, NumberStyles.Integer, culture, out output))
                return output;

            return null;
        }
    }
}
