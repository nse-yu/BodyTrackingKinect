using KinectDemo2.Custom.Control.Graph.series;
using System.ComponentModel;

namespace KinectDemo2.Custom.Control.Graph.segment
{
    public abstract class ChartSegment : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets the animation value of the associated series for the segment.
        /// </summary>
        public float AnimatedValue { get; }

        /// <summary>
        /// Gets or sets a brush value to customize the segment appearance.
        /// </summary>
        public Brush Fill { get; set; }

        /// <summary>
        /// Gets or sets a value to change the opacity of the segment.
        /// </summary>
        public float Opacity { get; set; }

        /// <summary>
        /// Gets the series associated with the segment.
        /// </summary>
        public ChartSeries Series { get; }

        /// <summary>
        /// Gets or sets a brush value to customize the border appearance of the segment.
        /// </summary>
        public Brush Stroke { get; set; }

        /// <summary>
        /// Gets or sets the stroke dash array to customize the appearance of stroke.
        /// </summary>
        public DoubleCollection StrokeDashArray { get; set; }

        /// <summary>
        /// Gets or sets a value to change the thickness of the segment's border.
        /// </summary>
        public double StrokeWidth { get; set; }

        protected virtual void Draw(ICanvas canvas)
        {

        }
        protected virtual void OnLayout()
        {

        }

    }
}
