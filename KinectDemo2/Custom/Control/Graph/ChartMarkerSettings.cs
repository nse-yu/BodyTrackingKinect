using KinectDemo2.Custom.Control.Graph.series;

namespace KinectDemo2.Custom.Control.Graph
{
    /// <summary>
    /// データマーカーのシンボルをカスタマイズすることで、データポイントの位置が強調され、データの視覚化が改善されます。
    /// ChartMarkerSettingsクラスは、マーカーシンボルをカスタマイズするためのプロパティを提供します。タイプ、塗りつぶし、ストローク、ストローク幅、幅、高さなどを変更することができます。
    /// 
    /// シリーズ
    /// データマーカーシンボルは、Line、Spline、およびAreaシリーズなどのシリーズタイプに表示されます。要件や仕様に基づいて、シリーズとデータマーカーをデータの視覚化に追加できます。
    /// データマーカーを表示するには、ChartMarkerSettingsのインスタンスを作成し、それをMarkerSettingsに設定してシリーズに追加します。
    /// </summary>
    public class ChartMarkerSettings : BindableObject
    {
        public static readonly BindableProperty FillProperty = 
            BindableProperty.Create(nameof(Fill), typeof(Brush), typeof(ChartMarkerSettings));

        public static readonly BindableProperty HeightProperty = 
            BindableProperty.Create(nameof(Height), typeof(double), typeof(ChartMarkerSettings));

        public static readonly BindableProperty StrokeProperty = 
            BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(ChartMarkerSettings));

        public static readonly BindableProperty StrokeWidthProperty = 
            BindableProperty.Create(nameof(StrokeWidth), typeof(double), typeof(ChartMarkerSettings));

        public static readonly BindableProperty TypeProperty = 
            BindableProperty.Create(nameof(Type), typeof(ShapeType), typeof(ChartMarkerSettings));

        public static readonly BindableProperty WidthProperty = 
            BindableProperty.Create(nameof(Width), typeof(double), typeof(ChartMarkerSettings));

        public Brush Fill
        {
            get => (Brush)GetValue(FillProperty);
            set => SetValue(FillProperty, value);
        }

        public double Height
        {
            get => (double)GetValue(HeightProperty);
            set => SetValue(HeightProperty, value);
        }

        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }

        public double StrokeWidth
        {
            get => (double)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }

        public ShapeType Type
        {
            get => (ShapeType)GetValue(TypeProperty);
            set => SetValue(TypeProperty, value);
        }

        public double Width
        {
            get => (double)GetValue(WidthProperty);
            set => SetValue(WidthProperty, value);
        }

    }
}
