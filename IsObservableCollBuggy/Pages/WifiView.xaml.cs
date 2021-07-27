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
        private readonly WifiConnection _wifiConnection;

        public const string LOCATION_PERMISSION_REQUEST = "LOCATION_PERMISSION_REQUEST";

        public WifiView()
        {
            InitializeComponent();
            _wifiConnection = new WifiConnection(DependencyService.Get<IWifiConnectionReceiver>(), DependencyService.Get<IToastMessage>(), DependencyService.Get<IBrodcastSevice>());
            BindingContext = _wifiConnection;
        }
        private void FocusEntryOnConnectToNetwork()
        {
            Password.Focus();
        }

        private void FocusEntryOnAddNetwork()
        {
            HiddenNetworkSsid.Focus();
        }

        protected void ViewLifecycleEffect_OnLoaded(object sender, EventArgs e)
        {
            _wifiConnection.OnAttached();
            _wifiConnection.RaiseOnAddNetworkVisible += FocusEntryOnAddNetwork;
            _wifiConnection.RaiseOnConnectToWifiVisible += FocusEntryOnConnectToNetwork;
            // TODO: comment this when launching from diagnostics page
            FocusWifiTab();
        }

        protected void ViewLifecycleEffect_OnUnloaded(object sender, EventArgs e)
        {
            _wifiConnection.RaiseOnAddNetworkVisible -= FocusEntryOnAddNetwork;
            _wifiConnection.RaiseOnConnectToWifiVisible -= FocusEntryOnConnectToNetwork;
            _wifiConnection.OnDettached();
        }


        private void FocusWifiTab(bool isSelected = false)
        {
            MessagingCenter.Send(this, LOCATION_PERMISSION_REQUEST);
            MessagingCenter.Subscribe<IAccessLocationPermission, bool>(this, "MainActivity.RequestLocationPermision", (s, a) => SetIsWifiTabVisible(a));
        }

        private void SetIsWifiTabVisible(bool isPermissionGranted)
        {
            if (isPermissionGranted)
            {
                MessagingCenter.Unsubscribe<IAccessLocationPermission, bool>(this, "MainActivity.RequestLocationPermision");
            }
        }

        private void NetworkListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item == null) return;

            var lv = (ListView)sender;

            //((Wifi)e.Item).State = "=)"; This set the value in the cell

            //var items = (ObservableCollection<Wifi>)lv.ItemsSource;
            //var current = (Wifi)e.Item; 
            //items.Select((w) => w.State == WifiStates.Connected.ToString() && w.Ssid != current.Ssid ? w.State = "+)": w.State = "=("); this doesnt update the listview

            lv.SelectedItem = null;
        }
    }
}