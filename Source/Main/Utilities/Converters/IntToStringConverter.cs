using System;
using System.Globalization;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    internal class IntToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            return ((int)value).ToString(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int output;

            if (int.TryParse((string)value, NumberStyles.Integer, culture, out output))
                return output;

            return null;
        }
    }
}
