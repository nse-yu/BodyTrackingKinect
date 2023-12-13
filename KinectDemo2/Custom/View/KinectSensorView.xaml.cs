using KinectDemo2.Custom.ViewModel;
using System.Windows.Input;

namespace KinectDemo2.Custom.Page;

public partial class KinectSensorView : ContentView
{
    public static readonly BindableProperty LoadedCommandProperty = BindableProperty.Create(
        nameof(LoadedCommand), typeof(ICommand), typeof(KinectSensorView), null, propertyChanged: OnLoadedPropertyChanged
        );
    
    public static readonly BindableProperty NotFoundCommandProperty = BindableProperty.Create(
        nameof(NotFoundCommand), typeof(ICommand), typeof(KinectSensorView), null, propertyChanged: OnNotFoundPropertyChanged
        );

    public ICommand LoadedCommand
    {
        get => (ICommand)GetValue(LoadedCommandProperty);
        set => SetValue(LoadedCommandProperty, value);
    }
    public ICommand NotFoundCommand 
    {
        get => (ICommand)GetValue(NotFoundCommandProperty);
        set => SetValue(NotFoundCommandProperty, value);
    }

    internal KinectSensorView(KinectSensorViewModel viewModel)
	{
        BindingContext = viewModel;
		InitializeComponent();
    }

    private static void OnLoadedPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        throw new NotImplementedException();
    }
    private static void OnNotFoundPropertyChanged(BindableObject bindable, object oldValue, object newValue)
    {
        throw new NotImplementedException();
    }

}