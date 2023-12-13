using KinectDemo2.Custom.Control.Button.Base;

namespace KinectDemo2.Custom.Control.Button;

public partial class IconButton : ClickableButton
{
    public static readonly BindableProperty IconProperty = BindableProperty.Create(
        nameof(Icon), typeof(ImageSource), typeof(IconButton), propertyChanged: OnIconChanged
        );
    public static readonly BindableProperty IconWidthProperty = BindableProperty.Create(
        nameof(IconWidth), typeof(double), typeof(IconButton), propertyChanged: OnWidthChanged
        );
    public static readonly BindableProperty IconHeightProperty = BindableProperty.Create(
        nameof(IconHeight), typeof(double), typeof(IconButton), propertyChanged: OnHeightChanged
        );
    
    public ImageSource Icon
    {
        get => (ImageSource)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    public double IconWidth
    {
        get => (double)GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }
    public double IconHeight
    {
        get => (double)GetValue(IconHeightProperty);
        set => SetValue(IconHeightProperty, value);
    }

    public IconButton()
	{
		InitializeComponent();
	}
    private static void OnHeightChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if(oldValue == newValue) return;
        var img = ((IconButton)bindable).iconImage;
        img.HeightRequest = (double)newValue;
    }
    private static void OnWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue == newValue) return;
        var img = ((IconButton)bindable).iconImage;
        img.WidthRequest = (double)newValue;
    }
    private static void OnIconChanged(BindableObject bindable, object oldValue, object newValue)
    {
        if (oldValue == newValue) return;
        var img = ((IconButton)bindable).iconImage;
        img.Source = (ImageSource)newValue;
    }
}