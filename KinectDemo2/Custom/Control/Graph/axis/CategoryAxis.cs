using KinectDemo2.Custom.Control.Graph.util;
using SkiaSharp;

namespace KinectDemo2.Custom.Control.Graph.axis
{
    public class CategoryAxis : ChartAxis
    {
        public static readonly BindableProperty IntervalProperty =
            BindableProperty.Create(nameof(Interval), typeof(double), typeof(CategoryAxis));

        public static readonly BindableProperty LabelPlacementProperty =
            BindableProperty.Create(nameof(LabelPlacement), typeof(LabelPlacement), typeof(CategoryAxis));

        /// <summary>
        /// Gets or sets a value that can be used to customize the interval between the axis labels.
        /// If this property is not set, the interval will be calculated automatically.
        /// </summary>
        public double Interval { get; set; }

        /// <summary>
        /// Gets or sets a value that determines whether to place the axis label in between or on the tick lines.
        /// BetweenTicks - Used to place the axis label between the ticks.
        /// OnTicks - Used to place the axis label with the tick as the center.
        /// </summary>
        public LabelPlacement LabelPlacement { get; set; }

        protected override void DrawGridLine(SKCanvas canvas, double position, float x1, float y1, float x2, float y2)
        {
            throw new NotImplementedException();
        }

        protected override void DrawMajorTick(SKCanvas canvas, double tickPosition, PointF point1, PointF point2)
        {
            throw new NotImplementedException();
        }

        protected override void DrawMajorTickValue(SKCanvas canvas, object value, PointF point)
        {
            throw new NotImplementedException();
        }

        protected override void DrawMinorTick(SKCanvas canvas, double tickPosition, PointF point1, PointF point2)
        {
            throw new NotImplementedException();
        }
    }
}
