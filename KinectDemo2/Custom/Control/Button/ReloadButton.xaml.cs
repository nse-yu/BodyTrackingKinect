using System.Windows.Input;

namespace KinectDemo2.Custom.Control.Button;

public partial class ReloadButton : ContentView
{
	public static readonly BindableProperty ReloadingCommandProperty = BindableProperty.Create(
		nameof(ReloadingCommand), typeof(ICommand), typeof(ReloadButton), null
		);
	public static readonly BindableProperty ReloadedCommandProperty = BindableProperty.Create(
		nameof(ReloadedCommand), typeof(ICommand), typeof(ReloadButton), null
		);

	public ICommand ReloadingCommand
	{
		get => (ICommand)GetValue( ReloadingCommandProperty );
		set => SetValue( ReloadingCommandProperty, value );
	}
	public ICommand ReloadedCommand
	{
		get => (ICommand)GetValue( ReloadedCommandProperty );
		set => SetValue( ReloadedCommandProperty, value );
	}

	public ReloadButton()
	{
		InitializeComponent();
        ReloadOnTappedRecog.Tapped += OnReloadTapped;
	}

    private async void OnReloadTapped(object sender, TappedEventArgs e)
    {
		indicator.IsRunning = true;
        ReloadingCommand?.Execute(null);

		await Task.Delay(3000);

		ReloadedCommand?.Execute(null);
		indicator.IsRunning = false;
    }
}