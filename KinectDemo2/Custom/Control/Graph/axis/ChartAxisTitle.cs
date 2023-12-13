using KinectDemo2.Custom.Control.Graph.axis.style;

namespace KinectDemo2.Custom.Control.Graph.axis
{
    /// <summary>
    /// To customize the chart axis's title, add the ChartAxisTitle instance to the Title property as shown in the following code sample.
    /// </summary>
    public class ChartAxisTitle : ChartLabelStyle
    {
        public static readonly BindableProperty TextProperty =
            BindableProperty.Create(nameof(Text), typeof(string), typeof(ChartAxisTitle), defaultValue: "");

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}