using System;
using System.Windows;
using System.Windows.Controls;
using SRL.Commons.Model;

namespace SRL.Main.Utilities
{
    internal class OptionControlTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            Option option = item as Option;

            if (option == null)
                return null;

            switch (option.Type)
            {
                case Option.ValueType.Integer:
                    return IntegerOptionTemplate;
                case Option.ValueType.Double:
                    return DoubleOptionTemplate;
                case Option.ValueType.Boolean:
                    return BooleanOptionTemplate;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public DataTemplate IntegerOptionTemplate { get; set; }
        public DataTemplate DoubleOptionTemplate { get; set; }
        public DataTemplate BooleanOptionTemplate { get; set; }
    }
}
