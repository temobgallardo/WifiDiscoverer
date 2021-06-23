using IsObservableCollBuggy.Models.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Models.Interfaces
{
    public interface IWifiConnectionReceiver
    {
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
        bool SetWifiEnabled(bool enabled);
        void StartScan();
    }
}