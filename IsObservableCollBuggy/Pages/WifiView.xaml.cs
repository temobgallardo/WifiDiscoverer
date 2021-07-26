using IsObservableCollBuggy.Models;
using IsObservableCollBuggy.Models.Models;
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

        protected void ViewLifecycleEffect_OnLoaded(object sender, EventArgs e)
        {
            _wifiConnection.OnAttached();
            // TODO: comment this when launching from diagnostics page
            FocusWifiTab();
        }

        protected void ViewLifecycleEffect_OnUnloaded(object sender, EventArgs e)
        {
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
            var si = (Wifi)lv.SelectedItem;
            si.IsSelected = false;
            ((ListView)sender).SelectedItem = null;
        }
    }
}