using System;
using System.Globalization;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    /// <summary>
    /// Converter converts <see cref="double"/> value to <see cref="string"/> if value is not null.
    /// Otherwise it returns empty string.
    /// <see cref="DoubleToStringConverter"/> implements <see cref="IValueConverter"/>.
    /// </summary>
    internal class DoubleToStringConverter : IValueConverter
    {
        /// <summary>
        /// Converts value from <see cref="double"/> to <see cref="string"/>.
        /// If value is null, converter returns empty string.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter of conversion.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>
        /// Converted double value to <see cref="string"/> type if value is not null. 
        /// Empty string otherwise.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            return ((double) value).ToString(culture);
        }

        /// <summary>
        /// Converts back <see cref="string"/> value to <see cref="double"/> if it is possible
        /// to parse <see cref="string"/>. Returns null otherwise.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter of conversion.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>
        /// Converted <see cref="string"/> to <see cref="double"/> if it is possible
        /// to parse value. Otherwise returns null.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double output;

            if (double.TryParse((string) value, NumberStyles.Float, culture, out output))
                return output;

            return null;
        }
    }
}