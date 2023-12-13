using KinectDemo2.Custom.ViewModel;

namespace KinectDemo2.Custom.Page.Base;

public partial class PageBase : ContentPage
{
	private readonly Action cameraStartAction = null;
	private readonly Action cameraStopAction = null;
	public PageBase()
	{
		InitializeComponent();
		var vm = new PageBaseViewModel();
		cameraStartAction = vm.ContextActivateCameraDelegate;
		cameraStopAction = vm.ContextStopCameraDelegate;
        menu_activate_camera.Clicked += ActivateCameraClicked;
        menu_stop_camera.Clicked += StopCameraClicked;
		BindingContext = vm;
	}

    private void StopCameraClicked(object sender, EventArgs e)
    {
		cameraStopAction?.Invoke();
    }

    private void ActivateCameraClicked(object sender, EventArgs e)
    {
		cameraStartAction?.Invoke();
    }

    protected void SetPageTitle(string title = "")
	{
		this.Resources["PageTitle"] = title;
	}
}