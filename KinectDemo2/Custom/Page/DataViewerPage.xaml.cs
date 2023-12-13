using KinectDemo2.Custom.Page.Base;
using KinectDemo2.Custom.ViewModel;

namespace KinectDemo2.Custom.Page;

public partial class DataViewerPage : PageBase
{
	public DataViewerPage(DataViewerViewModel viewModel)
	{
		BindingContext = viewModel;
		InitializeComponent();
		SetPageTitle("Score Viewer");
	}
}