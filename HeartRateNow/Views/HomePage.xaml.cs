using HeartRateNow.Models;
using System;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;


namespace HeartRateNow.Views
{
    public sealed partial class HomePage : Page
    {
        private readonly AppProperties myAppViewModel = App.MyApp;
        public AppProperties MyAppViewModel
        {
            get { return myAppViewModel; }
        }

        public HomePage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            // code here

            // code here
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

        private async void WindowsBluetoothSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            bool result = await Launcher.LaunchUriAsync(new Uri("ms-settings-bluetooth:"));
        }

        private void EnumerateAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToEnumerateDevicePage();
        }
    }
}
