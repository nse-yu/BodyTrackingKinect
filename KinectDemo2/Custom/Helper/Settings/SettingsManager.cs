using KinectDemo2.Custom.Service.Python.ML;

namespace KinectDemo2.Custom.Helper.Settings
{
    public class SettingsManager : IReadableSettingsManager, IWritableSettingsManager
    {
        public const string SETTINGS_INTERVAL_KEY = "captures_interval";
        public const string SETTINGS_REPORT_INTERVAL_KEY = "report_interval";
        public const string SETTINGS_RESTRICTNESS_KEY = "restrictness_detection";
        public const string SETTINGS_MODEL_USE_KEY = "model_in_use";
        public const string SETTINGS_SHOW_POPUP_TODAY = "show_popup_today";
        public const string SETTINGS_SHOW_POPUP_DATE = "show_popup_date";
        public const string SETTINGS_AUTO_SETTING_MODE = "auto_setting_mode";
        public const string SETTINGS_DEFAULT_SEND_SELECTION = "default_send_selection";

        public static int RestrictnessMax { get; } = 10;
        public static int RestrictnessMin { get; } = 1;
        public static List<int> Intervals { get; private set; } = new()
        {
            0,
            3,
            5,
            7,
            10,
            20,
            30,
            50
        };
        public static List<string> ScoreReportIntervals { get; private set; } = new()
        {
            "always",
            "30s",
            "1m",
            "2m",
            "3m",
            "5m",
            "7m",
            "10m"
        };
        private static Array Models { get; } = Enum.GetValues(typeof(MLModel));

        public static int Interval() => Intervals[PickNoException<int>(SETTINGS_INTERVAL_KEY)];
        public static int ReportIntervalFromSecond() => ReportInterval() switch
        {
            "always" => 0,
            "30s" => 30,
            "1m" => 60,
            "2m" => 120,
            "3m" => 180,
            "5m" => 300,
            "7m" => 420,
            "10m" => 600,
            _ => throw new NotSupportedException()
        };
        public static string ReportInterval() => ScoreReportIntervals[PickNoException<int>(SETTINGS_REPORT_INTERVAL_KEY)];
        public static double Restrictness() => PickNoException<double>(SETTINGS_RESTRICTNESS_KEY);
        public static bool IsShowPopupToday()
        {
            if (PickNoException<bool>(SETTINGS_SHOW_POPUP_TODAY)) return true;

            return DateTime.Now.Date >= PopupLastUpdate().Date.AddDays(1);
        }
        public static bool IsAutoSetting() => PickNoException<bool>(SETTINGS_AUTO_SETTING_MODE);
        public static bool IsSendDefault() => PickNoException<bool>(SETTINGS_DEFAULT_SEND_SELECTION);
        public static DateTime PopupLastUpdate() => PickNoException<DateTime>(SETTINGS_SHOW_POPUP_DATE);
        public static MLModel Model() => (MLModel)Models.GetValue(PickNoException<int>(SETTINGS_MODEL_USE_KEY));
        public static List<MLModel> ModelList() => Enumerable.Range(0, Models.Length).Select(i => (MLModel)Models.GetValue(i)).ToList();

        public static void SetRestrictness(double restrictness) 
        {
            if(restrictness > RestrictnessMax) restrictness = RestrictnessMax;
            if(restrictness < RestrictnessMin) restrictness = RestrictnessMin;

            Save<double>((SETTINGS_RESTRICTNESS_KEY, restrictness));
        }

        public static T Pick<T>(string key)
        {
            if(!Contains(key)) throw new KeyNotFoundException($"Specified key {key} is not found in the preference...");

            return (T)Preferences.Default.Get(key, default(T));
        }
        public static T PickNoException<T>(string key)
        {
            if (!Contains(key)) return default;
            return (T)Preferences.Default.Get(key, default(T));
        }
        public static void InitIfNotExist<T>(params (string key, object value)[] keyValue)
        {
            foreach(var (key, value) in keyValue)
            {
                if (!Contains(key)) Preferences.Default.Set(key, (T)value);
            }
        }
        public static void Save<T>(params (string key, object value)[] keyValue)
        {
            foreach (var (key, value) in keyValue)
            {
                if(Contains(key)) Preferences.Default.Remove(key);
                Preferences.Default.Set(key, (T)value);
            }
        }
        public static bool Contains(string key) => Preferences.Default.ContainsKey(key);
    }
}
