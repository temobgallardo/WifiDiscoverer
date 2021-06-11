using IsObservableCollBuggy.Models.Models;
using System.Collections.Generic;

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
        bool Connect(Wifi wifi);
        bool Disconnect();
        bool Forget(Wifi wifi);
        bool ConnectToAlreadyConfigured(int networkId);
        bool ConnectToRemembered(Wifi wifi);
        bool AlreadyConnected(Wifi wifi);
        bool SetWifiEnabled(bool enabled);
        void StartScan();
    }
}