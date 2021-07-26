using Android.App;
using Android.Content;
using Android.Net.Wifi;
using IsObservableCollBuggy.Droid.BroadcastReceivers;
using Models.Interfaces;
using Xamarin.Forms;

[assembly: Dependency(typeof(BroadcastService))]
namespace IsObservableCollBuggy.Droid.BroadcastReceivers
{
    public class BroadcastService : IBrodcastSevice
    {
        public static Activity Activity;
        private WifiConnectionReceiver _wifiReceiver;
        private bool _alreadyRegister;

        public static void Init(Activity activity) => Activity = activity;

        public void Register(IBroadcastReceieverCallback broadcastReceieverCallback)
        {
            if (_alreadyRegister) return;

            if (_wifiReceiver == null)
            {
                _wifiReceiver = new WifiConnectionReceiver();
            }

            var intentFilter = new IntentFilter();
            intentFilter.AddAction(WifiManager.ScanResultsAvailableAction);
            intentFilter.AddAction(WifiManager.NetworkStateChangedAction);
            _wifiReceiver.Callback = broadcastReceieverCallback;

            Activity?.RegisterReceiver(_wifiReceiver, intentFilter);
            _alreadyRegister = true;
        }

        public void UnRegister()
        {
            if (_wifiReceiver == null) return;

            Activity.UnregisterReceiver(_wifiReceiver);
            _wifiReceiver.Dispose();
            _wifiReceiver = null;
            _alreadyRegister = false;
        }
    }
}