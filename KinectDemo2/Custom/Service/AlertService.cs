namespace KinectDemo2.Custom.Service
{
    public interface IAlertService
    {
        Task DisplayAlert(string title, string message, string cancel = "OK");
    }

    public class AlertService : IAlertService
    {
        public const string ALERT_TITLE_ERROR = "Error";
        public const string ALERT_TITLE_RESULT = "Result";
        public const string ALERT_TITLE_WAIT = "Wait!!";
        public Task DisplayAlert(string title, string message, string cancel = "OK")
        {
            return Application.Current.MainPage.DisplayAlert(title, message, cancel);
        }
    }
}
