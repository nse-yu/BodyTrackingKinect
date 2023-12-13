using KinectDemo2.Custom.ViewModel;
using System.Globalization;

namespace KinectDemo2.Custom.Control.Converter
{
    internal class SliderValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var slValue = Math.Round((double)value);
            var mid = Math.Round(Average(SettingsViewModel.SLIDER_MIN, SettingsViewModel.SLIDER_MAX));

            if (slValue >= Math.Round(SettingsViewModel.SLIDER_MIN) && slValue < mid) return "low";

            if (slValue == mid) return "middle";

            return "high";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private static double Average(params double[] nums) => nums.Sum() / nums.Length;
    }
}
