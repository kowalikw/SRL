using System;
using FirstFloor.ModernUI.Windows.Controls;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using SRL.Commons.Model;
using SRL.Main.Utilities.Converters;
using SRL.Main.ViewModel;

namespace SRL.Main.View
{
    /// <summary>
    /// Interaction logic for OptionsDialogViewModel.xaml
    /// </summary>
    public partial class OptionsDialogView : ModernDialog
    {
        public List<AlgorithmOption> Result { get; private set; }

        private OptionsDialogViewModel _viewModel;

        public OptionsDialogView(List<AlgorithmOption> options)
        {
            InitializeComponent();
            _viewModel = new OptionsDialogViewModel(options);
            DataContext = _viewModel;


            // Define dialog buttons
            Buttons = new[] { OkButton, CancelButton };

            Binding okButtonBinding = new Binding(nameof(_viewModel.AreValuesValid));
            okButtonBinding.Source = _viewModel;
            okButtonBinding.Mode = BindingMode.OneWay;
            OkButton.SetBinding(IsEnabledProperty, okButtonBinding);

            // Create grid rows
            for (int i = 0; i < options.Count; i++)
                Grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(30) });

            for (int i = 0; i < options.Count; i++)
            {
                var opt = options[i];

                // Option name label
                Label label = new Label();
                label.Content = opt.Names[Settings.Default.Language];
                label.ToolTip = opt.Tooltips[Settings.Default.Language];

                label.Margin = new Thickness(0, 0, 30, 10);
                label.VerticalAlignment = VerticalAlignment.Top;

                label.SetValue(Grid.RowProperty, i);
                label.SetValue(Grid.ColumnProperty, 0);
                Grid.Children.Add(label);

                // Option proper
                Control optionControl;
                Binding optionControlBinding = new Binding(nameof(opt.Value));
                DependencyProperty bindingPropertyType;

                if (opt.Type == AlgorithmOption.ValueType.Boolean)
                {
                    optionControl = new CheckBox();
                    bindingPropertyType = CheckBox.IsCheckedProperty;
                    
                }
                else if (opt.Type == AlgorithmOption.ValueType.Integer)
                {
                    optionControl = new TextBox();
                    bindingPropertyType = TextBox.TextProperty;
                    optionControlBinding.Converter = new IntToStringConverter();
                }
                else if (opt.Type == AlgorithmOption.ValueType.Double)
                {
                    optionControl = new TextBox();
                    bindingPropertyType = TextBox.TextProperty;
                    optionControlBinding.Converter = new DoubleToStringConverter();
                }
                else
                    throw new ArgumentException();

                optionControl.SourceUpdated += (o, e) => _viewModel.RaiseOptionValuesChanged();

                optionControlBinding.Source = opt;
                optionControlBinding.ValidatesOnDataErrors = true;
                optionControlBinding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
                optionControlBinding.Mode = BindingMode.OneWayToSource;
                optionControlBinding.NotifyOnSourceUpdated = true;
                optionControl.SetBinding(bindingPropertyType, optionControlBinding);

                optionControl.Width = 80;
                optionControl.VerticalAlignment = VerticalAlignment.Top;

                optionControl.SetValue(Grid.RowProperty, i);
                optionControl.SetValue(Grid.ColumnProperty, 1);
                Grid.Children.Add(optionControl);
            }



        }

    }
}
