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
            }
        }

        public bool ConnectToAlreadyConfigured(int networkId)
        {
            return ConnectByNetworkId(networkId);
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

        public bool ConnectToWifi(Wifi wifi, bool isHidden)
        {
            if (!_wifiManager.IsWifiEnabled)
                return false;

            if (isHidden)
            {
                return ConnectToHidden(wifi);
            }

            var networkId = _wifiManager.AddNetwork(MapWifiToConfiguration(wifi, false));

            if (networkId <= 0) return false;

            return ConnectByNetworkId(networkId);
        }

        public bool ConnectToRememberedNetwork(Wifi wifi, bool isHidden)
        {
            var configuredNetworks = _wifiManager.ConfiguredNetworks;
            var configured = configuredNetworks.FirstOrDefault((w) => w.Ssid == $"\"{wifi.Ssid}\"");

            if (configured != null)
            {
                return ConnectToAlreadyConfigured(configured.NetworkId);
            }

            return false;
        }

        bool ConnectToHidden(Wifi wifi)
        {
            var configuration = MapWifiToConfiguration(wifi, true);



            return true;
        }

        bool ConnectByNetworkId(int networkId)
        {
            _wifiManager.Disconnect();
            _wifiManager.EnableNetwork(networkId, true);
            return _wifiManager.Reconnect();
        }

        WifiConfiguration MapWifiToConfiguration(Wifi wifi, bool isHidden)
        {
            WifiConfiguration configuration = new WifiConfiguration
            {
                Ssid = string.Format($"\"{wifi.Ssid}\""),
                PreSharedKey = string.Format($"\"{wifi.Password}\""),
            };

            if (isHidden)
            {
                configuration.HiddenSSID = true;
                configuration.Bssid = string.Format($"\"{wifi.Bssid}\"");
            }

            return configuration;
        }

        public bool SetWifiEnabled(bool enabled) => _wifiManager.SetWifiEnabled(enabled);
        

        public void StartScan() => _wifiManager.StartScan();
        
    }
}