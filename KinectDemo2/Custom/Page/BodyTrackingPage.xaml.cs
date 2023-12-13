using KinectDemo2.Custom.Page.Base;
using KinectDemo2.Custom.ViewModel;

namespace KinectDemo2.Custom.Page;

public partial class BodyTrackingPage : PageBase
{
	private static readonly uint duration = 800;
	public static double NotificationBarWidth = 400;
	public static double InitialTranslationX = -NotificationBarWidth;

	public BodyTrackingPage(KinectBodyTrackingViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
		SetPageTitle("posture detection");
	}

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
		if (BindingContext is KinectBodyTrackingViewModel viewModel)
		{
			viewModel.AnimatedCallback(AnimateAsync);
		}
    }

    /* 
	public BodyTrackingPage(KinectBodyTrackingViewModel viewModel)
	{
		viewModel.AnimatedCallback(AnimateAsync);
		BindingContext = viewModel;
		InitializeComponent();
	}
	 */

    private async void AnimateAsync(object value)
	{
		if (value is string message) notifyBarText.Text = message;
		if (value is object obj) notifyBarText.Text = obj.ToString();

		await notifyBar.TranslateTo(0, 0, duration / 2);
		await Task.Delay((int)duration * 5);
		await notifyBar.TranslateTo(InitialTranslationX, 0, duration / 3);
	}
}