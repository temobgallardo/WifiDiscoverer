using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Elements
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CustomSwitch : Frame

    {
        public bool IsToggledLocal { get => (bool)GetValue(IsToggledLocalProperty); set => SetValue(IsToggledLocalProperty, value); }
        public static readonly BindableProperty IsToggledLocalProperty = BindableProperty.Create(nameof(IsToggledLocal), typeof(bool), typeof(CustomSwitch), false, BindingMode.TwoWay, propertyChanged: IsToggledPropertyChanged);
        private static void IsToggledPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue) return;

            var control = (CustomSwitch)bindable;
            control.@switch.IsToggled = (bool)newValue;
        }

        public string SwitchText { get => (string)GetValue(SwitchTextProperty); set => SetValue(SwitchTextProperty, value); }
        public static readonly BindableProperty SwitchTextProperty = BindableProperty.Create(nameof(SwitchText), typeof(string), typeof(CustomSwitch), string.Empty, BindingMode.OneWay, propertyChanged: SwitchTextPropertyChanged);
        private static void SwitchTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue) return;

            var control = (CustomSwitch)bindable;
            control.label.Text = (string)newValue;
        }

        public Color TextColor { get => (Color)GetValue(TextColorProperty); set => SetValue(TextColorProperty, value); }
        public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(CustomSwitch), Color.White, propertyChanged: TextColorPropertyChanged);
        private static void TextColorPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (oldValue == newValue) return;

            var control = (CustomSwitch)bindable;
            control.label.TextColor = (Color)newValue;
        }

        public CustomSwitch()
        {
            InitializeComponent();
        }

        private void TapGestureRecognizer_Tapped(object sender, System.EventArgs e)
        {
            IsToggledLocal = !IsToggledLocal;
        }
    }
}