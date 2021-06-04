using System.Windows.Input;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Controls
{
    public class EntryWithCustomKeyboard : Entry
    {
        public static readonly BindableProperty EnterCommandProperty = BindableProperty.Create(
            nameof(EnterCommand),
            typeof(ICommand),
            typeof(EntryWithCustomKeyboard),
            default(ICommand),
            BindingMode.OneWay
        );

        public ICommand EnterCommand
        {
            get => (ICommand)GetValue(EnterCommandProperty);
            set => SetValue(EnterCommandProperty, value);
        }

        public string ControlName
        {
            get => (string)GetValue(ControlNameProperty);
            set => SetValue(ControlNameProperty, value);
        }

        public static readonly BindableProperty ControlNameProperty =
            BindableProperty.Create(
                nameof(ControlName),
                typeof(string),
                typeof(EntryWithCustomKeyboard),
                null);
    }
}
