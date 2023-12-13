using System.Globalization;

namespace KinectDemo2.Custom.Control.Converter
{
    internal class NegaPosiConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var num = (double)value;

            return -num;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
