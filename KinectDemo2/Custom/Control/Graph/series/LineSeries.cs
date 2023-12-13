using KinectDemo2.Custom.Control.Graph.segment;

namespace KinectDemo2.Custom.Control.Graph.series
{
    public class LineSeries : XYDataSeries
    {
        public static readonly BindableProperty MarkerSettingsProperty = 
            BindableProperty.Create(nameof(MarkerSettings), typeof(ChartMarkerSettings), typeof(XYDataSeries));

        public static readonly BindableProperty ShowMarkersProperty = 
            BindableProperty.Create(nameof(ShowMarkers), typeof(bool), typeof(XYDataSeries));

        public static readonly BindableProperty StrokeDashArrayProperty = 
            BindableProperty.Create(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(XYDataSeries));

        /// <summary>
        /// Gets or sets the option for customize the series markers.
        /// </summary>
        public ChartMarkerSettings MarkerSettings
        {
            get => (ChartMarkerSettings)GetValue(MarkerSettingsProperty);
            set => SetValue(MarkerSettingsProperty, value);
        }

        /// <summary>
        /// Gets or sets the value indicating whether to show markers for the series data point.
        /// </summary>
        public bool ShowMarkers
        {
            get => (bool)GetValue(ShowMarkersProperty);
            set => SetValue(ShowMarkersProperty, value);
        }

        /// <summary>
        /// Gets or sets the stroke dash array to customize the appearance of stroke.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }


        protected override ChartSegment CreateSegment()
        {
            return new LineSegment()
            {
                StrokeDashArray = StrokeDashArray,
            };
        }

        public virtual void DrawMarker(ICanvas canvas, int index, ShapeType type, Rect rect)
        {
        }

    }
}
