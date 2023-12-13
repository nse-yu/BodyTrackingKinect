using KinectDemo2.Custom.Control.Graph.segment;

namespace KinectDemo2.Custom.Control.Graph.series
{
    /// <summary>
    /// To render a series, create an instance of RangeAreaSeries class, and add it to the Series collection.
    /// EnableTooltip - A tooltip displays information while tapping or mouse hovering above a segment.To display the tooltip on a chart, you need to set the EnableTooltip property as true in RangeAreaSeries class, and also refer TooltipBehavior property.
    /// Data Label - Data labels are used to display values related to a chart segment.To render the data labels, you need to set the ShowDataLabels property as true in RangeAreaSeries class. To customize the chart data labels alignment, placement, and label styles, you need to create an instance of CartesianDataLabelSettings and set to the DataLabelSettings property.
    /// Animation - To animate the series, set True to the EnableAnimation property.
    /// LegendIcon - To customize the legend icon using the LegendIcon property.
    /// </summary>
    public class RangeAreaSeries : RangeSeriesBase
    {
        public static readonly BindableProperty ShowMarkersProperty 
            = BindableProperty.Create(nameof(ShowMarkers), typeof(bool), typeof(RangeAreaSeries));

        public static readonly BindableProperty StrokeDashArrayProperty 
            = BindableProperty.Create(nameof(StrokeDashArray), typeof(DoubleCollection), typeof(RangeAreaSeries));

        /// <summary>
        /// Gets or sets the value indicating whether to show markers for the series data point.
        /// </summary>
        public bool ShowMarkers
        {
            get => (bool)GetValue(ShowMarkersProperty);
            set => SetValue(ShowMarkersProperty, value);
        }

        /// <summary>
        /// Gets or sets the stroke dash array to customize the appearance of the series border.
        /// </summary>
        public DoubleCollection StrokeDashArray
        {
            get => (DoubleCollection)GetValue(StrokeDashArrayProperty);
            set => SetValue(StrokeDashArrayProperty, value);
        }

        protected override ChartSegment CreateSegment()
        {
            throw new NotImplementedException();
        }
        protected virtual void DrawMarker(ICanvas canvas, int index, ShapeType type, Rect rect)
        {
            throw new NotImplementedException();
        }

        public override int GetDataPointIndex(float pointX, float pointY)
        {
            return 0;
        }

    }
}
