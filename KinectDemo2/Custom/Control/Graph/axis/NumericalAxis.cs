using SkiaSharp;
using SkiaSharp.Views.Maui;

namespace KinectDemo2.Custom.Control.Graph.axis
{
    public class NumericalAxis : RangeAxisBase
    {
        public static readonly BindableProperty IntervalProperty =
            BindableProperty.Create(nameof(Interval), typeof(double), typeof(NumericalAxis), defaultValue: .1);

        public static readonly BindableProperty MaximumProperty =
            BindableProperty.Create(nameof(Maximum), typeof(double), typeof(NumericalAxis), defaultValue: (double)1);

        public static readonly BindableProperty MinimumProperty =
            BindableProperty.Create(nameof(Minimum), typeof(double), typeof(NumericalAxis), defaultValue: (double)0);

        public double Interval
        {
            get => (double)GetValue(IntervalProperty);
            set => SetValue(IntervalProperty, value);
        }
        public double Maximum
        {
            get => (double)GetValue(MaximumProperty);
            set => SetValue(MaximumProperty, value);
        }
        public double Minimum
        {
            get => (double)GetValue(MinimumProperty);
            set => SetValue(MinimumProperty, value);
        }

        private static readonly float tickNumSize = 20;

        public override void DrawXAxis(SKCanvas canvas, SKRect arrangeRect, float plotAreaHeight)
        {
            // TODO: Calculate actual drawing area
            base.DrawXAxis(canvas, arrangeRect, plotAreaHeight);

            // TODO: Draw ticks
            for (var pos = (float)Minimum; pos <= Maximum; pos += (float)Interval)
            {
                var ptr = pos * (arrangeRect.Width / (float)Maximum) + arrangeRect.Left;
                PointF p1 = new(ptr, arrangeRect.Top);
                PointF p2 = new(ptr, arrangeRect.Top + arrangeRect.Height * .1f);
                
                // TODO: Draw major ticks
                DrawMajorTick(canvas, pos, p1, p2);
                DrawMajorTickValue(canvas, pos, new PointF(p2.X, p2.Y + tickNumSize));

                // TOOD: Draw grid lines
                DrawGridLine(canvas, pos, ptr, arrangeRect.Top, ptr, arrangeRect.Top - plotAreaHeight);
                
                // TODO: Draw minor ticks
                //DrawMinorTick();
            }
            using var titlePaint = new SKPaint()
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = BrushToSKColor(Title.Stroke),
                TextAlign = SKTextAlign.Center,
                IsAntialias = true,
                TextSize = (float)Title.FontSize,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
            };
            canvas.DrawText(Title.Text, arrangeRect.MidX, arrangeRect.MidY + titlePaint.TextSize / 2, titlePaint);

        }

        public override void DrawYAxis(SKCanvas canvas, SKRect arrangeRect, float plotAreaWidth)
        {
            // TODO: Calculate actual drawing area
            base.DrawYAxis(canvas, arrangeRect, plotAreaWidth);

            // TODO: Draw ticks
            for (var pos = (float)Minimum; pos <= Maximum; pos += (float)Interval)
            {
                var ptr = arrangeRect.Bottom - pos * arrangeRect.Height;
                PointF p1 = new(arrangeRect.Right, ptr);
                PointF p2 = new(arrangeRect.Right - arrangeRect.Width * .1f, ptr);

                // TODO: Draw major ticks
                DrawMajorTick(canvas, pos, p1, p2);
                DrawMajorTickValue(canvas, pos, new PointF(p2.X - tickNumSize, p2.Y));

                // TOOD: Draw grid lines
                DrawGridLine(canvas, pos, arrangeRect.Right, ptr, arrangeRect.Right + plotAreaWidth, ptr);
                
                // TODO: Draw minor ticks
                //DrawMinorTick();
            }
            // テキストを縦に描画するために回転変換を適用
            canvas.Save(); // 現在の描画状態を保存
            canvas.RotateDegrees(-90, arrangeRect.MidX, arrangeRect.MidY); // テキストを垂直方向に回転

            using var titlePaint = new SKPaint()
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = BrushToSKColor(Title.Stroke),
                TextAlign = SKTextAlign.Center,
                IsAntialias = true,
                TextSize = (float)Title.FontSize,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
            };
            canvas.DrawText(Title.Text, arrangeRect.MidX - titlePaint.TextSize / 2, arrangeRect.MidY, titlePaint);
            canvas.Restore(); // 描画状態を元に戻す
        }

        protected override void DrawMajorTick(SKCanvas canvas, double tickPosition, PointF point1, PointF point2)
        {
            // Implement your logic to draw a major tick at the specified tickPosition on the canvas
            using var tickPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = BrushToSKColor(AxisLineStyle.Stroke),
                StrokeWidth = (float)AxisLineStyle.StrokeWidth,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
            };
            canvas.DrawLine(point1.ToSKPoint(), point2.ToSKPoint(), tickPaint);
        }
        protected override void DrawMajorTickValue(SKCanvas canvas, object value, PointF point)
        {
            using var numPaint = new SKPaint()
            {
                Style = SKPaintStyle.StrokeAndFill,
                Color = SKColors.Black,
                TextSize = tickNumSize,
                TextAlign = SKTextAlign.Center,
                IsAntialias = true
            };
            canvas.DrawText(value is float v ? v.ToString("F1") : value.ToString(), point.ToSKPoint(), numPaint);
        }

        protected override void DrawMinorTick(SKCanvas canvas, double tickPosition, PointF point1, PointF point2)
        {

        }
        protected override void DrawGridLine(SKCanvas canvas, double position, float x1, float y1, float x2, float y2)
        {
            // Implement your logic to draw a major tick at the specified tickPosition on the canvas
            using var tickPaint = new SKPaint()
            {
                Style = SKPaintStyle.Stroke,
                Color = BrushToSKColor(MakeColorLighter(AxisLineStyle.Stroke, .3)),
                StrokeWidth = (float)AxisLineStyle.StrokeWidth,
                StrokeCap = SKStrokeCap.Round,
                StrokeJoin = SKStrokeJoin.Round,
            };
            canvas.DrawLine(x1, y1, x2, y2, tickPaint);
        }
    }
}
