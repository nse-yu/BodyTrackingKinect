using System.Globalization;

namespace KinectDemo2.Custom.Control.Converter
{
    class ModelIndexToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value switch
            {
                0 => "lightning",
                1 => "thunder",
                2 => "mediapipe",
                _ => "lightning",
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "lightning" => 0,
                "thunder" => 1,
                "mediapipe" => 2,
                _ => 0
            };
        }
    }

    internal class MonitorValueToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)value switch
            {
                0 => "all",
                1 => "alert",
                _ => "monitor"
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (string)value switch
            {
                "all" => 0,
                "alert" => 1,
                "monitor" => 2,
                _ => 0
            };
        }
    }
}
