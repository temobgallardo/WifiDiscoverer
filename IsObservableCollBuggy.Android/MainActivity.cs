using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using IsObservableCollBuggy.Droid.BroadcastReceivers;
using Android.Content;
using Android.Net.Wifi;

namespace IsObservableCollBuggy.Droid
{
    [Activity(Label = "IsObservableCollBuggy", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        readonly WifiConnectionReceiver _wifiReceiver = new WifiConnectionReceiver();

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnResume()
        {
            base.OnResume();

            var intentFilter = new IntentFilter();
            intentFilter.AddAction(WifiManager.ScanResultsAvailableAction);
            RegisterReceiver(_wifiReceiver, intentFilter);
        }
        protected override void OnPause()
        {
            UnregisterReceiver(_wifiReceiver);
            base.OnPause();
        }
    }
}