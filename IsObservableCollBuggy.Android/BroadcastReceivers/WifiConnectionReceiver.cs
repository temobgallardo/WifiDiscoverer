using Android.Content;
using Android.Net.Wifi;
using IsObservableCollBuggy.Droid.BroadcastReceivers;
using IsObservableCollBuggy.Models.Models;
using Models.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Forms;

[assembly: Dependency(typeof(WifiConnectionReceiver))]
namespace IsObservableCollBuggy.Droid.BroadcastReceivers
{
    // TODO: Researcg if WifiManager can live on its own to get inline with Separation of Concern Principle
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
        string _deviceMacAddress;
        public string DeviceMacAddress { get => GetMacAddres(); }

        string GetMacAddres()
        {
            if (!string.IsNullOrEmpty(_deviceMacAddress) && _deviceMacAddress != "02:00:00:00:00:00") return _deviceMacAddress;

            foreach(var i in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.Name != "wlan0") continue;

                StringBuilder macBuilder = new StringBuilder();

                int j = 0;
                foreach(var c in i.GetPhysicalAddress().ToString())
                {
                    if (j >= 2)
                    {
                        macBuilder.Append(":");
                        j = 0;
                    }

                    macBuilder.Append(c);
                    j++;
                }

                return _deviceMacAddress = macBuilder.ToString();
            }

            return string.Empty;
        }

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

        public bool Connect(Wifi wifi)
        {
            if (wifi == null) return false;

            if (!_wifiManager.IsWifiEnabled)
                return false;

            if (string.IsNullOrEmpty(wifi.Password)) return false;

            var conf = MapWifiToConfiguration(wifi);
            var networkId = _wifiManager.AddNetwork(conf);
            return ConnectToAlreadyConfigured(networkId);
        }

        public bool ConnectToRemembered(Wifi wifi)
        {
            if (wifi == null) return false;

            var current = AlreadyConfigured(wifi);
            if (current == null) return false;
            if (current.NetworkId < 0) return false;

            return ConnectToAlreadyConfigured(current.NetworkId);
        }

        public bool AlreadyConnected(Wifi wifi)
        {
            if (wifi == null) return false;

            var isCurrentSsid = !string.IsNullOrEmpty(wifi.Ssid) && _wifiManager.ConnectionInfo.SSID == $"\"{wifi.Ssid}\"";

            return isCurrentSsid && _wifiManager.ConnectionInfo.NetworkId > -1;
        }

        public WifiConfiguration AlreadyConfigured(Wifi wifi)
        {

            if(string.IsNullOrEmpty(wifi.Bssid)) return _wifiManager.ConfiguredNetworks.FirstOrDefault((w) => { return w.Ssid == $"\"{wifi.Ssid}\""; });

            return _wifiManager.ConfiguredNetworks.FirstOrDefault((w) => { return w.Bssid == $"\"{wifi.Bssid}\"" || w.Ssid == $"\"{wifi.Ssid}\""; });
        }

        public bool ConnectToAlreadyConfigured(int networkId) { 
            if (networkId < 0) return false;

            var isDisconnected = _wifiManager.Disconnect();
            var isDisabled = _wifiManager.ConnectionInfo.NetworkId < 0 || _wifiManager.DisableNetwork(_wifiManager.ConnectionInfo.NetworkId);
            var isEnabled = _wifiManager.EnableNetwork(networkId, true);
            var isReconnected = _wifiManager.Reconnect();
            return isDisconnected && isDisabled && isEnabled && isReconnected;
        }

        WifiConfiguration MapWifiToConfiguration(Wifi wifi)
        {
            WifiConfiguration configuration = new WifiConfiguration
            {
                Ssid = string.Format($"\"{wifi.Ssid}\""),
                PreSharedKey = string.Format($"\"{wifi.Password}\""),
                StatusField = WifiStatus.Enabled,
                HiddenSSID = wifi.IsHidden
            };

            return configuration; //SetupProtocolUsed(wifi, configuration);
        }

        WifiConfiguration SetupProtocolUsed(Wifi wifi, WifiConfiguration conf)
        {
            var passWithQuotes = $"\"{wifi.Password}\"";
            if (wifi.Capabilities.ToUpper().Contains("WEP"))
            {
                conf.AllowedKeyManagement.Set((int)KeyManagementType.None);
                conf.AllowedAuthAlgorithms.Set((int)AuthAlgorithmType.Open);
                conf.AllowedAuthAlgorithms.Set((int)AuthAlgorithmType.Shared);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Ccmp);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Tkip);

                var regex = new Regex("^[0-9a-fA-F]+$");
                conf.WepKeys[0] = regex.IsMatch(wifi.Password) ? wifi.Password : passWithQuotes;
                conf.WepTxKeyIndex = 0;
            }
            else if (wifi.Capabilities.ToUpper().Contains("WPA"))
            {
                conf.AllowedProtocols.Set((int)ProtocolType.Rsn);
                conf.AllowedKeyManagement.Set((int)KeyManagementType.WpaPsk);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Ccmp);
                conf.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Tkip);
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
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Ccmp);
                conf.AllowedGroupCiphers.Set((int)GroupCipherType.Tkip);
            }

            return conf;
        }

        public bool Disconnect() => _wifiManager.Disconnect();

        public bool Forget(Wifi wifi)
        {
            if (wifi is null) return false;

            WifiConfiguration conf = AlreadyConfigured(wifi);

            if (conf is null) return false;

            return _wifiManager.DisableNetwork(conf.NetworkId);
        }

        public bool SetWifiEnabled(bool enabled) => _wifiManager.SetWifiEnabled(enabled);

        public void StartScan() => _wifiManager.StartScan();
    }
}