using HeartRateNow.Models;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace HeartRateNow.Views
{
    public sealed partial class AboutPage : Page
    {
        private readonly AppProperties myAppViewModel = App.MyApp;
        public AppProperties MyAppViewModel
        {
            get { return myAppViewModel; }
        }

        public AboutPage()
        {
            InitializeComponent();

            SizeChanged += AboutPage_SizeChanged;

            Loaded += AboutPage_Loaded;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // code here
            Package myPackage = Package.Current;

            HeartRateNowImage.Source = new BitmapImage(myPackage.Logo);

            AppDisplayNameTextBlock.Text = myPackage.DisplayName;

            PublisherTextBlock.Text = myPackage.PublisherDisplayName;

            PackageVersion version = myPackage.Id.Version;
            VersionTextBlock.Text = $"Version {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
            // code here
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            // code here
            // code here
        }

        private void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            SetPageContentStackPanelWidth();
        }

        private void AboutPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetPageContentStackPanelWidth();
        }


        private void SetPageContentStackPanelWidth()
        {
            PageContentStackPanel.Width = ActualWidth -
                PageContentScrollViewer.Margin.Left -
                PageContentScrollViewer.Padding.Right;
        }

        private void HomeAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHomePage();
        }
    }
}
