using HeartRateNow.Helpers;
using HeartRateNow.Models;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace HeartRateNow.Views
{
    public sealed partial class SettingsPage : Page
    {
        private readonly AppProperties myAppViewModel = App.MyApp;
        public AppProperties MyAppViewModel
        {
            get { return myAppViewModel; }
        }

        public SettingsPage()
        {
            InitializeComponent();

            Loaded += DevicePage_Loaded;

            SettingsPageMainGrid.DataContext = MyAppViewModel;

            InitializeVoiceChooserComboBox();
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
            HelpRoamingSettings.SaveCurrentSessionRoamingData();
            // code here
        }

        private void DevicePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (MyAppViewModel.DisableLockScreen)
            {
                DisableLockScreenRadioButton.IsChecked = true;
            }
            else
            {
                EnableLockScreenRadioButton.IsChecked = true;
            }
        }

        private void HomeAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHomePage();
        }

        private void HeartRateAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            MyAppViewModel.CurrentMainPage.GoToHeartRatePage();
        }

        private async void MuteAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Serialize UI update to the the main UI thread.
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MyAppViewModel.SpeakAppBarButtonVisibility = Visibility.Visible;
            });
        }

        private async void SpeakAppBarButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Serialize UI update to the the main UI thread.
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                MyAppViewModel.SpeakAppBarButtonVisibility = Visibility.Collapsed;
            });
        }

        private void EnableLockScreenRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                MyAppViewModel.DisableLockScreen = false;
                HelpLockScreen.ChangeLockScreenBehaviour();
            }
        }

        private void DisableLockScreenRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is RadioButton radioButton)
            {
                MyAppViewModel.DisableLockScreen = true;
                HelpLockScreen.ChangeLockScreenBehaviour();
            }
        }

        #region comboboxes
        /// <summary>
        /// This creates items out of the system installed voices. The voices are then displayed in a listbox.
        /// This allows the user to change the voice of the synthesizer in your app based on their preference.
        /// </summary>
        private void InitializeVoiceChooserComboBox()
        {
            // Get all of the installed voices.
            IReadOnlyList<VoiceInformation> voices = SpeechSynthesizer.AllVoices;

            // Get the currently selected voice.
            VoiceInformation currentVoice = MyAppViewModel.MySpeechSynthesizer.Voice;

            //foreach (VoiceInformation voice in voices.OrderBy(p => p.Language))
            foreach (VoiceInformation voice in voices)
            {
                ComboBoxItem comboBoxItem = new ComboBoxItem
                {
                    Name = voice.DisplayName,
                    Tag = voice,
                    Content = voice.DisplayName + " (Language: " + voice.Language + ")"

                };
                VoiceChooserComboBox.Items.Add(comboBoxItem);

                // Check to see if we're looking at the current voice and set it as selected in the listbox.
                if (currentVoice.Id == voice.Id)
                {
                    comboBoxItem.IsSelected = true;
                    VoiceChooserComboBox.SelectedItem = comboBoxItem;
                }
            }
        }

        private void VoiceChooserComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem comboBoxItem = (ComboBoxItem)(VoiceChooserComboBox.SelectedItem);
            VoiceInformation voice = (VoiceInformation)(comboBoxItem.Tag);
            MyAppViewModel.MySpeechSynthesizer.Voice = voice;
        }
        #endregion comboboxes
    }
}
