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
            AlgorithmOption option = item as AlgorithmOption;

            if (option == null)
                return null;

            switch (option.Type)
            {
                case AlgorithmOption.ValueType.Integer:
                    return IntegerOptionTemplate;
                case AlgorithmOption.ValueType.Double:
                    return DoubleOptionTemplate;
                case AlgorithmOption.ValueType.Boolean:
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
