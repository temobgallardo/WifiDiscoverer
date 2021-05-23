﻿using System.Windows.Input;
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

                if (_currentWifi == value && _currentWifi.IsSelected) return;

                if (_currentWifi != null)
                {
                    _currentWifi.IsSelected = false;
                }

                SetProperty(ref _currentWifi, UpdateIsSelected(value));

                ActivateConnectNetworkElement();
            }
        }

        Wifi UpdateIsSelected(Wifi value)
        {
            value.IsSelected = true;
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

        const int _refreshCounterMax = 5;
        int _refreshCounter = 5;
        bool _firstTime = true;
        readonly IWifiConnectionReceiver _wifiConnectionService;

        public ICommand EnableWifiCommand { get; private set; }
        public ICommand SelectedWifiCommand { get; private set; }
        public ICommand RefreshCommand { get; private set; }
        public ICommand ConnectCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }
        public ICommand ConnectToHidenNetworkCommand { get; private set; }
        
        public WifiConnection(IWifiConnectionReceiver wifiConnectionReceiver, INavigation navigation) : base()
        {
            //_navigationService = navigation; 
            _wifiConnectionService = wifiConnectionReceiver;

            Wifis = new ObservableCollection<Wifi>();
            EnableWifiToggle = true;

            EnableWifiCommand = new Command(EnableWifi);
            RefreshCommand = new Command(RefreshWifis, canExecute: () => EnableWifiToggle);
            ConnectCommand = new Command((w) => Connect(w as Wifi));
            CancelCommand = new Command(Cancel);
            ConnectToHidenNetworkCommand = new Command((w) => ConnectToHidenNetwork(w as Wifi), canExecute: (w) => EnableWifiToggle);
        }

        void ConnectToHidenNetwork(Wifi w)
        {
            ActivateAddHiddenNetworkElement();
        }

        void Cancel()
        {
            ActivateNetworkListView();
        }

        void Connect(Wifi w)
        {
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
        }

        void RefreshWifis()
        {
            if (_refreshCounter-- < _refreshCounterMax) return;

            _wifiConnectionService.StartScan();

            LoadWifis();
            IsRefreshing = false;
            _refreshCounter = 5;
        }

        bool LoadWifis()
        {
            //var ran = new Random();
            //var fakeWifi = new Wifi
            //{
            //    Ssid = ran.Next(0, 100).ToString()
            //};
            //var wifis = new List<Wifi>
            //{
            //    fakeWifi,
            //    fakeWifi,
            //    fakeWifi,
            //    fakeWifi
            //};
            //Wifis.Clear();
            //Wifis.AddRange(wifis);

            var wifis = _wifiConnectionService.Wifis;
            if (wifis == null) return false;

            Wifis.Clear();
            var filtered = wifis.Where((w) => !string.IsNullOrEmpty(w.Ssid));
            if (!filtered.Any()) return false;

            Wifis.AddRange(filtered);
            return false;
        }

        void RefreshCanExecutes()
        {
            (RefreshCommand as Command).ChangeCanExecute();
            (ConnectToHidenNetworkCommand as Command).ChangeCanExecute();
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

            //if (!EnableWifiToggle) return;

            EnableWifiToggle = !EnableWifiToggle;

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