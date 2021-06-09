using Android.Widget;
using IsObservableCollBuggy.Droid.Services;
using Models.Interfaces;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(ToastMessage))]
namespace IsObservableCollBuggy.Droid.Services
{
    public class ToastMessage : IToastMessage
    {
        public void LongAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Long).Show();
        }

        public void ShortAlert(string message)
        {
            Toast.MakeText(Application.Context, message, ToastLength.Short).Show();
        }
    }
}