using HeartRateNow.Models;
using System.Threading.Tasks;

namespace HeartRateNow.Helpers
{
    public sealed class HelpClearData
    {
        private static AppProperties myAppViewModel = App.MyApp;
        public static AppProperties MyAppViewModel { get => myAppViewModel; }

        public static void ClearData()
        {
            Task.Run(() => ClearDataItems());
        }

        public static void ClearDataItems()
        {
            lock (MyAppViewModel.HeartRateMeasurements)
            {
                MyAppViewModel.HeartRateMeasurements.Clear();
            }
        }
    }
}
