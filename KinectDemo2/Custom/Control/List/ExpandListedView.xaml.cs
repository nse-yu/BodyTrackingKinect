using System.Collections.ObjectModel;
using System.Windows.Input;

namespace KinectDemo2.Custom.Control.List;

public partial class ExpandListedView : ContentView
{
    public static readonly BindableProperty ColorProperty =
        BindableProperty.Create(nameof(Color), typeof(Color), typeof(ExpandListedView), defaultValue: Colors.White);
	public static readonly BindableProperty ExpandedProperty = 
		BindableProperty.Create(nameof(Expanded), typeof(bool), typeof(ExpandListedView), defaultBindingMode: BindingMode.TwoWay, propertyChanged: ExpandedChanged);
	public static readonly BindableProperty ButtonWidthProperty = 
		BindableProperty.Create(nameof(ButtonWidth), typeof(double), typeof(ExpandListedView), propertyChanged: ButtonWidthChanged, defaultValue: 30.0);

    private static readonly uint length = 500;
	private static readonly double rotate = 180;
	private static readonly double radius_rate = .2;
	private static Thickness expandedPadding = new((double)ButtonWidthProperty.DefaultValue * .15, 0, (double)ButtonWidthProperty.DefaultValue * .35, 0);
    private static Thickness closedPadding = new((double)ButtonWidthProperty.DefaultValue * .2, 0, (double)ButtonWidthProperty.DefaultValue * .3, 0);

    public double MaxHeight { get; } = 100;
    public double MinHeight { get; } = 10;
    public Thickness ButtonPadding { get; protected set; } = closedPadding;
    public CornerRadius ButtonRadius { get; } = new(0, (double)ButtonWidthProperty.DefaultValue * radius_rate, 0, (double)ButtonWidthProperty.DefaultValue * radius_rate);
    public ICommand TappedCommand { get; }
    public ICommand ExpandedCommand { get; set; }
    public ObservableCollection<View> Contents { get; } = new ObservableCollection<View>();
    public Color Color
	{
		get => (Color)GetValue(ColorProperty);
		set => SetValue(ColorProperty, value);
	}
	public bool Expanded
	{
		get => (bool)GetValue(ExpandedProperty);
		set => SetValue(ExpandedProperty, value);
	}
	public double ButtonWidth
	{
		get => (double)GetValue(ButtonWidthProperty);
		set => SetValue(ButtonWidthProperty, value);
	}

	public ExpandListedView()
	{
		InitializeComponent();
		BindingContext = this;
        Contents.CollectionChanged += CollectionChanged;
        recog.Tapped += OnTapped;
	}

    private void CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
		if (e.NewItems == null) return;
		foreach(View item in e.NewItems)
		{
			list.Children.Add(item);
		}
    }


    private static void ButtonWidthChanged(BindableObject bindable, object oldValue, object newValue)
    {
		var control = bindable as ExpandListedView;
		var width = (double)newValue;

		expandedPadding = new(width * .1, 0, width * .4, 0);
		closedPadding = new(width * .2, 0, width * .3, 0);

		control.ButtonPadding = control.Expanded ? expandedPadding : closedPadding;
		control.rect.CornerRadius = new(0, width * radius_rate, 0, width * radius_rate);
		control.OnPropertyChanged(nameof(ButtonPadding));
    }

    private static void ExpandedChanged(BindableObject bindable, object oldValue, object newValue)
    {
		var control = bindable as ExpandListedView;
		var expanded = (bool)newValue;

		if(expanded) control.Open();
		else control.Close();
    }

	private void Close(bool animate = false) 
	{
		// TranslateToにおける0とは、移動前の座標のこと。だから、画面の左端のことではない。
		var actualLength = animate ? length : 0;

		img.RotateTo(0, length: actualLength);
		list.TranslateTo(-(list.Width), 0, length: actualLength);
		button.TranslateTo(-(list.Width), 0, length: actualLength);
		ButtonPadding = closedPadding;
		OnPropertyChanged(nameof(ButtonPadding));
	} 
	private void Open(bool animate = false)
	{
        var actualLength = animate ? length : 0;

        img.RotateTo(rotate, length: actualLength);
        list.TranslateTo(0, 0, length: actualLength);
        button.TranslateTo(0, 0, length: actualLength);
        ButtonPadding = expandedPadding;
		OnPropertyChanged(nameof(ButtonPadding));
	}

    private void OnTapped(object sender, TappedEventArgs e)
    {
		if((Expanded = !Expanded))
		{
			// if expanded
			ExpandedCommand?.Execute(this);
			Open(true);
			return;
		}
		Close(true);
    }
}