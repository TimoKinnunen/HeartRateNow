using System;

namespace HeartRateNow.Models
{
    public class RenderingOptions
    {
        public double MinHeartRateValue { get; set; }
        public double MinRenderingHeartRateValue { get; set; }
        public double MeanHeartRateValue { get; set; }
        public double RedHeartRateValue { get; set; }
        public double YellowHeartRateValue { get; set; }
        public double GreenHeartRateValue { get; set; }
        public double MaxRenderingHeartRateValue { get; set; }
        public double MaxHeartRateValue { get; set; }
        public DateTimeOffset MinTimestamp { get; set; }
        public DateTimeOffset MaxTimestamp { get; set; }
    }
}
