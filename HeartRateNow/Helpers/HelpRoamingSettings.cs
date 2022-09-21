using HeartRateNow.Models;
using System.Linq;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml;

namespace HeartRateNow.Helpers
{
    public sealed class HelpRoamingSettings
    {
        public static ApplicationDataContainer MyRoamingSettings = ApplicationData.Current.RoamingSettings;

        private static AppProperties myAppViewModel = App.MyApp;
        public static AppProperties MyAppViewModel { get => myAppViewModel; }

        #region save values to next session
        public static void SaveRoamingValue(string containerName, string containerValue) => MyRoamingSettings.Values[containerName] = containerValue.Trim();

        public static void SaveCurrentSessionRoamingData()
        {
            SaveRoamingValue("VoiceId", MyAppViewModel.MySpeechSynthesizer.Voice.Id);

            SaveRoamingValue("SpeechInterval", MyAppViewModel.SpeechInterval.ToString());

            SaveRoamingValue("RedZoneHeartRateAboveText", MyAppViewModel.RedZoneHeartRateAboveText.ToString());
            SaveRoamingValue("YellowZoneHeartRateAboveText", MyAppViewModel.YellowZoneHeartRateAboveText.ToString());
            SaveRoamingValue("GreenZoneHeartRateAboveText", MyAppViewModel.GreenZoneHeartRateAboveText.ToString());

            SaveRoamingValue("RedZoneHeartRateValue", MyAppViewModel.RedZoneHeartRateValue.ToString());
            SaveRoamingValue("YellowZoneHeartRateValue", MyAppViewModel.YellowZoneHeartRateValue.ToString());
            SaveRoamingValue("GreenZoneHeartRateValue", MyAppViewModel.GreenZoneHeartRateValue.ToString());

            SaveRoamingValue("DatapointsInMinutes", MyAppViewModel.DatapointsInMinutes.ToString());

            switch (MyAppViewModel.SpeakAppBarButtonVisibility)
            {
                case Visibility.Collapsed:
                    SaveRoamingValue("SpeakAppBarButtonVisibility", "Collapsed");
                    break;
                case Visibility.Visible:
                    SaveRoamingValue("SpeakAppBarButtonVisibility", "Visible");
                    break;
            }

            switch (MyAppViewModel.DisableLockScreen)
            {
                case true:
                    SaveRoamingValue("DisableLockScreen", "DisableLockScreen");
                    break;
                default:
                    SaveRoamingValue("DisableLockScreen", "EnableLockScreen");
                    break;
            }
        }
        #endregion save values to next session

        #region load values from previous session
        public static string LoadRoamingValue(string containerName)
        {
            if (MyRoamingSettings.Values.TryGetValue(containerName, out object value))
            {
                return (string)value;
            }

            return null;
        }

        public static void LoadPreviousSessionRoamingData()
        {
            string stringVoiceId = LoadRoamingValue("VoiceId");
            if (!string.IsNullOrEmpty(stringVoiceId))
            {
                MyAppViewModel.MySpeechSynthesizer.Voice = SpeechSynthesizer.AllVoices.Where(v => v.Id == stringVoiceId).FirstOrDefault();
            }

            string stringSpeakAppBarButtonVisibility = LoadRoamingValue("SpeakAppBarButtonVisibility");
            if (!string.IsNullOrEmpty(stringSpeakAppBarButtonVisibility))
            {
                switch (stringSpeakAppBarButtonVisibility)
                {
                    case "Collapsed":
                        // setting this property also sets App.ChosenDeviceProperties.ChosenDeviceIsSpeaking
                        // don't touch App.ChosenDeviceProperties.ChosenDeviceIsSpeaking here
                        // setting this property also sets App.ChosenDeviceProperties.MuteAppBarButtonVisibility
                        MyAppViewModel.SpeakAppBarButtonVisibility = Visibility.Collapsed;
                        break;
                    default:
                        // setting this property also sets App.ChosenDeviceProperties.ChosenDeviceIsSpeaking
                        // don't touch App.ChosenDeviceProperties.ChosenDeviceIsSpeaking here
                        // setting this property also sets App.ChosenDeviceProperties.MuteAppBarButtonVisibility
                        MyAppViewModel.SpeakAppBarButtonVisibility = Visibility.Visible;
                        break;
                }
            }

            string stringSpeechInterval = LoadRoamingValue("SpeechInterval");
            if (!string.IsNullOrEmpty(stringSpeechInterval))
            {
                MyAppViewModel.SpeechInterval = int.Parse(stringSpeechInterval);
            }

            string redZoneHeartRateAboveText = LoadRoamingValue("RedZoneHeartRateAboveText");
            if (!string.IsNullOrEmpty(redZoneHeartRateAboveText))
            {
                MyAppViewModel.RedZoneHeartRateAboveText = redZoneHeartRateAboveText.ToString();
            }

            string yellowZoneHeartRateAboveText = LoadRoamingValue("YellowZoneHeartRateAboveText");
            if (!string.IsNullOrEmpty(yellowZoneHeartRateAboveText))
            {
                MyAppViewModel.YellowZoneHeartRateAboveText = yellowZoneHeartRateAboveText.ToString();
            }

            string greenZoneHeartRateAboveText = LoadRoamingValue("GreenZoneHeartRateAboveText");
            if (!string.IsNullOrEmpty(greenZoneHeartRateAboveText))
            {
                MyAppViewModel.GreenZoneHeartRateAboveText = greenZoneHeartRateAboveText.ToString();
            }

            string redZoneHeartRateValue = LoadRoamingValue("RedZoneHeartRateValue");
            if (!string.IsNullOrEmpty(redZoneHeartRateValue))
            {
                MyAppViewModel.RedZoneHeartRateValue = int.Parse(redZoneHeartRateValue);
            }

            string yellowZoneHeartRateValue = LoadRoamingValue("YellowZoneHeartRateValue");
            if (!string.IsNullOrEmpty(yellowZoneHeartRateValue))
            {
                MyAppViewModel.YellowZoneHeartRateValue = int.Parse(yellowZoneHeartRateValue);
            }

            string greenZoneHeartRateValue = LoadRoamingValue("GreenZoneHeartRateValue");
            if (!string.IsNullOrEmpty(greenZoneHeartRateValue))
            {
                MyAppViewModel.GreenZoneHeartRateValue = int.Parse(greenZoneHeartRateValue);
            }

            string stringDatapointsInMinutes = LoadRoamingValue("DatapointsInMinutes");
            if (!string.IsNullOrEmpty(stringDatapointsInMinutes))
            {
                MyAppViewModel.DatapointsInMinutes = int.Parse(stringDatapointsInMinutes);
            }

            // load value from previous session
            string stringDisableLockScreen = LoadRoamingValue("DisableLockScreen");
            if (!string.IsNullOrEmpty(stringDisableLockScreen))
            {
                switch (stringDisableLockScreen)
                {
                    case "DisableLockScreen":
                        // this sets App.ChosenDeviceProperties.EnableLockScreen = false;
                        MyAppViewModel.DisableLockScreen = true;
                        break;
                    default:
                        // this sets App.ChosenDeviceProperties.EnableLockScreen = true;
                        MyAppViewModel.DisableLockScreen = false;
                        break;
                }
                HelpLockScreen.ChangeLockScreenBehaviour();
            }
        }
        #endregion load values from previous session
    }
}
