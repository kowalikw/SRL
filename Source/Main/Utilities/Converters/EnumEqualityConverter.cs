using System;
using System.Globalization;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    /// <summary>
    /// Converter converts two enum values to <see cref="bool"/> value.
    /// It returns true if enums are equal, false otherwise.
    /// </summary>
    internal class EnumEqualityConverter : IValueConverter
    {
        /// <summary>
        /// Converts two enum values to <see cref="bool"/> value.
        /// It returns true if enums are equal, false otherwise.
        /// </summary>
        /// <param name="value">First enum value.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Second enum value.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>True if enums are equal, false otherwise.</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        /// <summary>
        /// Converts back equality of two enums to second enum parameter.
        /// Returns enum if value is true, do nothing otherwise.
        /// </summary>
        /// <param name="value">Value of equality.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Second enum.</param>
        /// <param name="culture"><see cref="CultureInfo"/> object.</param>
        /// <returns>Second enum if equality is true, do notjing otherwise.</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(true) ? parameter : Binding.DoNothing;
        }
    }
}
