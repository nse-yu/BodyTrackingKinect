namespace KinectDemo2.Custom.Control.Graph.util
{
    public sealed class DoubleRange
    {

        /// <summary>
        /// Gets an empty DoubleRange.
        /// </summary>
        public static DoubleRange Empty { get; } = new DoubleRange(0, 0);

        /// <summary>
        /// Gets the difference between the end and start of a DoubleRange.
        /// </summary>
        public double Delta => End - Start;

        /// <summary>
        /// Gets the end value of a DoubleRange.
        /// </summary>
        public double End { get; }

        /// <summary>
        /// Gets the start value of a DoubleRange.
        /// </summary>
        public double Start { get; }

        /// <summary>
        /// Gets the median value of a DoubleRange.
        /// </summary>
        public double Median => (Start + End) / 2;

        /// <summary>
        /// Gets a value indicating whether a DoubleRange is empty.
        /// </summary>
        public bool IsEmpty { get; }


        /// <summary>
        /// Initializes a new instance of the DoubleRange class with the specified start and end values.
        /// </summary>
        /// <param name="start">The start value of the range.</param>
        /// <param name="end">The end value of the range.</param>
        public DoubleRange(double start, double end)
        {
            Start = Math.Min(start, end);
            End = Math.Max(start, end);
            IsEmpty = false;
        }

        /// <summary>
        /// Determines whether a given range is inside this DoubleRange.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Inside(DoubleRange range)
        {
            return Start <= range.Start && End >= range.End;
        }

        /// <summary>
        /// Determines whether a given value is inside this DoubleRange.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Inside(double value)
        {
            return Start <= value && End >= value;
        }

        /// <summary>
        /// Determines whether this DoubleRange intersects with another range.
        /// </summary>
        /// <param name="range"></param>
        /// <returns></returns>
        public bool Intersects(DoubleRange range)
        {
            return Start <= range.End && End >= range.Start;
        }

        /// <summary>
        /// Determines whether this DoubleRange intersects with a given start and end value.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool Intersects(double start, double end)
        {
            return Start <= end && End >= start;
        }

        /// <summary>
        /// Offsets a DoubleRange by a specified value.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DoubleRange Offset(DoubleRange range, double value)
        {
            return new DoubleRange(range.Start + value, range.End + value);
        }

        /// <summary>
        /// Scales a DoubleRange by a specified value.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DoubleRange Scale(DoubleRange range, double value)
        {
            return new DoubleRange(range.Start * value, range.End * value);
        }

        /// <summary>
        /// Finds the union of two DoubleRanges.
        /// </summary>
        /// <param name="leftRange"></param>
        /// <param name="rightRange"></param>
        /// <returns></returns>
        public static DoubleRange Union(DoubleRange leftRange, DoubleRange rightRange)
        {
            double start = Math.Min(leftRange.Start, rightRange.Start);
            double end = Math.Max(leftRange.End, rightRange.End);
            return new DoubleRange(start, end);
        }

        /// <summary>
        /// Finds the union of a DoubleRange with a specified value.
        /// </summary>
        /// <param name="range"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DoubleRange Union(DoubleRange range, double value)
        {
            double start = Math.Min(range.Start, value);
            double end = Math.Max(range.End, value);
            return new DoubleRange(start, end);
        }

        /// <summary>
        /// Finds the union of a set of values.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static DoubleRange Union(params double[] values)
        {
            double start = values.Min();
            double end = values.Max();
            return new DoubleRange(start, end);
        }

    }
}
