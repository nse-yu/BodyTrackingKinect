using KinectDemo2.Custom.Service;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Globalization;

namespace KinectDemo2.Custom.Control.Converter
{
    public class IdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            var id = (JointId)value;
            return KinectService.JOINTS[id];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
