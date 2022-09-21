using HeartRateNow.Helpers;
using HeartRateNow.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Media.SpeechSynthesis;
using Windows.Security.Cryptography;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace HeartRateNow.Views
{
    public sealed partial class ConnectPage : Page
    {
        private AppProperties myAppViewModel = App.MyApp;
        public AppProperties MyAppViewModel
        {
            get { return myAppViewModel; }
        }

        private DispatcherTimer speechTimer;
        public DispatcherTimer SpeechTimer { get => speechTimer; set => speechTimer = value; }

        private double counterIntervalSeconds;
        public double CounterIntervalSeconds { get => counterIntervalSeconds; set => counterIntervalSeconds = value; }

        private MediaElement mainPageMediaElement;
        public MediaElement MainPageMediaElement { get => mainPageMediaElement; set => mainPageMediaElement = value; }

        private BluetoothLEDevice selectedBluetoothLeDevice;
        public BluetoothLEDevice SelectedBluetoothLeDevice { get => selectedBluetoothLeDevice; set => selectedBluetoothLeDevice = value; }

        private GattDeviceService heartRateGattDeviceService;
        public GattDeviceService HeartRateGattDeviceService { get => heartRateGattDeviceService; set => heartRateGattDeviceService = value; }

        private GattCharacteristic heartRateCharacteristic;
        public GattCharacteristic HeartRateCharacteristic { get => heartRateCharacteristic; set => heartRateCharacteristic = value; }

        private GattDeviceService batteryGattDeviceService;
        public GattDeviceService BatteryGattDeviceService { get => batteryGattDeviceService; set => batteryGattDeviceService = value; }

        private GattCharacteristic batteryCharacteristic;
        public GattCharacteristic BatteryCharacteristic { get => batteryCharacteristic; set => batteryCharacteristic = value; }

        private bool isValueChangedHandlerRegistered;
        public bool IsValueChangedHandlerRegistered { get => isValueChangedHandlerRegistered; set => isValueChangedHandlerRegistered = value; }

        public ConnectPage()
        {
            InitializeComponent();

            MyAppViewModel.CurrentConnectPage = this;

            CounterIntervalSeconds = MyAppViewModel.SpeechInterval;

            SpeechTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

            SpeechTimer.Tick += SpeechTimer_Tick;

            MainPageMediaElement = MyAppViewModel.CurrentMainPage.MultimediaMediaElement;

            Application.Current.Suspending += Current_Suspending;
        }

        private async void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();

            await DisconnectSession();

            deferral.Complete();
        }

        private async void DisconnectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                string deviceDisconnectText = MyAppViewModel.SelectedBLEDeviceName == null ? "BLE heart rate sensor is disconnected." : $"{MyAppViewModel.SelectedBLEDeviceName} is disconnected.";
                MyAppViewModel.CurrentMainPage.NotifyUser(deviceDisconnectText, NotifyType.StatusMessage);

                await DisconnectSession();

                button.IsEnabled = false;
                ConnectButton.IsEnabled = !button.IsEnabled;
                HelpRoamingSettings.SaveRoamingValue("SelectedBLEDeviceId", string.Empty);
            }
        }

        private async Task DisconnectSession()
        {
            try
            {
                SpeechTimer.Stop();

                MainPageMediaElement.Stop();

                MyAppViewModel.HeartRateMeasurement = 0;

                MyAppViewModel.HeartRateMeasurements.Clear();

                if (HeartRateCharacteristic != null)
                {
                    HeartRateCharacteristic.ProtectionLevel = GattProtectionLevel.Plain;

                    HeartRateCharacteristic.ValueChanged -= HeartRateCharacteristic_ValueChanged;

                    GattCommunicationStatus status = await HeartRateCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                        GattClientCharacteristicConfigurationDescriptorValue.None);

                    HeartRateCharacteristic?.Service.Dispose();

                    HeartRateCharacteristic = null;
                }

                if (BatteryCharacteristic != null)
                {
                    BatteryCharacteristic?.Service.Dispose();

                    BatteryCharacteristic = null;
                }

                if (BatteryGattDeviceService != null)
                {
                    BatteryGattDeviceService?.Dispose();

                    BatteryGattDeviceService = null;
                }

                if (HeartRateGattDeviceService != null)
                {
                    HeartRateGattDeviceService?.Dispose();

                    HeartRateGattDeviceService = null;
                }

                IsValueChangedHandlerRegistered = false;

                SelectedBluetoothLeDevice?.Dispose();

                SelectedBluetoothLeDevice = null;

                MyAppViewModel.SelectedBluetoothLEDevice = null;

                MyAppViewModel.SelectedBLEDeviceId = string.Empty;

                MyAppViewModel.SelectedBLEDeviceName = string.Empty;
            }
            catch (Exception)
            {
                //MessageDialog messageDialog = new MessageDialog(ex.Message);
                //await messageDialog.ShowAsync();
            }
        }

        private async void SpeechTimer_Tick(object sender, object e)
        {
            CounterIntervalSeconds++;
            if (CounterIntervalSeconds >= MyAppViewModel.SpeechInterval)
            {
                CounterIntervalSeconds = 0;
                if (MyAppViewModel.ChosenDeviceIsSpeaking && IsValueChangedHandlerRegistered)
                {
                    await DoSpeak();
                }
            }
        }

        private async Task DoSpeak()
        {
            string text = $"{MyAppViewModel.HeartRateMeasurement}";
            if (!String.IsNullOrEmpty(text))
            {
                try
                {
                    // Create a stream from the text. This will be played using a media element.
                    SpeechSynthesisStream synthesisStream = await MyAppViewModel.MySpeechSynthesizer.SynthesizeTextToStreamAsync(text);
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        // Set the source and start playing the synthesized audio stream.
                        MainPageMediaElement.SetSource(synthesisStream, synthesisStream.ContentType);
                    });
                }
                catch (System.IO.FileNotFoundException)
                {
                    // If media player components are unavailable, (eg, using a N SKU of windows), we won't
                    // be able to start media playback. Handle this gracefully
                    MessageDialog messageDialog = new MessageDialog("Media player components unavailable");
                    await messageDialog.ShowAsync();
                }
                catch (Exception)
                {
                    // If the text is unable to be synthesized, throw an error message to the user.
                    MessageDialog messageDialog = new MessageDialog("Unable to synthesize text");
                    await messageDialog.ShowAsync();
                }
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // code here
            if (string.IsNullOrEmpty(MyAppViewModel.SelectedBLEDeviceId))
            {
                MyAppViewModel.SelectedBLEDeviceName = "Please select a BLE heart rate sensor.";
                MyAppViewModel.CurrentMainPage.NotifyUser("Please select a BLE heart rate sensor.", NotifyType.ErrorMessage);
                return;
            }

            if (SelectedBluetoothLeDevice == null)
            {
                ConnectButton.IsEnabled = true;
                DisonnectButton.IsEnabled = !ConnectButton.IsEnabled;
            }
            else
            {
                ConnectButton.IsEnabled = false;
                DisonnectButton.IsEnabled = !ConnectButton.IsEnabled;
            }

            // code here
            base.OnNavigatedTo(e);
        }

        private async void ConnectButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button != null)
            {
                if (!IsValueChangedHandlerRegistered)
                {
                    if (string.IsNullOrEmpty(MyAppViewModel.SelectedBLEDeviceId))
                    {
                        MyAppViewModel.SelectedBLEDeviceName = "Please select a BLE heart rate sensor.";
                        MyAppViewModel.CurrentMainPage.NotifyUser("Please select a BLE heart rate sensor.", NotifyType.ErrorMessage);
                        return;
                    }
                    await Connect();
                    MyAppViewModel.CurrentMainPage.GoToHeartRatePage();
                }
                else
                {
                    MyAppViewModel.CurrentMainPage.GoToHeartRatePage();
                }
            }
        }

        public async Task Connect()
        {
            string deviceConnectText = $"Connecting to {MyAppViewModel.SelectedBLEDeviceName} in a few seconds...";
            MyAppViewModel.CurrentMainPage.NotifyUser(deviceConnectText, NotifyType.StatusMessage);
            try
            {
                SelectedBluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(MyAppViewModel.SelectedBLEDeviceId);
            }
            catch (Exception ex) when ((uint)ex.HResult == 0x800710df)
            {
                // ERROR_DEVICE_NOT_AVAILABLE because the Bluetooth radio is not on.
            }
            catch (Exception)
            {
                HelpRoamingSettings.SaveRoamingValue("SelectedBLEDeviceId", string.Empty);
            }

            if (SelectedBluetoothLeDevice != null)
            {
                ConnectButton.IsEnabled = false;
                DisonnectButton.IsEnabled = !ConnectButton.IsEnabled;

                MyAppViewModel.SelectedBLEDeviceName = SelectedBluetoothLeDevice.Name;
                try
                {
                    await GetHeartRate();

                    await GetBattery();
                }
                catch
                {
                }

                if (HeartRateCharacteristic != null)
                {
                    try
                    {
                        GattCommunicationStatus gattCommunicationStatus = await HeartRateCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                            GattClientCharacteristicConfigurationDescriptorValue.Notify);
                        if (gattCommunicationStatus == GattCommunicationStatus.Success)
                        {
                            if (!IsValueChangedHandlerRegistered)
                            {
                                //prenumerate for heart rate measurements
                                HeartRateCharacteristic.ValueChanged += HeartRateCharacteristic_ValueChanged;
                                IsValueChangedHandlerRegistered = true;

                                SpeechTimer.Start();
                                //recordings starts and we save data with this file name
                                MyAppViewModel.FileNameDateTime = HelpFileName.GetNewFileName();
                                HelpRoamingSettings.SaveRoamingValue("SelectedBLEDeviceId", MyAppViewModel.SelectedBLEDeviceId);
                                // save also name for next session
                                HelpRoamingSettings.SaveRoamingValue("SelectedBLEDeviceName", MyAppViewModel.SelectedBLEDeviceName);
                            }
                            MyAppViewModel.CurrentMainPage.NotifyUser("Successfully registered for encrypted notifications.", NotifyType.StatusMessage);
                        }
                        HeartRateCharacteristic.ProtectionLevel = GattProtectionLevel.EncryptionRequired;
                    }
                    catch (UnauthorizedAccessException ex)
                    {
                        // This usually happens when a device reports that it supports notify, but it actually doesn't.
                        MyAppViewModel.CurrentMainPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                    }
                }
            }
            else
            {
                string deviceFailedText = $"Failed to connect to {MyAppViewModel.SelectedBLEDeviceName} BLE heart rate sensor. Somebody has to wear the BLE heart rate sensor. Moisten the strap. Read more on 'Help and trouble shooting'.";
                MyAppViewModel.CurrentMainPage.NotifyUser(deviceFailedText, NotifyType.ErrorMessage);
            }
        }

        private async Task GetBattery()
        {
            GattDeviceServicesResult gattDeviceServicesResult = await SelectedBluetoothLeDevice.GetGattServicesForUuidAsync(GattServiceUuids.Battery, BluetoothCacheMode.Uncached);

            if (gattDeviceServicesResult.Status == GattCommunicationStatus.Success)
            {
                BatteryGattDeviceService = gattDeviceServicesResult.Services.FirstOrDefault();

                if (BatteryGattDeviceService != null)
                {
                    GattCharacteristicsResult gattCharacteristicsResult = await batteryGattDeviceService.GetCharacteristicsForUuidAsync(GattCharacteristicUuids.BatteryLevel, BluetoothCacheMode.Uncached);

                    if (gattCharacteristicsResult.Status == GattCommunicationStatus.Success)
                    {
                        BatteryCharacteristic = gattCharacteristicsResult.Characteristics.FirstOrDefault();
                        if (BatteryCharacteristic != null)
                        {
                            GattReadResult gattReadResult = await BatteryCharacteristic.ReadValueAsync();
                            if (gattReadResult.Status == GattCommunicationStatus.Success)
                            {
                                CryptographicBuffer.CopyToByteArray(gattReadResult.Value, out byte[] data);
                                MyAppViewModel.BatteryLevel = data[0];
                            }
                        }
                    }
                }
            }
        }

        private async Task GetHeartRate()
        {
            GattDeviceServicesResult gattDeviceServicesResultHeartRate = await SelectedBluetoothLeDevice.GetGattServicesForUuidAsync(GattServiceUuids.HeartRate, BluetoothCacheMode.Uncached);

            if (gattDeviceServicesResultHeartRate.Status == GattCommunicationStatus.Unreachable)
            {
                string deviceUnreacableText = $"{MyAppViewModel.SelectedBLEDeviceName} BLE heart rate sensor is unreachable. Read more on 'Help and trouble shooting'.";
                MyAppViewModel.CurrentMainPage.NotifyUser(deviceUnreacableText, NotifyType.ErrorMessage);
            }

            if (gattDeviceServicesResultHeartRate.Status == GattCommunicationStatus.Success)
            {
                HeartRateGattDeviceService = gattDeviceServicesResultHeartRate.Services.FirstOrDefault();

                if (HeartRateGattDeviceService != null)
                {
                    GattCharacteristicsResult gattCharacteristicsResult = await heartRateGattDeviceService.GetCharacteristicsForUuidAsync(GattCharacteristicUuids.HeartRateMeasurement, BluetoothCacheMode.Uncached);
                    if (gattCharacteristicsResult.Status == GattCommunicationStatus.Success)
                    {
                        // BT_Code: Get all the child characteristics of a service.
                        HeartRateCharacteristic = gattCharacteristicsResult.Characteristics.FirstOrDefault();

                        if (HeartRateCharacteristic == null)
                        {
                            MyAppViewModel.CurrentMainPage.NotifyUser("Failed to connect to HeartRateMeasurement characteristic.", NotifyType.ErrorMessage);
                        }
                    }
                }
                else
                {
                    MyAppViewModel.CurrentMainPage.NotifyUser("Failed to connect to HeartRate service.", NotifyType.ErrorMessage);
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // code here
            // code here
        }

        private void DeviceAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToDevicePage();
        }

        private void HeartRateAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHeartRatePage();
        }

        private async void HeartRateCharacteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // BT_Code: An Indicate or Notify reported that the value has changed.
            CryptographicBuffer.CopyToByteArray(args.CharacteristicValue, out byte[] data);
            DateTimeOffset timestamp = args.Timestamp;

            HeartRateMeasurementModel heartRateMeasurementModel = new HeartRateMeasurementModel
            {
                HeartRateValue = ParseHeartRateValue(data),
                Timestamp = timestamp
            };

            //add to collection
            lock (MyAppViewModel.HeartRateMeasurements)
            {
                MyAppViewModel.HeartRateMeasurements.Add(heartRateMeasurementModel);
            }

            // create in right thread
            // Serialize UI update to the the main UI thread.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MyAppViewModel.HeartRateMeasurement = heartRateMeasurementModel.HeartRateValue;
                MyAppViewModel.HeartRateMeasurementAsSound = heartRateMeasurementModel.HeartRateValue.ToString();
                MyAppViewModel.HeartRateMeasurementTimeStamp = heartRateMeasurementModel.Timestamp.LocalDateTime.ToString();

                MyAppViewModel.CurrentConnectPage.HeartRateLatestValue.Text = $"Heart rate is {heartRateMeasurementModel.HeartRateValue} bpm at {heartRateMeasurementModel.Timestamp.LocalDateTime:HH:mm:ss}.";

                if (MyAppViewModel.CurrentHeartRatePage != null)
                {
                    MyAppViewModel.CurrentHeartRatePage.PlotChart();
                }
            });
        }

        /// <summary>
        /// Process the raw data received from the device into application usable data,
        /// according the the Bluetooth Heart Rate Profile.
        /// https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.heart_rate_measurement.xml&u=org.bluetooth.characteristic.heart_rate_measurement.xml
        /// This function throws an exception if the data cannot be parsed.
        /// </summary>
        /// <param name="data">Raw data received from the heart rate monitor.</param>
        /// <returns>The heart rate measurement value.</returns>
        private static ushort ParseHeartRateValue(byte[] data)
        {
            // Heart Rate profile defined flag values
            const byte heartRateValueFormat = 0x01;

            byte flags = data[0];
            bool isHeartRateValueSizeLong = ((flags & heartRateValueFormat) != 0);

            if (isHeartRateValueSizeLong)
            {
                return BitConverter.ToUInt16(data, 1);
            }
            else
            {
                return data[1];
            }
        }

        private void EnumerateAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToEnumerateDevicePage();
        }

        private void HomeAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHomePage();
        }
    }
}
