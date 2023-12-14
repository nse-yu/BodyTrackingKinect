using KinectDemo2.Custom.Constants;
using KinectDemo2.Custom.Helper.OpenAI;
using KinectDemo2.Custom.Helper.Processing;
using KinectDemo2.Custom.Model.tracking;
using KinectDemo2.Custom.Service;
using KinectDemo2.Custom.Service.Python.ML;
using System.Windows.Input;

namespace KinectDemo2.Custom.ViewModel
{
    public class DataViewerViewModel : BaseViewModel
    {
        public const double HabitViewerOriginalWidth = 20;
        public const double HabitViewerOriginalHeight = 20;
        public const double HabitViewerMaxWidth = 600;
        public const double HabitViewerMaxHeight = 200;


        private readonly IDBService _dbService;
        private readonly CompletionApi _completionApi;

        private int _selectedMonitorValue = 0;
        private int _selectedModelIndex = 1;
        private int _selectedDaySelectionIndex = 0;
        private bool _isDateToggled = false;
        private bool _isAssistantProcessing = false;
        private string _notificationMessage = "No message...";
        private string _habitMessage = "";
        private List<BodyTrackingScore> _trackingList = new();
        private List<BodyTrackingScore> _trackingAverageList = new();
        private DateTime _selectedLowerDate = DateTime.Now.Date;
        private DateTime _selectedUpperDate = DateTime.Now.Date;
        private TimeSpan _selectedLowerTime = TimeSpan.Zero;
        private TimeSpan _selectedUpperTime = new(23, 59, 59);

        public static List<int> DaySections { get; } = new()
        {
            1, 5, 15
        };
        public static List<string> ModelNames { get; } = new()
        {
            "MediaPipe",
            "Azure Kinect",
            "MoveNet - Lightning",
            "MoveNet - Thunder",
        };
        public static List<string> DayStringSelections { get; } = DaySections.Select(i => i+"日").ToList();
        public string ChartTitle { get; private set; } = "Scores filtered by specified section";
        public string ChartAverageTitle { get; private set; } = "Score averages per day";
        public string StatisticsTitle { get; private set; } = "Statistics";
        public string StatisticsSubTitle { get; private set; }
        public int SelectedMonitorValue
        {
            get => _selectedMonitorValue;
            set
            {
                if (_selectedMonitorValue == value) return;
                _selectedMonitorValue = value;
                OnPropertyChanged(nameof(SelectedMonitorValue));
            }
        }
        public int SelectedModelIndex
        {
            get => _selectedModelIndex;
            set
            {
                if(value == _selectedModelIndex) return;
                _selectedModelIndex = value;
                OnPropertyChanged(nameof(SelectedModelIndex));
                RequestUpdate();
            }
        }
        public int SelectedDaySelectionIndex
        {
            get => _selectedDaySelectionIndex;
            set
            {
                if (value == _selectedDaySelectionIndex) return;
                _selectedDaySelectionIndex = value;
                OnPropertyChanged(nameof(SelectedDaySelectionIndex));
                UpdateStatistics();
            }
        }
        public bool IsDateToggled
        {
            get => _isDateToggled;
            set
            {
                if (_isDateToggled == value) return;

                if ((_isDateToggled = value)) InitTimes();
                else InitDates();

                OnPropertyChanged(nameof(IsDateToggled));
            }
        }
        public bool IsAssistantProcessing
        {
            get => _isAssistantProcessing;
            set
            {
                if (_isAssistantProcessing == value) return;
                _isAssistantProcessing = value;
                OnPropertyChanged(nameof(IsAssistantProcessing));
            }
        }
        public string NotificationMessage
        {
            get => _notificationMessage;
            set
            {
                if (_notificationMessage == value) return;
                _notificationMessage = value;
                OnPropertyChanged(nameof(NotificationMessage));
            }
        }
        public string AIAssistantMessage
        {
            get => _habitMessage;
            set
            {
                if (_habitMessage == value) return;
                _habitMessage = value;
                OnPropertyChanged(nameof(AIAssistantMessage));
            }
        }
        public bool IsExpanded { get; set; } = false;
        public List<BodyTrackingScore> TrackingList
        {
            get => _trackingList;
            set
            {
                if (_trackingList == value) return;
                _trackingList = value;
                OnPropertyChanged(nameof(TrackingList));
            }
        }
        public List<BodyTrackingScore> TrackingAverageList
        {
            get => _trackingAverageList;
            set
            {
                if (_trackingAverageList == value) return;
                _trackingAverageList = value;
                OnPropertyChanged(nameof(TrackingAverageList));
            }
        }
        public TrackingStatistics Statistics { get; private set; } = new TrackingStatistics();
        public DateTime SelectedLowerDate
        {
            get => _selectedLowerDate;
            set
            {
                if (_selectedLowerDate == value) return;
                _selectedLowerDate = value;
                OnPropertyChanged(nameof(SelectedLowerDate));
            }
        }
        public DateTime SelectedUpperDate
        {
            get => _selectedUpperDate;
            set
            {
                if (_selectedUpperDate == value) return;
                _selectedUpperDate = value;
                OnPropertyChanged(nameof(SelectedUpperDate));

                if (!IsDateToggled)
                {
                    _selectedLowerDate = _selectedUpperDate.Date;
                    OnPropertyChanged(nameof(SelectedLowerDate));
                }
            }
        }
        public TimeSpan SelectedLowerTime
        {
            get => _selectedLowerTime;
            set
            {
                if (_selectedLowerTime == value) return;
                _selectedLowerTime = value;
                OnPropertyChanged(nameof(SelectedLowerTime));
            }
        }
        public TimeSpan SelectedUpperTime
        {
            get => _selectedUpperTime;
            set
            {
                if (_selectedUpperTime == value) return;
                _selectedUpperTime = value;
                OnPropertyChanged(nameof(SelectedUpperTime));
            }
        }
        public ICommand FilterClickedCommand { get; set; }
        public ICommand HabitInformationTappedCommand { get; set; }
        public ICommand HabitNotificationReloadCommand { get; set; }
        

        public DataViewerViewModel(IDBService dbService)
        {
            _dbService = dbService;
            _completionApi = new(CompletionsApiConstants.API_KEY) { MaxTokens = 30 };
            SetCommand();
            RequestUpdate();
        }

        private void SetCommand()
        {
            FilterClickedCommand = new Command(RequestUpdate);
            HabitInformationTappedCommand = new Command(AnimateHabitInfomation());
            HabitNotificationReloadCommand = new Command(() => UpdateHabitReminder());
        }

        private async ValueTask ResponseLikeChatGPT(string message, int duration = 600)
        {
            var restMessageLength = message.Length;
            var currentIndex = 0;

            while (restMessageLength > 0)
            {
                await Task.Delay(duration);
                var randIndex = restMessageLength == 1 ? 1 : new Random((int)DateTime.Now.Ticks).Next(minValue: 0, maxValue: restMessageLength);
                AIAssistantMessage += message.Substring(currentIndex, randIndex);
                currentIndex += randIndex;
                restMessageLength -= randIndex;
            }
        }
        private Action<object> AnimateHabitInfomation()
        {
            return async arg =>
            {
                /*
                IsExpanded = !IsExpanded;
                View expandable_icon = arg as View;

                double heightTo = IsExpanded ? HabitViewerMaxHeight : HabitViewerOriginalHeight;
                double widthTo = IsExpanded ? HabitViewerMaxWidth : HabitViewerOriginalWidth;
                var anime = CreateScaleAnimation(expandable_icon, heightTo, widthTo);
                anime.Commit(expandable_icon, "expand", 16, animation_duration, null, (v, c) =>
                {
                    expandable_icon.WidthRequest = widthTo;
                    expandable_icon.HeightRequest = heightTo;
                });

                //var response = await _completionApi.GetCompletion("hello, gpt assistant!");
                var response = "Hello! I'm AI assistant.\r\nI can help you to improve your health!!";
                if (IsExpanded) await ResponseLikeChatGPT(response);
                else AIAssistantMessage = string.Empty;
                */
                var response = "Hello! I'm AI assistant.\r\nI can help you to improve your health!!";
                await ResponseLikeChatGPT(response);
                await Task.Delay(30000);
                InitAssistant();
            };
        }
        private void RequestUpdate()
        {
            string model = GetCurrentModelByIndex();
            Task.Run(async () =>
            {
                try
                {
                    await UpdateTrackingList(model);
                    UpdateTrackingAverageList();
                    UpdateStatistics();
                    UpdateHabitReminder();
                }
                catch (HttpRequestException)
                {
                    return;
                }
            });
        }
        private async Task UpdateTrackingList(string model)
        {
            TrackingList = (await FetchScores(model)).Where(tracking => SelectedMonitorValue.ToString() switch
            {
                "0" => true,
                "1" => !tracking.MonitoringMode,
                "2" => tracking.MonitoringMode,
                _ => false
            }).ToList();
        }
        private void UpdateTrackingAverageList()
        {
            TrackingAverageList = TrackingList
                .GroupBy(tracking => tracking.Time.Date)
                .Select(group => group.Aggregate(0.0, (o, n) => o + n.Score, result =>
                {
                    var sample = group.First();
                    return new BodyTrackingScore() { UserId = sample.UserId, Model = sample.Model, MonitoringMode = sample.MonitoringMode, Time = sample.Time.Date, Score = (float)(result / group.Count()) };
                }))
                .ToList();
        }
        private async void UpdateStatistics()
        {
            int past = DaySections[_selectedDaySelectionIndex];
            string model = GetCurrentModelByIndex();
            var trackings = await _dbService.GetScoresAsync(model, DateTime.Now.Date.Subtract(TimeSpan.FromDays(past)), DateTime.Now);
            var scoresDouble = trackings.Select(record => record.Score).ToList();
            var median = MyMath.CalculateMedian(scoresDouble);
            var mean = MyMath.CalculateMean(scoresDouble);
            var std = MyMath.CalculateSTD(scoresDouble);
            Statistics.SetProperties(mean, median, std);
            OnPropertyChanged(nameof(Statistics));

            UpdateStatisticsSubTitle();
        }
        private void UpdateStatisticsSubTitle()
        {
            int past = DaySections[_selectedDaySelectionIndex];
            StatisticsSubTitle = $"過去{past}日間の統計";
            OnPropertyChanged(nameof(StatisticsSubTitle));
        }
        private async void UpdateHabitReminder()
        {
            var habits = await _dbService.GetHabitsAsync(start: new DateTime(2023, 12, 10), stop: SelectedUpperDate);
            var posturalAbnormalityCode = string.Join("",(from habit in habits select habit.PosturalAbnormalityCode));
            (double rateA, double rateB, double rateC) = StringUtils.CalculateAlphabetRatios(posturalAbnormalityCode);
            var now = DateTime.Now;
            NotificationMessage = 
                $"Aの兆候が{rateA*10}割、Bの兆候が{rateB*10}割、Cの兆候が{rateC*10}割ほど確認できました。\r\n" +
                $"【計測期間: {now.AddDays(-(now.Day - 1)):M} ~ {now.Date:M}】";
        }


        private void InitDates()
        {
            _selectedUpperDate = DateTime.Now.Date;
            _selectedLowerDate = DateTime.Now.Date;
            OnPropertyChanged(nameof(SelectedUpperDate));
            OnPropertyChanged(nameof(SelectedLowerDate));
        }
        private void InitTimes()
        {
            _selectedUpperTime = new(23, 59, 59);
            _selectedLowerTime = TimeSpan.Zero;
            OnPropertyChanged(nameof(SelectedUpperTime));
            OnPropertyChanged(nameof(SelectedLowerTime));
        }
        private void InitAssistant()
        {
            AIAssistantMessage = string.Empty;
            IsAssistantProcessing = false;
        }

        private async Task<List<BodyTrackingScore>> FetchScores(string modelName)
        {
            return await _dbService.GetScoresAsync(
                    modelName,
                    SelectedLowerDate.Add(SelectedLowerTime),
                    SelectedUpperDate.Add(SelectedUpperTime)
                    );
        }
        private string GetCurrentModelByIndex()
        {
            return _selectedModelIndex switch
            {
                0 => MLModel.MEDIAPIPE.ToString(),
                1 => "kinect",
                2 => MLModel.MOVENET_LIGHTNING.ToString(),
                3 => MLModel.MOVENET_THUNDER.ToString(),
                _ => MLModel.MEDIAPIPE.ToString(),
            };
        }
    }
}