using System.Windows.Input;

namespace KinectDemo2.Custom.Control.Button.Base;

public partial class ProcessingButton : ClickableButton
{
    public static readonly BindableProperty IsProcessingProperty = BindableProperty.Create(
        nameof(IsProcessing), typeof(bool), typeof(ProcessingButton), defaultBindingMode: BindingMode.TwoWay, propertyChanged: IsProcessingChanged
        );
    public static readonly BindableProperty ProcessedCommandProperty = BindableProperty.Create(
        nameof(ProcessedCommand), typeof(ICommand), typeof(ProcessingButton), null
        );
    public static readonly BindableProperty PreProcessCommandProperty = BindableProperty.Create(
        nameof(PreProcessCommand), typeof(ICommand), typeof(ProcessingButton), null
        );
    public static readonly BindableProperty StoppedCommandProperty = BindableProperty.Create(
        nameof(StoppedCommand), typeof(ICommand), typeof(ProcessingButton), null
        );

    public bool IsProcessing
    {
        get => (bool)GetValue(IsProcessingProperty);
        set => SetValue(IsProcessingProperty, value);
    }
    public ICommand StoppedCommand
    {
        get => (ICommand)GetValue(StoppedCommandProperty);
        set => SetValue(StoppedCommandProperty, value);
    }
    public ICommand ProcessedCommand
    {
        get => (ICommand)GetValue(ProcessedCommandProperty);
        set => SetValue(ProcessedCommandProperty, value);
    }
    public ICommand PreProcessCommand
    {
        get => (ICommand)GetValue(PreProcessCommandProperty);
        set => SetValue(PreProcessCommandProperty, value);
    }

    public ProcessingButton()
	{
		InitializeComponent();
	}

    override protected void OnClicked(object sender, TappedEventArgs e)
    {
        IsProcessing = !IsProcessing;

        if(!IsProcessing)
        {
            StoppedCommand?.Execute(null);
            return;
        }

        PreProcessCommand?.Execute(null);
        ClickedCommand?.Execute(null);
        ProcessedCommand?.Execute(null);
    }
    private static void IsProcessingChanged(BindableObject bindable, object oldValue, object newValue)
    {
        var control = bindable as ProcessingButton;
        control.ProcessingDelegate();
    }
    protected virtual void ProcessingDelegate()
    {

    }
}