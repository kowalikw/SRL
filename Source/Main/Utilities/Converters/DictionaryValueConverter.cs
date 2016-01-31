using System;
using System.Collections;
using System.Globalization;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    /// <summary>
    /// Converts a dictionary and a key to corresponding value.
    /// </summary>
    internal class DictionaryValueConverter : IMultiValueConverter
    {
        /// <summary>
        /// Gets value by key from a dictionary.
        /// </summary>
        /// <param name="values">Two element array: [0] is an <see cref="IDictionary"/> and [1] a key.</param>
        /// <param name="targetType">Target type.</param>
        /// <param name="parameter">Parameter. Not used.</param>
        /// <param name="culture">Conversion culture. Not used.</param>
        /// <returns></returns>
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values != null && values.Length >= 2)
            {
                var dictionary = values[0] as IDictionary;
                var key = values[1];

                if (dictionary != null && key != null)
                {
                    return dictionary[key];
                }
            }
            return Binding.DoNothing;
        }

        /// <summary>
        /// Not implemented.
        /// </summary>
        /// <param name="value">n/a</param>
        /// <param name="targetTypes">n/a</param>
        /// <param name="parameter">n/a</param>
        /// <param name="culture">n/a</param>
        /// <returns>n/a</returns>
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
