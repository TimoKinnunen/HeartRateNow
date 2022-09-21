namespace HeartRateNow.Models
{
    public class HeartRateDataPoint
    {
        public int Id { get; set; }
        public double OffsetX { get; set; }
        public double OffsetY { get; set; }
        //public double HeartRateValue { get; set; }
        public uint HeartRateValue { get; set; }
    }
}
