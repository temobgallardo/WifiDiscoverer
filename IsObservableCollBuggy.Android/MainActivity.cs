using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using IsObservableCollBuggy.Droid.BroadcastReceivers;
using Android.Content;
using Android.Net.Wifi;
using Android;
using Android.Support.V4.App;
using Android.Util;
using Android.Support.Design.Widget;
using Xamarin.Forms;
using Views = Android.Views;
using IsObservableCollBuggy.Models;
using Models.Interfaces;

namespace IsObservableCollBuggy.Droid
{
    [Activity(Label = "IsObservableCollBuggy", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : Xamarin.Forms.Platform.Android.FormsAppCompatActivity, IAccessLocationPermission
    {
        readonly WifiConnectionReceiver _wifiReceiver = new WifiConnectionReceiver();
        readonly string TAG = nameof(MainActivity);
        readonly int REQUEST_LOCATION = 0;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if(requestCode == REQUEST_LOCATION)
            {
                Log.Info(TAG, "Received response for Location permission request.");
                // Check if the only required permission has been granted
                if (grantResults.Length == 1 && grantResults[0] == Permission.Granted)
                {
                    // Camera permission has been granted, preview can be displayed
                    Log.Info(TAG, "Location permission has now been granted. Showing preview.");
                    Snackbar.Make(new CoordinatorLayout(this), Resource.String.permission_location_granted, Snackbar.LengthShort).Show();
                    MessagingCenter.Send(this, TAG + "." + "RequestLocationPermision", true);
                }
                else
                {
                    Log.Info(TAG, "Location permission was NOT granted.");
                    Snackbar.Make(new CoordinatorLayout(this), Resource.String.permission_location_not_granted, Snackbar.LengthShort).Show();
                    MessagingCenter.Send(this, TAG + "." + "RequestLocationPermision", false);
                }                
            }
            else
            {
                MessagingCenter.Send(this, TAG + "." + "RequestLocationPermision", false);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }
        protected override void OnResume()
        {
            base.OnResume();

            var intentFilter = new IntentFilter();
            intentFilter.AddAction(WifiManager.ScanResultsAvailableAction);
            RegisterReceiver(_wifiReceiver, intentFilter);

            MessagingCenter.Subscribe<Diagnostics>(this, Diagnostics.DIAGNOSTIC_LOCATION_PERMISSION_REQUEST, (s) => ShowWifiTab());
        }
        protected override void OnPause()
        {
            UnregisterReceiver(_wifiReceiver);
            MessagingCenter.Unsubscribe<Diagnostics>(this, Diagnostics.DIAGNOSTIC_LOCATION_PERMISSION_REQUEST);
            base.OnPause();
        }
        public void ShowWifiTab()
        {
            Log.Info(TAG, "Wifi Tab pressed. Checking permissions.");

            // Verify that all required contact permissions have been granted.
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.ReadContacts) != (int)Permission.Granted
                || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteContacts) != (int)Permission.Granted)
            {
                // Contacts permissions have not been granted.
                Log.Info(TAG, "Location permissions has NOT been granted. Requesting permissions.");
                RequestLocationPermission();
            }
            else
            {
                // Contact permissions have been granted. Show the contacts fragment.
                Log.Info(TAG, "Location permissions have already been granted. Displaying contact details.");
            }
        }
        void RequestLocationPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                Log.Info(TAG, "Displaying Access Location permission rationale to provide additional context.");

                var requiredPermissions = new String[] { Manifest.Permission.AccessFineLocation };
                Snackbar.Make(new CoordinatorLayout(this),
                               Resource.String.permission_location_rationale,
                               Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.permission_location_ok_button,
                                   new Action<Views.View>(delegate (Views.View obj)
                                   {
                                       ActivityCompat.RequestPermissions(this, requiredPermissions, REQUEST_LOCATION);
                                   }
                        )
                ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, new String[] { Manifest.Permission.AccessFineLocation }, REQUEST_LOCATION);
            }
        }
    }
}