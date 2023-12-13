using Microsoft.Maui.Graphics.Text;

namespace KinectDemo2.Custom.Control.Graph.axis.style
{
    /// <summary>
    /// To customize the axis labels appearance, create an instance of the ChartAxisLabelStyle class and set to the LabelStyle property.
    /// It provides more options to customize the chart axis label.
    /// LabelAlignment - To position the axis label, refer to this LabelAlignment property.
    /// LabelFormat - To customize the numeric or date-time format of the axis label, refer to this LabelFormat property.
    /// TextColor - To customize the text color, refer to this TextColor property.
    /// Background - To customize the background color, refer to this Background property.
    /// Stroke - To customize the stroke color, refer to this Stroke property.
    /// StrokeWidth - To modify the stroke width, refer to this StrokeWidth property.
    /// </summary>
    public class ChartAxisLabelStyle : ChartLabelStyle
    {
        public static readonly BindableProperty LabelAlignmentProperty =
            BindableProperty.Create(nameof(LabelAlignment), typeof(ChartAxisLabelAlignment), typeof(ChartAxisLabelStyle));
        public ChartAxisLabelAlignment LabelAlignment { get; set; }
    }
}
