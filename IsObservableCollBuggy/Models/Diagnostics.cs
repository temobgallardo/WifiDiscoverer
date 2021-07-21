using Models.Interfaces;
using System;
using System.Windows.Input;
using Xamarin.Forms;
using WifiPage.Models.Commons;

namespace IsObservableCollBuggy.Models
{
    public class Diagnostics : ObservableModel, IDisposable
    {
        public const string DIAGNOSTIC_LOCATION_PERMISSION_REQUEST = "DIAGNOSTIC_LOCATION_PERMISSION_REQUEST";

        public ICommand MotorMoveToBotCommand { get; }
        public ICommand MotorMoveToMidCommand { get; }

        public ICommand DevelopmentServerCommand { get; }
        public ICommand StagingServerCommand { get; }
        public ICommand ProductionServerCommand { get; }
        public ICommand GoBackToStrikeSpotOrCycleReadyCommand { get; }
        public ICommand BatteryConnectCommand { get; }
        public ICommand BatteryDisconnectCommand { get; }
        public ICommand ManualLoginCommand { get; }
        public ICommand NfcLoginCommand { get; }
        public ICommand BothLoginCommand { get; }
        public ICommand TabClickCommand { get; }
        public ICommand SaveNewLampCommand { get; }
        public ICommand AdbEnableCommand { get; }
        public ICommand AdbDisableCommand { get; }
        public ICommand SendAtlasLogsCommand { get; }
        public ICommand SendSqliteCommand { get; }
        public ICommand ExitCommand { get; }
        bool _isLocationPermissionGrantedForWifi;

        #region Bindable Props

        private bool _isMoveBottomButtonEnabled = false;
        public bool IsMoveBottomButtonEnabled
        {
            get => _isMoveBottomButtonEnabled;
            set => SetProperty(ref _isMoveBottomButtonEnabled, value);
        }

        private bool _isMoveMiddleButtonEnabled = false;
        public bool IsMoveMiddleButtonEnabled
        {
            get => _isMoveMiddleButtonEnabled;
            set => SetProperty(ref _isMoveMiddleButtonEnabled, value);
        }

        private bool _isMoveTopButtonEnabled = false;
        public bool IsMoveTopButtonEnabled
        {
            get => _isMoveTopButtonEnabled;
            set => SetProperty(ref _isMoveTopButtonEnabled, value);
        }

        private bool _isDevelopmentServerEnabled = true;
        public bool IsDevelopmentServerEnabled
        {
            get => _isDevelopmentServerEnabled;
            set => SetProperty(ref _isDevelopmentServerEnabled, value);
        }

        private bool _isStagingServerEnabled = true;
        public bool IsStagingServerEnabled
        {
            get => _isStagingServerEnabled;
            set => SetProperty(ref _isStagingServerEnabled, value);
        }

        private bool _isProductionServerEnabled = true;
        public bool IsProductionServerEnabled
        {
            get => _isProductionServerEnabled;
            set => SetProperty(ref _isProductionServerEnabled, value);
        }

        private string _greetingLabelText = string.Empty;
        public string GreetingLabelText
        {
            get => _greetingLabelText;
            set => SetProperty(ref _greetingLabelText, value);

        }

        private bool _isBatteryDisconnectButtonEnabled = false;
        public bool BatteryDisconnectButtonEnabled
        {
            get => _isBatteryDisconnectButtonEnabled;
            set => SetProperty(ref _isBatteryDisconnectButtonEnabled, value);
        }

        private bool _isBatteryConnectButtonEnabled = true;
        public bool BatteryConnectButtonEnabled
        {
            get => _isBatteryConnectButtonEnabled;
            set => SetProperty(ref _isBatteryConnectButtonEnabled, value);
        }

        private bool _isManualLoginButtonEnabled = true;
        public bool ManualLoginButtonEnabled
        {
            get => _isManualLoginButtonEnabled;
            set => SetProperty(ref _isManualLoginButtonEnabled, value);
        }

        private bool _isNfcLoginButtonEnabled = false;
        public bool NfcLoginButtonEnabled
        {
            get => _isNfcLoginButtonEnabled;
            set => SetProperty(ref _isNfcLoginButtonEnabled, value);
        }

        private bool _isBothLoginButtonEnabled = false;
        public bool BothLoginButtonEnabled
        {
            get => _isBothLoginButtonEnabled;
            set => SetProperty(ref _isBothLoginButtonEnabled, value);
        }

        private bool _isStatusTabVisible = false;
        public bool IsStatusTabVisible
        {
            get => _isStatusTabVisible;
            set => SetProperty(ref _isStatusTabVisible, value);
        }

        private bool _isDomeTabVisible = false;
        public bool IsDomeTabVisible
        {
            get => _isDomeTabVisible;
            set => SetProperty(ref _isDomeTabVisible, value);
        }

        private bool _isBatteryTabVisible = false;
        public bool IsBatteryTabVisible
        {
            get => _isBatteryTabVisible;
            set => SetProperty(ref _isBatteryTabVisible, value);
        }

        private bool _isAboutTabVisible = false;
        public bool IsAboutTabVisible
        {
            get => _isAboutTabVisible;
            set => SetProperty(ref _isAboutTabVisible, value);
        }

        private bool _isAdvanced = false;
        public bool IsAdvanced
        {
            get => _isAdvanced;
            set => SetProperty(ref _isAdvanced, value);
        }

        private bool _isBasic = true;
        public bool IsBasic
        {
            get => _isBasic;
            set => SetProperty(ref _isBasic, value);
        }

        private bool _isManual = true;
        public bool IsManual
        {
            get => _isManual;
            set => SetProperty(ref _isManual, value);
        }

        private bool _isLampTabVisible = false;
        public bool IsLampTabVisible
        {
            get => _isLampTabVisible;
            set => SetProperty(ref _isLampTabVisible, value);
        }

        private bool _isLoginTabVisible = false;
        public bool IsLoginTabVisible
        {
            get => _isLoginTabVisible;
            set => SetProperty(ref _isLoginTabVisible, value);
        }

        private bool _isServerMenuTabVisible = false;
        public bool IsServerMenuTabVisible
        {
            get => _isServerMenuTabVisible;
            set => SetProperty(ref _isServerMenuTabVisible, value);
        }

        private bool _isAdbTabVisible = false;
        public bool IsAdbTabVisible
        {
            get => _isAdbTabVisible;
            set => SetProperty(ref _isAdbTabVisible, value);
        }

        private string _lampSerialNumber;
        public string LampSerialNumber
        {
            get => _lampSerialNumber;
            set => SetProperty(ref _lampSerialNumber, value);
            
        }

        private string _lampInstructionMessage;
        public string LampInstructionMessage
        {
            get => _lampInstructionMessage;
            set => SetProperty(ref _lampInstructionMessage, value);
        }

        private string _installOrReplaceLampLabel;
        public string InstallOrReplaceLampLabel
        {
            get => _installOrReplaceLampLabel;
            set => SetProperty(ref _installOrReplaceLampLabel, value);
        }

        private bool _isLampInstalled;
        public bool IsLampInstalled
        {
            get => _isLampInstalled;
            set => SetProperty(ref _isLampInstalled, value);
        }

        private string _lampPulseCount;
        public string LampPulseCount
        {
            get => _lampPulseCount;
            set => SetProperty(ref _lampPulseCount, value);
        }

        private bool _isAdbDisabled;
        public bool IsAdbDisabled
        {
            get => _isAdbDisabled;
            set => SetProperty(ref _isAdbDisabled, value);
        }

        private bool _isAdbEnabled;
        public bool IsAdbEnabled
        {
            get => _isAdbEnabled;
            set => SetProperty(ref _isAdbEnabled, value);
        }

        private bool _isSendLogsEnabled;
        public bool IsSendLogsEnabled
        {
            get => _isSendLogsEnabled;
            set => SetProperty(ref _isSendLogsEnabled, value);
        }

        private bool _isSendDbEnabled;
        public bool IsSendDbEnabled
        {
            get => _isSendDbEnabled;
            set => SetProperty(ref _isSendDbEnabled, value);
        }

        private bool _isPlugInAcBtnVisible = false;
        public bool IsPlugInAcBtnVisible
        {
            get => _isPlugInAcBtnVisible;
            set => SetProperty(ref _isPlugInAcBtnVisible, value);
        }

        private string _acErrorMessage;
        public string AcErrorMessage
        {
            get => _acErrorMessage;
            set => SetProperty(ref _acErrorMessage, value);
        }

        private string _atlasVersion;
        public string AtlasVersion
        {
            get => _atlasVersion;
            set => SetProperty(ref _atlasVersion, value);
        }

        private string _deviceId;
        public string DeviceId
        {
            get => _deviceId;
            set => SetProperty(ref _deviceId, value);
        }

        private string _commitHash;
        public string CommitHash
        {
            get => _commitHash;
            set => SetProperty(ref _commitHash, value);
        }

        private string _branchName;
        public string BranchName
        {
            get => _branchName;
            set => SetProperty(ref _branchName, value);
        }

        private string _buildTime;
        public string BuildTime
        {
            get => _buildTime;
            set => SetProperty(ref _buildTime, value);
        }

        private string _buildMachine;
        public string BuildMachine
        {
            get => _buildMachine;
            set => SetProperty(ref _buildMachine, value);
        }

        private string _usersLastSyncTimeStamp;
        public string UsersLastSyncTimeStamp
        {
            get => _usersLastSyncTimeStamp;
            set => SetProperty(ref _usersLastSyncTimeStamp, value);
        }

        private string _roomsLastSyncTimeStamp;
        public string RoomsLastSyncTimeStamp
        {
            get => _roomsLastSyncTimeStamp;
            set => SetProperty(ref _roomsLastSyncTimeStamp, value);
        }

        private string _lampLastSyncTimeStamp;
        public string LampLastSyncTimeStamp
        {
            get => _lampLastSyncTimeStamp;
            set => SetProperty(ref _lampLastSyncTimeStamp, value);
        }

        private string _gladosFirmwareVersion;
        public string GladosFirmwareVersion
        {
            get => _gladosFirmwareVersion;
            set => SetProperty(ref _gladosFirmwareVersion, value);
        }

        private string _gladosHardwareVersion;
        public string GladosHardwareVersion
        {
            get => _gladosHardwareVersion;
            set => SetProperty(ref _gladosHardwareVersion, value);
        }

        private string _safetyControlFirmwareVersion;
        public string SafetyControlFirmwareVersion
        {
            get => _safetyControlFirmwareVersion;
            set => SetProperty(ref _safetyControlFirmwareVersion, value);
        }

        private string _remoteConeFirmwareVersion;
        public string RemoteConeFirmwareVersion
        {
            get => _remoteConeFirmwareVersion;
            set => SetProperty(ref _remoteConeFirmwareVersion, value);
        }

        private string _remoteConeHardwareVersion;
        public string RemoteConeHardwareVersion
        {
            get => _remoteConeHardwareVersion;
            set => SetProperty(ref _remoteConeHardwareVersion, value);
        }

        private string _remoteConeSPFirmwareVersion;
        public string RemoteConeSPFirmwareVersion
        {
            get => _remoteConeSPFirmwareVersion;
            set => SetProperty(ref _remoteConeSPFirmwareVersion, value);
        }

        private string _powerDistributionFirmwareVersion;
        public string PowerDistributionFirmwareVersion
        {
            get => _powerDistributionFirmwareVersion;
            set => SetProperty(ref _powerDistributionFirmwareVersion, value);
        }

        private string _powerDistributionHardwareVersion;
        public string PowerDistributionHardwareVersion
        {
            get => _powerDistributionHardwareVersion;
            set => SetProperty(ref _powerDistributionHardwareVersion, value);
        }

        private string _primaryMotionConeFirmwareVersion;
        public string PrimaryMotionConeFirmwareVersion
        {
            get => _primaryMotionConeFirmwareVersion;
            set => SetProperty(ref _primaryMotionConeFirmwareVersion, value);
        }

        private string _primaryMotionConeHardwareVersion;
        public string PrimaryMotionConeHardwareVersion
        {
            get => _primaryMotionConeHardwareVersion;
            set => SetProperty(ref _primaryMotionConeHardwareVersion, value);
        }

        private string _secondaryMotionConeFirmwareVersion;
        public string SecondaryMotionConeFirmwareVersion
        {
            get => _secondaryMotionConeFirmwareVersion;
            set => SetProperty(ref _secondaryMotionConeFirmwareVersion, value);
        }

        private string _secondaryMotionConeHardwareVersion;
        public string SecondaryMotionConeHardwareVersion
        {
            get => _secondaryMotionConeHardwareVersion;
            set => SetProperty(ref _secondaryMotionConeHardwareVersion, value);
        }

        private string _idDongleFirmwareVersion;
        public string IDDongleFirmwareVersion
        {
            get => _idDongleFirmwareVersion;
            set => SetProperty(ref _idDongleFirmwareVersion, value);
        }

        private string _idDongleHardwareVersion;
        public string IdDongleHardwareVersion
        {
            get => _idDongleHardwareVersion;
            set => SetProperty(ref _idDongleHardwareVersion, value);
        }

        private bool _isSaveNewLampEnabled = true;
        public bool IsSaveNewLampEnabled
        {
            get => _isSaveNewLampEnabled;
            set => SetProperty(ref _isSaveNewLampEnabled, value);
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set => SetProperty(ref _serialNumber, value);
        }

        private string _messageLabelText;
        public string MessageLabelText
        {
            get => _messageLabelText;
            set => SetProperty(ref _messageLabelText, value);
        }

        private Color _messageTextColor;
        public Color MessageTextColor
        {
            get => _messageTextColor;
            set => SetProperty(ref _messageTextColor, value);
        }

        private bool _isMessageVisible = false;
        public bool IsMessageVisible
        {
            get => _isMessageVisible;
            set => SetProperty(ref _isMessageVisible, value);
        }

        private bool _isBackBtnEnabled = true;
        public bool IsBackBtnEnabled
        {
            get => _isBackBtnEnabled;
            set => SetProperty(ref _isBackBtnEnabled, value); /*});*/
        }

        private bool _isWifiTabVisible = false;
        public bool IsWifiTabVisible
        {
            get => _isWifiTabVisible;
            set => SetProperty(ref _isWifiTabVisible, value);
        }
        #endregion

        public Diagnostics()
        {
            TabClickCommand = new Command((tab) => SelectTab(tab));
        }

        public void OnAppearing()
        {
            EnableWifiTab();
            //EnableTabBasedOnPermission();
        }

        void EnableTabBasedOnPermission()
        {
            if (_isLocationPermissionGrantedForWifi)
            {
                EnableWifiTab();
                return;
            }

            //EnableStatusTab();
        }

        void EnableStatusTab2(string tabToEnable, bool enable)
        {
            switch (tabToEnable)
            {
                case nameof(IsStatusTabVisible):
                    IsStatusTabVisible = enable;
                    break;
                case nameof(IsDomeTabVisible):
                    IsDomeTabVisible = enable;
                    break;
                case nameof(IsBatteryTabVisible):
                    IsBatteryTabVisible = enable;
                    break;
                case nameof(IsLampTabVisible):
                    IsLampTabVisible = enable;
                    break;
                case nameof(IsLoginTabVisible):
                    IsLoginTabVisible = enable;
                    break;
                case nameof(IsAdbTabVisible):
                    IsAdbTabVisible = false;
                    break;
                case nameof(IsAboutTabVisible):
                    IsAboutTabVisible = false;
                    break;
                case nameof(IsServerMenuTabVisible):
                    IsServerMenuTabVisible = false;
                    break;
                case nameof(IsWifiTabVisible):
                    IsWifiTabVisible = false;
                    break;
            }
        }

        void EnableStatusTab()
        {
            IsStatusTabVisible = true;
            IsDomeTabVisible = false;
            IsBatteryTabVisible = false;
            IsLampTabVisible = false;
            IsLoginTabVisible = false;
            IsAdbTabVisible = false;
            IsAboutTabVisible = false;
            IsServerMenuTabVisible = false;
            IsWifiTabVisible = false;
        }
        void EnableWifiTab()
        {
            IsStatusTabVisible = false;
            IsDomeTabVisible = false;
            IsBatteryTabVisible = false;
            IsLampTabVisible = false;
            IsLoginTabVisible = false;
            IsAdbTabVisible = false;
            IsAboutTabVisible = false;
            IsServerMenuTabVisible = false;
            FocusWifiTab(isSelected: true);
        }

        public ICommand MotorMoveToTopCommand { get; }

        public void OnDisappearing()
        {
        }

        private void SelectTab(object tab)
        {
            var tabTypeValue = (string)tab;
            IsStatusTabVisible = tabTypeValue == "Status";
            IsDomeTabVisible = tabTypeValue == "Dome";
            IsBatteryTabVisible = tabTypeValue == "Battery";
            IsLampTabVisible = tabTypeValue == "Lamp";
            IsLoginTabVisible = tabTypeValue == "Login";
            IsAdbTabVisible = tabTypeValue == "Adb";
            IsAboutTabVisible = tabTypeValue == "About";
            IsServerMenuTabVisible = tabTypeValue == "ServerMenu";
            FocusWifiTab(tabTypeValue == "Wifi");
        }

        private void FocusWifiTab(bool isSelected)
        {
            IsWifiTabVisible = isSelected;

            // Checking Android Location Permission or requesting them
            if (IsWifiTabVisible)
            {
                // TODO; Make a services instead. This services will request on the screen for Location Permission.
                MessagingCenter.Send(this, DIAGNOSTIC_LOCATION_PERMISSION_REQUEST);
                MessagingCenter.Subscribe<IAccessLocationPermission, bool>(this, "MainActivity.RequestLocationPermision", (s, a) => SetIsWifiTabVisible(a));
            }
        }

        private void SetIsWifiTabVisible(bool isPermissionGranted)
        {
            IsWifiTabVisible = isPermissionGranted;
            _isLocationPermissionGrantedForWifi = isPermissionGranted;

            if (isPermissionGranted)
            {
                MessagingCenter.Unsubscribe<IAccessLocationPermission, bool>(this, "MainActivity.RequestLocationPermision");
            }
        }

        public void Dispose()
        {
            MessagingCenter.Unsubscribe<IAccessLocationPermission>(this, "MainActivity.RequestLocationPermision");
        }
    }
}

