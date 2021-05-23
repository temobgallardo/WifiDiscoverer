using XamarinUniversity.Infrastructure;

namespace IsObservableCollBuggy.Models
{
    public class StatusBar : SimpleViewModel
    {
        #region Properties

        private string _timeClockLabelText;
        public string TimeClockLabelText
        {
            get => _timeClockLabelText;
            set
            {
                _timeClockLabelText = value;
                RaisePropertyChanged(() => TimeClockLabelText);
            }
        }

        private string _logicDeviceName;
        public string LogicDeviceName
        {
            get => _logicDeviceName;
            set
            {
                _logicDeviceName = value;
                RaisePropertyChanged(() => LogicDeviceName);
            }
        }

        private string _roomTagsDetectedImageName = "light_strike_off";
        public string RoomTagsDetectedImageName
        {
            get => _roomTagsDetectedImageName;
            set
            {
                _roomTagsDetectedImageName = value;
                RaisePropertyChanged(() => RoomTagsDetectedImageName);
            }
        }

        private string _cellSignalLevelImageName = "signal_zero";
        public string CellSignalLevelImageName
        {
            get => _cellSignalLevelImageName;
            set
            {
                _cellSignalLevelImageName = value;
                RaisePropertyChanged(() => CellSignalLevelImageName);
            }
        }

        private string _flightModeImageName = "flight_mode";
        public string FlightModeImageName
        {
            get => _flightModeImageName;
            set
            {
                _flightModeImageName = value;
                RaisePropertyChanged(() => FlightModeImageName);
            }
        }

        private string _wifiSignalLevelImageName = string.Empty;
        public string WifiSignalLevelImageName
        {
            get => _wifiSignalLevelImageName;
            set
            {
                _wifiSignalLevelImageName = value;
                RaisePropertyChanged(() => WifiSignalLevelImageName);
            }
        }

        private string _batteryLevelImageName = "Battery_Icon_Full";
        public string BatteryLevelImageName
        {
            get => _batteryLevelImageName;
            set
            {
                _batteryLevelImageName = value;
                RaisePropertyChanged(() => BatteryLevelImageName);
            }
        }

        private string _batteryTime = "";
        public string BatteryTime
        {
            get => _batteryTime;
            set
            {
                _batteryTime = value;
                RaisePropertyChanged(() => BatteryTime);
            }
        }

        private bool _isFlightModeOff = true;
        public bool IsFlightModeOff
        {
            get => _isFlightModeOff;
            set
            {
                _isFlightModeOff = value;
                RaisePropertyChanged(() => IsFlightModeOff);
            }
        }
        #endregion

    }
}
