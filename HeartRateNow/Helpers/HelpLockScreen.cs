using HeartRateNow.Models;

namespace HeartRateNow.Helpers
{
    public sealed class HelpLockScreen
    {
        private static AppProperties myAppViewModel = App.MyApp;
        public static AppProperties MyAppViewModel { get => myAppViewModel; }

        public static void ChangeLockScreenBehaviour()
        {
            // disable Windows lock screen
            if (MyAppViewModel.DisableLockScreen)
            {
                DisableLockScreen();
            }
            else
            {
                EnableLockScreen();
            }
        }

        private static void DisableLockScreen()
        {
            try
            {
                // Activates a display request. 
                //  When a display request is activated, the device's display remains on while the app is visible.
                MyAppViewModel.KeepScreenOnRequest.RequestActive();
                MyAppViewModel.DisplayRequestCount++;
            }
            catch
            {
            }
        }

        private static void EnableLockScreen()
        {
            try
            {
                while (MyAppViewModel.DisplayRequestCount > 0)
                {
                    // Deactivates a display request.
                    MyAppViewModel.KeepScreenOnRequest.RequestRelease();
                    MyAppViewModel.DisplayRequestCount--;
                }
            }
            catch
            {
            }
        }
    }
}
