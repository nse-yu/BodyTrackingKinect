namespace KinectDemo2.Custom.Control.Graph.series
{
    public abstract class RangeSeriesBase : CartesianSeries
    {
        public static readonly BindableProperty HighProperty = 
            BindableProperty.Create(nameof(High), typeof(string), typeof(RangeSeriesBase));

        public static readonly BindableProperty LowProperty = 
            BindableProperty.Create(nameof(Low), typeof(string), typeof(RangeSeriesBase));

        public static readonly BindableProperty StrokeProperty = 
            BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(RangeSeriesBase));

        public static readonly BindableProperty StrokeWidthProperty = 
            BindableProperty.Create(nameof(StrokeWidth), typeof(double), typeof(RangeSeriesBase));

        public string High
        {
            get => (string)GetValue(HighProperty);
            set => SetValue(HighProperty, value);
        }
        public string Low
        {
            get => (String)GetValue(LowProperty);
            set => SetValue(LowProperty, value);
        }
        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }
        public double StrokeWidth
        {
            get => (double)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

    }
}
