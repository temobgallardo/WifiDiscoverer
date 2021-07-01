﻿using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.Util;
using IsObservableCollBuggy.Droid.BroadcastReceivers;
using IsObservableCollBuggy.Models.Models;
using Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(WifiConnectionReceiver))]
namespace IsObservableCollBuggy.Droid.BroadcastReceivers
{
    // TODO: Research if WifiManager can live on its own to get inline with Separation of Concern Principle
    [BroadcastReceiver(Enabled = false, Exported = false)]
    public class WifiConnectionReceiver : BroadcastReceiver, IWifiConnectionReceiver
    {
        static readonly string TAG = nameof(WifiConnectionReceiver);
        readonly string _wifiConnectionReceiverMessage = TAG;

        readonly WifiManager _wifiManager;
        NetworkInfo _networkInfo { get; set; }
        SupplicantState _supplicantState;
        Wifi _currentWifi;
        Wifi CurrentWifi { get => _currentWifi; set => _currentWifi = value; }
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

            foreach (var i in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (i.Name != "wlan0") continue;

                StringBuilder macBuilder = new StringBuilder();

                int j = 0;
                foreach (var c in i.GetPhysicalAddress().ToString())
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

        public Wifi ConnectedWifi { get => ConnectionInfoToWifi(_wifiManager.ConnectionInfo); }
        public IList<Wifi> Wifis { get => ParseScanResultToWifi(_wifiManager.ScanResults); }

        public event EventHandler<NetworkConnectedEventArgs> RaiseNetworkConnected;

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
                case WifiManager.SupplicantConnectionChangeAction:
                    var isConnected = intent.GetBooleanExtra(WifiManager.ExtraSupplicantConnected, false);
                    break;
                case WifiManager.NetworkStateChangedAction:
                    _networkInfo = intent.GetParcelableExtra(WifiManager.ExtraNetworkInfo) as NetworkInfo;
                    break;
                case WifiManager.SupplicantStateChangedAction:
                    _supplicantState = intent.GetParcelableExtra(WifiManager.ExtraNewState) as SupplicantState;
                    var isSupplicantError = intent.HasExtra(WifiManager.ExtraSupplicantError);

                    if (_supplicantState == SupplicantState.Disconnected && isSupplicantError)
                    {
                        RemoveNetwork();
                    }
                    break;
            }
        }

        void RemoveNetwork()
        {
            try
            {

                _currentWifi.IsConnected = false;
                RaiseNetworkConnected?.Invoke(this, new NetworkConnectedEventArgs(_currentWifi));

                var current = AlreadyConfigured(_currentWifi);
                if (current == null) return;
                if (current.NetworkId < 0) return;

                _wifiManager.RemoveNetwork(current.NetworkId);
            }
            catch (Exception ex)
            {
                Log.Error(TAG, ex.Message);
            }
        }

        public async Task<bool> RemoveNetworkAsync(Wifi wifi)
        {
            if (wifi is null) return false;

            wifi.IsConnected = false;
            RaiseNetworkConnected?.Invoke(this, new NetworkConnectedEventArgs(wifi));

            var current = AlreadyConfigured(wifi);
            if (current == null) return false;
            if (current.NetworkId < 0) return false;

            return _wifiManager.RemoveNetwork(current.NetworkId);
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
                    Frequency = scan.Frequency,
                    Level = scan.Level,
                    Ssid = scan.Ssid,
                    Timestamp = scan.Timestamp,
                    IsConnected = _wifiManager.ConnectionInfo.SSID == $"\"{scan.Ssid}\""
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

        public async Task<bool> ConnectAsync(Wifi wifi)
        {
            if (wifi == null) return false;

            if (!_wifiManager.IsWifiEnabled)
                return false;

            if (string.IsNullOrEmpty(wifi.Password)) return false;

            int networkId;
            var config = AlreadyConfigured(wifi);
            if (config is null || config?.NetworkId < 0)
            {
                var conf = MapWifiToConfiguration(wifi);
                networkId = _wifiManager.AddNetwork(conf);
            }
            else
            {
                networkId = config.NetworkId;
            }

            await ConnectToAlreadyConfiguredAsync(networkId);

            await Task.Delay(1 * 1000);

            var isConnected = _wifiManager.ConnectionInfo.SSID == $"\"{wifi.Ssid}\"";
            wifi.IsConnected = isConnected;
            RaiseNetworkConnected?.Invoke(this, new NetworkConnectedEventArgs(wifi));
            return isConnected;
        }

        public async Task<bool> ConnectToRememberedAsync(Wifi wifi)
        {
            if (wifi == null) return false;

            CurrentWifi = wifi;
            //CurrentWifi = new Wifi { 
            //    Bssid = wifi.Bssid, 
            //    Capabilities = wifi.Capabilities, 
            //    Frequency = wifi.Frequency, 
            //    IsConnected = wifi.IsConnected, 
            //    IsHidden = wifi.IsHidden, 
            //    IsSelected = wifi.IsSelected, 
            //    Level = wifi.Level, 
            //    Password = wifi.Password, 
            //    Timestamp = wifi.Timestamp, 
            //    Ssid = wifi.Ssid
            //};

            var current = AlreadyConfigured(wifi);
            if (current == null) return false;
            if (current.NetworkId < 0) return false;

            await ConnectToAlreadyConfiguredAsync(current.NetworkId);

            await Task.Delay(1 * 1000);

            var isConnected = _wifiManager.ConnectionInfo.SSID == $"\"{wifi.Ssid}\"";
            wifi.IsConnected = isConnected;
            RaiseNetworkConnected?.Invoke(this, new NetworkConnectedEventArgs(wifi));

            //if (_supplicantState == SupplicantState.Invalid) return false;
            //if (_networkInfo.GetDetailedState() == NetworkInfo.DetailedState.Disconnected) return false;
            return isConnected;
        }

        public async Task<bool> AlreadyConnectedAsync(Wifi wifi)
        {
            if (wifi == null) return false;

            CurrentWifi = new Wifi
            {
                Bssid = wifi.Bssid,
                Capabilities = wifi.Capabilities,
                Frequency = wifi.Frequency,
                IsConnected = wifi.IsConnected,
                IsHidden = wifi.IsHidden,
                IsSelected = wifi.IsSelected,
                Level = wifi.Level,
                Password = wifi.Password,
                Timestamp = wifi.Timestamp,
                Ssid = wifi.Ssid
            };

            var isCurrentSsid = !string.IsNullOrEmpty(wifi.Ssid) && _wifiManager.ConnectionInfo.SSID == $"\"{wifi.Ssid}\"";
            wifi.IsConnected = isCurrentSsid;
            RaiseNetworkConnected?.Invoke(this, new NetworkConnectedEventArgs(wifi));
            return isCurrentSsid && _wifiManager.ConnectionInfo.NetworkId > -1;
        }

        public WifiConfiguration AlreadyConfigured(Wifi wifi)
        {
            return _wifiManager.ConfiguredNetworks.FirstOrDefault((w) => { return w.Bssid == $"\"{wifi.Bssid}\"" || w.Ssid == $"\"{wifi.Ssid}\""; });
        }

        public async Task<bool> ConnectToAlreadyConfiguredAsync(int networkId)
        {
            if (networkId < 0) return false;
            try
            {
                var isDisconnected = _wifiManager.Disconnect();
                var isDisabled = _wifiManager.ConnectionInfo.NetworkId < 0;
                _wifiManager.DisableNetwork(_wifiManager.ConnectionInfo.NetworkId);

                await Task.Delay(4 * 1000);

                var isEnabled = _wifiManager.EnableNetwork(networkId, true);
                var isReconnected = _wifiManager.Reconnect();

                return isEnabled && isReconnected;
            }
            catch (Exception ex)
            {
                Log.Debug(TAG, $"{nameof(ConnectToAlreadyConfiguredAsync)} - Error while trying to connect. Message: {ex.Message}. StackTrace: {ex.StackTrace}");
            }

            return false;
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

            return SetupProtocolUsed(wifi, configuration);
        }

        WifiConfiguration SetupProtocolUsed(Wifi wifi, WifiConfiguration conf)
        {
            if (string.IsNullOrEmpty(wifi.Capabilities))
                return conf;

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

        public async Task<bool> DisconnectAsync() => _wifiManager.Disconnect();

        public async Task<bool> ForgetAsync(Wifi wifi)
        {
            if (wifi is null) return false;

            WifiConfiguration conf = AlreadyConfigured(wifi);

            if (conf is null) return false;

            return _wifiManager.DisableNetwork(conf.NetworkId);
        }

        public bool SetWifiEnabled(bool enabled) => _wifiManager.SetWifiEnabled(enabled);

        public void StartScan() => _wifiManager.StartScan();

        private Wifi ConnectionInfoToWifi(WifiInfo info)
        {
            return new Wifi
            {
                Ssid = info.SSID,
                Bssid = info.MacAddress,
                Frequency = info.Frequency,
                IsConnected = true
            };
        }
    }
}