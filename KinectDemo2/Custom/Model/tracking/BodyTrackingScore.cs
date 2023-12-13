namespace KinectDemo2.Custom.Model.tracking
{
    public class BodyTrackingScore
    {
        public string UserId { get; set; }
        public string Model { get; set; }
        public float Score { get; set; }
        public bool MonitoringMode { get; set; }
        public DateTime Time { get; set; }
    }
}
