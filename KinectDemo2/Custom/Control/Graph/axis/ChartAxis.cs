using KinectDemo2.Custom.Control.Graph.axis.style;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using ChartAxisLabelStyle = KinectDemo2.Custom.Control.Graph.axis.style.ChartAxisLabelStyle;
using DoubleRange = KinectDemo2.Custom.Control.Graph.util.DoubleRange;

namespace KinectDemo2.Custom.Control.Graph.axis
{
    /// <summary>
    /// The ChartAxis is used to locate a data point inside the chart area. 
    /// Charts typically have two axes that are used to measure and categorize data.
    /// The Vertical(Y) axis always uses numerical scale.
    /// The Horizontal(X) axis supports the Category, Numeric and Date-time.
    /// </summary>
    public abstract class ChartAxis : Element
    {
        public static readonly BindableProperty AxisLineOffsetProperty = 
            BindableProperty.Create(nameof(AxisLineOffset), typeof(double), typeof(ChartAxis));

        public static readonly BindableProperty CrossAxisNameProperty = 
            BindableProperty.Create(nameof(CrossAxisName), typeof(string), typeof(ChartAxis));

        public static readonly BindableProperty AxisLineStyleProperty = 
            BindableProperty.Create(nameof(AxisLineStyle), typeof(ChartLineStyle), typeof(ChartAxis), defaultValue: new ChartLineStyle());

        public static readonly BindableProperty IsInversedProperty = 
            BindableProperty.Create(nameof(IsInversed), typeof(bool), typeof(ChartAxis));

        public static readonly BindableProperty IsVisibleProperty = 
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(ChartAxis));

        public static readonly BindableProperty LabelRotationProperty = 
            BindableProperty.Create(nameof(LabelRotation), typeof(double), typeof(ChartAxis));

        public static readonly BindableProperty LabelStyleProperty = 
            BindableProperty.Create(nameof(LabelStyle), typeof(ChartAxisLabelStyle), typeof(ChartAxis));

        public static readonly BindableProperty NameProperty = 
            BindableProperty.Create(nameof(Name), typeof(string), typeof(ChartAxis));

        public static readonly BindableProperty PlotOffsetEndProperty = 
            BindableProperty.Create(nameof(PlotOffsetEnd), typeof(double), typeof(ChartAxis));

        public static readonly BindableProperty PlotOffsetStartProperty = 
            BindableProperty.Create(nameof(PlotOffsetStart), typeof(double), typeof(ChartAxis));

        public static readonly BindableProperty TitleProperty =
            BindableProperty.Create(nameof(Title), typeof(ChartAxisTitle), typeof(ChartAxis), defaultValue: new ChartAxisTitle());

        /// <summary>
        /// Gets or sets a value to provide padding to the axis line.
        /// </summary>
        public double AxisLineOffset
        {
            get => (double)GetValue(AxisLineOffsetProperty);
            set => SetValue(AxisLineOffsetProperty, value);
        }

        /// <summary>
        /// Gets or sets the value for the CrossAxisName of chart axis.
        /// </summary>
        public string CrossAxisName
        {
            get => GetValue(CrossAxisNameProperty) as string;
            set => SetValue(CrossAxisNameProperty, value);
        }

        /// <summary>
        /// To customize the axis line appearance, you need to create an instance of the ChartLineStyle class and set to the AxisLineStyle property.
        /// </summary>
        public ChartLineStyle AxisLineStyle
        {
            get => GetValue(AxisLineStyleProperty) as ChartLineStyle;
            set => SetValue(AxisLineStyleProperty, value);
        }

        /// <summary>
        /// Gets or sets the value that indicates whether the axis' visible range is inversed.
        /// When the axis is inversed, it will render points from right to left for the horizontal axis, and top to bottom for the vertical axis.
        /// </summary>
        public bool IsInversed
        {
            get => (bool)GetValue(IsInversedProperty);
            set => SetValue(IsInversedProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether to show/hide the chart axis.
        /// </summary>
        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }

        /// <summary>
        /// Gets or sets the value for the rotation angle of the axis labels.
        /// Label rotation angle can be set from -90 to 90 degrees.
        /// </summary>
        public double LabelRotation
        {
            get => (double)GetValue(LabelRotationProperty);
            set => SetValue(LabelRotationProperty, value);
        }

        /// <summary>
        /// Gets or sets the value to customize the appearance of chart axis labels.
        /// To customize the axis labels appearance, you need to create an instance of the ChartAxisLabelStyle class and set to the LabelStyle property.
        /// </summary>
        public ChartAxisLabelStyle LabelStyle
        {
            get => GetValue(LabelStyleProperty) as ChartAxisLabelStyle;
            set => SetValue(LabelStyleProperty, value);
        }
        
        /// <summary>
        /// Gets or sets the unique name of the axis, which will be used to identify the segment axis of the strip line.
        /// </summary>
        public string Name
        {
            get => GetValue(NameProperty) as string;
            set => SetValue(NameProperty, value);
        }

        /// <summary>
        /// Gets or sets a value to provide padding to the axis at end position.
        /// PlotOffsetEnd applies padding at end of the plot area where the axis and its elements are rendered in the chart with padding at the end.
        /// </summary>
        public double PlotOffsetEnd
        {
            get => (double)GetValue(PlotOffsetEndProperty);
            set => SetValue(PlotOffsetEndProperty, value);
        }
        
        /// <summary>
        /// Gets or sets a value to provide padding to the axis at the start position.
        /// PlotOffsetStart applies padding at the start of a plot area where the axis and its elements are rendered in a chart with padding at the start.
        /// </summary>
        public double PlotOffsetStart
        {
            get => (double)GetValue(PlotOffsetStartProperty);
            set => SetValue(PlotOffsetStartProperty, value);
        }

        public ChartAxisTitle Title
        {
            get => GetValue(TitleProperty) as ChartAxisTitle;
            set => SetValue(TitleProperty, value);
        }

        public ChartAxis()
        {

        }

        public virtual double CalculateActualInterval(DoubleRange range, Size availableSize)
        {
            // Implement your logic to calculate the actual interval based on the range and available size
            // Here is a simple example that divides the range into equal intervals based on the available width

            double width = availableSize.Width;
            int divisions = 10; // Number of equal intervals

            if (width > 0 && divisions > 0)
            {
                double intervalWidth = width / divisions;
                double interval = range.Delta / divisions;
                return Math.Ceiling(interval / intervalWidth) * interval;
            }

            return 0;
        }

        /// <summary>
        /// Drawing flow for a x-axis.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="arrangeRect"></param>
        public virtual void DrawXAxis(SKCanvas canvas, SKRect arrangeRect, float plotAreaHeight)
        {
            // TODO: Draw lines
            DrawAxisLine(canvas, (float)arrangeRect.Left, (float)arrangeRect.Top, (float)arrangeRect.Right, (float)arrangeRect.Top);
        }

        /// <summary>
        /// Drawing flow for a y-axis.
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="arrangeRect"></param>
        public virtual void DrawYAxis(SKCanvas canvas, SKRect arrangeRect, float plotAreaWidth)
        {
            // TODO: Draw lines
            DrawAxisLine(canvas, (float)arrangeRect.Right, (float)arrangeRect.Top, (float)arrangeRect.Right, (float)arrangeRect.Bottom);
        }

        /// <summary>
        /// Draw a line pointed at start(x1, y1) to end(x2, y2).
        /// </summary>
        /// <param name="canvas">Drawing canvas</param>
        /// <param name="x1">start point on x-axis</param>
        /// <param name="y1">start point on y-axis</param>
        /// <param name="x2">end point on x-axis</param>
        /// <param name="y2">end point on y-axis</param>
        protected virtual void DrawAxisLine(SKCanvas canvas, float x1, float y1, float x2, float y2)
        {
            if (AxisLineStyle == null) return;

            // Implement your logic to draw the axis line on the canvas
            using var paint = new SKPaint();

            paint.Style = SKPaintStyle.Stroke;
            paint.Color = BrushToSKColor(AxisLineStyle.Stroke);
            paint.StrokeWidth = (float)AxisLineStyle.StrokeWidth;
            paint.StrokeCap = SKStrokeCap.Round;
            paint.StrokeJoin = SKStrokeJoin.Round;

            canvas.DrawLine(x1, y1, x2, y2, paint);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas">Drawing canvas</param>
        /// <param name="position"></param>
        /// <param name="x1">start point on x-axis</param>
        /// <param name="y1">start point on y-axis</param>
        /// <param name="x2">end point on x-axis</param>
        /// <param name="y2">end point on y-axis</param>
        protected abstract void DrawGridLine(SKCanvas canvas, double position, float x1, float y1, float x2, float y2);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="tickPosition"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        protected abstract void DrawMajorTick(SKCanvas canvas, double tickPosition, PointF point1, PointF point2);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="tickPosition"></param>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        protected abstract void DrawMinorTick(SKCanvas canvas, double tickPosition, PointF point1, PointF point2);

        protected abstract void DrawMajorTickValue(SKCanvas canvas, object value, PointF point);

        /// <summary>
        /// 
        /// </summary>
        protected override void OnBindingContextChanged()
        {
            // Implement any necessary logic when the binding context changes
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="label"></param>
        public virtual void OnLabelCreated(ChartAxisLabel label)
        {
            // Implement any necessary logic when a chart axis label is created
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public double PointToValue(double x, double y)
        {
            // Implement your logic to convert a point coordinate to a corresponding value on the axis
            return 0; // Placeholder return value
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float ValueToPoint(double value)
        {
            // Implement your logic to convert a value on the axis to a corresponding point coordinate
            return 0; // Placeholder return value
        }

        /// <summary>
        /// Converts a Brush to an SKColor.
        /// Default to using black color if the Brush is not a SolidColorBrush
        /// </summary>
        /// <param name="brush">The Brush to convert.</param>
        /// <returns>The corresponding SKColor.</returns>
        public static SKColor BrushToSKColor(Brush brush)
        {
            return new SKColor(brush is SolidColorBrush colorBrush ? colorBrush.Color.ToUint() : Colors.Black.ToUint());
        }

        public static Brush MakeColorLighter(Brush brush, double alpha)
        {
            if (brush is SolidColorBrush solidColorBrush)
            {
                Color color = solidColorBrush.Color;
                Color lighterColor = Color.FromRgba(
                    (byte)(color.Red),
                    (byte)(color.Green),
                    (byte)(color.Blue),
                    alpha
                );
                return new SolidColorBrush(lighterColor);
            }
            
            // 薄くする方法がサポートされていないBrushの場合は元のBrushを返す
            return brush;
        }

    }
}
