using KinectDemo2.Custom.Page.Base;
using KinectDemo2.Custom.ViewModel;

namespace KinectDemo2.Custom.Page;

public partial class SettingsPage : PageBase
{
    private readonly Action OnAppearingAction = null;

    public SettingsPage(SettingsViewModel viewModel)
    {
        OnAppearingAction = viewModel.OnAppearingDelegate;
        BindingContext = viewModel;
        InitializeComponent();
		SetPageTitle("Settings");
	}

    protected override void OnAppearing()
    {
        OnAppearingAction?.Invoke();
        base.OnAppearing();
    }
}