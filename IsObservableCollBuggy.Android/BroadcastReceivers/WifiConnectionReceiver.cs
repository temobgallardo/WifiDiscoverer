using Android.Content;
using Android.Net.Wifi;
using IsObservableCollBuggy.Droid.BroadcastReceivers;
using IsObservableCollBuggy.Models.Models;
using Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

[assembly: Dependency(typeof(WifiConnectionReceiver))]
namespace IsObservableCollBuggy.Droid.BroadcastReceivers
{
    [BroadcastReceiver(Enabled = false, Exported = false)]
    public class WifiConnectionReceiver : BroadcastReceiver, IWifiConnectionReceiver
    {
        static readonly string TAG = nameof(WifiConnectionReceiver);
        readonly string _wifiConnectionReceiverMessage = TAG;

        readonly WifiManager _wifiManager;

        IList<ScanResult> _scanResults;
        IList<ScanResult> ScanResults
        {
            set
            {
                if (value != null) _scanResults = value;
            }
            get => _scanResults;
        }
        public string WifiConnectionReceiverMessage { get => _wifiConnectionReceiverMessage; }
        public bool IsWifiEnabled { get => _wifiManager.IsWifiEnabled; }
        public IList<Wifi> Wifis { get => ParseScanResultToWifi(_wifiManager.ScanResults); }

        public WifiConnectionReceiver()
        {
            _wifiManager = Android.App.Application.Context.GetSystemService(Context.WifiService) as WifiManager;
            ScanResults = new List<ScanResult>();
        }

        public override void OnReceive(Context context, Intent intent)
        {
            if (string.IsNullOrEmpty(intent.Action)) return;

            switch (intent.Action)
            {
                case WifiManager.ScanResultsAvailableAction:
                    ScanResultsAvailable(intent.GetBooleanExtra(WifiManager.ExtraResultsUpdated, false));
                    break;
                case WifiManager.NetworkStateChangedAction:
                    var configuration = intent.GetParcelableExtra(WifiManager.ExtraNetworkInfo) as WifiConfiguration;
                    if (configuration == null) return;

                    bool a = ConnectToAlreadyConfigured(configuration.NetworkId);
                    break;
            }
        }

        public bool ConnectToAlreadyConfigured(int networkId)
        {
            return _wifiManager.EnableNetwork(networkId, true);
        }

        void ScanResultsAvailable(bool success)
        {
            if (success)
            {
                ScanSuccess();
            }
            else
            {
                ScanFailure();
            }
        }

        IList<Wifi> ParseScanResultToWifi(IList<ScanResult> scans)
        {
            return scans.Select((scan) =>
            {
                return new Wifi
                {
                    Bssid = scan.Bssid,
                    Capabilities = scan.Capabilities,
                    CenterFreq0 = scan.CenterFreq0,
                    CenterFreq1 = scan.CenterFreq1,
                    ChannelWidth = scan.ChannelWidth,
                    Frequency = scan.Frequency,
                    Level = scan.Level,
                    Ssid = scan.Ssid,
                    Timestamp = scan.Timestamp
                };
            }).ToList();
        }

        public IList<Wifi> ScanFailure()
        {
            var scanResults = _wifiManager.ScanResults;

            if (ScanResults.Equals(scanResults)) return Wifis;

            return Wifis;
        }

        public IList<Wifi> ScanSuccess()
        {
            ScanResults = _wifiManager.ScanResults;

            if (ScanResults.Count == 0) return new List<Wifi>();

            return ParseScanResultToWifi(ScanResults);
        }

        public bool ConnectToWifi(Wifi wifi, string password)
        {
            if (!_wifiManager.IsWifiEnabled)
                return false;

            var configuredNetworks = _wifiManager.ConfiguredNetworks;
            var isAlreadyConfigured = configuredNetworks.Any((w) => w.Ssid == string.Format($"\"{0}\"", wifi.Ssid));

            if (isAlreadyConfigured)
            {
                // Connect to the already configured network;
                WifiConfiguration configured = configuredNetworks.First((c) => c.Ssid == wifi.Ssid);

                return _wifiManager.EnableNetwork(configured.NetworkId, true);
            }

            WifiConfiguration configuration = new WifiConfiguration
            {
                Ssid = string.Format($"\"{wifi.Ssid}\""),
                PreSharedKey = string.Format($"\"{password}\"")
            };

            _wifiManager.AddNetwork(configuration);
            return true;
        }

        public bool SetWifiEnabled(bool enabled)
        {
            return _wifiManager.SetWifiEnabled(enabled);
        }

        public void StartScan()
        {
            _wifiManager.StartScan();
        }
    }
}