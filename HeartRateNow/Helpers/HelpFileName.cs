using System;

namespace HeartRateNow.Helpers
{
    public class HelpFileName
    {
        public static string GetNewFileName()
        {
            return string.Format("{0}_{1}.json", "HeartRateNow", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
        }
    }
}
