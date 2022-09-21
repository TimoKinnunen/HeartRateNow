using HeartRateNow.Helpers;
using HeartRateNow.Models;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Color = Windows.UI.Color;

namespace HeartRateNow.Views
{
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer hideTextTimer;

        private int counterIntervalSeconds;

        public MediaElement MultimediaMediaElement
        {
            get { return multimediaMediaElement; }
        }

        private AppProperties myAppViewModel = App.MyApp;
        public AppProperties MyAppViewModel
        {
            get { return myAppViewModel; }
        }

        public ListView MenuNavigationListView
        {
            get { return NavigationListView; }
        }
        public MainPage()
        {
            InitializeComponent();

            MyAppViewModel.CurrentMainPage = this;

            Loaded += MainPage_Loaded;

            hideTextTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) };

            hideTextTimer.Tick += HideTextTimer_Tick;
        }

        private async void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            ObservableCollection<NavigationMenuItem> allNavigationMenuItems = GetNavigationMenuItems();

            NavigationMenuCollectionViewSource.Source = allNavigationMenuItems;

            if (allNavigationMenuItems.Count > 0)
            {
                NavigationListView.SelectedItem = allNavigationMenuItems.FirstOrDefault();
            }

            MyAppViewModel.SelectedBLEDeviceId = HelpRoamingSettings.LoadRoamingValue("SelectedBLEDeviceId");
            MyAppViewModel.SelectedBLEDeviceName = HelpRoamingSettings.LoadRoamingValue("SelectedBLEDeviceName");
            if (string.IsNullOrEmpty(MyAppViewModel.SelectedBLEDeviceId))
            {
                GoToEnumerateDevicePage();
            }
            else
            {
                GoToConnectPage();
                await MyAppViewModel.CurrentConnectPage.Connect();
                MyAppViewModel.CurrentMainPage.GoToHeartRatePage();
            }
        }

        private void HideTextTimer_Tick(object sender, object e)
        {
            counterIntervalSeconds++;
            if (counterIntervalSeconds >= 5)
            {
                counterIntervalSeconds = 0;
                //hide text by setting it to empty
                NotifyUser(string.Empty, NotifyType.StatusMessage);
                //after 5 seconds stop the timer
                hideTextTimer.Stop();
            }
        }

        public void GoToHomePage()
        {
            MainFrame.Navigate(typeof(HomePage));
            MenuNavigationListView.SelectedIndex = 0;
        }
        public void GoToEnumerateDevicePage()
        {
            MainFrame.Navigate(typeof(EnumerateDevicePage));
            MenuNavigationListView.SelectedIndex = 1;
        }

        public void GoToConnectPage()
        {
            MainFrame.Navigate(typeof(ConnectPage));
            MenuNavigationListView.SelectedIndex = 2;
        }

        public void GoToHeartRatePage()
        {
            MainFrame.Navigate(typeof(HeartRatePage));
            MenuNavigationListView.SelectedIndex = 3;
        }

        public void GoToDevicePage()
        {
            MainFrame.Navigate(typeof(SettingsPage));
            MenuNavigationListView.SelectedIndex = 4;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // code here
            ObservableCollection<NavigationMenuItem> allNavigationMenuItems = GetNavigationMenuItems();

            NavigationMenuCollectionViewSource.Source = allNavigationMenuItems;

            if (allNavigationMenuItems.Count > 0)
            {
                NavigationListView.SelectedItem = allNavigationMenuItems.FirstOrDefault();
            }

            NotifyUser(string.Empty, NotifyType.StatusMessage);
            // code here
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // code here
            // code here
        }

        private static ObservableCollection<NavigationMenuItem> GetNavigationMenuItems()
        {
            return new ObservableCollection<NavigationMenuItem>()
            {
                new NavigationMenuItem(){
                    Id = 0,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE10F",
                    MenuToolTip = "Home",
                    MenuItemName = "Home"
                },

                new NavigationMenuItem(){
                    Id = 1,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE702",
                    MenuToolTip = "Enumerate BLE devices",
                    MenuItemName = "Enumerate BLE devices"
                },

                new NavigationMenuItem(){
                    Id = 2,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE007",
                    MenuToolTip = "Connect to BLE heart rate sensor",
                    MenuItemName = "Connect to BLE heart rate sensor"
                },

                new NavigationMenuItem(){
                    Id = 3,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE95E",
                    MenuToolTip = "Heart rate",
                    MenuItemName = "Heart rate"
                },

                new NavigationMenuItem(){
                    Id = 4,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE115",
                    MenuToolTip = "Settings",
                    MenuItemName = "Settings"
                },

                new NavigationMenuItem(){
                    Id = 5,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE946",
                    MenuToolTip = "About",
                    MenuItemName = "About"
                },

                new NavigationMenuItem(){
                    Id = 6,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE946",
                    MenuToolTip = "Privacy statement",
                    MenuItemName = "Privacy statement"
                },

                new NavigationMenuItem(){
                    Id = 7,
                    MenuFontFamily = "Segoe MDL2 Assets",
                    MenuGlyph = "\xE946",
                    MenuToolTip = "Help and trouble shooting",
                    MenuItemName = "Help and trouble shooting"
                },
            };
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MenuSplitView.IsPaneOpen = MenuSplitView.IsPaneOpen ? false : true;
        }

        private void NavigationListView_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (sender is ListView listView)
            {
                if (listView.SelectedItem is NavigationMenuItem navigationMenuItem)
                {
                    switch (navigationMenuItem.Id)
                    {
                        case 0:
                            MainFrame.Navigate(typeof(HomePage));
                            break;
                        case 1:
                            MainFrame.Navigate(typeof(EnumerateDevicePage));
                            break;
                        case 2:
                            MainFrame.Navigate(typeof(ConnectPage));
                            break;
                        case 3:
                            MainFrame.Navigate(typeof(HeartRatePage));
                            break;
                        case 4:
                            MainFrame.Navigate(typeof(SettingsPage));
                            break;
                        case 5:
                            MainFrame.Navigate(typeof(AboutPage));
                            break;
                        case 6:
                            MainFrame.Navigate(typeof(PrivacyStatementPage));
                            break;
                        case 7:
                            MainFrame.Navigate(typeof(HelpPage));
                            break;
                        default:
                            MainFrame.Navigate(typeof(HomePage));
                            break;
                    }
                    MenuSplitView.IsPaneOpen = false;
                }
            }
        }

        public static class ColorHelper
        {
            public static SolidColorBrush GetColorFromHexa(string hexaColor)
            {
                if (string.IsNullOrEmpty(hexaColor))
                {
                    throw new ArgumentOutOfRangeException("HexaColor argument is empty.");
                }
                if (hexaColor.Length != 9)
                {
                    throw new ArgumentOutOfRangeException("HexaColor argument must be nine characters long.");
                }
                return new SolidColorBrush(
                    Color.FromArgb(
                        255,
                        Convert.ToByte(hexaColor.Substring(3, 2), 16),
                        Convert.ToByte(hexaColor.Substring(5, 2), 16),
                        Convert.ToByte(hexaColor.Substring(7, 2), 16)
                    )
                );
            }
        }

        /// <summary>
        /// Used to display messages to the user
        /// </summary>
        /// <param name="strMessage"></param>
        /// <param name="type"></param>
        public void NotifyUser(string strMessage, NotifyType type)
        {
            switch (type)
            {
                case NotifyType.StatusMessage:
                    //StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Green);
                    StatusBorder.Background = ColorHelper.GetColorFromHexa("#FF32CD32");
                    break;
                case NotifyType.ErrorMessage:
                    //StatusBorder.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    StatusBorder.Background = ColorHelper.GetColorFromHexa("#FFFF0000");
                    break;
            }
            StatusTextBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusTextBlock.Text != String.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusTextBlock.Text != String.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusPanel.Visibility = Visibility.Visible;
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusPanel.Visibility = Visibility.Collapsed;
            }

            // Collapse the StatusTextBlock if it has no text to conserve real estate.
            StatusBorder.Visibility = (StatusTextBlock.Text != string.Empty) ? Visibility.Visible : Visibility.Collapsed;
            if (StatusTextBlock.Text != string.Empty)
            {
                StatusBorder.Visibility = Visibility.Visible;
                StatusBorder.Visibility = Visibility.Visible;

                counterIntervalSeconds = 0;
                if (hideTextTimer != null && !hideTextTimer.IsEnabled)
                {
                    hideTextTimer.Start();
                }
            }
            else
            {
                StatusBorder.Visibility = Visibility.Collapsed;
                StatusBorder.Visibility = Visibility.Collapsed;
            }
        }

        private void StatusBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            //hide
            NotifyUser(string.Empty, NotifyType.StatusMessage);
        }
    }

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };
}
