using System.Globalization;

namespace KinectDemo2.Custom.Control.Converter
{
    internal class DaySelectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<int> intList) return intList.Select(i => String.Format("{0}日", i)).ToList();
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
