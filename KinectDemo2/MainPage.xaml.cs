using KinectDemo2.Custom.Page.Base;
using KinectDemo2.Custom.ViewModel;

namespace KinectDemo2;

public partial class MainPage : PageBase
{
    public MainPage(KinectSensorViewModel viewModel)
    {
        BindingContext = viewModel;
        InitializeComponent();
        SetPageTitle("Sensor View");
    }
}

