using KinectDemo2.Custom.Model.tracking;
using System.Globalization;

namespace KinectDemo2.Custom.Control.Converter
{
    public class TrackingDataConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var data = value as BodyTrackingScore;
            return parameter.ToString() switch
            {
                "Time" => data.Time.ToString(),
                "Model" => data.Model.ToString(),
                "Score" => data.Score.ToString("F2"),
                "Id" => data.UserId.ToString(),
                _ => data.ToString(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
