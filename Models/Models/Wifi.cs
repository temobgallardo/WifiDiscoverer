using System.Collections.Generic;
using WifiPage.Models.Commons;

namespace IsObservableCollBuggy.Models.Models
{
    public class Wifi : ObservableModel
    {
        string _ssid;
        public string Ssid { get => _ssid; set => SetProperty(ref _ssid, value); }
        public long Timestamp { get; set; }
        public int WifiStandard { get; }
        public bool IsPasspointNetwork { get; }
        public int Level { get; set; }
        public string Capabilities { get; set; }
        string _bssid;
        public string Bssid { get => _bssid; set => SetProperty(ref _bssid, value); }
        public int Frequency { get; set; }
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }
        bool _isHidden;
        public bool IsHidden { get => _isHidden; set => SetProperty(ref _isHidden, value); }
        private bool _isConnected;
        public bool IsConnected { get => _isConnected; set => SetProperty(ref _isConnected, value); }
        private string _state;
        public string State { get => _state; set => SetProperty(ref _state, value); }
    }

    public class WifiComprarer : IEqualityComparer<Wifi>
    {
        public bool Equals(Wifi x, Wifi y)
        {
            if (x is null || y is null) return false;

            return x.Ssid == y.Ssid;
        }

        public int GetHashCode(Wifi wifi)
        {
            //Check whether the object is null
            if (wifi is null) return 0;

            return wifi.Ssid == null ? 0 : wifi.Ssid.GetHashCode();
        }
    }

    public enum WifiStates
    {
        Connected,
        Connecting,
        Disconnecting,
        Disconnected
    }
}
