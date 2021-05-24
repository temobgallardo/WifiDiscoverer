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
        public int ChannelWidth { get; set; }
        public int CenterFreq1 { get; set; }
        public int CenterFreq0 { get; set; }
        public string Capabilities { get; set; }
        string _bssid;
        public string Bssid { get => _bssid; set => SetProperty(ref _bssid, value); }
        public int Frequency { get; set; }
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }
    }
}
