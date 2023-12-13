using CommunityToolkit.WinUI.Notifications;

namespace KinectDemo2.Custom.Helper.Utils
{
    internal class ThreadSafeNotifier
    {
        public static void Notify(string title = "", string comment = "")
        {
            MainThread.InvokeOnMainThreadAsync(() =>
            {
                App.AlertService.DisplayAlert(title: title, message: comment);
            });
        }

        public static async void NotifyOnSystem(string text = "")
        {
            await MainThread.InvokeOnMainThreadAsync(() =>
            {
                new ToastContentBuilder()
                    .AddText(text)
                    .Show();
            });
        }
    }
}
