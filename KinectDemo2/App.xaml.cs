using CommunityToolkit.Maui.Views;
using KinectDemo2.Custom.Helper.Settings;
using KinectDemo2.Custom.Model.tracking;
using KinectDemo2.Custom.PopUp;
using KinectDemo2.Custom.Service;

namespace KinectDemo2;

public partial class App : Application
{
    public static IServiceProvider Services { get; set; }
    public static IAlertService AlertService { get; set; }
    public static IKinectService KinectService { get; set; }
    public static IPythonService PythonService { get; set; }
    public static IDBService DBService { get; set; }

    public App(IServiceProvider provider)
    {
        InitializeComponent();

        Services = provider;
        AlertService = Services.GetService<IAlertService>();
        PythonService = Services.GetService<IPythonService>();
        KinectService = Services.GetService<IKinectService>();
        DBService = Services.GetService<IDBService>();

        MainPage = new AppShell();
        MainPage.Loaded += OnPageLoaded;
    }

    private void OnPageLoaded(object sender, EventArgs e)
    {
        if (SettingsManager.Contains(SettingsManager.SETTINGS_SHOW_POPUP_TODAY) &&
            SettingsManager.Contains(SettingsManager.SETTINGS_SHOW_POPUP_DATE) &&
            !SettingsManager.IsShowPopupToday()
            ) return;

        if(!SettingsManager.Contains(SettingsManager.SETTINGS_SHOW_POPUP_TODAY))
            SettingsManager.Save<bool>((SettingsManager.SETTINGS_SHOW_POPUP_TODAY, true));

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            List<BodyTrackingScore> scores = null;
            try
            {
                scores = await DBService.GetScoresAsync(DateTime.Now.AddDays(-1).Date, DateTime.Now.Date.AddTicks(-1));
            }catch (Exception) 
            {
                return;
            }
            var average = scores.Count > 0 ? (from record in scores select record.Score).Average() : 0;
            var result = await Current.MainPage.ShowPopupAsync(new InfoPopup(average.ToString("F1"), average / 100));
            if (result is bool bResult && bResult)
            {
                SettingsManager.Save<DateTime>((SettingsManager.SETTINGS_SHOW_POPUP_DATE, DateTime.Now));
                SettingsManager.Save<bool>((SettingsManager.SETTINGS_SHOW_POPUP_TODAY, false));
            }

        });
    }

    protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        window.Destroying += OnWindowDestroying;
        return window;
    }

    private void OnWindowDestroying(object sender, EventArgs e)
    {
        KinectService.DisposeAll();
    }
}
