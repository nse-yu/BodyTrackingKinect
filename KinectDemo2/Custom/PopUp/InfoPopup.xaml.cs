using CommunityToolkit.Maui.Views;

namespace KinectDemo2.Custom.PopUp;

public partial class InfoPopup : Popup
{
	public readonly static Color PopIndicatorColor = Colors.LightGreen;
	public readonly static double PopHeight = 500;
	public readonly static double PopWidth = 500;
	public readonly static double BubbleHeight = PopHeight * .7;
	public readonly static double BubbleWidth = PopWidth * .7;
	public readonly static float PopRadius = (float)Math.Min(PopHeight, PopWidth) / 2;
	public readonly static double BoxHeight = BubbleHeight;
	public readonly static double InitialBoxTranslationY = BoxHeight;

	private readonly Animation boxExtension;
	private readonly Animation boxGradation;
	private readonly Animation scoreVisualization;
	private readonly Animation titleVisualization;

	private const int length = 3000;
	private const int rate = 16;
	private const int colorStep = 10;
	private const string DEFAULT_MAIN_TITLE = "ð“ú‚Ì•½‹ÏƒXƒRƒA";

	private static readonly Color boxColorTo = Colors.ForestGreen;
	private static readonly double labelOpacityTo = 1.0;
	private static double boxTranslateYto;

    private static readonly double stepR = (boxColorTo.Red - PopIndicatorColor.Red) / colorStep;
    private static readonly double stepG = (boxColorTo.Green - PopIndicatorColor.Green) / colorStep;
    private static readonly double stepB = (boxColorTo.Blue - PopIndicatorColor.Blue) / colorStep;

	private string _title;
	public string Title
	{
		get => _title;
		set
		{
			if (_title == value) return;
			_title = value;
			OnPropertyChanged(nameof(Title));
		}
	}

	private string _message;
	public string Message
	{
		get => _message;
		set
		{
			_message = value;
			OnPropertyChanged(nameof(Message));
		}
	}

	private bool _isChecked;
	public bool IsChecked
	{
		get => _isChecked;
		set
		{
			if(_isChecked == value) return;
			_isChecked = value;
			ResultWhenUserTapsOutsideOfPopup = _isChecked;
			OnPropertyChanged(nameof(IsChecked));
		}
	}

	private bool _showCheckBox = true;
	public bool ShowCheckBox
	{
		get => _showCheckBox;
		set
		{
			if(_showCheckBox == value) return;
			_showCheckBox = value;
			OnPropertyChanged(nameof(ShowCheckBox));
		}
	}

    public InfoPopup(string message, double ratio, bool showCheckBox = true, string mainTitle = DEFAULT_MAIN_TITLE)
	{
		InitializeComponent();
		BindingContext = this;

		Message = message;
		Title = mainTitle;
		ShowCheckBox = showCheckBox;
        ResultWhenUserTapsOutsideOfPopup = _isChecked;
		boxTranslateYto = ratio < 1 ? (1 - ratio) * InitialBoxTranslationY : 0;
        Size = new Size(PopWidth, PopHeight);
		boxExtension = new Animation(v => boxView.TranslationY = v, InitialBoxTranslationY, boxTranslateYto);
		scoreVisualization = new Animation(v => score.Opacity = v, 0, labelOpacityTo);
		titleVisualization = new Animation(v => title.Opacity = v, 0, labelOpacityTo);
		boxGradation = new Animation(v => boxView.Color = Color.FromRgb(PopIndicatorColor.Red + stepR * v, PopIndicatorColor.Green + stepG * v, PopIndicatorColor.Blue + stepB * v), 1, colorStep);

        AnimateBoxViewAsync();
	}

    private void AnimateBoxViewAsync()
    {
        var popAnimation = new Animation
		{
			{ 0, .4, titleVisualization },
			{ 0, .6, boxExtension },
			{ 0, .6, boxGradation },
			{ .6, 1, scoreVisualization }
        };
        popAnimation.Commit(
            Application.Current.MainPage,
			"PopAnimation",
			rate,
			length,
			null,
			(v, c) => { boxView.TranslationY = boxTranslateYto; score.Opacity = labelOpacityTo; boxView.Color = boxColorTo; title.Opacity = labelOpacityTo; }
			);
    }
}