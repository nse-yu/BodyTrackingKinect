namespace KinectDemo2.Custom.Control.Graph.axis.style
{
    /// <summary>
    /// It provides more options to customize the label.
    /// TextColor - To customize the text color, refer to this TextColor property.
    /// Background - To customize the background color, refer to this Background property.
    /// Stroke - To customize the stroke color, refer to this Stroke property.
    /// StrokeWidth - To modify the stroke width, refer to this StrokeWidth property.
    /// Margin - To adjust the outer margin for labels, refer to this Margin property.
    /// LabelFormat - To customize the label format for labels, refer to this LabelFormat property.
    /// CornerRadius - To defines the rounded corners for labels, refer to this CornerRadius property.
    /// CornerRadius - To change the text size for labels, refer to this FontSize property.
    /// FontFamily - To change the font family for labels, refer to this FontFamily property.
    /// FontAttributes - To change the font attributes for labels, refer to this FontAttributes property.
    /// </summary>
    public class ChartLabelStyle : Element
    {
        public static readonly BindableProperty BackgroundProperty =
            BindableProperty.Create(nameof(Background), typeof(Brush), typeof(ChartLabelStyle));

        public static readonly BindableProperty CornerRadiusProperty =
            BindableProperty.Create(nameof(CornerRadius), typeof(CornerRadius), typeof(ChartLabelStyle), defaultValue: new CornerRadius());

        public static readonly BindableProperty FontAttributesProperty =
            BindableProperty.Create(nameof(FontAttributes), typeof(FontAttributes), typeof(ChartLabelStyle));

        public static readonly BindableProperty FontFamilyProperty =
            BindableProperty.Create(nameof(FontFamily), typeof(string), typeof(ChartLabelStyle));

        public static readonly BindableProperty FontSizeProperty =
            BindableProperty.Create(nameof(FontSize), typeof(double), typeof(ChartLabelStyle), defaultValue: (double)20.0);

        public static readonly BindableProperty LabelFormatProperty =
            BindableProperty.Create(nameof(LabelFormat), typeof(string), typeof(ChartLabelStyle));

        public static readonly BindableProperty MarginProperty =
            BindableProperty.Create(nameof(Margin), typeof(Thickness), typeof(ChartLabelStyle));

        public static readonly BindableProperty StrokeProperty =
            BindableProperty.Create(nameof(Stroke), typeof(Brush), typeof(ChartLabelStyle), defaultValue: Brush.Black);

        public static readonly BindableProperty StrokeWidthProperty =
            BindableProperty.Create(nameof(StrokeWidth), typeof(double), typeof(ChartLabelStyle), defaultValue: (double)2);

        public static readonly BindableProperty TextColorProperty =
            BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(ChartLabelStyle), defaultValue: Colors.Black);

        public Color TextColor
        {
            get => (Color)GetValue(TextColorProperty);
            set => SetValue(TextColorProperty, value);
        }
        public Brush Background
        {
            get => (Brush)GetValue(BackgroundProperty);
            set => SetValue(BackgroundProperty, value);
        }
        public Thickness Margin
        {
            get => (Thickness)GetValue(MarginProperty);
            set => SetValue(MarginProperty, value);
        }
        public double StrokeWidth
        {
            get => (double)GetValue(StrokeWidthProperty);
            set => SetValue(StrokeWidthProperty, value);
        }
        public Brush Stroke
        {
            get => (Brush)GetValue(StrokeProperty);
            set => SetValue(StrokeProperty, value);
        }
        public double FontSize
        {
            get => (double)GetValue(FontSizeProperty);
            set => SetValue(FontSizeProperty, value);
        }
        public string FontFamily
        {
            get => (string)GetValue(FontFamilyProperty);
            set => SetValue(FontFamilyProperty, value);
        }
        public FontAttributes FontAttributes
        {
            get => (FontAttributes)GetValue(FontAttributesProperty);
            set => SetValue(FontAttributesProperty, value);
        }
        public string LabelFormat
        {
            get => (string)GetValue(LabelFormatProperty);
            set => SetValue(LabelFormatProperty, value);
        }
        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

    }
}
