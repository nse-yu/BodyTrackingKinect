namespace KinectDemo2.Custom.Control.Graph.axis.style
{
    /// <summary>
    /// It provides more options to customize the chart lines.
    /// Stroke - To customize the stroke color, refer to this Stroke property.
    /// StrokeWidth - To modify the stroke width, refer to this StrokeWidth property.
    /// StrokeDashArray - To customize the line with dashes and gaps, refer to this StrokeDashArray property.
    /// </summary>
    public class ChartLineStyle : Element
    {
        public static readonly BindableProperty StrokeDashArrayProperty = 
            BindableProperty.Create(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(ChartLineStyle));

        public static readonly BindableProperty StrokeProperty = 
            BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(ChartLineStyle), defaultValue: Brush.Gray);

        public static readonly BindableProperty StrokeWidthProperty = 
            BindableProperty.Create(nameof(StrokeWidth), typeof(double), typeof(ChartLineStyle), defaultValue: (double)4.0);

        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }
        public double StrokeWidth
        {
            get => (double)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }
        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

    }
}