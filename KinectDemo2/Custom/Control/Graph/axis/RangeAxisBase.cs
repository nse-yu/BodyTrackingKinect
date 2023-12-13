using KinectDemo2.Custom.Control.Graph.axis.style;
using ChartLineStyle = KinectDemo2.Custom.Control.Graph.axis.style.ChartLineStyle;

namespace KinectDemo2.Custom.Control.Graph.axis
{
    public abstract class RangeAxisBase : ChartAxis
    {
        public static readonly BindableProperty MinorGridLineStyleProperty =
            BindableProperty.Create(nameof(MinorGridLineStyle), typeof(ChartLineStyle), typeof(RangeAxisBase), defaultValue: new ChartLineStyle());

        public static readonly BindableProperty MinorTicksPerIntervalProperty =
            BindableProperty.Create(nameof(MinorTicksPerInterval), typeof(int), typeof(RangeAxisBase));

        public static readonly BindableProperty MinorTickStyleProperty =
            BindableProperty.Create(nameof(MinorTickStyle), typeof(ChartAxisTickStyle), typeof(RangeAxisBase), defaultValue: new ChartAxisTickStyle());

        public static readonly BindableProperty ShowMinorGridLinesProperty =
            BindableProperty.Create(nameof(ShowMinorGridLines), typeof(bool), typeof(RangeAxisBase));


        public ChartLineStyle MinorGridLineStyle
        {
            get => GetValue(MinorGridLineStyleProperty) as ChartLineStyle;
            set => SetValue(MinorGridLineStyleProperty, value);
        }
        public int MinorTicksPerInterval
        {
            get => (int)GetValue(MinorTicksPerIntervalProperty);
            set => SetValue(MinorTicksPerIntervalProperty, value);
        }

        public ChartAxisTickStyle MinorTickStyle
        {
            get => GetValue(MinorTickStyleProperty) as ChartAxisTickStyle;
            set => SetValue(MinorTickStyleProperty, value);
        }

        public bool ShowMinorGridLines
        {
            get => (bool)GetValue(ShowMinorGridLinesProperty);
            set => SetValue(ShowMinorGridLinesProperty, value);
        }
    }
}
