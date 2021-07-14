using Models.Interfaces;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using XamarinUniversity.Infrastructure;
using Xamarin.Essentials;

namespace IsObservableCollBuggy.Models
{
    public class Diagnostics : SimpleViewModel, IDisposable
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
            set
            {
                _isMoveBottomButtonEnabled = value;
                RaisePropertyChanged(() => IsMoveBottomButtonEnabled);
            }
        }

        private bool _isMoveMiddleButtonEnabled = false;
        public bool IsMoveMiddleButtonEnabled
        {
            get => _isMoveMiddleButtonEnabled;
            set
            {
                _isMoveMiddleButtonEnabled = value;
                RaisePropertyChanged(() => IsMoveMiddleButtonEnabled);
            }
        }

        private bool _isMoveTopButtonEnabled = false;
        public bool IsMoveTopButtonEnabled
        {
            get => _isMoveTopButtonEnabled;
            set
            {
                _isMoveTopButtonEnabled = value;
                RaisePropertyChanged(() => IsMoveTopButtonEnabled);
            }
        }

        private bool _isDevelopmentServerEnabled = true;
        public bool IsDevelopmentServerEnabled
        {
            get => _isDevelopmentServerEnabled;
            set
            {
                _isDevelopmentServerEnabled = value;
                RaisePropertyChanged(() => IsDevelopmentServerEnabled);
            }
        }

        private bool _isStagingServerEnabled = true;
        public bool IsStagingServerEnabled
        {
            get => _isStagingServerEnabled;
            set
            {
                _isStagingServerEnabled = value;
                RaisePropertyChanged(() => IsStagingServerEnabled);
            }
        }

        private bool _isProductionServerEnabled = true;
        public bool IsProductionServerEnabled
        {
            get => _isProductionServerEnabled;
            set
            {
                _isProductionServerEnabled = value;
                RaisePropertyChanged(() => IsProductionServerEnabled);
            }
        }

        private string _greetingLabelText = string.Empty;
        public string GreetingLabelText
        {
            get => _greetingLabelText;
            set
            {
                _greetingLabelText = value;
                RaisePropertyChanged(() => GreetingLabelText);
            }
        }

        private bool _isBatteryDisconnectButtonEnabled = false;
        public bool BatteryDisconnectButtonEnabled
        {
            get => _isBatteryDisconnectButtonEnabled;
            set
            {
                _isBatteryDisconnectButtonEnabled = value;
                RaisePropertyChanged(() => BatteryDisconnectButtonEnabled);
            }
        }

        private bool _isBatteryConnectButtonEnabled = true;
        public bool BatteryConnectButtonEnabled
        {
            get => _isBatteryConnectButtonEnabled;
            set
            {
                _isBatteryConnectButtonEnabled = value;
                RaisePropertyChanged(() => BatteryConnectButtonEnabled);
            }
        }

        private bool _isManualLoginButtonEnabled = true;
        public bool ManualLoginButtonEnabled
        {
            get => _isManualLoginButtonEnabled;
            set
            {
                _isManualLoginButtonEnabled = value;
                RaisePropertyChanged(() => ManualLoginButtonEnabled);
            }
        }

        private bool _isNfcLoginButtonEnabled = false;
        public bool NfcLoginButtonEnabled
        {
            get => _isNfcLoginButtonEnabled;
            set
            {
                _isNfcLoginButtonEnabled = value;
                RaisePropertyChanged(() => NfcLoginButtonEnabled);
            }
        }

        private bool _isBothLoginButtonEnabled = false;
        public bool BothLoginButtonEnabled
        {
            get => _isBothLoginButtonEnabled;
            set
            {
                _isBothLoginButtonEnabled = value;
                RaisePropertyChanged(() => BothLoginButtonEnabled);
            }
        }

        private bool _isStatusTabVisible = false;
        public bool IsStatusTabVisible
        {
            get => _isStatusTabVisible;
            set
            {
                _isStatusTabVisible = value;
                RaisePropertyChanged(() => IsStatusTabVisible);
            }
        }

        private bool _isDomeTabVisible = false;
        public bool IsDomeTabVisible
        {
            get => _isDomeTabVisible;
            set
            {
                _isDomeTabVisible = value;
                RaisePropertyChanged(() => IsDomeTabVisible);
            }
        }

        private bool _isBatteryTabVisible = false;
        public bool IsBatteryTabVisible
        {
            get => _isBatteryTabVisible;
            set
            {
                _isBatteryTabVisible = value;
                RaisePropertyChanged(() => IsBatteryTabVisible);
            }
        }

        private bool _isAboutTabVisible = false;
        public bool IsAboutTabVisible
        {
            get => _isAboutTabVisible;
            set
            {
                _isAboutTabVisible = value;
                RaisePropertyChanged(() => IsAboutTabVisible);
            }
        }

        private bool _isAdvanced = false;
        public bool IsAdvanced
        {
            get => _isAdvanced;
            set
            {
                _isAdvanced = value;
                RaisePropertyChanged(() => IsAdvanced);
            }
        }

        private bool _isBasic = true;
        public bool IsBasic
        {
            get => _isBasic;
            set
            {
                _isBasic = value;
                RaisePropertyChanged(() => IsBasic);
            }
        }

        private bool _isManual = true;
        public bool IsManual
        {
            get => _isManual;
            set
            {
                _isManual = value;
                RaisePropertyChanged(() => IsManual);
            }
        }

        private bool _isLampTabVisible = false;
        public bool IsLampTabVisible
        {
            get => _isLampTabVisible;
            set
            {
                _isLampTabVisible = value;
                RaisePropertyChanged(() => IsLampTabVisible);
            }
        }

        private bool _isLoginTabVisible = false;
        public bool IsLoginTabVisible
        {
            get => _isLoginTabVisible;
            set
            {
                _isLoginTabVisible = value;
                RaisePropertyChanged(() => IsLoginTabVisible);
            }
        }

        private bool _isServerMenuTabVisible = false;
        public bool IsServerMenuTabVisible
        {
            get => _isServerMenuTabVisible;
            set
            {
                _isServerMenuTabVisible = value;
                RaisePropertyChanged(() => IsServerMenuTabVisible);
            }
        }

        private bool _isAdbTabVisible = false;
        public bool IsAdbTabVisible
        {
            get => _isAdbTabVisible;
            set
            {
                _isAdbTabVisible = value;
                RaisePropertyChanged(() => IsAdbTabVisible);
            }
        }

        private string _lampSerialNumber;
        public string LampSerialNumber
        {
            get => _lampSerialNumber;
            set
            {
                _lampSerialNumber = value;
                RaisePropertyChanged(() => LampSerialNumber);
            }
        }

        private string _lampInstructionMessage;
        public string LampInstructionMessage
        {
            get => _lampInstructionMessage;
            set
            {
                _lampInstructionMessage = value;
                RaisePropertyChanged(() => LampInstructionMessage);
            }
        }

        private string _installOrReplaceLampLabel;
        public string InstallOrReplaceLampLabel
        {
            get => _installOrReplaceLampLabel;
            set
            {
                _installOrReplaceLampLabel = value;
                RaisePropertyChanged(() => InstallOrReplaceLampLabel);
            }
        }

        private bool _isLampInstalled;
        public bool IsLampInstalled
        {
            get => _isLampInstalled;
            set
            {
                _isLampInstalled = value;
                RaisePropertyChanged(() => IsLampInstalled);
            }
        }

        private string _lampPulseCount;
        public string LampPulseCount
        {
            get => _lampPulseCount;
            set
            {
                _lampPulseCount = value;
                RaisePropertyChanged(() => LampPulseCount);
            }
        }

        private bool _isAdbDisabled;
        public bool IsAdbDisabled
        {
            get => _isAdbDisabled;
            set
            {
                _isAdbDisabled = value;
                RaisePropertyChanged(() => IsAdbDisabled);
            }
        }

        private bool _isAdbEnabled;
        public bool IsAdbEnabled
        {
            get => _isAdbEnabled;
            set
            {
                _isAdbEnabled = value;
                RaisePropertyChanged(() => IsAdbEnabled);
            }
        }

        private bool _isSendLogsEnabled;
        public bool IsSendLogsEnabled
        {
            get => _isSendLogsEnabled;
            set
            {
                _isSendLogsEnabled = value;
                RaisePropertyChanged(() => IsSendLogsEnabled);
            }
        }

        private bool _isSendDbEnabled;
        public bool IsSendDbEnabled
        {
            get => _isSendDbEnabled;
            set
            {
                _isSendDbEnabled = value;
                RaisePropertyChanged(() => IsSendDbEnabled);
            }
        }

        private bool _isPlugInAcBtnVisible = false;
        public bool IsPlugInAcBtnVisible
        {
            get => _isPlugInAcBtnVisible;
            set
            {
                _isPlugInAcBtnVisible = value;
                RaisePropertyChanged(() => IsPlugInAcBtnVisible);
            }
        }

        private string _acErrorMessage;
        public string AcErrorMessage
        {
            get => _acErrorMessage;
            set
            {
                _acErrorMessage = value;
                RaisePropertyChanged(() => AcErrorMessage);
            }
        }

        private string _atlasVersion;
        public string AtlasVersion
        {
            get => _atlasVersion;
            set
            {
                _atlasVersion = value;
                RaisePropertyChanged(() => AtlasVersion);
            }
        }

        private string _deviceId;
        public string DeviceId
        {
            get => _deviceId;
            set
            {
                _deviceId = value;
                RaisePropertyChanged(() => DeviceId);
            }
        }

        private string _commitHash;
        public string CommitHash
        {
            get => _commitHash;
            set
            {
                _commitHash = value;
                RaisePropertyChanged(() => CommitHash);
            }
        }

        private string _branchName;
        public string BranchName
        {
            get => _branchName;
            set
            {
                _branchName = value;
                RaisePropertyChanged(() => BranchName);
            }
        }

        private string _buildTime;
        public string BuildTime
        {
            get => _buildTime;
            set
            {
                _buildTime = value;
                RaisePropertyChanged(() => BuildTime);
            }
        }

        private string _buildMachine;
        public string BuildMachine
        {
            get => _buildMachine;
            set
            {
                _buildMachine = value;
                RaisePropertyChanged(() => BuildMachine);
            }
        }

        private string _usersLastSyncTimeStamp;
        public string UsersLastSyncTimeStamp
        {
            get => _usersLastSyncTimeStamp;
            set
            {
                _usersLastSyncTimeStamp = value;
                RaisePropertyChanged(() => UsersLastSyncTimeStamp);
            }
        }

        private string _roomsLastSyncTimeStamp;
        public string RoomsLastSyncTimeStamp
        {
            get => _roomsLastSyncTimeStamp;
            set
            {
                _roomsLastSyncTimeStamp = value;
                RaisePropertyChanged(() => RoomsLastSyncTimeStamp);
            }
        }

        private string _lampLastSyncTimeStamp;
        public string LampLastSyncTimeStamp
        {
            get => _lampLastSyncTimeStamp;
            set
            {
                _lampLastSyncTimeStamp = value;
                RaisePropertyChanged(() => LampLastSyncTimeStamp);
            }
        }

        private string _gladosFirmwareVersion;
        public string GladosFirmwareVersion
        {
            get => _gladosFirmwareVersion;
            set
            {
                _gladosFirmwareVersion = value;
                RaisePropertyChanged(() => GladosFirmwareVersion);
            }
        }

        private string _gladosHardwareVersion;
        public string GladosHardwareVersion
        {
            get => _gladosHardwareVersion;
            set
            {
                _gladosHardwareVersion = value;
                RaisePropertyChanged(() => GladosHardwareVersion);
            }
        }

        private string _safetyControlFirmwareVersion;
        public string SafetyControlFirmwareVersion
        {
            get => _safetyControlFirmwareVersion;
            set
            {
                _safetyControlFirmwareVersion = value;
                RaisePropertyChanged(() => SafetyControlFirmwareVersion);
            }
        }

        private string _remoteConeFirmwareVersion;
        public string RemoteConeFirmwareVersion
        {
            get => _remoteConeFirmwareVersion;
            set
            {
                _remoteConeFirmwareVersion = value;
                RaisePropertyChanged(() => RemoteConeFirmwareVersion);
            }
        }

        private string _remoteConeHardwareVersion;
        public string RemoteConeHardwareVersion
        {
            get => _remoteConeHardwareVersion;
            set
            {
                _remoteConeHardwareVersion = value;
                RaisePropertyChanged(() => RemoteConeHardwareVersion);
            }
        }

        private string _remoteConeSPFirmwareVersion;
        public string RemoteConeSPFirmwareVersion
        {
            get => _remoteConeSPFirmwareVersion;
            set
            {
                _remoteConeSPFirmwareVersion = value;
                RaisePropertyChanged(() => RemoteConeSPFirmwareVersion);
            }
        }

        private string _powerDistributionFirmwareVersion;
        public string PowerDistributionFirmwareVersion
        {
            get => _powerDistributionFirmwareVersion;
            set
            {
                _powerDistributionFirmwareVersion = value;
                RaisePropertyChanged(() => PowerDistributionFirmwareVersion);
            }
        }

        private string _powerDistributionHardwareVersion;
        public string PowerDistributionHardwareVersion
        {
            get => _powerDistributionHardwareVersion;
            set
            {
                _powerDistributionHardwareVersion = value;
                RaisePropertyChanged(() => PowerDistributionHardwareVersion);
            }
        }

        private string _primaryMotionConeFirmwareVersion;
        public string PrimaryMotionConeFirmwareVersion
        {
            get => _primaryMotionConeFirmwareVersion;
            set
            {
                _primaryMotionConeFirmwareVersion = value;
                RaisePropertyChanged(() => PrimaryMotionConeFirmwareVersion);
            }
        }

        private string _primaryMotionConeHardwareVersion;
        public string PrimaryMotionConeHardwareVersion
        {
            get => _primaryMotionConeHardwareVersion;
            set
            {
                _primaryMotionConeHardwareVersion = value;
                RaisePropertyChanged(() => PrimaryMotionConeHardwareVersion);
            }
        }

        private string _secondaryMotionConeFirmwareVersion;
        public string SecondaryMotionConeFirmwareVersion
        {
            get => _secondaryMotionConeFirmwareVersion;
            set
            {
                _secondaryMotionConeFirmwareVersion = value;
                RaisePropertyChanged(() => SecondaryMotionConeFirmwareVersion);
            }
        }

        private string _secondaryMotionConeHardwareVersion;
        public string SecondaryMotionConeHardwareVersion
        {
            get => _secondaryMotionConeHardwareVersion;
            set
            {
                _secondaryMotionConeHardwareVersion = value;
                RaisePropertyChanged(() => SecondaryMotionConeHardwareVersion);
            }
        }

        private string _idDongleFirmwareVersion;
        public string IDDongleFirmwareVersion
        {
            get => _idDongleFirmwareVersion;
            set
            {
                _idDongleFirmwareVersion = value;
                RaisePropertyChanged(() => IDDongleFirmwareVersion);
            }
        }

        private string _idDongleHardwareVersion;
        public string IdDongleHardwareVersion
        {
            get => _idDongleHardwareVersion;
            set
            {
                _idDongleHardwareVersion = value;
                RaisePropertyChanged(() => IdDongleHardwareVersion);
            }
        }

        private bool _isSaveNewLampEnabled = true;
        public bool IsSaveNewLampEnabled
        {
            get => _isSaveNewLampEnabled;
            set
            {
                _isSaveNewLampEnabled = value;
                RaisePropertyChanged(() => IsSaveNewLampEnabled);
            }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set
            {
                _serialNumber = value;
                RaisePropertyChanged(() => SerialNumber);
            }
        }

        private string _messageLabelText;
        public string MessageLabelText
        {
            get => _messageLabelText;
            set
            {
                _messageLabelText = value;
                RaisePropertyChanged(() => MessageLabelText);
            }
        }

        private Color _messageTextColor;
        public Color MessageTextColor
        {
            get => _messageTextColor;
            set
            {
                _messageTextColor = value;
                RaisePropertyChanged(() => MessageTextColor);
            }
        }

        private bool _isMessageVisible = false;
        public bool IsMessageVisible
        {
            get => _isMessageVisible;
            set
            {
                _isMessageVisible = value;
                RaisePropertyChanged(() => IsMessageVisible);
            }
        }

        private bool _isBackBtnEnabled = true;
        public bool IsBackBtnEnabled
        {
            get => _isBackBtnEnabled;
            set
            {
                _isBackBtnEnabled = value;
                /*Device.BeginInvokeOnMainThread(() => {*/
                RaisePropertyChanged(() => IsBackBtnEnabled); /*});*/
            }
        }

        private bool _isWifiTabVisible = false;
        public bool IsWifiTabVisible
        {
            get => _isWifiTabVisible;
            set
            {
                _isWifiTabVisible = value;

                RaisePropertyChanged(() => IsWifiTabVisible);
            }
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

