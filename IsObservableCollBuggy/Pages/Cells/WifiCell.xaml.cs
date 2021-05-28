using System.Windows.Input;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Pages.Cells
{
    public partial class WifiCell : ViewCell
    {
        public ICommand ForgetCommand
        {
            get => GetValue(ForgetCommandProperty) as ICommand;
            set => SetValue(ForgetCommandProperty, value);
        }
        public static readonly BindableProperty ForgetCommandProperty = BindableProperty.Create(nameof(ForgetCommand), typeof(ICommand), typeof(WifiCell), null);

        public WifiCell()
        {
            InitializeComponent();
        }
    }
}