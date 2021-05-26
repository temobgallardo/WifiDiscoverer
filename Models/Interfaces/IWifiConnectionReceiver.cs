using IsObservableCollBuggy.Models.Models;
using System.Collections.Generic;

namespace Models.Interfaces
{
    public interface IWifiConnectionReceiver
    {
        string WifiConnectionReceiverMessage { get; }
        bool IsWifiEnabled { get; }
        IList<Wifi> Wifis { get; }
        IList<Wifi> ScanFailure();
        IList<Wifi> ScanSuccess();
        bool ConnectToWifi(Wifi wifi);
        bool ConnectToAlreadyConfigured(int networkId);
        bool ConnectToRememberedNetwork(Wifi wifi);
        bool AlreadyConnected(Wifi wifi);
        bool SetWifiEnabled(bool enabled);
        void StartScan();
    }
}