using System;
using System.Globalization;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    internal class DoubleToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            return ((double) value).ToString(culture);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double output;

            if (double.TryParse((string) value, NumberStyles.None, culture, out output))
                return output;

            return null;
        }
    }
}