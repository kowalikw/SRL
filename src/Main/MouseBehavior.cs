using System;
using System.Windows;
using System.Windows.Input;

namespace SRL.Main
{
    public static class MouseBehavior
    {
        public static readonly DependencyProperty MouseUpCommandProperty =
            DependencyProperty.RegisterAttached(
                "MouseUpCommand",
                typeof (ICommand),
                typeof (MouseBehavior),
                new FrameworkPropertyMetadata(MouseUpCommandChanged));

        public static ICommand GetMouseUpCommand(UIElement uiElement)
        {
            return (ICommand)uiElement.GetValue(MouseUpCommandProperty);
        }

        public static void SetMouseUpCommand(UIElement uiElement, ICommand value)
        {
            uiElement.SetValue(MouseUpCommandProperty, value);
        }

        private static void MouseUpCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement element = (FrameworkElement) d;

            element.MouseUp += OnMouseUp;
        }

        private static void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            FrameworkElement element = (FrameworkElement) sender;

            ICommand command = GetMouseUpCommand(element);

            if (command.CanExecute(e))
                command.Execute(e);
        }
    }
}
