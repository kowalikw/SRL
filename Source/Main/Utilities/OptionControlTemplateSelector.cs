using System;
using System.Windows;
using System.Windows.Controls;
using SRL.Commons.Model;

namespace SRL.Main.Utilities
{
    /// <summary>
    /// Selector that chooses control template of an <see cref="Option"/> based on its type.
    /// </summary>
    internal class OptionControlTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Returns <see cref="DataTemplate"/> based on <see cref="Option.ValueType"/>.
        /// </summary>
        /// <param name="item"><see cref="Option.ValueType"/> value.</param>
        /// <param name="container">Ignored.</param>
        /// <returns>One of <see cref="DataTemplate"/> properties.</returns>
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

        /// <summary>
        /// <see cref="DataTemplate"/> of <see cref="int"/> based options.
        /// </summary>
        public DataTemplate IntegerOptionTemplate { get; set; }
        /// <summary>
        /// <see cref="DataTemplate"/> of <see cref="double"/> based options.
        /// </summary>
        public DataTemplate DoubleOptionTemplate { get; set; }
        /// <summary>
        /// <see cref="DataTemplate"/> of <see cref="bool"/> based options.
        /// </summary>
        public DataTemplate BooleanOptionTemplate { get; set; }
    }
}
