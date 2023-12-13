using KinectDemo2.Custom.Control.Graph.axis;
using KinectDemo2.Custom.Control.Graph.series;
using SkiaSharp;
using SkiaSharp.Views.Maui;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace KinectDemo2.Custom.Control.Graph;

/// <summary>
/// Visualize the data flow of Series property on a Cartesian Coordinate System 
/// with XAxes and YAxes Representing the x-axis and y-axis.
/// </summary>
public partial class CaresianChart : ChartBase
{
    public static readonly BindableProperty SeriesProperty =
        BindableProperty.Create(nameof(Series), typeof(ChartSeriesCollection), typeof(CaresianChart), defaultValue: new ChartSeriesCollection());

    /// <summary>
    /// Horizontal(X) axis supports the Category, Numeric and Date time.
    /// </summary>
    public ObservableCollection<ChartAxis> XAxes { get; } = new ObservableCollection<ChartAxis>();

    /// <summary>
    /// Vertical(Y) axis always uses numerical scale.
    /// </summary>
	public ObservableCollection<NumericalAxis> YAxes { get; } = new ObservableCollection<NumericalAxis>();

    /// <summary>
    /// ChartSeries is the visual representation of data. SfCartesianChart offers many types
    /// such as Line, Fast line, Spline, Column, Scatter, Area and SplineArea series. 
    /// Based on your requirements and specifications, any type of series can be added for data visualization.
    /// </summary>
    public ChartSeriesCollection Series
    {
        get => (ChartSeriesCollection)GetValue(SeriesProperty);
        set => SetValue(SeriesProperty, value);
    }

    private static readonly float plotAreaRatioHorizontal = .9f;
    private static readonly float plotAreaRatioVertical = .7f;
    private static readonly float titleAreaRatio = .1f;

    public CaresianChart()
	{
		InitializeComponent();
        XAxes.CollectionChanged += AxesChanged;
        YAxes.CollectionChanged += AxesChanged;
        Series.CollectionChanged += SeriesChanged;
    }

    private void SeriesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateSurface();
    }

    private void AxesChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
        InvalidateSurface();
    }

    protected static void OnPaintSurfaceDelegator(SKPaintSurfaceEventArgs args, Action<SKCanvas> action)
    {
        action?.Invoke(args.Surface.Canvas);
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
        OnPaintSurfaceDelegator(e, canvas =>
        {
            canvas.Clear();

            // Assigned rectangle object
            var rect = e.Info.Rect;
            
            // Height of the title area from the top of canvas
            var titleAreaHeight = (float)(rect.Height * titleAreaRatio);

            // Width of the plot area bottom of the title area
            var plotAreaWidth = (float)(rect.Width * plotAreaRatioHorizontal);

            // Height of the plot area bottom of the title area
            var plotAreaHeight = (float)(rect.Height * plotAreaRatioVertical);

            if (Title != null)
            {
                var titleRect = SKRect.Create(rect.Left + (rect.Width - plotAreaWidth), rect.Top, plotAreaWidth, titleAreaHeight);

                // decide what the text looks like
                using var textPaint = new SKPaint()
                {
                    Style = SKPaintStyle.Fill,
                    TextAlign = SKTextAlign.Center,
                    IsAntialias = true
                };
                using var rectPaint = new SKPaint()
                {
                    IsAntialias = true,
                    Style = SKPaintStyle.Stroke,
                    TextAlign= SKTextAlign.Center,
                };
                var titleText = "";
                float radius = 0;

                switch (Title)
                {
                    case ChartAxisTitle title:
                        textPaint.Color = title.TextColor.ToSKColor();
                        textPaint.TextSize = (float)title.FontSize;
                        rectPaint.Color = new SKColor(title.Stroke is SolidColorBrush colorBrush ? colorBrush.Color.ToUint() : Colors.Black.ToUint());
                        rectPaint.StrokeWidth = (float)title.StrokeWidth;
                        titleText = title.Text;
                        radius = (float)title.CornerRadius.TopRight;
                        break;
                    case string title:
                        textPaint.Color = SKColors.Black;
                        rectPaint.Color = SKColors.Transparent;
                        rectPaint.StrokeWidth = 2;
                        titleText = title;
                        radius = 0;
                        break;
                    default:
                        break;
                }

                // テキストの描画範囲を取得
                SKRect textBounds = new();
                textPaint.MeasureText(titleText, ref textBounds);

                // 四角形の領域を計算
                float padding = 10;

                var titleStrokeRect = new SKRect(
                    titleRect.MidX - textBounds.Width / 2 - padding,
                    titleRect.MidY - textBounds.Height / 2 - padding * 2,
                    titleRect.MidX + textBounds.Width / 2 + padding,
                    titleRect.MidY + textBounds.Height / 2 + padding);

                // draw rectangle surrounding text
                //canvas.DrawRect(rect, rectPaint);
                canvas.DrawRoundRect(titleStrokeRect, new SKSize(radius, radius), rectPaint);

                // draw some text
                canvas.DrawText(titleText, titleRect.MidX, titleRect.MidY, textPaint);
            }
            if (XAxes != null && YAxes != null)
            {
                var xAxisRect = SKRect.Create(rect.Left + (float)(rect.Width - plotAreaWidth), titleAreaHeight + plotAreaHeight, plotAreaWidth, rect.Height * (1 - titleAreaRatio - plotAreaRatioVertical));
                var yAxisRect = SKRect.Create(rect.Left, rect.Top + titleAreaHeight, (float)(rect.Width - plotAreaWidth), plotAreaHeight);
                
                XAxes.ToList().ForEach(axis =>
                {
                    axis.DrawXAxis(canvas, xAxisRect, plotAreaHeight);
                });
                YAxes.ToList().ForEach(axis =>
                {
                    axis.DrawYAxis(canvas, yAxisRect, plotAreaWidth);
                });
                
            }
            /*
            if (Series != null)
            {
                Series.ToList().ForEach(entry =>
                {
                    (LineSeries)entry
                    switch (newValue)
                    {
                        case IEnumerable<object> itemsSource:
                            itemsSource.ToList().ForEach(item =>
                            {
                                var propertyInfo = item.GetType().GetProperty(control.XBindingPath) ?? throw new ArgumentException(nameof(XBindingPath));
                                var value = propertyInfo?.GetValue(item);
                            });
                            break;
                        default:
                            throw new ArgumentException("XBindingPath property detected invalid value of type.", nameof(newValue));
                    }
                });

            }
            */
        });
    }
}