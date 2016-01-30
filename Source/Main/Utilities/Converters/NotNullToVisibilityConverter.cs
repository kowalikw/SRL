using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    /// <summary>
    /// Converter converts value to <see cref="Visibility.Visible"/> if value is not null.
    /// Otherwise it returns <see cref="Visibility.Hidden"/>.
    /// </summary>
    internal class NotNullToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts value to <see cref="Visibility.Visible"/> if value is not null.
        /// Otherwise converter converts value to <see cref="Visibility.Hidden"/>.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter of conversion.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>
        /// <see cref="Visibility.Visible"/> if value is not null. <see cref="Visibility.Hidden"/> otherwise.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return Visibility.Hidden;
            else
                return Visibility.Visible;
        }
        
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
