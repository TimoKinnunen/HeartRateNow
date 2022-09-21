using HeartRateNow.Helpers;
using HeartRateNow.Models;
using HeartRateNow.Views;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Globalization;
using Windows.Media.SpeechSynthesis;
using Windows.System.Display;
using Windows.System.UserProfile;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace HeartRateNow
{
    sealed partial class App : Application
    {
        public static AppProperties MyApp { get; set; }

        public App()
        {
            InitializeComponent();
            Construct();
            Suspending += OnSuspending;

            MyApp = new AppProperties()
            {
                KeepScreenOnRequest = new DisplayRequest(),
                HeartRateMeasurements = new ObservableCollection<HeartRateMeasurementModel>(),
                MySpeechSynthesizer = new SpeechSynthesizer(),
                FileNameDateTime = HelpFileName.GetNewFileName(),
                SpeechTimer = new DispatcherTimer { Interval = TimeSpan.FromSeconds(1) },
                MySpeechMediaElement = new MediaElement()
            };

            MyApp.CounterSpeechTimerSeconds = MyApp.SpeechInterval;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = false;
            }
#endif
            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (!(Window.Current.Content is Frame rootFrame))
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                ApplicationLanguages.PrimaryLanguageOverride = GlobalizationPreferences.Languages[0];
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                HelpRoamingSettings.LoadPreviousSessionRoamingData();

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SuspendingDeferral deferral = e.SuspendingOperation.GetDeferral();
            HelpRoamingSettings.SaveCurrentSessionRoamingData();
            deferral.Complete();
        }

        // Add any application contructor code in here.
        partial void Construct();

        // Add any OnLaunched customization here.
        partial void LaunchCompleted(LaunchActivatedEventArgs e);
    }
}
