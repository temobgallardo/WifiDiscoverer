using IsObservableCollBuggy.Models.Models;
using Models.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinUniversity.Infrastructure;

namespace IsObservableCollBuggy.Models
{
    public class WifiConnection : SimpleViewModel, INotifyPropertyChanged
    {
        bool _enableWifiToggle;
        public bool EnableWifiToggle
        {
            get => _enableWifiToggle;
            set
            {
                SetPropertyValue(ref _enableWifiToggle, value);
                EnableWifi(value);
            }
        }

        ObservableCollection<Wifi> _wifis;
        public ObservableCollection<Wifi> Wifis
        {
            get => _wifis;
            set => base.SetPropertyValue(ref _wifis, value);
        }

        Wifi _currentWifi;
        public Wifi CurrentWifi
        {
            get => _currentWifi;
            set
            {
                if (value == null) return;

                if (_currentWifi == value && _currentWifi.IsSelected)
                {
                    Task.Run(async () => await ActivateConnectNetworkElementOrConnectRememberedAsync());
                    return;
                }

                if (_currentWifi != null)
                {
                    _currentWifi.IsSelected = false;
                }

                SetPropertyValue(ref _currentWifi, UpdateIsSelected(value, true));

                Task.Run(async () => await ActivateConnectNetworkElementOrConnectRememberedAsync());
            }
        }

        Wifi _connected;
        public Wifi Connected
        {
            get => _connected;
            set => SetPropertyValue(ref _connected, UpdateIsSelected(value, true));
        }

        Wifi UpdateIsSelected(Wifi value, bool isSelected)
        {
            if (value == null) return value;

            value.IsSelected = isSelected;
            return value;
        }

        bool _isRefreshing;
        public bool IsRefreshing
        {
            get => _isRefreshing;
            set => SetPropertyValue(ref _isRefreshing, value);
        }

        bool _networkListIsVisible;
        public bool NetworkListIsVisible
        {
            get => _networkListIsVisible;
            set => SetPropertyValue(ref _networkListIsVisible, value);
        }

        bool _connectNetworkIsVisible;
        public bool ConnectNetworkIsVisible
        {
            get => _connectNetworkIsVisible;
            set => SetPropertyValue(ref _connectNetworkIsVisible, value);
        }

        bool _addHiddenNetworkIsVisible;
        public bool AddHiddenNetworkIsVisible
        {
            get => _addHiddenNetworkIsVisible;
            set => SetPropertyValue(ref _addHiddenNetworkIsVisible, value);
        }

        bool _footerButtonsVisible;
        public bool FooterButtonsVisible
        {
            get => _footerButtonsVisible;
            set => SetPropertyValue(ref _footerButtonsVisible, value);
        }

        Wifi _hiddenNetwork;
        public Wifi HiddenNetwork
        {
            get => _hiddenNetwork;
            set => SetPropertyValue(ref _hiddenNetwork, value);
        }

        string _deviceMacAddress;
        public string DeviceMacAddress
        {
            get => _deviceMacAddress;
            set
            {
                if (!EnableWifiToggle && !string.IsNullOrEmpty(_deviceMacAddress)) return;

                SetPropertyValue(ref _deviceMacAddress, value);
            }
        }

        bool _firstTime = true;
        readonly IWifiConnectionReceiver _wifiConnectionService;
        readonly IToastMessage _toastMessage;

        public ICommand SelectedWifiCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenHiddenNetworkConnectPageCommand { get; private set; }
        public ICommand AddNetworkCommand { get; private set; }
        public ICommand ForgetCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }
        public ICommand RemoveNetworkCommand { get; private set; }
        
        public WifiConnection(IWifiConnectionReceiver wifiConnectionReceiver, IToastMessage toastMessage) : base()
        {
            _wifiConnectionService = wifiConnectionReceiver;
            _toastMessage = toastMessage;

            Wifis = new ObservableCollection<Wifi>();
            HiddenNetwork = new Wifi();

            RefreshCommand = new Command(RefreshWifis, canExecute: () => EnableWifiToggle);
            ConnectCommand = new Command(async () => await ConnectAsync());
            CancelCommand = new Command(Cancel);
            OpenHiddenNetworkConnectPageCommand = new Command((s) => OpenHiddenNetworkConnectPage(), canExecute: (w) => EnableWifiToggle);
            AddNetworkCommand = new Command(async () => await AddNetworkAsync());
            ForgetCommand = new Command(Forget);
            DisconnectCommand = new Command((o) => Disconnect(), canExecute: (w) => EnableWifiToggle);
            RemoveNetworkCommand = new Command(async (s) => await RemoveNetworkAsync(), canExecute: (w) => EnableWifiToggle);
        }

        private async Task RemoveNetworkAsync()
        {
            var isRemoved = await _wifiConnectionService.RemoveNetworkAsync(CurrentWifi);
            //if (isRemoved)
            //{
            //    await ToastOnMainThreadAsync($"Network {CurrentWifi.Ssid} has been forgotten");
            //}

            Device.BeginInvokeOnMainThread(() => UpdateIsConnectedInWifis());
        }

        // TODO: Tell the user that the wifi has disconnected successfuly
        private void Disconnect() => _wifiConnectionService.DisconnectAsync();

        // TODO: Tell the user that the wifi has been fogotten
        void Forget(object obj) => _wifiConnectionService.ForgetAsync(obj as Wifi);

        private async Task AddNetworkAsync()
        {
            if (!EnableWifiToggle) return;

            HiddenNetwork.IsHidden = true;
            await AddNetworkOrConnectRememberedAsync (HiddenNetwork);
            ActivateNetworkListView();
        }

        void OpenHiddenNetworkConnectPage()
        {
            ActivateAddHiddenNetworkElement();
        }

        void Cancel()
        {
            ActivateNetworkListView();
        }

        async Task ConnectAsync()
        {
            var isConnected = await _wifiConnectionService.ConnectAsync(CurrentWifi);
            if (isConnected)
            {
                Connected = CurrentWifi;
            }

            await NotifyUserIfConnectedAsync (isConnected, CurrentWifi);
            ActivateNetworkListView();
        }

        public void OnAttached()
        {
            _wifiConnectionService.RaiseNetworkConnected += async (s, e) => await NetworkConnected(s, e);
            InitializeData();
        }

        private async Task NetworkConnected(object sender, NetworkConnectedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() => UpdateIsConnectedInWifis(e.ConnectedNetwork));
            var connectedString = e.ConnectedNetwork.IsConnected ? "are connected to" : "did not connect to";
            await ToastOnMainThreadAsync($"You {connectedString} '{e.ConnectedNetwork.Ssid}'.");
        }

        void InitializeData()
        {
            // When enabled wifi list is loaded.
            EnableWifiToggle = _wifiConnectionService.IsWifiEnabled;
            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;
            LoadWifis();
            UpdateIsConnectedInWifis();
        }

        void RefreshWifis()
        {
            ActivateNetworkListView();

            IsRefreshing = true;

            _wifiConnectionService.StartScan();
            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;

            LoadWifis();
            IsRefreshing = false;
        }

        bool LoadWifis()
        {
            if (!EnableWifiToggle)
            {
                Wifis.Clear();
                return false;
            }

            var wifis = _wifiConnectionService.Wifis;
            if (wifis == null) return false;

            Wifis.Clear();
            var wifiWithNames = wifis.Where((w) => !string.IsNullOrEmpty(w.Ssid));
            if (!wifiWithNames.Any()) return false;

            var distinct = wifiWithNames.Distinct(new WifiComprarer());
            Wifis.AddRange(distinct);
            return false;
        }

        void RefreshCanExecutes()
        {
            (RefreshCommand as Command).ChangeCanExecute();
            (OpenHiddenNetworkConnectPageCommand as Command).ChangeCanExecute();
            (DisconnectCommand as Command).ChangeCanExecute();
        }

        // TODO: Deprecate on API level 29 since it is not allowed for apps to disable/enable wifi
        void EnableWifi(bool isEnabled)
        {
            ActivateNetworkListView();

            if (_firstTime && isEnabled)
            {
                _firstTime = false;
                RefreshCanExecutes();
                LoadWifis();
                return;
            }

            var success = _wifiConnectionService.SetWifiEnabled(isEnabled);

            if (!success)
            {
                ToastOnMainThreadAsync("Wifi could not be enabled/disabled. Please, try again!");
                return;
            }

            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;

            RefreshCanExecutes();
            LoadWifis();
        }

        void ActivateNetworkListView()
        {
            NetworkListIsVisible = true;
            ConnectNetworkIsVisible = false;
            AddHiddenNetworkIsVisible = false;
            FooterButtonsVisible = true;
        }

        void ActivateConnectNetworkElement()
        {
            NetworkListIsVisible = false;
            ConnectNetworkIsVisible = true;
            AddHiddenNetworkIsVisible = false;
            FooterButtonsVisible = false;
        }

        void ActivateAddHiddenNetworkElement()
        {
            NetworkListIsVisible = false;
            ConnectNetworkIsVisible = false;
            AddHiddenNetworkIsVisible = true; 
            FooterButtonsVisible = false;
        }

        private async Task ActivateConnectNetworkElementOrConnectRememberedAsync()
        {
            await ToastOnMainThreadAsync($"Connecting to '{CurrentWifi.Ssid}' a moment, please.");

            var connected = await _wifiConnectionService.AlreadyConnectedAsync(CurrentWifi);
            if (connected)
            {
                await ToastOnMainThreadAsync($"Wifi '{CurrentWifi.Ssid}' already connected");
                return;
            }

            var remembered = await _wifiConnectionService.ConnectToRememberedAsync(CurrentWifi);
            if (remembered)
            {
                await ToastOnMainThreadAsync($"Wifi '{CurrentWifi.Ssid}' already configured. Connecting...");
                return;
            }

            ActivateConnectNetworkElement();
        }

        private async Task AddNetworkOrConnectRememberedAsync(Wifi wifi)
        {
            var connected = await _wifiConnectionService.AlreadyConnectedAsync(CurrentWifi);
            if (connected)
            {
                await ToastOnMainThreadAsync ($"Wifi '{CurrentWifi.Ssid}' already connected");
                return;
            }

            var remembered = await _wifiConnectionService.ConnectToRememberedAsync(CurrentWifi);
            if (remembered)
            {
                await ToastOnMainThreadAsync ($"Wifi '{CurrentWifi.Ssid}' already configured. Connecting...");
                return;
            }

            await NotifyUserIfConnectedAsync(await _wifiConnectionService.ConnectAsync(wifi), wifi);
        }

        async Task NotifyUserIfConnectedAsync(bool connected, Wifi wifi = null)
        {
            var ssid = wifi is null ? CurrentWifi.Ssid : wifi.Ssid;
            if (connected)
            {
                await ToastOnMainThreadAsync($"Connected succesfully to '{ssid}'");
                return;
            }

            if (await _wifiConnectionService.AlreadyConnectedAsync(CurrentWifi))
            {
                await ToastOnMainThreadAsync($"Connected succesfully to '{ssid}'");
            }
            else
            {
                await ToastOnMainThreadAsync($"Unable to connect to '{ssid}'. It can be due to the password or connection issues. Try again, please!");
            }
        }

        private async Task ToastOnMainThreadAsync(string msg) => await Device.InvokeOnMainThreadAsync(() => _toastMessage.LongAlert(msg));

        private void UpdateIsConnectedInWifis()
        {
            foreach (var wifi in Wifis)
            {
                wifi.IsConnected = _wifiConnectionService.ConnectedWifi.Ssid == $"\"{wifi.Ssid}\"";
            }
        }

        private void UpdateIsConnectedInWifis(Wifi network)
        {
            foreach (var wifi in Wifis)
            {
                wifi.IsConnected = _wifiConnectionService.ConnectedWifi.Ssid == $"\"{wifi.Ssid}\"";
            }

            var current = Wifis.FirstOrDefault((w) => w.Ssid == network.Ssid);
            if (current is null) return;
            current.IsConnected = network.IsConnected;
        }

        public void OnDettached()
        {
            Wifis.Clear();
        }
    }
}
