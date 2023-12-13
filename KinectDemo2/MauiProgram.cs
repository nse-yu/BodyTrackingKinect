using CommunityToolkit.Maui;
using KinectDemo2.Custom.Page;
using KinectDemo2.Custom.Service;
using KinectDemo2.Custom.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SkiaSharp.Views.Maui.Controls.Hosting;
using Syncfusion.Maui.Core.Hosting;

namespace KinectDemo2;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .ConfigureSyncfusionCore()
            .RegisterServices()
            .RegisterViewModels()
            .RegisterPages()
            .RegisterSyncfusion()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddFont("ZenMaruGothic-Medium.ttf", "ZenMaruGothicMedium");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
    public static MauiAppBuilder RegisterSyncfusion(this MauiAppBuilder mauiAppBuilder)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath("C:\\Users\\boban\\Documents\\programming\\maui\\KinectDemo2\\KinectDemo2")
            //.SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("license.json")
            .Build();
        var licenseKey = config["LicenseKey"];
        Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense(licenseKey);

        return mauiAppBuilder;
    }
    public static MauiAppBuilder RegisterServices(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<IKinectService, KinectService>();
        mauiAppBuilder.Services.AddSingleton<IAlertService, AlertService>();
        mauiAppBuilder.Services.AddSingleton<IPythonService, PythonService>();
        mauiAppBuilder.Services.AddTransient<IDBService, DBService>();

        return mauiAppBuilder;
    }
    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddTransient<MetaDataViewModel>();
        mauiAppBuilder.Services.AddTransient<KinectSensorViewModel>();
        mauiAppBuilder.Services.AddTransient<KinectBodyTrackingViewModel>();
        mauiAppBuilder.Services.AddTransient<DataViewerViewModel>();
        mauiAppBuilder.Services.AddTransient<SettingsViewModel>();

        return mauiAppBuilder;
    }
    public static MauiAppBuilder RegisterPages(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<MainPage>();
        mauiAppBuilder.Services.AddSingleton<MetaDataPage>();
        mauiAppBuilder.Services.AddSingleton<BodyTrackingPage>();
        mauiAppBuilder.Services.AddSingleton<DataViewerPage>();
        mauiAppBuilder.Services.AddSingleton<SettingsPage>();

        return mauiAppBuilder;
    }
}