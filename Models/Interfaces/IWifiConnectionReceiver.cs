using IsObservableCollBuggy.Models.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IWifiConnectionReceiver
    {
        event EventHandler<NetworkConnectedEventArgs> RaiseNetworkConnected;
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
        private Wifi _connectedNetwork;
        public Wifi ConnectedNetwork { get => _connectedNetwork; set => _connectedNetwork = value; }
        public NetworkConnectedEventArgs(Wifi wifi)
        {
            _connectedNetwork = wifi;
        }
    }
}