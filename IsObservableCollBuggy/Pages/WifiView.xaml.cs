
using IsObservableCollBuggy.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiView : ContentView
    {
        readonly WifiConnection _wifiConnection;
        public WifiView()
        {
            InitializeComponent();
            _wifiConnection = new WifiConnection(null, Navigation);
            BindingContext = _wifiConnection;
        }
    }
}