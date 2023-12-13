using System.Collections.ObjectModel;

namespace KinectDemo2.Custom.Layout;

public partial class HeaderContainer : ContentView
{
    public static readonly BindableProperty StrokeColorProperty =
        BindableProperty.Create(nameof(StrokeColor), typeof(SolidColorBrush), typeof(HeaderContainer), Brush.Black);
    public static readonly BindableProperty HeaderFillColorProperty =
        BindableProperty.Create(nameof(HeaderFillColor), typeof(Color), typeof(HeaderContainer), defaultValue: Colors.Transparent);
    public static readonly BindableProperty ContentFillColorProperty =
        BindableProperty.Create(nameof(ContentFillColor), typeof(Color), typeof(HeaderContainer), defaultValue: Colors.Transparent);
    public static readonly BindableProperty StrokeThicknessProperty =
        BindableProperty.Create(nameof(StrokeThickness), typeof(double), typeof(HeaderContainer), defaultValue: 1.0);
    public static readonly BindableProperty HeaderPaddingProperty =
        BindableProperty.Create(nameof(HeaderPadding), typeof(Thickness), typeof(HeaderContainer), defaultValue: new Thickness(10));
    public static readonly BindableProperty ContentPaddingProperty =
        BindableProperty.Create(nameof(ContentPadding), typeof(Thickness), typeof(HeaderContainer), defaultValue: new Thickness(10));


    public ObservableCollection<View> Header { get; } = new ObservableCollection<View>();
    public ObservableCollection<View> ContainerContent { get; } = new ObservableCollection<View>();
    public SolidColorBrush StrokeColor
    {
        get => (SolidColorBrush)GetValue(StrokeColorProperty);
        set => SetValue(StrokeColorProperty, value);
    }
    public Color HeaderFillColor
    {
        get => (Color)GetValue(HeaderFillColorProperty);
        set => SetValue(HeaderFillColorProperty, value);
    }
    public Color ContentFillColor
    {
        get => (Color)GetValue(ContentFillColorProperty);
        set => SetValue(ContentFillColorProperty, value);
    }
    public double StrokeThickness
    {
        get => (double)GetValue(StrokeThicknessProperty);
        set => SetValue(StrokeThicknessProperty, value);
    }
    public Thickness HeaderPadding
    {
        get => (Thickness)GetValue(HeaderPaddingProperty);
        set => SetValue(HeaderPaddingProperty, value);
    }
    public Thickness ContentPadding
    {
        get => (Thickness)GetValue(ContentPaddingProperty);
        set => SetValue(ContentPaddingProperty, value);
    }

    public HeaderContainer()
	{
		InitializeComponent();
        BindingContext = this;
        Header.CollectionChanged += HeaderChanged;
        ContainerContent.CollectionChanged += ContentChanged;
    }

    private void HeaderChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems == null) return;
        foreach (View item in e.NewItems)
        {
            container_header.Children.Add(item);
        }
    }
    private void ContentChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems == null) return;
        foreach (View item in e.NewItems)
        {
            container_content.Children.Add(item);
        }
    }
}