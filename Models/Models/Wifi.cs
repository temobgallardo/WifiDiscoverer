using WifiPage.Models.Commons;

namespace IsObservableCollBuggy.Models.Models
{
    public class Wifi : ObservableModel
    {
        public string Ssid { get; set; }
        public long Timestamp { get; set; }
        public int WifiStandard { get; }
        public bool IsPasspointNetwork { get; }
        public int Level { get; set; }
        public int ChannelWidth { get; set; }
        public int CenterFreq1 { get; set; }
        public int CenterFreq0 { get; set; }
        public string Capabilities { get; set; }
        public string Bssid { get; set; }
        public int Frequency { get; set; }
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set => SetProperty(ref _isSelected, value); }
        private string _password;
        public string Password { get => _password; set => SetProperty(ref _password, value); }
    }
}
