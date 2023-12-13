using KinectDemo2.Custom.Control.Graph.legend;
using SkiaSharp;
using SkiaSharp.Views.Maui.Controls;
using SkiaSharp.Views.Maui;
using System.ComponentModel;

namespace KinectDemo2.Custom.Control.Graph
{
    public abstract class ChartBase : SKCanvasView, INotifyPropertyChanged
    {
        private SKPoint? touchLocation;

        public static readonly BindableProperty LegendProperty =
            BindableProperty.Create(nameof(Legend), typeof(ChartLegend), typeof(ChartBase), propertyChanged: OnLegendChanged);

        public static readonly BindableProperty TitleProperty = 
            BindableProperty.Create(nameof(Title), typeof(object), typeof(ChartBase), propertyChanged: OnTitleChanged);

        /// <summary>
        /// To render a legend, create an instance of ChartLegend, and assign it to the Legend property.
        /// </summary>
        public ChartLegend Legend
        {
            get => (ChartLegend)GetValue(LegendProperty);
            set => SetValue(LegendProperty, value);
        }

        /// <summary>
        /// Gets or sets the title for chart. It supports the string or any view as title.
        /// </summary>
        public object Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public ChartBase()
        {
            EnableTouchEvents = true;
            MinimumWidthRequest = 500;
            MinimumHeightRequest = 500;
        }

        /*
        /// <summary>
        /// This property is provided only for class inherits ChartBase. 
        /// To render a plot area, create an instance of ChartBaseDrawable, and assign it to this property.
        /// </summary>
        public ChartBaseDrawable ChartDrawable
        {
            get => (ChartBaseDrawable)GetValue(ChartDrawableProperty);
            set
            {
                SetValue(ChartDrawableProperty, value);
                OnPropertyChanged(nameof(ChartDrawable));
            }
        }
        */

        /*
        private static void OnChartDrawableChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OnChangedDelegator(bindable, control =>
            {
                control.Drawable = newValue as CaresianChartDrawable;
                control.Invalidate();
            });
        }
        */

        protected static void OnChangedDelegator(BindableObject bindable, Action<ChartBase> action)
        {
            if (bindable is ChartBase controls)
            {
                action?.Invoke(controls);
            }
        }

        private static void OnLegendChanged(BindableObject bindable, object oldValue, object newValue)
        {
            /*
            OnChangedDelegator(bindable, control =>
            {
                control.PlotAreaView.Legend = newValue as ChartLegend;
                control.PlotAreaView.Invalidate();
            });
            */
        }

        private static void OnTitleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            OnChangedDelegator(bindable, control =>
            {
                control.InvalidateSurface();
            });
        }

        protected override void OnTouch(SKTouchEventArgs e)
        {
            touchLocation = e.InContact ? e.Location : null;

            InvalidateSurface();

            e.Handled = true;
        }


        private void DisplayInfoChanged(object sender, DisplayInfoChangedEventArgs e)
        {
            if(e.DisplayInfo.Width <= 400)
            {
                var s = 3;
            }
        }
    }
}
