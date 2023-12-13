using KinectDemo2.Custom.Control.Graph.segment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectDemo2.Custom.Control.Graph.series
{
    public class XYDataSeries : CartesianSeries
    {
        public static readonly BindableProperty YBindingPathProperty = 
            BindableProperty.Create(nameof(YBindingPath), typeof(string), typeof(XYDataSeries));

        public static readonly BindableProperty StrokeWidthProperty = 
            BindableProperty.Create(nameof(StrokeWidth), typeof(double), typeof(XYDataSeries));

        public string YBindingPath
        {
            get => (string)GetValue(YBindingPathProperty);
            set => SetValue(YBindingPathProperty, value);
        }
        public double StrokeWidth
        {
            get => (double)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        protected override ChartSegment CreateSegment()
        {
            throw new NotImplementedException();
        }
    }
}
