using KinectDemo2.Custom.Control.Button.Base;

namespace KinectDemo2.Custom.Control.Button;

public partial class PlayButton : ProcessingButton
{
    public PlayButton()
	{
		InitializeComponent();
	}

    override protected void OnClicked(object sender, TappedEventArgs e)
    {
        IsProcessing = !IsProcessing;

        if (!IsProcessing)
        {
            StoppedCommand?.Execute(null);
            return;
        }

        PreProcessCommand?.Execute(null);
        ClickedCommand?.Execute(null);
        ProcessedCommand?.Execute(null);
    }

    protected override void ProcessingDelegate()
    {
        stop.IsVisible = IsProcessing;
    }
}