using KinectDemo2.Custom.Control.Graph.axis;

namespace KinectDemo2.Custom.Control.Graph.series
{
    public abstract class CartesianSeries : ChartSeries
    {
        public static readonly BindableProperty LabelProperty = 
            BindableProperty.Create(nameof(Label), typeof(string), typeof(CartesianSeries));

        public static readonly BindableProperty ActualXAxisProperty = 
            BindableProperty.Create(nameof(ActualXAxis), typeof(ChartAxis), typeof(CartesianSeries));

        public static readonly BindableProperty ActualYAxisProperty = 
            BindableProperty.Create(nameof(ActualYAxis), typeof(ChartAxis), typeof(CartesianSeries));

        public static readonly BindableProperty XAxisNameProperty = 
            BindableProperty.Create(nameof(XAxisName), typeof(string), typeof(CartesianSeries));

        public static readonly BindableProperty YAxisNameProperty = 
            BindableProperty.Create(nameof(YAxisName), typeof(string), typeof(CartesianSeries));

        public string Label
        {
            get => (string)GetValue(LabelProperty);
            set => SetValue(LabelProperty, value);
        }
        public ChartAxis ActualXAxis
        {
            get => GetValue(ActualXAxisProperty) as ChartAxis;
            set => SetValue(ActualXAxisProperty, value);
        }
        public ChartAxis ActualYAxis
        {
            get => GetValue(ActualYAxisProperty) as ChartAxis;
            set => SetValue(ActualYAxisProperty, value);
        }
        public string XAxisName
        {
            get => GetValue(XAxisNameProperty) as string;
            set => SetValue(XAxisNameProperty, value);
        }
        public string YAxisName
        {
            get => GetValue(YAxisNameProperty) as string;
            set => SetValue(YAxisNameProperty, value);
        }

    }
}
