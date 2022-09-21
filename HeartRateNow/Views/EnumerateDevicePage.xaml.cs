using HeartRateNow.Models;
using System;
using System.Collections.ObjectModel;
using Windows.Devices.Enumeration;
using Windows.UI.Core;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace HeartRateNow.Views
{
    public sealed partial class EnumerateDevicePage : Page
    {
        private AppProperties myAppViewModel = App.MyApp;
        public AppProperties MyAppViewModel
        {
            get { return myAppViewModel; }
        }

        private DeviceWatcher deviceWatcher;
        public DeviceWatcher DeviceWatcher { get => deviceWatcher; set => deviceWatcher = value; }

        private ObservableCollection<BluetoothLEDeviceDisplay> resultCollection;
        public ObservableCollection<BluetoothLEDeviceDisplay> ResultCollection { get => resultCollection; set => resultCollection = value; }

        public EnumerateDevicePage()
        {
            InitializeComponent();

            ResultCollection = new ObservableCollection<BluetoothLEDeviceDisplay>();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // code here
            // code here
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            // code here
            // Save the selected device's ID for use on other pages.
            if (ResultsListView.SelectedItem is BluetoothLEDeviceDisplay bleDeviceDisplay)
            {
                MyAppViewModel.SelectedBLEDeviceId = bleDeviceDisplay.Id;
                MyAppViewModel.SelectedBLEDeviceName = bleDeviceDisplay.Name;
            }
            // code here
            base.OnNavigatedFrom(e);
        }

        #region Device discovery
        private void EnumerateButton_Click()
        {
            if (DeviceWatcher == null)
            {
                StartBleDeviceWatcher();

                MyAppViewModel.CurrentMainPage.NotifyUser("Device watcher started.", NotifyType.StatusMessage);
            }
            else
            {
                StopBleDeviceWatcher();

                MyAppViewModel.CurrentMainPage.NotifyUser("Device watcher stopped.", NotifyType.StatusMessage);
            }
        }

        private void StopBleDeviceWatcher()
        {
            if (DeviceWatcher != null)
            {
                // Unregister the event handlers.
                DeviceWatcher.Added -= DeviceWatcher_Added;
                DeviceWatcher.Updated -= DeviceWatcher_Updated;
                DeviceWatcher.Removed -= DeviceWatcher_Removed;
                DeviceWatcher.EnumerationCompleted -= DeviceWatcher_EnumerationCompleted;
                DeviceWatcher.Stopped -= DeviceWatcher_Stopped;

                // Stop the watcher.
                DeviceWatcher.Stop();
                DeviceWatcher = null;
            }

            EnumerateButton.Content = "Start enumerating";
        }

        /// <summary>
        ///     Starts a device watcher that looks for all nearby BT devices (paired or unpaired). Attaches event handlers and
        ///     populates the collection of devices.
        /// </summary>
        private void StartBleDeviceWatcher()
        {
            EnumerateButton.Content = "Stop enumerating";

            // Additional properties we would like about the device.
            //string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };

            //unpaired device 20170604 "Creators Update"
            //"System.Devices.Aep.Bluetooth.Le.IsConnectable"
            //unpaired device 20170604
            string[] requestedProperties = { "System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected", "System.Devices.Aep.Bluetooth.Le.IsConnectable" };

            // BT_Code: Currently Bluetooth APIs don't provide a selector to get ALL devices that are both paired and non-paired.
            DeviceWatcher =
                    DeviceInformation.CreateWatcher("(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")",
                        requestedProperties,
                        DeviceInformationKind.AssociationEndpoint);

            // Register event handlers before starting the watcher.
            DeviceWatcher.Added += DeviceWatcher_Added;
            DeviceWatcher.Updated += DeviceWatcher_Updated;
            DeviceWatcher.Removed += DeviceWatcher_Removed;
            DeviceWatcher.EnumerationCompleted += DeviceWatcher_EnumerationCompleted;
            DeviceWatcher.Stopped += DeviceWatcher_Stopped;

            // Start over with an empty collection.
            ResultCollection.Clear();

            // Start the watcher.
            DeviceWatcher.Start();
        }

        private BluetoothLEDeviceDisplay FindBluetoothLEDeviceDisplay(string id)
        {
            foreach (BluetoothLEDeviceDisplay bleDeviceDisplay in ResultCollection)
            {
                if (bleDeviceDisplay.Id == id)
                {
                    return bleDeviceDisplay;
                }
            }
            return null;
        }

        private async void DeviceWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == DeviceWatcher)
                {
                    // Make sure device name isn't blank or already present in the list.
                    if (deviceInfo.Name != string.Empty && FindBluetoothLEDeviceDisplay(deviceInfo.Id) == null)
                    {
                        ResultCollection.Add(new BluetoothLEDeviceDisplay(deviceInfo));
                    }
                }
            });
        }

        private async void DeviceWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == DeviceWatcher)
                {
                    BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                    if (bleDeviceDisplay != null)
                    {
                        bleDeviceDisplay.Update(deviceInfoUpdate);
                    }
                }
            });
        }

        private async void DeviceWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate deviceInfoUpdate)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == DeviceWatcher)
                {
                    // Find the corresponding DeviceInformation in the collection and remove it.
                    BluetoothLEDeviceDisplay bleDeviceDisplay = FindBluetoothLEDeviceDisplay(deviceInfoUpdate.Id);
                    if (bleDeviceDisplay != null)
                    {
                        ResultCollection.Remove(bleDeviceDisplay);
                    }
                }
            });
        }

        private async void DeviceWatcher_EnumerationCompleted(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == DeviceWatcher)
                {
                    MyAppViewModel.CurrentMainPage.NotifyUser($"{ResultCollection.Count} devices found. Enumeration completed.",
                        NotifyType.StatusMessage);
                }
            });
        }

        private async void DeviceWatcher_Stopped(DeviceWatcher sender, object e)
        {
            // We must update the collection on the UI thread because the collection is databound to a UI element.
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                // Protect against race condition if the task runs after the app stopped the deviceWatcher.
                if (sender == DeviceWatcher)
                {
                    MyAppViewModel.CurrentMainPage.NotifyUser("No longer watching for devices.",
                            sender.Status == DeviceWatcherStatus.Aborted ? NotifyType.ErrorMessage : NotifyType.StatusMessage);
                }
            });
        }
        #endregion Device discovery

        private void HomeAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHomePage();
        }

        private void HeartRateAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHeartRatePage();
        }

        private void ConnectAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToConnectPage();
        }
    }
}
