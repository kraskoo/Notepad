namespace Notepad
{
    using System.Windows.Input;

    public static class CustomCommands
    {
        public static readonly RoutedUICommand IncrementFontSize = new RoutedUICommand(
            "IncrementFontSize",
            "IncrementFontSize",
            typeof(CustomCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Add, ModifierKeys.Control)
            });

        public static readonly RoutedUICommand DecrementFontSize = new RoutedUICommand(
            "DecrementFontSize",
            "DecrementFontSize",
            typeof(CustomCommands),
            new InputGestureCollection
            {
                new KeyGesture(Key.Subtract, ModifierKeys.Control)
            });
    }
}