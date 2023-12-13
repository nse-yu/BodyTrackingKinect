using SkiaSharp;
using SkiaSharp.Views.Maui;
using SkiaSharp.Views.Maui.Controls;

namespace KinectDemo2.Custom.Control.Switch.Base;

public partial class SwitchBase : ContentView
{
	public static double MinHeight { get; set; } = 40;
	public static double MinWidth { get; set; } = 80;

    public static readonly BindableProperty OnTextColorProperty = 
		BindableProperty.Create(nameof(OnTextColor), typeof(Color), typeof(SwitchBase), defaultValue: Colors.White, propertyChanged: OnTextColorChanged);

    public static readonly BindableProperty OffTextColorProperty = 
		BindableProperty.Create(nameof(OffTextColor), typeof(Color), typeof(SwitchBase), defaultValue: Colors.Red, propertyChanged: OffTextColorChanged);

    public static readonly BindableProperty ThumbColorProperty = 
		BindableProperty.Create(nameof(ThumbColor), typeof(Color), typeof(SwitchBase), propertyChanged: PropsChanged);

    public static readonly BindableProperty OnColorProperty = 
		BindableProperty.Create(nameof(OnColor), typeof(Color), typeof(SwitchBase), propertyChanged: PropsChanged);
	
	public static readonly BindableProperty OffColorProperty = 
		BindableProperty.Create(nameof(OffColor), typeof(Color), typeof(SwitchBase), propertyChanged: PropsChanged);

	public static readonly BindableProperty OnTextProperty = 
		BindableProperty.Create(nameof(OnText), typeof(string), typeof(SwitchBase), defaultValue: "ON", propertyChanged: OnTextChanged);

    public static readonly BindableProperty OffTextProperty = 
		BindableProperty.Create(nameof(OffText), typeof(string), typeof(SwitchBase), defaultValue: "OFF", propertyChanged: OffTextChanged);

	public static readonly BindableProperty TextSizeProperty =
		BindableProperty.Create(nameof(TextSize), typeof(double), typeof(SwitchBase), defaultValue: 20.0, propertyChanged: TextSizeChanged);

    public static readonly BindableProperty TextVisibilityProperty = 
		BindableProperty.Create(nameof(TextVisibility), typeof(bool), typeof(SwitchBase), defaultValue: true, propertyChanged: TextVisibleToggle);

    public static readonly BindableProperty IsToggledProperty =
        BindableProperty.Create(nameof(IsToggled), typeof(bool), typeof(SwitchBase), defaultValue: false, propertyChanged: IsToggleChanged, defaultBindingMode: BindingMode.TwoWay);


	public bool IsToggled
	{
		get => (bool)GetValue(IsToggledProperty);
		set => SetValue(IsToggledProperty, value);
	}
	public Color ThumbColor
	{
		get => (Color)GetValue(ThumbColorProperty);
		set => SetValue(ThumbColorProperty, value);
	}
	public Color OnColor
	{
		get => (Color)GetValue(OnColorProperty);
		set => SetValue(OnColorProperty, value);
	}
	public Color OffColor
	{
		get => (Color)GetValue(OffColorProperty);
		set => SetValue(OffColorProperty, value);
	}

	public Color OnTextColor
	{
        get => (Color)GetValue(OnTextColorProperty);
		set => SetValue(OnTextColorProperty, value);
	}
	public Color OffTextColor
	{
        get => (Color)GetValue(OffTextColorProperty);
		set => SetValue(OffTextColorProperty, value);
	}
	public string OnText
	{
		get => (string)GetValue(OnTextProperty);
		set => SetValue(OnTextProperty, value);
	}
	public string OffText
	{
		get => (string)GetValue(OffTextProperty);
		set => SetValue(OffTextProperty, value);
	}

	public double TextSize
	{
		get => (double)GetValue(TextSizeProperty);
		set => SetValue(TextSizeProperty, value);
	}
	public bool TextVisibility
	{
		get => (bool)GetValue(TextVisibilityProperty);
		set => SetValue(TextVisibilityProperty, value);
	}

	private void AttachToggleToLabel(bool state)
	{
		label.Text = state ? OnText : OffText;
		label.TextColor = state ? OnTextColor : OffTextColor;
	}

	private void AttachPropsToDrawable()
	{
		graphics.FillColor = IsToggled ? OnColor : OffColor;
		graphics.ThumbColor = ThumbColor;
        graphics.InvalidateSurface();
    }

	public SwitchBase()
	{
		InitializeComponent();
        TappedRecog.Tapped += OnTapped;
		//AttachToggleToLabel(IsToggled);
	}
    private static void OnTextColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as SwitchBase;
        if (!control.IsToggled) return;
        control.label.TextColor = (Color)newValue;
    }
    private static void OffTextColorChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as SwitchBase;
        if (control.IsToggled) return;
        control.label.TextColor = (Color)newValue;
    }
    private static void OnTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as SwitchBase;
        if (!control.IsToggled) return;
        control.label.Text = (string)newValue;
    }
    private static void OffTextChanged(BindableObject bindable, object oldValue, object newValue)
    {
		var control = bindable as SwitchBase;
		if (control.IsToggled) return;
		control.label.Text = (string)newValue;
    }
    private static void TextSizeChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as SwitchBase;
        control.label.FontSize = (double)newValue;
    }
    private static void PropsChanged(BindableObject bindable, object oldValue, object newValue)
    {
		var control = bindable as SwitchBase;
		control.AttachPropsToDrawable();
    }
    private static void TextVisibleToggle(BindableObject bindable, object oldValue, object newValue)
    {
		var control = bindable as SwitchBase;
		control.label.IsVisible = (bool)newValue;
    }

    private void OnTapped(object sender, TappedEventArgs e)
    {
		IsToggled = !IsToggled;
		graphics.afterToggled = true;
    }

    private static void IsToggleChanged(BindableObject bindable, object oldValue, object newValue)
    {
		var control = bindable as SwitchBase;
		var state = (bool)newValue;
		
		// label
		control.AttachToggleToLabel(state);

		// drawing
		control.graphics.FillColor = state ? control.OnColor : control.OffColor;
		control.graphics.State = state;
		control.graphics.InvalidateSurface();
    }
}

public class SwitchCanvasView : SKCanvasView
{
	private float step;
	private float p = 0f;
	public bool running;
	private bool _state;
	private bool previousState;
	public bool afterToggled;
	private readonly int animationFrame = 10;
	private readonly IDispatcherTimer timer;

    public Color ThumbColor { get; set; }
    public Color FillColor { get; set; }
    public bool State 
	{
		get => _state;
		set
		{
			previousState = _state;
			_state = value;
		}
	}

	public SwitchCanvasView()
	{
        timer = Application.Current.Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromMilliseconds(16);
        timer.Tick += Tick;
    }

    protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
    {
		if (ThumbColor == null || FillColor == null) return;

        //base.OnPaintSurface(e);
		var canvas = e.Surface.Canvas;
		var rect = e.Info.Rect;

		canvas.Clear();

        float height = rect.Height * 3 / 5;
        float width = rect.Width;
		
        float widthArc = width * 1 / 4;
        float heightArc = height;

        float widthRect = width - widthArc * 2;
        float heightRect = height;

        float xCircle = widthArc / 2;
        float yCircle = heightArc / 2;
        float paddingCircle = Math.Abs(xCircle - yCircle);

		float y = rect.Height / 2 - heightArc / 2;

        var paintRect = new SKPaint()
		{
			Color = FillColor.ToSKColor(),
			IsAntialias = true,
			Style = SKPaintStyle.Fill,
		};
		canvas.DrawArc(
			SKRect.Create(0, y, widthArc, heightArc),
			90,
			180,
			true,
			paintRect
			);
		canvas.DrawRect(widthArc / 2, y, widthRect, heightRect, paintRect);
        canvas.DrawArc(
            SKRect.Create(widthRect, y, widthArc, heightArc),
            90,
            -180,
            true,
            paintRect
            );

		var paintCircle = new SKPaint()
        {
            Color = ThumbColor.ToSKColor(),
            IsAntialias = true,
            Style = SKPaintStyle.Fill,
        };

		running = (previousState != State) && afterToggled;

		if (!running) p = widthRect - paddingCircle * 2;

        canvas.DrawCircle(
            State ? xCircle + paddingCircle + p : xCircle + widthRect - paddingCircle - p,
            yCircle + y,
            Math.Min(xCircle, yCircle),
            paintCircle
            );

		if (!timer.IsRunning && running)
		{
			// ˆÚ“®• / ƒtƒŒ[ƒ€”
			p = 0f;
			step = (widthRect - paddingCircle * 2) / animationFrame;
			timer?.Start();
		}

		if (p >= step * animationFrame) 
		{
			p = 0f;
			running = false;
			afterToggled = false;
			timer?.Stop();
		}
    }

    private void Tick(object sender, EventArgs e)
    {
		p += step;
		InvalidateSurface();
    }
}