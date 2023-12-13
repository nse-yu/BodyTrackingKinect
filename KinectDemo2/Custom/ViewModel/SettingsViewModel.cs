using KinectDemo2.Custom.Helper.Settings;
using System.Text;
using System.Windows.Input;

namespace KinectDemo2.Custom.ViewModel
{
    public class SettingsViewModel : BaseViewModel
    {
        public static string DESC_MODEL_THUNDER { get; } = 
            "Thunder is intended for applications that require high accuracy.\r\n" +
            "Thunder is a higher capacity model (compared to MoveNet.SinglePose.Lightning) that performs better prediction quality\r\n" +
            "while still achieving real-time (>30FPS) speed. Naturally, thunder will lag behind the lightning, but it will pack a punch.";
        public static string DESC_MODEL_LIGHTNING { get; } =
            "Lightning is intended for latency-critical applications.\r\n" +
            "Lightning is a lower capacity model (compared to MoveNet.SinglePose.Thunder) that can run >50FPS\r\n" +
            "on most modern laptops while achieving good performance.";
        public static string DESC_MODEL_MEDIAPIPE { get; } =
            "The MediaPipe Pose Landmarker task lets you detect landmarks of human bodies in an image or video.\r\n " +
            "You can use this task to identify key body locations, analyze posture, and categorize movements.\r\n" +
            "This task uses machine learning (ML) models that work with single images or video.\r\n" +
            "The task outputs body pose landmarks in image coordinates and in 3-dimensional world coordinates.";

        private int _reportIntervalSelectedIndex = SettingsManager.PickNoException<int>(SettingsManager.SETTINGS_REPORT_INTERVAL_KEY);
        private int _intervalSelectedIndex = SettingsManager.PickNoException<int>(SettingsManager.SETTINGS_INTERVAL_KEY);
        private int _modelUseSelectedIndex = SettingsManager.PickNoException<int>(SettingsManager.SETTINGS_MODEL_USE_KEY);
        private double _sliderValue = SettingsManager.Restrictness();
        private bool _autoSettingMode = SettingsManager.IsAutoSetting();
        private bool _defaultSendSelection = SettingsManager.IsSendDefault();

        public static double SLIDER_MIN { get; } = SettingsManager.RestrictnessMin;
        public static double SLIDER_MAX { get; } = SettingsManager.RestrictnessMax;
        public static double SLIDER_AVE { get; } = Average(SLIDER_MIN, SLIDER_MAX);
        public static List<int> Intervals { get; private set; } = SettingsManager.Intervals;
        public static List<string> ReportIntervals { get; private set; } = SettingsManager.ScoreReportIntervals;

        public int ReportIntervalSelectedIndex
        {
            get => _reportIntervalSelectedIndex;
            set
            {
                if(value == _reportIntervalSelectedIndex) return;
                _reportIntervalSelectedIndex = value;
                OnPropertyChanged(nameof(ReportIntervalSelectedIndex));
            }
        }
        public int IntervalSelectedIndex
        {
            get => _intervalSelectedIndex;
            set
            {
                if(value == _intervalSelectedIndex) return;
                _intervalSelectedIndex = value;
                OnPropertyChanged(nameof(IntervalSelectedIndex));
            }
        }
        public int ModelUseSelectedIndex
        {
            get => _modelUseSelectedIndex;
            set
            {
                if(value == _modelUseSelectedIndex) return;
                _modelUseSelectedIndex = value;
                OnPropertyChanged(nameof(ModelUseSelectedIndex));
            }
        }
        public double SliderValue
        {
            get => _sliderValue;
            set
            {
                if(value == SliderValue) return;
                _sliderValue = value;
                OnPropertyChanged(nameof(SliderValue));
            }
        }
        public bool AutoSettingMode
        {
            get => _autoSettingMode;
            set
            {
                if (value == _autoSettingMode) return;
                _autoSettingMode = value;
                OnPropertyChanged(nameof(AutoSettingMode));
            }
        }
        public bool DefaultSendSelection
        {
            get => _defaultSendSelection;
            set
            {
                if(value == _defaultSendSelection) return;
                _defaultSendSelection = value;
                OnPropertyChanged(nameof(DefaultSendSelection));
            }
        }
        public ICommand SavedCommand { get; set; }
        public Action OnAppearingDelegate { get; private set; }

        public SettingsViewModel() 
        {
            SettingsManager.InitIfNotExist<int>(
                (SettingsManager.SETTINGS_INTERVAL_KEY, IntervalSelectedIndex),
                (SettingsManager.SETTINGS_REPORT_INTERVAL_KEY, ReportIntervalSelectedIndex),
                (SettingsManager.SETTINGS_MODEL_USE_KEY, ModelUseSelectedIndex)
                );
            SettingsManager.InitIfNotExist<double>(
                (SettingsManager.SETTINGS_RESTRICTNESS_KEY, SLIDER_AVE)
                );
            SettingsManager.InitIfNotExist<bool>(
                (SettingsManager.SETTINGS_AUTO_SETTING_MODE, AutoSettingMode),
                (SettingsManager.SETTINGS_DEFAULT_SEND_SELECTION, DefaultSendSelection)
                );

            SavedCommand = new Command(() =>
            {
                SettingsManager.Save<int>(
                    (SettingsManager.SETTINGS_INTERVAL_KEY, IntervalSelectedIndex),
                    (SettingsManager.SETTINGS_REPORT_INTERVAL_KEY, ReportIntervalSelectedIndex),
                    (SettingsManager.SETTINGS_MODEL_USE_KEY, ModelUseSelectedIndex)
                    );
                SettingsManager.Save<double>(
                    (SettingsManager.SETTINGS_RESTRICTNESS_KEY, SliderValue)
                    );
                SettingsManager.Save<bool>(
                    (SettingsManager.SETTINGS_AUTO_SETTING_MODE, AutoSettingMode),
                    (SettingsManager.SETTINGS_DEFAULT_SEND_SELECTION, DefaultSendSelection)
                    );
              
                var builder = new StringBuilder()
                    .AppendLine()
                    .Append($"{SettingsManager.SETTINGS_INTERVAL_KEY}: {SettingsManager.Interval()}s")
                    .AppendLine()
                    .Append($"{SettingsManager.SETTINGS_REPORT_INTERVAL_KEY}: {SettingsManager.ReportInterval()}")
                    .AppendLine()
                    .Append($"{SettingsManager.SETTINGS_RESTRICTNESS_KEY}: {SettingsManager.Restrictness()}")
                    .AppendLine()
                    .Append($"{SettingsManager.SETTINGS_AUTO_SETTING_MODE}: {SettingsManager.IsAutoSetting()}")
                    .AppendLine()
                    .Append($"{SettingsManager.SETTINGS_DEFAULT_SEND_SELECTION}: {SettingsManager.IsSendDefault()}")
                    .AppendLine()
                    .Append($"{SettingsManager.SETTINGS_MODEL_USE_KEY}: {SettingsManager.Model()}");
                App.AlertService.DisplayAlert(title: "Result", message: $"Successfully saved the following settings. {builder}");
            });

            OnAppearingDelegate = () =>
            {
                var latestRestrictness = SettingsManager.Restrictness();
                if (SliderValue == latestRestrictness) return;
                SliderValue = latestRestrictness;
            };
        }

        private static double Average(params double[] nums) => nums.Sum() / nums.Length;
    }
}
