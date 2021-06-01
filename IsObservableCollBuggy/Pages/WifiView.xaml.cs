
using IsObservableCollBuggy.Models;
using Models.Interfaces;
using System;
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
            _wifiConnection = new WifiConnection(DependencyService.Get<IWifiConnectionReceiver>(), Navigation);
            BindingContext = _wifiConnection;
        }

        protected void ViewLifecycleEffect_OnLoaded(object sender, EventArgs e)
        {
            _wifiConnection.OnAttached();
        }

        protected void ViewLifecycleEffect_OnUnloaded(object sender, EventArgs e)
        {
            _wifiConnection.OnDettached();
        }
    }
}