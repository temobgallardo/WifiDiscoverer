using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Models.Interfaces;
using Plugin.CurrentActivity;
using System;

namespace IsObservableCollBuggy.Droid.NativeImplementations
{
    public class SnackBarImpl : ISnackBar
    {
        public void Show(string message)
        {
            Android.Views.View view = CrossCurrentActivity.Current.Activity.FindViewById(Android.Resource.Id.Content);
            Snackbar.Make(view,
                           Resource.String.permission_location_rationale,
                           Snackbar.LengthIndefinite)
                    .SetAction(Resource.String.permission_location_ok_button,
                               new Action<Android.Views.View>(delegate (Android.Views.View obj)
                               {
                                   ActivityCompat.RequestPermissions(CrossCurrentActivity.Current.Activity, MainActivity.PERMISSIONS_LOCATION, MainActivity.REQUEST_LOCATION);
                               }
                    )
            ).Show();
        }
    }
}