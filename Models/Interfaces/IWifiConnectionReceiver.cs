using IsObservableCollBuggy.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IWifiConnectionReceiver
    {
        event EventHandler<NetworkConnectedEventArgs> RaiseNetworkConnected;
        IBroadcastReceieverCallback Callbacks { get; set; }
        string WifiConnectionReceiverMessage { get; }
        bool IsWifiEnabled { get; }
        string DeviceMacAddress { get; }
        Wifi ConnectedWifi { get; }
        IList<Wifi> Wifis { get; }
        IList<Wifi> ScanFailure();
        IList<Wifi> ScanSuccess();
        Task<bool> ConnectAsync(Wifi wifi);
        Task<bool> DisconnectAsync();
        Task<bool> ForgetAsync(Wifi wifi);
        Task<bool> ConnectToAlreadyConfiguredAsync(int networkId);
        Task<bool> ConnectToRememberedAsync(Wifi wifi);
        Task<bool> AlreadyConnectedAsync(Wifi wifi);
        Task<bool> RemoveNetworkAsync(Wifi wifi);
        bool SetWifiEnabled(bool enabled);
        void StartScan();
    }

    public class NetworkConnectedEventArgs : EventArgs
    {
        private Wifi _network;
        public Wifi Network { get => _network; set => _network = value; }
        private WifiStates _state;
        public WifiStates State { get => _state; set => _state = value; }
        public NetworkConnectedEventArgs(Wifi wifi, WifiStates state)
        {
            Network = wifi;
            State = state;
        }
    }

    public interface IBrodcastSevice
    {
        void Register(IBroadcastReceieverCallback broadcastReceieverCallback);
        void UnRegister();
    }

    public interface IBroadcastReceieverCallback
    {
        void Execute(Wifi network, WifiStates state);
    }
}