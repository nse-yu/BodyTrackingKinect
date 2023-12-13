using KinectDemo2.Custom.Control.Graph.legend;

namespace KinectDemo2.Custom.Control.Graph.drawable
{
    public abstract class ChartBaseDrawable : BindableObject, IDrawable
    {
        public object Title { get; set; }
        public ChartLegend Legend { get; set; }


        public abstract void Draw(ICanvas canvas, RectF dirtyRect);
            /*
        {
            canvas.DrawString(Title.ToString(), 100, 100, HorizontalAlignment.Center);
        }
            */

        /*
             protected override void OnPaintSurface(SKPaintSurfaceEventArgs args)
    {
        base.OnPaintSurface(args);

        var canvas = args.Surface.Canvas;
        var info = args.Info;

        // グラフの描画領域のサイズ
        var width = info.Width;
        var height = info.Height;

        // データポイントの数
        var count = ItemsSource?.Count ?? 0;

        if (count > 0)
        {
            // X軸の間隔とスケールを計算
            var xInterval = width / (count - 1);
            var xScale = width / (MaxValue - MinValue);

            // Y軸のスケールを計算
            var yScale = height / (MaxValue - MinValue);

            // 折れ線グラフを描画
            var path = new SKPath();
            path.MoveTo(0, height - ((ItemsSource[0].Y - MinValue) * yScale));

            for (int i = 1; i < count; i++)
            {
                var x = i * xInterval;
                var y = height - ((ItemsSource[i].Y - MinValue) * yScale);
                path.LineTo(x, y);
            }

            using (var paint = new SKPaint())
            {
                paint.Style = SKPaintStyle.Stroke;
                paint.Color = SKColors.Blue;
                paint.StrokeWidth = 2;

                canvas.DrawPath(path, paint);
            }
        }
    }

         */
    }
}
