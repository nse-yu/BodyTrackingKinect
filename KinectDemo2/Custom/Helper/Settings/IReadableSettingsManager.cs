using KinectDemo2.Custom.Service.Python.ML;

namespace KinectDemo2.Custom.Helper.Settings
{
    internal interface IReadableSettingsManager
    {
        abstract static bool Contains(string key);
        
        abstract static int Interval();
        abstract static int ReportIntervalFromSecond();
        abstract static string ReportInterval();
        abstract static double Restrictness();
        abstract static bool IsShowPopupToday();
        abstract static bool IsAutoSetting();
        abstract static DateTime PopupLastUpdate();
        abstract static MLModel Model();
        abstract static List<MLModel> ModelList();
        abstract static void SetRestrictness(double restrictness);
    }
}
