using IsObservableCollBuggy.Extensions;
using IsObservableCollBuggy.Models.Models;
using Models.Interfaces;
using System;
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
        bool _firstTime = true;
        readonly IWifiConnectionReceiver _wifiConnectionService;
        readonly IToastMessage _toastMessage;
        readonly IBrodcastSevice _broadcastService;

        bool _enableWifiToggle;
        public bool EnableWifiToggle
        {
            get => _enableWifiToggle;
            set
            {
                SetProperty(ref _enableWifiToggle, value);
                EnableWifi(value);

                // There is an issue in android API level 27 where if wifi is disabled through Android Settings it will still return true 
                // sometimes when calling WifiManager.IsWifiEnabled(). That is the reason of this check and subsequent checks.
                if (!value)
                {
                    RefreshCanExecutes();
                    Wifis.Clear();
                    return;
                }

                RefreshCanExecutes();
                LoadWifis();
            }
        }

        ObservableCollection<Wifi> _wifis;
        public ObservableCollection<Wifi> Wifis
        {
            get => _wifis;
            set => SetProperty(ref _wifis, value);
        }

        Wifi _currentWifi;
        public Wifi CurrentWifi
        {
            get => _currentWifi;
            set
            {
                if (!EnableWifiToggle) return;

                if (value == null) return;

                if (_currentWifi != null && _currentWifi.Ssid == value.Ssid)
                {
                    Task.Run(async () => await ActivateConnectNetworkElementOrConnectRememberedAsync());
                    SetProperty(ref _currentWifi, UpdateIsSelected(_currentWifi, !_currentWifi.IsSelected));
                    UpdateWifiState();
                    return;
                }

                if (_currentWifi != null)
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

        public event Action RaiseOnAddNetworkVisible;
        public event Action RaiseOnConnectToWifiVisible;
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
            // When enabled is set the Wifis list is loaded. 
            EnableWifiToggle = _wifiConnectionService.IsWifiEnabled;
            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;
        }

        private void UpdateWifiStateFirsTime()
        {
            Wifis.Select((w) => w.Ssid == _wifiConnectionService.ConnectedWifi.Ssid ? w = _wifiConnectionService.ConnectedWifi : w);
        }

        void RefreshWifis()
        {
            ActivateNetworkListView();

            IsRefreshing = true;

            _wifiConnectionService.StartScan();
            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;

            LoadWifis();
            //CurrentWifi = _wifiConnectionService.ConnectedWifi; does not trigger the state value in the cell
            UpdateWifiState();

            IsRefreshing = false;
        }

        bool LoadWifis()
        {
            var wifis = _wifiConnectionService.Wifis;
            if (wifis == null || wifis.Count == 0) return false;

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
        bool EnableWifi(bool isEnabled)
        {
            ActivateNetworkListView();

            if (_firstTime && isEnabled)
            {
                _firstTime = false;
                return _wifiConnectionService.IsWifiEnabled;
            }

            var success = _wifiConnectionService.SetWifiEnabled(isEnabled);

            if (!success) return success;

            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;
            UpdateWifiState();

            return success;
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

            Device.BeginInvokeOnMainThread(() => RaiseOnConnectToWifiVisible?.Invoke());
        }

        void ActivateAddHiddenNetworkElement()
        {
            NetworkListIsVisible = false;
            ConnectNetworkIsVisible = false;
            FooterButtonsVisible = false;
            AddHiddenNetworkIsVisible = true;

            Device.BeginInvokeOnMainThread(() => RaiseOnAddNetworkVisible?.Invoke());
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
            if (connected) return;

            var remembered = await _wifiConnectionService.ConnectToRememberedAsync(CurrentWifi);
            if (remembered) return;
        }

        private async Task ToastOnMainThreadAsync(string msg) => await Device.InvokeOnMainThreadAsync(() => _toastMessage.LongAlert(msg));

        private void UpdateWifiState()
        {
            var connected = _wifiConnectionService.ConnectedWifi;
            var isConnected = !connected.Ssid.Contains("unknown");

            if (Wifis is null || connected is null || !isConnected) return;

            if (Wifis.Count < 0) return;

            for (int i = 0; i < Wifis.Count; i++)
            {
                if (Wifis[i].Ssid != connected.Ssid)
                {
                    Wifis[i].State = string.Empty;
                    Wifis[i].IsConnected = false;
                }
                else
                {
                    Wifis[i].State = _wifiConnectionService.ConnectedWifi.State;
                    Wifis[i].IsConnected = true;
                }
            }
        }

        public void OnDettached()
        {
            System.Diagnostics.Debug.WriteLine($"{this.GetType().FullName} Detached Disposing");
            Wifis.Clear();
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
