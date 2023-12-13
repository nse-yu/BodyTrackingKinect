namespace KinectDemo2.Custom.Control.Graph.segment
{
    /// <summary>
    /// Represents a segment of a line chart.
    /// </summary>
    public class LineSegment : ChartSegment
    {
        /// <summary>
        /// Gets the x value for the starting point of the line segment.
        /// </summary>
        public float X1 { get; }

        /// <summary>
        /// Gets the x value for the ending point of the line segment.
        /// </summary>
        public float X2 { get; }

        /// <summary>
        /// Gets the y value for the starting point of the line segment.
        /// </summary>
        public float Y1 { get; }

        /// <summary>
        /// Gets the y value for the ending point of the line segment.
        /// </summary>
        public float Y2 { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="canvas"></param>
        protected override void Draw(ICanvas canvas)
        {

        }

        /// <summary>
        /// 
        /// </summary>
        protected override void OnLayout()
        {

        }

    }
}
