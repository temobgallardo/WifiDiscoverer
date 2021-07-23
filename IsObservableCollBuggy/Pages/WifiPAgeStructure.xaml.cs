using IsObservableCollBuggy.Models;
using Models.Interfaces;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace IsObservableCollBuggy.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiPageStructure : ContentPage
    {
        public const string LOCATION_PERMISSION_REQUEST = "LOCATION_PERMISSION_REQUEST";

        public WifiPageStructure()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            //FocusWifiTab(true);
        }

        private void FocusWifiTab(bool isSelected)
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
    }
}