using CommunityToolkit.Mvvm.ComponentModel;

namespace KinectDemo2.Custom.Model.tracking
{
    public class TrackingStatistics
    {
        private double _mean;
        private double _median;
        private double _std;

        public void SetProperties(double mean, double median, double std)
        {
            Mean = mean;
            Median = median;
            Std = std;
        }

        public double Mean
        {
            get => _mean;
            set => _mean = value;
        }
        public double Median
        {
            get => _median;
            set => _median = value;
        }
        public double Std
        {
            get => _std;
            set => _std = value;
        }
    }
}
