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
        readonly string TAG = nameof(MainActivity);
        public const int REQUEST_LOCATION = 0;

        public static readonly string[] PERMISSIONS_LOCATION = {
            Manifest.Permission.AccessCoarseLocation,
            Manifest.Permission.AccessFineLocation
        };
        private const int REQUEST_READ_PHONE_STATE = 5;
        private readonly string[] PERMISSIONS_READ_PHONE_STATE = {
            Manifest.Permission.ReadPhoneState,
            "android.permission.READ_PRIVILEGED_PHONE_STATE"
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            //CrossCurrentActivity.Current.Init(this, savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            Xamarin.Forms.Forms.Init(this, savedInstanceState);

            BroadcastService.Init(this);

            LoadApplication(new App());
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == REQUEST_LOCATION)
            {
                Log.Info(TAG, "Received response for Location permission request.");
                // Check if the only required permission has been granted
                if (grantResults.Length == 2 && grantResults[0] == Permission.Granted)
                {
                    // Location permission has been granted, preview can be displayed
                    Log.Info(TAG, "Location permission has now been granted. Showing preview.");
                    Snackbar.Make(FindViewById(Android.Resource.Id.Content), Resource.String.permission_location_granted, Snackbar.LengthShort).Show();
                    MessagingCenter.Send<IAccessLocationPermission, bool>(this, "MainActivity.RequestLocationPermision", true);
                }
                else
                {
                    Log.Info(TAG, "Location permission was NOT granted.");
                    Snackbar.Make(FindViewById(Android.Resource.Id.Content), Resource.String.permission_location_not_granted, Snackbar.LengthShort).Show();
                    MessagingCenter.Send<IAccessLocationPermission, bool>(this, "MainActivity.RequestLocationPermision", false);
                }
            }
            else
            {
                MessagingCenter.Send(this, TAG + "." + "RequestLocationPermision", false);
                base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            }
        }
        protected override void OnStart()
        {
            base.OnStart();

            MessagingCenter.Subscribe<Diagnostics>(this, Diagnostics.DIAGNOSTIC_LOCATION_PERMISSION_REQUEST, (s) => ShowWifiTab());

            RequestPrivilegedPhoneStatePermission();
        }
        protected override void OnResume()
        {
            base.OnResume();
        }
        protected override void OnPause()
        {
            base.OnPause();
        }
        protected override void OnStop()
        {
            MessagingCenter.Unsubscribe<Diagnostics>(this, Diagnostics.DIAGNOSTIC_LOCATION_PERMISSION_REQUEST);
            base.OnStop();
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
        public void ShowWifiTab()
        {
            Log.Info(TAG, "Wifi Tab pressed. Checking permissions.");

            // Verify that all required location permissions have been granted.
            if (Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation) != (int)Permission.Granted
                || Android.Support.V4.Content.ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation) != (int)Permission.Granted)
            {
                // Location permissions have not been granted.
                Log.Info(TAG, "Location permissions has NOT been granted. Requesting permissions.");
                RequestLocationPermission();
            }
            else
            {
                // Location permissions have been granted.
                Log.Info(TAG, "Location permissions have already been granted. Displaying contact details.");
            }
        }
        void RequestLocationPermission()
        {
            if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation))
            {
                Log.Info(TAG, "Displaying Access Location permission rationale to provide additional context.");

                Snackbar.Make(FindViewById(Android.Resource.Id.Content),
                               Resource.String.permission_location_rationale,
                               Snackbar.LengthIndefinite)
                        .SetAction(Resource.String.permission_location_ok_button,
                                   new Action<Views.View>(delegate (Views.View obj)
                                   {
                                       ActivityCompat.RequestPermissions(this, PERMISSIONS_LOCATION, REQUEST_LOCATION);
                                   }
                        )
                ).Show();
            }
            else
            {
                ActivityCompat.RequestPermissions(this, PERMISSIONS_LOCATION, REQUEST_LOCATION);
            }
        }

        void RequestPrivilegedPhoneStatePermission()
        {
            ActivityCompat.RequestPermissions(this, PERMISSIONS_READ_PHONE_STATE, REQUEST_READ_PHONE_STATE);
        }
    }
}