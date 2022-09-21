using HeartRateNow.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Windows.ApplicationModel.Background;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Media.SpeechSynthesis;
using Windows.System.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HeartRateNow.Models
{
    public class AppProperties : INotifyPropertyChanged, IDisposable
    {
        /// <summary>
        /// Frees resources held by this class.
        /// </summary>
        public void Dispose()
        {
        }

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        private ObservableCollection<HeartRateMeasurementModel> heartRateMeasurements;
        public ObservableCollection<HeartRateMeasurementModel> HeartRateMeasurements
        {
            get => heartRateMeasurements;
            set
            {
                if (heartRateMeasurements == value)
                { return; }

                heartRateMeasurements = value;
                OnPropertyChanged("HeartRateMeasurements");
            }
        }

        private BackgroundTaskRegistration myBackgroundTaskRegistration;
        public BackgroundTaskRegistration MyBackgroundTaskRegistration
        {
            get => myBackgroundTaskRegistration;
            set
            {
                if (myBackgroundTaskRegistration == value)
                { return; }

                myBackgroundTaskRegistration = value;
                OnPropertyChanged("MyBackgroundTaskRegistration");
            }
        }

        private BluetoothLEDevice selectedBluetoothLEDevice;
        public BluetoothLEDevice SelectedBluetoothLEDevice
        {
            get => selectedBluetoothLEDevice;
            set
            {
                if (selectedBluetoothLEDevice == value)
                { return; }

                selectedBluetoothLEDevice = value;
                OnPropertyChanged("SelectedBluetoothLEDevice");
            }
        }

        private GattDeviceService heartRateGattDeviceService;
        public GattDeviceService HeartRateGattDeviceService
        {
            get => heartRateGattDeviceService;
            set
            {
                if (heartRateGattDeviceService == value)
                { return; }

                heartRateGattDeviceService = value;
                OnPropertyChanged("HeartRateGattDeviceService");
            }
        }

        private GattCharacteristic heartRateGattCharacteristic;
        public GattCharacteristic HeartRateGattCharacteristic
        {
            get => heartRateGattCharacteristic;
            set
            {
                if (heartRateGattCharacteristic == value)
                { return; }

                heartRateGattCharacteristic = value;
                OnPropertyChanged("HeartRateGattCharacteristic");
            }
        }

        private GattDeviceService batteryGattDeviceService;
        public GattDeviceService BatteryGattDeviceService
        {
            get => batteryGattDeviceService;
            set
            {
                if (batteryGattDeviceService == value)
                { return; }

                batteryGattDeviceService = value;
                OnPropertyChanged("BatteryGattDeviceService");
            }
        }

        private GattCharacteristic batteryGattCharacteristic;
        public GattCharacteristic BatteryGattCharacteristic
        {
            get => batteryGattCharacteristic;
            set
            {
                if (batteryGattCharacteristic == value)
                { return; }

                batteryGattCharacteristic = value;
                OnPropertyChanged("BatteryGattCharacteristic");
            }
        }

        private bool isBackgroundTaskRegistered;
        public bool IsBackgroundTaskRegistered
        {
            get => isBackgroundTaskRegistered;
            set
            {
                if (isBackgroundTaskRegistered == value)
                { return; }

                isBackgroundTaskRegistered = value;
                OnPropertyChanged("IsBackgroundTaskRegistered");
            }
        }

        private MediaElement mySpeechMediaElement;
        public MediaElement MySpeechMediaElement
        {
            get => mySpeechMediaElement;
            set
            {
                if (mySpeechMediaElement == value)
                { return; }

                mySpeechMediaElement = value;
                OnPropertyChanged("MySpeechMediaElement");
            }
        }

        private DispatcherTimer speechTimer;
        public DispatcherTimer SpeechTimer
        {
            get => speechTimer;
            set
            {
                if (speechTimer == value)
                { return; }

                speechTimer = value;
                OnPropertyChanged("SpeechTimer");
            }
        }

        private double counterSpeechTimerSeconds;
        public double CounterSpeechTimerSeconds
        {
            get => counterSpeechTimerSeconds;
            set
            {
                if (counterSpeechTimerSeconds == value)
                { return; }

                counterSpeechTimerSeconds = value;
                OnPropertyChanged("CounterSpeechTimerSeconds");
            }
        }

        private bool canSpeak;
        public bool CanSpeak
        {
            get => canSpeak;
            set
            {
                if (canSpeak == value)
                { return; }

                canSpeak = value;
                OnPropertyChanged("CanSpeak");
            }
        }

        private string fileNameDateTime;
        public string FileNameDateTime
        {
            get => fileNameDateTime;
            set
            {
                if (fileNameDateTime == value)
                { return; }

                fileNameDateTime = value;
                OnPropertyChanged("FileNameDateTime");
            }
        }

        private double redZoneHeartRateValue = 120;
        public double RedZoneHeartRateValue
        {
            get => redZoneHeartRateValue;
            set
            {
                if (redZoneHeartRateValue == value)
                { return; }

                redZoneHeartRateValue = value;
                RedZoneHeartRateValueText = $"Red zone @ {value} bpm";
                OnPropertyChanged("RedZoneHeartRateValue");
            }
        }

        private string redZoneHeartRateAboveText = "Above red zone";
        public string RedZoneHeartRateAboveText
        {
            get => redZoneHeartRateAboveText;
            set
            {
                if (redZoneHeartRateAboveText == value)
                { return; }

                redZoneHeartRateAboveText = value;
                OnPropertyChanged("RedZoneHeartRateAboveText");
            }
        }

        private string redZoneHeartRateValueText = "Red zone @ 120 bpm";
        public string RedZoneHeartRateValueText
        {
            get => redZoneHeartRateValueText;
            set
            {
                if (redZoneHeartRateValueText == value)
                { return; }

                redZoneHeartRateValueText = value;
                OnPropertyChanged("RedZoneHeartRateValueText");
            }
        }

        private double yellowZoneHeartRateValue = 100;
        public double YellowZoneHeartRateValue
        {
            get => yellowZoneHeartRateValue;
            set
            {
                if (yellowZoneHeartRateValue == value)
                { return; }

                yellowZoneHeartRateValue = value;
                YellowZoneHeartRateValueText = $"Yellow zone @ {value} bpm";
                OnPropertyChanged("YellowZoneHeartRateValue");
            }
        }

        private string yellowZoneHeartRateAboveText = "Above yellow zone";
        public string YellowZoneHeartRateAboveText
        {
            get => yellowZoneHeartRateAboveText;
            set
            {
                if (yellowZoneHeartRateAboveText == value)
                { return; }

                yellowZoneHeartRateAboveText = value;
                OnPropertyChanged("YellowZoneHeartRateAboveText");
            }
        }

        private string yellowZoneHeartRateValueText = "Yellow zone @ 100 bpm";
        public string YellowZoneHeartRateValueText
        {
            get => yellowZoneHeartRateValueText;
            set
            {
                if (yellowZoneHeartRateValueText == value)
                { return; }

                yellowZoneHeartRateValueText = value;
                OnPropertyChanged("YellowZoneHeartRateValueText");
            }
        }

        private double greenZoneHeartRateValue = 80;
        public double GreenZoneHeartRateValue
        {
            get => greenZoneHeartRateValue;
            set
            {
                if (greenZoneHeartRateValue == value)
                { return; }

                greenZoneHeartRateValue = value;
                GreenZoneHeartRateValueText = $"Green zone @ {value} bpm";
                OnPropertyChanged("GreenZoneHeartRateValue");
            }
        }

        private string greenZoneHeartRateAboveText = "Above green zone";
        public string GreenZoneHeartRateAboveText
        {
            get => greenZoneHeartRateAboveText;
            set
            {
                if (greenZoneHeartRateAboveText == value)
                { return; }

                greenZoneHeartRateAboveText = value;
                OnPropertyChanged("GreenZoneHeartRateAboveText");
            }
        }

        private string greenZoneHeartRateValueText = "Green zone @ 80 bpm";
        public string GreenZoneHeartRateValueText
        {
            get => greenZoneHeartRateValueText;
            set
            {
                if (greenZoneHeartRateValueText == value)
                { return; }

                greenZoneHeartRateValueText = value;
                OnPropertyChanged("GreenZoneHeartRateValueText");
            }
        }

        private DisplayRequest keepScreenOnRequest;
        public DisplayRequest KeepScreenOnRequest
        {
            get => keepScreenOnRequest;
            set
            {
                if (keepScreenOnRequest == value)
                { return; }

                keepScreenOnRequest = value;
                OnPropertyChanged("KeepScreenOnRequest");
            }
        }

        private MainPage currentMainPage;
        public MainPage CurrentMainPage
        {
            get => currentMainPage;
            set
            {
                if (currentMainPage == value)
                { return; }

                currentMainPage = value;
                OnPropertyChanged("CurrentMainPage");
            }
        }

        private ConnectPage currentConnectPage;
        public ConnectPage CurrentConnectPage
        {
            get => currentConnectPage;
            set
            {
                if (currentConnectPage == value)
                { return; }

                currentConnectPage = value;
                OnPropertyChanged("CurrentConnectPage");
            }
        }

        private HeartRatePage currentHeartRatePage;
        public HeartRatePage CurrentHeartRatePage
        {
            get => currentHeartRatePage;
            set
            {
                if (currentHeartRatePage == value)
                { return; }

                currentHeartRatePage = value;
                OnPropertyChanged("CurrentHeartRatePage");
            }
        }

        private string selectedBLEDeviceId;
        public string SelectedBLEDeviceId
        {
            get => selectedBLEDeviceId;
            set
            {
                if (selectedBLEDeviceId == value)
                { return; }

                selectedBLEDeviceId = value;
                OnPropertyChanged("SelectedBLEDeviceId");
            }
        }

        private string selectedBLEDeviceName;
        public string SelectedBLEDeviceName
        {
            get => selectedBLEDeviceName;
            set
            {
                if (selectedBLEDeviceName == value)
                { return; }

                selectedBLEDeviceName = value;
                OnPropertyChanged("SelectedBLEDeviceName");
            }
        }

        private SpeechSynthesizer mySpeechSynthesizer;
        public SpeechSynthesizer MySpeechSynthesizer
        {
            get => mySpeechSynthesizer;
            set
            {
                if (mySpeechSynthesizer == value)
                { return; }

                mySpeechSynthesizer = value;
                OnPropertyChanged("MySpeechSynthesizer");
            }
        }

        private double speechInterval = 10; // seconds
        public double SpeechInterval
        {
            get => speechInterval;
            set
            {
                if (speechInterval == value)
                { return; }

                speechInterval = value;
                SpeechIntervalText = $"Speech interval is {SpeechInterval.ToString()} seconds";
                AppBarButtonSpeakingText = $"Speaking every {SpeechInterval.ToString()} seconds";
                OnPropertyChanged("SpeechInterval");
            }
        }

        private string speechIntervalText = "Speech interval is 10 seconds";
        public string SpeechIntervalText
        {
            get => speechIntervalText;
            set
            {
                if (speechIntervalText == value)
                { return; }

                speechIntervalText = value;
                OnPropertyChanged("SpeechIntervalText");
            }
        }

        private string appBarButtonSpeakingText = "Speaking every 10 seconds";
        public string AppBarButtonSpeakingText
        {
            get => appBarButtonSpeakingText;
            set
            {
                if (appBarButtonSpeakingText == value)
                { return; }

                appBarButtonSpeakingText = value;
                OnPropertyChanged("AppBarButtonSpeakingText");
            }
        }

        #region controlled by SpeakAppBarButtonVisibility
        // use this
        private Visibility speakAppBarButtonVisibility = Visibility.Visible;
        public Visibility SpeakAppBarButtonVisibility
        {
            get => speakAppBarButtonVisibility;

            set
            {
                if (speakAppBarButtonVisibility == value)
                { return; }

                speakAppBarButtonVisibility = value;
                switch (speakAppBarButtonVisibility)
                {
                    case Visibility.Collapsed:
                        MuteAppBarButtonVisibility = Visibility.Visible;
                        ChosenDeviceIsSpeaking = false;
                        break;
                    case Visibility.Visible:
                        MuteAppBarButtonVisibility = Visibility.Collapsed;
                        ChosenDeviceIsSpeaking = true;
                        break;
                    default:
                        break;
                }
                OnPropertyChanged("SpeakAppBarButtonVisibility");
            }
        }

        // don't touch
        // controlled by SpeakAppBarButtonVisibility
        private bool chosenDeviceIsSpeaking = true;
        public bool ChosenDeviceIsSpeaking
        {
            get => chosenDeviceIsSpeaking;

            set
            {
                if (chosenDeviceIsSpeaking == value)
                { return; }

                chosenDeviceIsSpeaking = value;
                OnPropertyChanged("ChosenDeviceIsSpeaking");
            }
        }

        // don't touch
        // controlled by SpeakAppBarButtonVisibility
        private Visibility muteAppBarButtonVisibility = Visibility.Collapsed;
        public Visibility MuteAppBarButtonVisibility
        {
            get => muteAppBarButtonVisibility;

            set
            {
                if (muteAppBarButtonVisibility == value)
                { return; }

                muteAppBarButtonVisibility = value;
                OnPropertyChanged("MuteAppBarButtonVisibility");
            }
        }
        #endregion controlled by SpeakAppBarButtonVisibility

        // use this
        private bool disableLockScreen = true;
        public bool DisableLockScreen
        {
            get => disableLockScreen;
            set
            {
                if (disableLockScreen == value)
                { return; }

                disableLockScreen = value;
                OnPropertyChanged("DisableLockScreen");
            }
        }

        private uint heartRateMeasurement = 0;
        public uint HeartRateMeasurement
        {
            get => heartRateMeasurement;
            set
            {
                if (heartRateMeasurement == value)
                { return; }

                heartRateMeasurement = value;
                OnPropertyChanged("HeartRateMeasurement");
            }
        }

        private string heartRateMeasurementAsSound = "0";
        public string HeartRateMeasurementAsSound
        {
            get => heartRateMeasurementAsSound;
            set
            {
                if (heartRateMeasurementAsSound == value)
                { return; }

                heartRateMeasurementAsSound = value;
                OnPropertyChanged("HeartRateMeasurementAsSound");
            }
        }

        private string heartRateMeasurementTimeStamp = DateTimeOffset.Now.LocalDateTime.ToString();
        public string HeartRateMeasurementTimeStamp
        {
            get => heartRateMeasurementTimeStamp;
            set
            {
                if (heartRateMeasurementTimeStamp == value)
                { return; }

                heartRateMeasurementTimeStamp = value;
                OnPropertyChanged("HeartRateMeasurementTimeStamp");
            }
        }

        private int displayRequestCount = 0;
        public int DisplayRequestCount
        {
            get => displayRequestCount;
            set
            {
                if (displayRequestCount == value)
                { return; }

                displayRequestCount = value;
                OnPropertyChanged("DisplayRequestCount");
            }
        }

        private int batteryLevel = 0;
        public int BatteryLevel
        {
            get => batteryLevel;
            set
            {
                if (batteryLevel == value)
                { return; }

                batteryLevel = value;
                BatteryLevelText = $"Battery level is {batteryLevel} %";
                OnPropertyChanged("BatteryLevel");
            }
        }

        private string batteryLevelText = "Battery level is 0 %";
        public string BatteryLevelText
        {
            get => batteryLevelText;
            set
            {
                if (batteryLevelText == value)
                { return; }

                batteryLevelText = value;
                OnPropertyChanged("BatteryLevelText");
            }
        }

        private double datapointsInMinutes = 10;
        public double DatapointsInMinutes
        {
            get => datapointsInMinutes;
            set
            {
                if (datapointsInMinutes == value)
                { return; }

                datapointsInMinutes = value;
                DatapointsInMinutesText = value == 1 ? $"Show heart rate (bpm) for latest {value} minute" : $"Show heart rate (bpm) for latest {value} minutes";
                PlainDatapointsInMinutesText = value == 1 ? $"heart rate (bpm) for latest {value} minute" : $"heart rate (bpm) for latest {value} minutes";
                OnPropertyChanged("DatapointsInMinutes");
            }
        }

        private string datapointsInMinutesText = "Show heart rate (bpm) for latest 10 minutes";
        public string DatapointsInMinutesText
        {
            get => datapointsInMinutesText;
            set
            {
                if (datapointsInMinutesText == value)
                { return; }

                datapointsInMinutesText = value;
                OnPropertyChanged("DatapointsInMinutesText");
            }
        }

        private string plainDatapointsInMinutesText = "heart rate (bpm) for latest 10 minutes";
        public string PlainDatapointsInMinutesText
        {
            get => plainDatapointsInMinutesText;
            set
            {
                if (plainDatapointsInMinutesText == value)
                { return; }

                plainDatapointsInMinutesText = value;
                OnPropertyChanged("PlainDatapointsInMinutesText");
            }
        }

        private string timeElapsed = "00:00:00";
        public string TimeElapsed
        {
            get => timeElapsed;
            set
            {
                if (timeElapsed == value)
                { return; }

                timeElapsed = value;
                OnPropertyChanged("TimeElapsed");
            }
        }

        private int topHeartRateMeasurement = 0;
        public int TopHeartRateMeasurement
        {
            get => topHeartRateMeasurement;
            set
            {
                if (topHeartRateMeasurement == value)
                { return; }

                topHeartRateMeasurement = value;
                TopHeartRateMeasurementText = $"{value} bpm";
                OnPropertyChanged("TopHeartRateMeasurement");
            }
        }

        private string topHeartRateMeasurementText = "400 bpm";
        public string TopHeartRateMeasurementText
        {
            get => topHeartRateMeasurementText;
            set
            {
                if (topHeartRateMeasurementText == value)
                { return; }

                topHeartRateMeasurementText = value;
                OnPropertyChanged("TopHeartRateMeasurementText");
            }
        }

        private int bottomHeartRateMeasurement = 0;
        public int BottomHeartRateMeasurement
        {
            get => bottomHeartRateMeasurement;
            set
            {
                if (bottomHeartRateMeasurement == value)
                { return; }

                bottomHeartRateMeasurement = value;
                BottomHeartRateMeasurementText = $"{value} bpm";
                OnPropertyChanged("BottomHeartRateMeasurement");
            }
        }

        private string bottomHeartRateMeasurementText = "0 bpm";
        public string BottomHeartRateMeasurementText
        {
            get => bottomHeartRateMeasurementText;
            set
            {
                if (bottomHeartRateMeasurementText == value)
                { return; }

                bottomHeartRateMeasurementText = value;
                OnPropertyChanged("BottomHeartRateMeasurementText");
            }
        }

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion Properties

        public override string ToString()
        {
            return selectedBLEDeviceName;
        }
    }
}
