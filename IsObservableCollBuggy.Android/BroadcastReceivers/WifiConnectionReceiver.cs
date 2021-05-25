using Android.Content;
using Android.Net.Wifi;
using IsObservableCollBuggy.Droid.BroadcastReceivers;
using IsObservableCollBuggy.Models.Models;
using Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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

                    break;
            }
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

        public bool ConnectToWifi(Wifi wifi)
        {
            if (!_wifiManager.IsWifiEnabled)
                return false;

            //if (ConnectToRememberedNetwork(wifi)) return true;

            int networkId = _wifiManager.ConnectionInfo.NetworkId;
            _wifiManager.RemoveNetwork(networkId);
            var conf = MapWifiToConfiguration(wifi, true);
            var networkId2 = _wifiManager.AddNetwork(conf);
            return ConnectByNetworkId(networkId2);
        }

        public bool ConnectToRememberedNetwork(Wifi wifi)
        {
            var configuredNetworks = _wifiManager.ConfiguredNetworks;
            var configured = configuredNetworks.FirstOrDefault((w) => w.Bssid == $"\"{wifi.Bssid}\"" || w.Ssid == $"\"{wifi.Ssid}\"");

            if (configured != null)
            {
                if (configured.NetworkId < 0) return false;

                return ConnectToAlreadyConfigured(configured.NetworkId);
            }

            return false;
        }

        public bool ConnectToAlreadyConfigured(int networkId) => ConnectByNetworkId(networkId);

        bool ConnectByNetworkId(int networkId)
        {
            if (networkId < 0) return false;

            _wifiManager.RemoveNetwork(_wifiManager.ConnectionInfo.NetworkId);
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
                StatusField = WifiStatus.Enabled
            };

            return SetupProtocolUsed(wifi, configuration);
        }

        WifiConfiguration SetupProtocolUsed(Wifi wifi, WifiConfiguration conf)
        {
            var passWithQuotes = $"\"{wifi.Password}\"";
            if (wifi.Capabilities.ToUpper().Contains("WEP"))
            {
                conf.AllowedKeyManagement.Set((int)KeyManagementType.None);
                conf.AllowedProtocols.Set((int)ProtocolType.Rsn);
                conf.AllowedProtocols.Set((int)ProtocolType.Wpa);
                conf.AllowedAuthAlgorithms.Set((int)AuthAlgorithmType.Open);
                conf.AllowedAuthAlgorithms.Set((int)AuthAlgorithmType.Shared);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Ccmp);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Tkip);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Wep40);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Wep104);

                var regex = new Regex("^[0-9a-fA-F]+$");
                conf.WepKeys[0] = regex.IsMatch(wifi.Password) ? wifi.Password : passWithQuotes;
                conf.WepTxKeyIndex = 0;
            }
            else if (wifi.Capabilities.ToUpper().Contains("WPA"))
            {
                conf.AllowedProtocols.Set((int)ProtocolType.Rsn);
                conf.AllowedProtocols.Set((int)ProtocolType.Wpa);
                conf.AllowedKeyManagement.Set((int)KeyManagementType.WpaPsk);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Ccmp);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Tkip);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Wep40);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Wep104);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Ccmp);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Tkip);

                conf.PreSharedKey = passWithQuotes;
            }
            else
            {
                conf.AllowedKeyManagement.Set((int)KeyManagementType.None);
                conf.AllowedProtocols.Set((int)ProtocolType.Rsn);
                conf.AllowedProtocols.Set((int)ProtocolType.Wpa);
                conf.AllowedAuthAlgorithms.Clear();
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Ccmp);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Tkip);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Wep40);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Wep104);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Ccmp);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Tkip);
            }

            return conf;
        }

        public bool SetWifiEnabled(bool enabled) => _wifiManager.SetWifiEnabled(enabled);


        public void StartScan() => _wifiManager.StartScan();

    }
}