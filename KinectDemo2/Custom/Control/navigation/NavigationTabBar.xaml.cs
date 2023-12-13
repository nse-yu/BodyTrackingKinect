namespace KinectDemo2.Custom.Control.navigation;

public partial class NavigationTabBar : ContentView
{
	public static readonly BindableProperty RoutesProeprty = 
		BindableProperty.Create(nameof(Routes), typeof(RouteCollection), typeof(NavigationTabBar));

	public static readonly BindableProperty TabTemplateProperty = 
		BindableProperty.Create(nameof(TabTemplate), typeof(DataTemplate), typeof(NavigationTabBar));
	
	public RouteCollection Routes { get; } = new RouteCollection();

	public DataTemplate TabTemplate
	{
		get => (DataTemplate)GetValue(TabTemplateProperty);
		set => SetValue(TabTemplateProperty, value);
	}

    public NavigationTabBar()
	{
		InitializeComponent();
        Routes.CollectionChanged += RoutesChanged;
	}
	~NavigationTabBar()
	{
		Routes.CollectionChanged -= RoutesChanged;
	}

    private void RoutesChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
		var tabs = e.NewItems.Cast<NavigationTab>();

		tabs.ToList().ForEach(tab =>
		{
			navLeft.Children.Add(CreateControl(tab));
		});
    }

	private VisualElement CreateControl(NavigationTab tab)
	{
		var wrapper = new VerticalStackLayout();	
		var icon = new ImageButton()
		{
			Source = tab.Icon,
			BackgroundColor = tab.IconBackgroundColor,
		};
		icon.Clicked += ItemClicked;
		
		

		wrapper.Children.Add(icon);
		return wrapper;
	}

	private void ItemClicked(object sender, EventArgs e)
	{
		
	}
}