using System;
using System.Windows.Data;

namespace SRL.Main.Utilities.Converters
{
    class FontSizeConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is double && parameter != null)
            {
                return int.Parse(value.ToString()) / double.Parse(parameter.ToString());
            }

            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new Exception("The method or operation is not implemented.");
        }
    }
}
