using System.Windows.Input;
using Xamarin.Forms;
using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using IsObservableCollBuggy.Models.Models;
using XamarinUniversity.Infrastructure;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Models.Interfaces;
using System.Linq;

namespace IsObservableCollBuggy.Models
{
    public class WifiConnection : SimpleViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        protected void RaisePropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T oldValue, T newValue, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(oldValue, newValue))
            {
                oldValue = newValue;
                RaisePropertyChanged(propertyName);
                return true;
            }
            else
            {
                return false;
            }
        }

        bool _enableWifiToggle;
        public bool EnableWifiToggle
        {
            get => _enableWifiToggle;
            set
            {
                SetProperty(ref _enableWifiToggle, value);
            }
        }

        ObservableCollection<Wifi> _wifis;
        public ObservableCollection<Wifi> Wifis
        {
            get => _wifis;
            set
            {
                //SetPropertyValue(ref _wifis, value);
                _wifis = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Wifis)));
            }
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
                    ActivateConnectNetworkElementOrConnectRemembered();
                    return;
                }

                if (_currentWifi != null)
                {
                    _currentWifi.IsSelected = false;
                }

                SetProperty(ref _currentWifi, UpdateIsSelected(value, true));

                ActivateConnectNetworkElementOrConnectRemembered();
            }
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

        public ICommand EnableWifiCommand { get; private set; }
        public ICommand SelectedWifiCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand OpenHiddenNetworkConnectPageCommand { get; private set; }
        public ICommand AddNetworkCommand { get; private set; }
        public ICommand ForgetCommand { get; private set; }

        public WifiConnection(IWifiConnectionReceiver wifiConnectionReceiver, INavigation navigation) : base()
        {
            //_navigationService = navigation; 
            _wifiConnectionService = wifiConnectionReceiver;

            Wifis = new ObservableCollection<Wifi>();
            EnableWifiToggle = true;
            HiddenNetwork = new Wifi();

            EnableWifiCommand = new Command(EnableWifi);
            RefreshCommand = new Command(RefreshWifis, canExecute: () => EnableWifiToggle);
            ConnectCommand = new Command(Connect);
            CancelCommand = new Command(Cancel);
            OpenHiddenNetworkConnectPageCommand = new Command((s) => OpenHiddenNetworkConnectPage(), canExecute: (w) => EnableWifiToggle);
            AddNetworkCommand = new Command(AddNetwork);
            ForgetCommand = new Command(Forget);
        }

        void Forget(object obj)
        {
            // TODO: Tell the user that the wifi has been fogotten
            _wifiConnectionService.Forget(obj as Wifi);
        }

        private void AddNetwork()
        {
            if (!EnableWifiToggle) return;
            HiddenNetwork.IsHidden = true;
            // Todo: Tell the user if connection was successful
            AddNetworkOrConnectRemembered(HiddenNetwork);
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

        void Connect()
        {
            // Todo: Tell the user if connection was successful
            _wifiConnectionService.Connect(CurrentWifi);
            ActivateNetworkListView();
        }

        public void OnAttached()
        {
            EnableWifiToggle = _wifiConnectionService.IsWifiEnabled;
            ActivateNetworkListView();
            InitializeData();
        }

        void InitializeData()
        {
            EnableWifiCommand?.Execute(null);
            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;
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
        }

        // TODO: Deprecate on API level 29 since it is not allowed for apps to disable/enable wifi
        void EnableWifi()
        {
            ActivateNetworkListView();

            if (_firstTime && _wifiConnectionService.IsWifiEnabled)
            {
                _firstTime = false;
                RefreshCanExecutes();
                LoadWifis();
                return;
            }

            var success = _wifiConnectionService.SetWifiEnabled(!EnableWifiToggle);

            // TODO: Tell the user that the wifi couldn't be enabled/disabled.
            if (!success) return;

            EnableWifiToggle = !EnableWifiToggle;

            DeviceMacAddress = _wifiConnectionService.DeviceMacAddress;

            RefreshCanExecutes();
            LoadWifis();
        }

        void ActivateNetworkListView()
        {
            NetworkListIsVisible = true;
            ConnectNetworkIsVisible = false;
            AddHiddenNetworkIsVisible = false;
        }

        void ActivateConnectNetworkElement()
        {
            NetworkListIsVisible = false;
            ConnectNetworkIsVisible = true;
            AddHiddenNetworkIsVisible = false;
        }

        private void ActivateConnectNetworkElementOrConnectRemembered()
        {
            IsRefreshing = true;

            //TODO: Tell the user this wifi is already configured or connected
            if (_wifiConnectionService.AlreadyConnected(CurrentWifi) || _wifiConnectionService.ConnectToRemembered(CurrentWifi))
            {
                IsRefreshing = false;
                return;
            }

            IsRefreshing = false;
            ActivateConnectNetworkElement();
        }

        private void AddNetworkOrConnectRemembered(Wifi wifi)
        {
            //TODO: Tell the user this wifi is already configured or connected
            if (_wifiConnectionService.AlreadyConnected(wifi) || _wifiConnectionService.ConnectToRemembered(wifi))
            {
                return;
            }

            _wifiConnectionService.Connect(wifi);
        }

        void ActivateAddHiddenNetworkElement()
        {
            NetworkListIsVisible = false;
            ConnectNetworkIsVisible = false;
            AddHiddenNetworkIsVisible = true;
        }

        public void OnDettached()
        {
            Wifis.Clear();
        }
    }
}
