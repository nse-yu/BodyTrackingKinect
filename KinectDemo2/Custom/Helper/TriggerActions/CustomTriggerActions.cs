namespace KinectDemo2.Custom.Helper.TriggerActions
{
    public class RotateTriggerActions : TriggerAction<VisualElement>
    {
        private readonly static string ANIMATION_NAME = "RotationAnimation";
        public bool Canceled { get; set; }
        protected override void Invoke(VisualElement sender)
        {
            if(Canceled)
            {
                sender.AbortAnimation(ANIMATION_NAME);
                sender.RotateTo(0, 0);

                return;
            }
            sender.Animate
            (
                name: ANIMATION_NAME, 
                callback: (double value) =>
                {
                    sender.Rotation = value;
                },
                start: 0, 
                end: 360, 
                length: 16 * 250,
                easing: Easing.Linear,
                repeat: () => true
            );
        }
    }
}
