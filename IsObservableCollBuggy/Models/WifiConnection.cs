using IsObservableCollBuggy.Extensions;
using IsObservableCollBuggy.Models.Models;
using Models.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using WifiPage.Models.Commons;
using Xamarin.Forms;

namespace IsObservableCollBuggy.Models
{
    public class WifiConnection : ObservableModel, INotifyPropertyChanged, IBroadcastReceieverCallback
    {
        bool _enableWifiToggle;
        public bool EnableWifiToggle
        {
            get => _enableWifiToggle;
            set
            {
                SetProperty(ref _enableWifiToggle, value);
                EnableWifi(value);
                RefreshCanExecutes();
                LoadWifis();
            }
        }

        ObservableCollection<Wifi> _wifis;
        public ObservableCollection<Wifi> Wifis
        {
            get => _wifis;
            set => base.SetProperty(ref _wifis, value);
        }

        Wifi _currentWifi;
        public Wifi CurrentWifi
        {
            get => _currentWifi;
            set
            {
                if (value == null) return;

                if (_currentWifi != null && _currentWifi.Ssid == value.Ssid)
                {
                    Task.Run(async () => await ActivateConnectNetworkElementOrConnectRememberedAsync());
                    SetProperty(ref _currentWifi, UpdateIsSelected(_currentWifi, !_currentWifi.IsSelected));
                    UpdateWifiState();
                    return;
                }

                if (_currentWifi != null && !_firstTime)
                {
                    _currentWifi.State = string.Empty;
                    SetProperty(ref _currentWifi, UpdateIsSelected(_currentWifi, false));
                }

                SetProperty(ref _currentWifi, UpdateIsSelected(value, true));

                Task.Run(async () => await ActivateConnectNetworkElementOrConnectRememberedAsync());
                UpdateWifiState();
            }
        }

        Wifi _connected;
        public Wifi Connected
        {
            get => _connected;
            set => SetProperty(ref _connected, UpdateIsSelected(value, true));
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
            set => SetProperty(ref _isRefreshing, value);
        }

        bool _networkListIsVisible;
        public bool NetworkListIsVisible
        {
            get => _networkListIsVisible;
            set => SetProperty(ref _networkListIsVisible, value);
        }

        bool _connectNetworkIsVisible;
        public bool ConnectNetworkIsVisible
        {
            get => _connectNetworkIsVisible;
            set => SetProperty(ref _connectNetworkIsVisible, value);
        }

        bool _addHiddenNetworkIsVisible;
        public bool AddHiddenNetworkIsVisible
        {
            get => _addHiddenNetworkIsVisible;
            set => SetProperty(ref _addHiddenNetworkIsVisible, value);
        }

        bool _footerButtonsVisible;
        public bool FooterButtonsVisible
        {
            get => _footerButtonsVisible;
            set => SetProperty(ref _footerButtonsVisible, value);
        }

        Wifi _hiddenNetwork;
        public Wifi HiddenNetwork
        {
            get => _hiddenNetwork;
            set => SetProperty(ref _hiddenNetwork, value);
        }

        string _deviceMacAddress;
        public string DeviceMacAddress
        {
            get => _deviceMacAddress;
            set
            {
                if (!EnableWifiToggle && !string.IsNullOrEmpty(_deviceMacAddress)) return;

                SetProperty(ref _deviceMacAddress, value);
            }
        }

        bool _firstTime = true;
        readonly IWifiConnectionReceiver _wifiConnectionService;
        readonly IToastMessage _toastMessage;
        readonly IBrodcastSevice _broadcastService;

        public ICommand SelectedWifiCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand AddHiddenNetworkCommand { get; private set; }
        public ICommand AddNetworkCommand { get; private set; }
        public ICommand ForgetCommand { get; private set; }
        public ICommand DisconnectCommand { get; private set; }
        public ICommand RemoveNetworkCommand { get; private set; }

        public WifiConnection(IWifiConnectionReceiver wifiConnectionReceiver, IToastMessage toastMessage, IBrodcastSevice brodcastSevice) : base()
        {
            _broadcastService = brodcastSevice;
            _wifiConnectionService = wifiConnectionReceiver;
            _toastMessage = toastMessage;

            Wifis = new ObservableCollection<Wifi>();
            HiddenNetwork = new Wifi();

            RefreshCommand = new Command(RefreshWifis, canExecute: () => EnableWifiToggle);
            ConnectCommand = new Command(async () => await ConnectAsync());
            CancelCommand = new Command(Cancel);
            AddHiddenNetworkCommand = new Command((s) => OpenHiddenNetworkConnectPage(), canExecute: (w) => EnableWifiToggle);
            AddNetworkCommand = new Command(async () => await AddNetworkAsync());
            ForgetCommand = new Command(async (o) => await ForgetAsync(o));
            DisconnectCommand = new Command(async (o) => await DisconnectAsync(), canExecute: (w) => EnableWifiToggle);
            RemoveNetworkCommand = new Command(async (s) => await RemoveNetworkAsync(), canExecute: (w) => EnableWifiToggle);
        }

        private async Task RemoveNetworkAsync()
        {
            var isRemoved = await _wifiConnectionService.RemoveNetworkAsync(CurrentWifi);
            Device.BeginInvokeOnMainThread(UpdateWifiState);
        }

        // TODO: Tell the user that the wifi has disconnected successfuly
        private async Task DisconnectAsync() => await _wifiConnectionService.DisconnectAsync();

        // TODO: Tell the user that the wifi has been fogotten
        async Task ForgetAsync(object obj) => await _wifiConnectionService.ForgetAsync(obj as Wifi);

        private async Task AddNetworkAsync()
        {
            if (!EnableWifiToggle) return;

            HiddenNetwork.IsHidden = true;
            HiddenNetwork.State = WifiStates.Connecting.ToString();
            await AddNetworkOrConnectRememberedAsync(HiddenNetwork);
            ActivateNetworkListView();
        }

        void OpenHiddenNetworkConnectPage()
        {
            ActivateAddHiddenNetworkElement();
        }

        void Cancel()
        {
            if (CurrentWifi != null)
            {
                CurrentWifi.State = string.Empty;
            }

            UpdateWifiState();

            ActivateNetworkListView();
        }

        async Task ConnectAsync()
        {
            await _wifiConnectionService.ConnectAsync(CurrentWifi);
            await Device.InvokeOnMainThreadAsync(UpdateWifiState);
            ActivateNetworkListView();
        }

        public void OnAttached()
        {
            System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Attaching");

            _broadcastService.Register(this);

            if (Wifis is null)
            {
                Wifis = new ObservableCollection<Wifi>();
            }

            InitializeData();
        }

        void InitializeData()
        {
            // When enabled wifi list is loaded.
            EnableWifiToggle = _wifiConnectionService.IsWifiEnabled;
            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;
            CurrentWifi = _wifiConnectionService.ConnectedWifi;

            LoadWifis();
            UpdateWifiState();
        }

        void RefreshWifis()
        {
            ActivateNetworkListView();

            IsRefreshing = true;

            _wifiConnectionService.StartScan();
            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;

            LoadWifis();
            UpdateWifiState();

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

            return true;
        }

        void RefreshCanExecutes()
        {
            (RefreshCommand as Command).ChangeCanExecute();
            (AddHiddenNetworkCommand as Command).ChangeCanExecute();
            (DisconnectCommand as Command).ChangeCanExecute();
            (RemoveNetworkCommand as Command).ChangeCanExecute();
        }

        // TODO: Deprecate on API level 29 since it is not allowed for apps to disable/enable wifi
        void EnableWifi(bool isEnabled)
        {
            ActivateNetworkListView();

            if (_firstTime && isEnabled)
            {
                _firstTime = false;
                return;
            }

            var success = _wifiConnectionService.SetWifiEnabled(isEnabled);

            if (!success)
            {
                //ToastOnMainThreadAsync("Wifi could not be enabled/disabled. Please, try again!");
                return;
            }

            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;
            UpdateWifiState();
        }

        void ActivateNetworkListView()
        {
            ConnectNetworkIsVisible = false;
            AddHiddenNetworkIsVisible = false;
            NetworkListIsVisible = true;
            FooterButtonsVisible = true;
        }

        void ActivateConnectNetworkElement()
        {
            NetworkListIsVisible = false;
            AddHiddenNetworkIsVisible = false;
            FooterButtonsVisible = false;
            ConnectNetworkIsVisible = true;
        }

        void ActivateAddHiddenNetworkElement()
        {
            NetworkListIsVisible = false;
            ConnectNetworkIsVisible = false;
            FooterButtonsVisible = false;
            AddHiddenNetworkIsVisible = true;
        }

        private async Task ActivateConnectNetworkElementOrConnectRememberedAsync()
        {
            var connected = await _wifiConnectionService.AlreadyConnectedAsync(CurrentWifi);
            if (connected) return;

            var remembered = await _wifiConnectionService.ConnectToRememberedAsync(CurrentWifi);
            if (remembered) return;

            ActivateConnectNetworkElement();
        }

        private async Task AddNetworkOrConnectRememberedAsync(Wifi wifi)
        {
            var connected = await _wifiConnectionService.AlreadyConnectedAsync(CurrentWifi);
            if (connected)
            {
                //await ToastOnMainThreadAsync ($"Wifi '{CurrentWifi.Ssid}' already connected");
                return;
            }

            var remembered = await _wifiConnectionService.ConnectToRememberedAsync(CurrentWifi);
            if (remembered)
            {
                //await ToastOnMainThreadAsync ($"Wifi '{CurrentWifi.Ssid}' already configured. Connecting...");
                return;
            }
        }


        private async Task ToastOnMainThreadAsync(string msg) => await Device.InvokeOnMainThreadAsync(() => _toastMessage.LongAlert(msg));

        private void UpdateWifiState()
        {
            var connected = _wifiConnectionService.ConnectedWifi;

            if (Wifis is null || connected is null) return;

            for (int i = 0; i < Wifis.Count; i++)
            {
                if (Wifis[i].Ssid != connected.Ssid)
                {
                    Wifis[i].State = string.Empty;
                    Wifis[i].IsConnected= false;
                }
                else
                {
                    Wifis[i].State = _wifiConnectionService.ConnectedWifi.State;
                    Wifis[i].IsConnected = true;
                }
            }

            var temp = Wifis.ToArray();

            Wifis.Clear();

            Wifis.AddRange(temp);
        }

        private void UpdateIsConnectedInWifis(Wifi network)
        {
            if (Wifis is null || network is null) return;

            Wifis.Select((w) => w.Ssid != network.Ssid ? w.IsConnected = false : w.IsConnected = network.IsConnected);
        }

        public void OnDettached()
        {
            System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached Disposing");
            Wifis.Clear();
            Wifis = null;
            _broadcastService.UnRegister();
        }

        public void Execute(Wifi wifi, WifiStates state)
        {
            if (CurrentWifi is null) return;

            var isCurrentWifi = wifi.Ssid == CurrentWifi.Ssid;

            System.Diagnostics.Debug.WriteLine($"{nameof(Execute)} - {CurrentWifi.Ssid} : {CurrentWifi.State}");

            if (!isCurrentWifi) return;

            if (WifiStates.Connecting == state)
            {
                CurrentWifi.IsConnected = false;
                CurrentWifi.State = state.ToString();
            }
            if (WifiStates.Connected == state)
            {
                CurrentWifi.IsConnected = true;
                CurrentWifi.State = state.ToString();
            }
            if (WifiStates.Disconnecting == state)
            {
                CurrentWifi.IsConnected = false;
                CurrentWifi.State = string.Empty;
                Wifis.Select(w => w.State = string.Empty);
            }
            if (WifiStates.Disconnected == state)
            {
                CurrentWifi.IsConnected = false;
                CurrentWifi.State = string.Empty;
                Wifis.Select(w => w.State = string.Empty);
            }
            if (WifiStates.OptainingIp == state)
            {
                CurrentWifi.IsConnected = false;
                CurrentWifi.State = state.ToString();
            }
        }
    }
}
