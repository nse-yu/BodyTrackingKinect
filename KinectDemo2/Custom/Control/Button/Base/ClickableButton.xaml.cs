using System.Windows.Input;

namespace KinectDemo2.Custom.Control.Button.Base;

public partial class ClickableButton : ContentView
{
    public static readonly BindableProperty ClickedCommandProperty = BindableProperty.Create(
        nameof(ClickedCommand), typeof(ICommand), typeof(ProcessingButton), null
        );

    public ICommand ClickedCommand
    {
        get => (ICommand)GetValue(ClickedCommandProperty);
        set => SetValue(ClickedCommandProperty, value);
    }
    public ClickableButton()
    {
        InitializeComponent();
        TappedRecog.Tapped += OnClicked;
    }

    virtual protected void OnClicked(object sender, TappedEventArgs e)
    {
        ClickedCommand?.Execute(null);
    }
}