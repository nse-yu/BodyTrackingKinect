using KinectDemo2.Custom.Control.Graph.legend;
using KinectDemo2.Custom.Control.Graph.segment;
using System.Collections.ObjectModel;

namespace KinectDemo2.Custom.Control.Graph.series
{
    public abstract class ChartSeries : BindableObject
    {
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create(nameof(ItemsSource), typeof(object), typeof(ChartSeries));

        public static readonly BindableProperty FillProperty =
            BindableProperty.Create(nameof(Fill), typeof(Brush), typeof(ChartSeries), default(Brush));
        
        public static readonly BindableProperty OpacityProperty =
            BindableProperty.Create(nameof(Opacity), typeof(double), typeof(ChartSeries), defaultValue: 1.0);

        public static readonly BindableProperty IsVisibleProperty =
            BindableProperty.Create(nameof(IsVisible), typeof(bool), typeof(ChartSeries), defaultValue: true);

        public static readonly BindableProperty IsVisibleOnLegendProperty =
            BindableProperty.Create(nameof(IsVisibleOnLegend), typeof(bool), typeof(ChartSeries), defaultValue: false);

        public static readonly BindableProperty LegendIconProperty =
            BindableProperty.Create(nameof(LegendIcon), typeof(ChartLegendIconType), typeof(ChartSeries));

        public static readonly BindableProperty PaletteBrushesProperty =
            BindableProperty.Create(nameof(PaletteBrushes), typeof(IList<Brush>), typeof(ChartSeries));

        public static readonly BindableProperty XBindingPathProperty =
            BindableProperty.Create(nameof(XBindingPath), typeof(string), typeof(ChartSeries));
        public bool IsVisible
        {
            get => (bool)GetValue(IsVisibleProperty);
            set => SetValue(IsVisibleProperty, value);
        }
        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }
        public bool IsVisibleOnLegend
        {
            get => (bool)GetValue(IsVisibleOnLegendProperty);
            set => SetValue(IsVisibleOnLegendProperty, value);
        }
        public ChartLegendIconType LegendIcon
        {
            get => (ChartLegendIconType)GetValue(LegendIconProperty);
            set => SetValue(LegendIconProperty, value);
        }
        public IList<Brush> PaletteBrushes
        {
            get => (IList<Brush>)GetValue(PaletteBrushesProperty);
            set => SetValue(PaletteBrushesProperty, value);
        }
        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValue(ItemsSourceProperty, value);
        }
        public double Opacity
        {
            get => (double)GetValue(OpacityProperty);
            set => SetValue(OpacityProperty, value);
        }
        public string XBindingPath
        {
            get => (string)GetValue(XBindingPathProperty);
            set => SetValue(XBindingPathProperty, value);
        }

        /*
        private static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var control = bindable as ChartSeries;

            if (control.XBindingPath == null) return;

            switch(newValue)
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
        }
        */
        
        protected abstract ChartSegment CreateSegment();

        protected virtual Animation CreateAnimation(Action<double> callback)
        {
            return null;
        }

        public virtual void DrawDataLabel(ICanvas canvas, Brush fillcolor, string label, PointF point, int index)
        {

        }

        public virtual void DrawSeries(ICanvas canvas, ReadOnlyObservableCollection<ChartSegment> segments, RectF clipRect)
        {

        }

        public virtual int GetDataPointIndex(float pointX, float pointY)
        {
            return 0;
        }

    }
}