using KinectDemo2.Custom.Page.Base;
using KinectDemo2.Custom.ViewModel;

namespace KinectDemo2.Custom.Page;

public partial class MetaDataPage : PageBase
{
    private readonly Action OnAppearingAction = null;
	public MetaDataPage(MetaDataViewModel viewModel)
	{
        OnAppearingAction = viewModel.OnAppearingDelegate;
		BindingContext = viewModel;
        InitializeComponent();
		SetPageTitle("Information");
	}

    protected override void OnAppearing()
    {
        OnAppearingAction?.Invoke();
        base.OnAppearing();
    }
}