using KinectDemo2.Custom.Control.Graph.axis;
using KinectDemo2.Custom.Control.Graph.series;

namespace KinectDemo2.Custom.Control.Graph.drawable
{
    public class CaresianChartDrawable : ChartBaseDrawable
    {
        /// <summary>
        /// Plot data
        /// </summary>
        public ChartSeriesCollection Series { get; set; }

        /// <summary>
        /// x-axis
        /// </summary>
        public List<ChartAxis> XAxes { get; set; }

        /// <summary>
        /// y-axis
        /// </summary>
        public List<NumericalAxis> YAxes { get; set; }

        // TODO: 描画には、GraphicsViewにある程度の描画領域が必要で、HeightRequest, WidthRequestなどで
        // 領域を確保しておかないと、Drawが呼び出されなくなる。
        public override void Draw(ICanvas canvas, RectF dirtyRect)
        {
            // TODO: Draw a Coordinate

            /*
            // TODO: Draw Axes
            XAxes.ForEach(yAxis =>
            {
                yAxis.DrawAxis(canvas, dirtyRect);
            });
            YAxes.ForEach(xAxis =>
            {
                xAxis.DrawAxis(canvas, dirtyRect);
            });
            */

            // TODO: Draw Points(Plot data)

            // TODO: Draw a Title
            canvas.DrawString(Title.ToString(), 100, 100, HorizontalAlignment.Center);
        }
    }
}