using KinectDemo2.Custom.Exceptions;
using KinectDemo2.Custom.Helper;
using KinectDemo2.Custom.Service;
using KinectDemo2.Custom.Service.Python;
using Microsoft.Azure.Kinect.BodyTracking;
using Microsoft.Azure.Kinect.Sensor;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Media.Capture;
using Windows.Media.MediaProperties;
using KinectFrame = Microsoft.Azure.Kinect.BodyTracking.Frame;
using JointKey = KinectDemo2.Custom.Model.kinect.KeyPointsWithScores.MoveNetJointKey;
using KinectDemo2.Custom.Helper.Settings;
using SkiaSharp;
using KinectDemo2.Custom.ViewModel.Base;
using KinectDemo2.Custom.Helper.Utils;
using CommunityToolkit.Maui.Views;
using KinectDemo2.Custom.PopUp;
using KinectDemo2.Custom.Model.kinect;
using KinectDemo2.Custom.Model.tracking;
using KinectDemo2.Custom.Helper.Processing;
using KinectDemo2.Custom.Constants;
using System.Text;
using KinectDemo2.Custom.Helper.OpenAI;

namespace KinectDemo2.Custom.ViewModel
{
    public class KinectBodyTrackingViewModel : KinectDataStreamBaseViewModel
    {
        private const double IMAGE_RESIZE_RATIO = .6;
        private const int TIMER_PERIOD = 60000;

        private static readonly IDBService _DBService = App.DBService;
        private static readonly IPythonService _PythonService = App.PythonService;
        private static readonly Dictionary<JointId, JointKey> KEY_MAPPING_MOVENET_KINECT_JOINT = new()
        {
            { JointId.Nose, JointKey.NOSE },
            { JointId.EyeLeft, JointKey.LEFT_EYE },
            { JointId.EyeRight, JointKey.RIGHT_EYE },
            { JointId.EarLeft, JointKey.LEFT_EAR },
            { JointId.EarRight, JointKey.RIGHT_EAR },
            { JointId.ShoulderLeft, JointKey.LEFT_SHOULDER },
            { JointId.ShoulderRight, JointKey.RIGHT_SHOULDER },
            { JointId.ElbowLeft, JointKey.LEFT_ELBOW },
            { JointId.ElbowRight, JointKey.RIGHT_ELBOW },
            { JointId.HandLeft, JointKey.LEFT_WRIST },
            { JointId.HandRight, JointKey.RIGHT_WRIST },
            { JointId.HipLeft, JointKey.LEFT_HIP },
            { JointId.HipRight, JointKey.RIGHT_HIP },
            { JointId.KneeLeft, JointKey.LEFT_KNEE },
            { JointId.KneeRight, JointKey.RIGHT_KNEE },
            { JointId.AnkleLeft, JointKey.LEFT_ANKLE },
            { JointId.AnkleRight, JointKey.RIGHT_ANKLE },
        };
        private static readonly Dictionary<JointId, PoseLandmarkKey> KEY_MAPPING_MEDIAPIPE_KINECT_JOINT = new()
        {
            { JointId.Nose, PoseLandmarkKey.NOSE },
            { JointId.EyeLeft, PoseLandmarkKey.LEFT_EYE },
            { JointId.EyeRight, PoseLandmarkKey.RIGHT_EYE },
            { JointId.EarLeft, PoseLandmarkKey.LEFT_EAR },
            { JointId.EarRight, PoseLandmarkKey.RIGHT_EAR },
            { JointId.ShoulderLeft, PoseLandmarkKey.LEFT_SHOULDER },
            { JointId.ShoulderRight, PoseLandmarkKey.RIGHT_SHOULDER },
            { JointId.ElbowLeft, PoseLandmarkKey.LEFT_ELBOW },
            { JointId.ElbowRight, PoseLandmarkKey.RIGHT_ELBOW },
            { JointId.WristLeft, PoseLandmarkKey.LEFT_WRIST },
            { JointId.WristRight, PoseLandmarkKey.RIGHT_WRIST },
            { JointId.ThumbLeft, PoseLandmarkKey.LEFT_THUMB },
            { JointId.ThumbRight, PoseLandmarkKey.RIGHT_THUMB },
            { JointId.HipLeft, PoseLandmarkKey.LEFT_HIP },
            { JointId.HipRight, PoseLandmarkKey.RIGHT_HIP },
            { JointId.KneeLeft, PoseLandmarkKey.LEFT_KNEE },
            { JointId.KneeRight, PoseLandmarkKey.RIGHT_KNEE },
            { JointId.AnkleLeft, PoseLandmarkKey.LEFT_ANKLE },
            { JointId.AnkleRight, PoseLandmarkKey.RIGHT_ANKLE },
        };
        private static Action<ProgressCallbackArguments> progress;

        private readonly object locker = new();
        private readonly List<BodyTrackingScore> ScoreReports = new();
        private readonly BodyTrackingHabit HabitReport = new() { UserId = InfluxConstants.FIELD_USER_ID };

        private MediaCapture mediaCaptureSource;
        private LowLagPhotoCapture lowLagCaptureSession;
        private Action<object> NotificationBarAction;

        private bool isFlushed = true;
        private bool _isSubmitScores = SettingsManager.IsSendDefault();
        private bool _isMonitoringModeOn = false;
        private bool _isPlaying = false;
        private bool _isPredicting = false;
        private double _progress = 0;
        private string _progressLabel = string.Empty;
        private Timer habitReportTimer = null;


        public bool IsSubmitScores
        {
            get => _isSubmitScores;
            set
            {
                if (_isSubmitScores == value) return;
                _isSubmitScores = value;
                OnPropertyChanged(nameof(IsSubmitScores));
            }
        }
        public bool IsMonitoringModeOn
        {
            get => _isMonitoringModeOn;
            set
            {
                if (value == _isMonitoringModeOn) return;
                _isMonitoringModeOn = value;
                OnPropertyChanged(nameof(IsMonitoringModeOn));
            }
        }
        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                if (value == _isPlaying) return;
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        public bool IsPredicting
        {
            get => _isPredicting;
            set
            {
                if (value == _isPredicting) return;
                _isPredicting = value;
                OnPropertyChanged(nameof(IsPredicting));
            }
        }
        public double Progress
        {
            get => _progress;
            set
            {
                if (value == _progress) return;
                _progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }
        public string ProgressLabel
        {
            get => _progressLabel;
            set
            {
                if (value == _progressLabel) return;
                _progressLabel = value;
                OnPropertyChanged(nameof(ProgressLabel));
            }
        }
        public ObservableCollection<KinectJoint> Joints { get; set; } = new(from key in KinectService.JOINTS.Keys select new KinectJoint(key));
        public ICommand ReloadingCommand { get; private set; }
        public ICommand ReloadedCommand { get; private set; }
        public ICommand BeforePlayCommand { get; private set; }
        public ICommand AfterPlayCommand { get; private set; }
        public ICommand PlayingCommand { get; private set; }
        public ICommand StoppedCommand { get; private set; }
        public ICommand CameraClickedCommand { get; private set; }
        public ICommand SaveToCsvCommand { get; private set; }
        public ICommand MLClickedCommand { get; private set; }


        public KinectBodyTrackingViewModel(IKinectService kinectService, IAlertService alertService) : base(kinectService, alertService)
        {
            SetCommands();
            try
            {
                LoadDevices();
            }
            catch (NotHandledKinectException e)
            {
                HandleUnLoaded(e);
            }
            finally
            {
                ApplyConnectedToText();
                progress = (ProgressCallbackArguments args) =>
                {
                    Progress = args.progress;
                    ProgressLabel = args.message;
                };
            }
        }
        ~KinectBodyTrackingViewModel()
        {
            habitReportTimer.Dispose();
            _PythonService.Shutdown();
        }

        public void SetCommands()
        {
            ReloadingCommand = new Command(ReloadingMethod);
            ReloadedCommand = new Command(NotifyCountOfLoadedDevices);
            BeforePlayCommand = new Command(InitToken);
            StoppedCommand = new Command(CancelToken);
            PlayingCommand = new Command(PlayingMethod());
            CameraClickedCommand = new Command(CameraMethod());
            SaveToCsvCommand = new Command(SaveCsvMethod());
            MLClickedCommand = new Command(MLMethod());
        }

        private void ReloadingMethod()
        {
            try
            {
                LoadDevices();
            }
            catch (AzureKinectException e)
            {
                UndoPlaying();
                ThreadSafeNotifier.Notify(title: $"{AlertService.ALERT_TITLE_ERROR}: {e.Message}", KinectService.ALERT_CAMERA_NOT_STARTED);
            }
            finally
            {
                ApplyConnectedToText();
            }
        }
        private Action PlayingMethod()
        {
            return async () =>
            {
                bool requestStop = await AlertIfModelRunning();
                requestStop = requestStop || await AlertIfNoDevice();
                requestStop = requestStop || await AlertIfStoppedCamera();
                requestStop = requestStop || await AlertIfIntervalNotFound();
                requestStop = requestStop || await AlertIfIntervalInvalid(5);

                if (requestStop)
                {
                    UndoPlaying();
                    return;
                }

                AnimateNotificationBar(SystemMessages.NOTIFY_STARTING_KINECT);
                InitToken();
                InitProgress();
                _ = GenerateHabitTask();

                var interval = GetAdjustedInterval();
                try
                {
                    await Task.Run(async () =>
                    {
                        while (true)
                        {
                            if (!IsPlaying) return;

                            UpdateProgressBar(.3, "Searching bodies...");

                            using var indexedFrame = GetBodyFrameByIndex(0);

                            if (indexedFrame.IsNoBody)
                            {
                                await Task.Delay(2000);
                                continue;
                            }

                            UpdateJoints(indexedFrame.GetSkeleton());
                            UpdateAnnotatedImage(indexedFrame.GetFrame());

                            UpdateProgressBar(.7, "Predicting...");

                            if (!IsPlaying) return;

                            CancelIfRequested();

                            double meanScore = AlertIfBadPostureOnKinect();

                            if (IsSubmitScores)
                            {
                                AppendReport(new BodyTrackingScore()
                                {
                                    Model = "kinect",
                                    Score = (float)meanScore,
                                    Time = DateTime.Now,
                                    MonitoringMode = IsMonitoringModeOn,
                                    UserId = InfluxConstants.FIELD_USER_ID
                                });

                                if (isFlushed)
                                {
                                    _ = GenerateSubmitTask(TimeSpan.FromSeconds(SettingsManager.ReportIntervalFromSecond()))
                                    .ContinueWith(GenerateSettingTasks())
                                    .ContinueWith(GenerateSubmitCleanupTask());
                                    UpdateIsFlushedSafer(false);
                                }
                            }

                            UpdateProgressBar(1.0, $"Complete");

                            await Task.Delay(interval);

                            InitProgress();
                        }
                    }, _cancelTokenSource.Token);
                }
                catch (NotHandledKinectException e)
                {
                    await App.AlertService.DisplayAlert(title: $"{AlertService.ALERT_TITLE_ERROR}: {e.Message}", KinectService.ALERT_CAMERA_NOT_STARTED);
                }
                catch (OperationCanceledException)
                {
                    await MainThread.InvokeOnMainThreadAsync(() => AnimateNotificationBar("監視および送信を終了しています..."));
                }
                finally
                {
                    UndoPlaying();
                    InitProgress();
                    ResetHabitTimer();
                }
            };
        }
        private Action CameraMethod()
        {
            return async () =>
            {
                bool requestStop = await AlertIfNoDevice();

                if (requestStop) return;

                Task pyPlotTask = GeneratePyPlotTask();

                try
                {
                    using var indexedFrame = GetBodyFrameByIndex(0);
                    UpdateJoints(indexedFrame.GetSkeleton());
                    UpdateDynamicAnnotatedImage(indexedFrame.GetFrame());
                    //TODO await pyPlotTask;

                    _PythonService.PlotJoints(Joints.ToArray());

                    double meanScore = AlertIfBadPostureOnKinect();
                    _ = await App.Current.MainPage.ShowPopupAsync(new InfoPopup($"{meanScore:F2}点", meanScore / 100, false, "スコアは...."));
                }
                catch (Exception e)
                {
                    await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: e.Message);
                }
            };
        }
        private Action SaveCsvMethod()
        {
            return async () =>
            {
                CancellationToken saveToken = new();
                try
                {
                    var successed = await SaveJointsToCsv(Joints, saveToken);
                    if (!successed) return;
                }
                catch (OperationCanceledException)
                {
                    await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: SystemMessages.OPERATION_CANCELED_COMMENT);
                    return;
                }
                await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_RESULT, message: "保存が完了しました");
            };
        }
        private Action MLMethod()
        {
            return async () =>
            {
                int INTERVAL_SAFETY = SettingsManager.Intervals[3];

                bool requestStop = await AlertIfIntervalNotFound();
                requestStop = requestStop || await AlertIfKinectRunning();
                requestStop = requestStop || await AlertIfIntervalInvalid(INTERVAL_SAFETY);
                requestStop = requestStop || await AlertIfModelNotSelected();

                if (requestStop)
                {
                    UndoPredicting();
                    return;
                }

                var modelInUse = SettingsManager.Model();
                var model = PosePredictionModel.GetInstance(modelInUse).SetCallBack(progress).SetCancellationToken(_token);

                InitToken();
                InitProgress();
                UpdateProgressBar(.2, "Importing libraries...");
                await GenerateMLTask();
                AnimateNotificationBar($"Started with the {modelInUse} model.");

                var interval = GetAdjustedInterval();
                try
                {
                    await Task.Run(async () =>
                    {
                        SKBitmap input_img, resized_input_img = null;

                        void DisposeUsedImages()
                        {
                            resized_input_img?.Dispose();
                            input_img?.Dispose();
                        }

                        while (true)
                        {
                            if (!IsPredicting) return;

                            CancelIfRequested();

                            input_img = await CaptureFromStdCamera(ImageEncodingProperties.CreateJpeg());
                            resized_input_img = ResizeUseRatio(input_img, new RatioRect(input_img.Width, input_img.Height, IMAGE_RESIZE_RATIO));

                            var poseLandmarks = model.Predict(resized_input_img, out SKBitmap output_img);

                            DisposeUsedImages();

                            if (poseLandmarks == null)
                            {
                                output_img?.Dispose();
                                await Task.Delay(1000);
                                continue;
                            }

                            MapLandmarksToJoints(poseLandmarks);
                            ShowImage(output_img);
                            double meanScore = AlertIfBadPostureOnStdCamera();
                            output_img?.Dispose();

                            if (IsSubmitScores)
                            {
                                AppendReport(new BodyTrackingScore()
                                {
                                    Model = modelInUse.ToString(),
                                    Score = (float)meanScore,
                                    Time = DateTime.Now,
                                    MonitoringMode = IsMonitoringModeOn,
                                    UserId = InfluxConstants.FIELD_USER_ID
                                });

                                if (isFlushed)
                                {
                                    _ = GenerateSubmitTask(TimeSpan.FromSeconds(SettingsManager.ReportIntervalFromSecond()))
                                    .ContinueWith(GenerateSettingTasks())
                                    .ContinueWith(GenerateSubmitCleanupTask());
                                    UpdateIsFlushedSafer(false);
                                }
                            }

                            await Task.Delay(interval);
                            InitProgress();
                        }
                    }, _cancelTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    // キャンセル時はあえて何もしない
                }
                catch (Exception e)
                {
                    await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: $"{e.Message}\r\n{e.StackTrace}", "OK");
                }
                finally
                {
                    UndoPredicting();
                    FinishCameraSession();
                    InitProgress();
                    _PythonService.DisposeModel();
                }
            };
        }


        private void LoadDevices()
        {
            _KinectService.LoadDevices();
        }
        private void LoadCaptures()
        {
            try
            {
                if (!IsPlaying) return;
                (ImageRGB, _) = _KinectService.GetSourceFromKinect();
            }
            catch (AzureKinectException e)
            {
                // 十中八九、カメラ未起動
                MainThread.InvokeOnMainThreadAsync(() =>
                {
                    App.AlertService.DisplayAlert(title: $"{AlertService.ALERT_TITLE_ERROR}: {e.Message}", KinectService.ALERT_CAMERA_NOT_STARTED);
                });
                IsPlaying = false;
                return;
            }
        }


        private async Task<SKBitmap> CaptureFromStdCamera(ImageEncodingProperties encodingProperties)
        {
            mediaCaptureSource ??= await StartCaptureSession();
            lowLagCaptureSession ??= await mediaCaptureSource.PrepareLowLagPhotoCaptureAsync(encodingProperties);

            var capturedImage = await lowLagCaptureSession.CaptureAsync();

            if (capturedImage == null) await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: "Can't get an image, please to try later...");

            return ConvertPhotoToImage(capturedImage);
        }
        private async Task<bool> SaveJointsToCsv(Collection<KinectJoint> joints, CancellationToken token)
        {
            string query = GenerateCsvQueryFromJoints(joints);

            var savedResult = await KinectJointIO.SaveJointAsync(query, "output.csv", token);

            return savedResult.IsSuccessful;
        }
        private async Task<MediaCapture> StartCaptureSession()
        {
            var mediaCaptureSessionSource = new MediaCapture();
            await mediaCaptureSessionSource.InitializeAsync();
            return mediaCaptureSessionSource;
        }
        private async void FinishCameraSession()
        {
            if (lowLagCaptureSession == null) return;
            await lowLagCaptureSession.FinishAsync();
            lowLagCaptureSession = null;
        }


        private void CancelIfRequested()
        {
            if (!_token.IsCancellationRequested) return;
            _token.ThrowIfCancellationRequested();
        }
        private void InitProgress()
        {
            UpdateProgressBar(0, "");
        }
        private void ResetHabitTimer()
        {
            habitReportTimer.Dispose();
            habitReportTimer = null;
        }
        private void ApplyConnectedToText()
        {
            TextConnectedDevices = _KinectService.GetCountOfDevices() + TEXT_CONNECTED_COMMENT;
        }
        private void AppendReport(BodyTrackingScore report)
        {
            ScoreReports.Add(report);
        }
        private void UpdateAnnotatedImage(KinectFrame frame)
        {
            using var rgbImg = frame.Capture.Color;
            using var poseImg = _KinectService.DrawLandmarksOnImage(rgbImg, Joints.ToList());
            ImageRGB = _KinectService.ConvertImageToSource(poseImg);
        }
        private void UpdateDynamicAnnotatedImage(KinectFrame frame)
        {
            using var rgbImg = frame.Capture.Color;
            using var poseImg = _KinectService.DrawDynamicLandmarksOnImage(rgbImg, Joints.ToList());
            ImageRGB = _KinectService.ConvertImageToSource(poseImg);
        }
        private void UpdateJoints(Skeleton skeleton)
        {
            Joints.ToList().ForEach(joint =>
            {
                var newJoint = skeleton.GetJoint(joint.Id);
                joint.SetPosition(newJoint.Position).SetQuaternion(newJoint.Quaternion);
            });
        }
        private void UndoPlaying() => IsPlaying = false;
        private void UndoPredicting() => IsPredicting = false;
        private void UpdateProgressBar(double progress, string label = "")
        {
            Progress = progress;
            ProgressLabel = label;
        }
        private void UpdateIsFlushedSafer(bool state)
        {
            lock (locker)
            {
                isFlushed = state;
            }
        }
        private void MapLandmarksToJoints(List<PoseLandmark> landmarks)
        {
            if (landmarks == null) throw new ArgumentNullException(Exception5WQuestionsFactory.Init()
                .What("Null value was passed.").When(DateTime.Now).Where(nameof(MapLandmarksToJoints)).Get(typeof(ArgumentNullException)));

            Joints.ToList().Where(joint => KEY_MAPPING_MEDIAPIPE_KINECT_JOINT.ContainsKey(joint.Id)).ToList().ForEach(joint =>
            {
                var newJoint = Enumerable.FirstOrDefault(landmarks, result => KEY_MAPPING_MEDIAPIPE_KINECT_JOINT[joint.Id] == result.Key);
                joint.SetPosition(newJoint.X, newJoint.Y, newJoint.Z);
            });
        }


        private void HandleUnLoaded(NotHandledKinectException e)
        {
            _AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: $"{e.Message}\r\nデバイスの電源を確認してください");
        }
        private void NotifyCountOfLoadedDevices()
        {
            var message = _KinectService.GetCountOfDevices() > 0 ? KinectService.ALERT_DEVICE_FOUND : KinectService.ALERT_DEVICE_NOT_FOUND;
            App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_RESULT, message: message);
        }
        private bool AlertIfBadPostureOrMonitoring(params PostureReviewResult[] results)
        {
            if (IsMonitoringModeOn) return false;
            if (results.All(result => result.IsRightPosture)) return false;

            string alertMessage = string.Join(" と ", results.Where(result => !result.IsRightPosture).Select(result => result.Part).ToList());
            ThreadSafeNotifier.NotifyOnSystem(alertMessage);
            return true;
        }
        private double AlertIfBadPostureOnStdCamera()
        {
            var restrictness = SettingsManager.Restrictness();
            var result1 = PostureReviewer.HorizontalLevelForCamera(Joints, restrictness);
            var result2 = PostureReviewer.TriangleBalanceForCamera(Joints, restrictness);
            AlertIfBadPostureOrMonitoring(result1, result2);

            return new double[] { result1.Score, result2.Score }.Average();
        }
        private double AlertIfBadPostureOnKinect()
        {
            var restrictness = SettingsManager.Restrictness();

            var result1 = PostureReviewer.HorizontalLevelForKinect(Joints, restrictness);
            var result2 = PostureReviewer.NeckDepthForKinect(Joints, restrictness);
            var result3 = PostureReviewer.NeckMisalignmentForKinect(Joints, restrictness);

            AlertIfBadPostureOrMonitoring(result1, result2, result3);
            AppendHabitReport(result1.Score, result2.Score, result3.Score);

            // TODO: 点数評価に、NeckMisalignmentForKinect(頸の位置)が含まれていない。
            return new double[] { result1.Score, result2.Score }.Average();
        }
        private async Task<bool> AlertIfModelNotSelected()
        {
            if (SettingsManager.Contains(SettingsManager.SETTINGS_MODEL_USE_KEY)) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_WAIT, message: $"使用するモデルを設定してください");
            return true;
        }
        private async Task<bool> AlertIfNoDevice()
        {
            if (_KinectService.GetCountOfDevices() > 0) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: KinectService.ALERT_DEVICE_NOT_FOUND);
            return true;
        }
        private async Task<bool> AlertIfStoppedCamera()
        {
            if (_KinectService.GetDefaultDevice().IsActivated) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: KinectService.ALERT_CAMERA_NOT_STARTED);
            return true;
        }
        private async Task<bool> AlertIfModelRunning()
        {
            if (!IsPredicting) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: "まずAIを停止してください。");
            return true;
        }
        private async Task<bool> AlertIfKinectRunning()
        {
            if (!IsPlaying) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: "まずKinectを停止してください。");
            return true;
        }
        private async Task<bool> AlertIfIntervalNotFound()
        {
            if (SettingsManager.Contains(SettingsManager.SETTINGS_INTERVAL_KEY)) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_WAIT, message: "更新間隔を設定してください");
            return true;
        }
        private async Task<bool> AlertIfIntervalInvalid(int threshold)
        {
            if (SettingsManager.Interval() >= threshold) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_WAIT, message: "計算負荷を抑えるため、更新間隔は5秒以上にしてください");
            return true;
        }


        private void ShowImage(SKBitmap image)
        {
            if (image == null) throw new Exception($"Null value was passed : {nameof(image)}");
            ImageRGB = _KinectService.ConvertImageToSource(image);
        }
        public void AnimatedCallback(Action<object> action)
        {
            NotificationBarAction = action;
        }
        private void AnimateNotificationBar(string message)
        {
            NotificationBarAction?.Invoke(message);
        }


        private Task GeneratePyPlotTask()
        {
            return Task.Run(() =>
            {
                if (ShouldInitPyService()) _PythonService.Init();
                _PythonService.ImportLibrary(false, PYLIB.MATPLOTLIB_PYPLOT);
            });
        }
        private Task GenerateMLTask()
        {
            return Task.Run(() =>
            {
                if (ShouldInitPyService()) _PythonService.Init();
                if (_PythonService.HasAll(PYLIB.SYS, PYLIB.TENSORFLOW_HUB, PYLIB.MY_MODULE, PYLIB.MATPLOTLIB_PYPLOT)) return;

                _PythonService.ImportLibrary(true, PYLIB.SYS, PYLIB.TENSORFLOW_HUB, PYLIB.MY_MODULE);
                _PythonService.ImportLibrary(false, PYLIB.MATPLOTLIB_PYPLOT);
            });
        }
        private Task<IEnumerable<double>> GenerateSubmitTask(TimeSpan interval)
        {
            return Task.Run(async () =>
            {
                await Task.Delay(interval);
                await _DBService.SaveScores(ScoreReports.ToList());
                return ScoreReports.Select(report => (double)report.Score);
            });
        }
        private Func<Task<IEnumerable<double>>, string> GenerateSettingTasks()
        {
            return result => 
            {
                if (!SettingsManager.IsAutoSetting()) return string.Empty;

                var scores = result.Result;

                if (scores.Count() <= 1) return string.Empty;

                var emaValues = MyMath.CalculateEMA(scores, .2);
                var len = emaValues.Count;
                var slope = len > 1 ? (emaValues[len - 1] - emaValues[len - 2]) / emaValues[len - 2] : 0;

                var range = SettingsManager.RestrictnessMax - SettingsManager.RestrictnessMin;
                SettingsManager.SetRestrictness(SettingsManager.Restrictness() + range * .1 * (slope > 0 ? 1 : -1));

                return $"{(slope > 0 ? "改善" : "悪化")}傾向にあるので、難易度を{(slope > 0 ? "上げ" : "下げ")}ました。";
            };
        }
        private Func<Task<string>, Task> GenerateSubmitCleanupTask()
        {
            return async resultFromSettingTask =>
            {
                var message = resultFromSettingTask.Result;
                await MainThread.InvokeOnMainThreadAsync(() => AnimateNotificationBar(SystemMessages.NOTIFY_SENDING_REPORT + (message.Length > 0 ? $"\r\n{message}" : "")));
                ScoreReports.Clear();
                UpdateIsFlushedSafer(true);
            };
        }
        private Task GenerateHabitTask()
        {
            return Task.Run(() =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    habitReportTimer = new Timer(new TimerCallback(SendHabitReportsAsync), null, TIMER_PERIOD, TIMER_PERIOD);
                });
            });
        }
        private async void SendHabitReportsAsync(object state)
        {
            await _DBService.SaveHabits(new() { HabitReport });
            HabitReport.InitCode();
        }


        private SKBitmap ResizeUseRatio(SKBitmap image, RatioRect ratioRect) => image.Resize(new SKImageInfo((int)ratioRect.GetWidth(), (int)ratioRect.GetHeight()), SKFilterQuality.Medium);
        private SKBitmap ConvertPhotoToImage(CapturedPhoto photo)
        {
            var imageStream = photo.Frame.AsStreamForRead();
            var sk_rgba_img = SKBitmap.Decode(imageStream, bitmapInfo: new((int)photo.Frame.Width, (int)photo.Frame.Height, SKColorType.Rgba8888));
            return sk_rgba_img;
        }
        private string StringifyKinectJoints(Collection<KinectJoint> joints) => string.Join("", joints.Select(j => $"{j.Id},{KinectService.JOINTS[j.Id]},{j.PositionX},{j.PositionY},{j.PositionZ},{j.QuaternionW},{j.QuaternionX},{j.QuaternionY},{j.QuaternionZ}\r\n"));
        private string GenerateCsvQueryFromJoints(Collection<KinectJoint> joints)
        {
            string query = $"jointId,label,positionX,positionY,positionZ,QuaternionW,QuaternionX,QuaternionY,QuaternionZ\r\n{StringifyKinectJoints(joints)}";
            return query.Remove(query.Length - 2);
        }
        private KinectFrame GetFrame() => _KinectService.GetBodyFrame();
        private KinectIndexedFrame GetBodyFrameByIndex(uint bodyIndex) => new(bodyIndex, GetFrame());
        private bool ShouldInitPyService() => !((PythonService)_PythonService).IsInitialized;
        private void AppendHabitReport(double score_horizontal_level, double score_neck_depth, double score_neck_misalignment, double score_border = 50)
        {
            var codeBuffer = new StringBuilder();
            
            if (score_horizontal_level < score_border) codeBuffer.Append(PosturalAbnormalityConstants.ABNORMALITY_SCOLIOSIS);
            if (score_neck_depth < score_border || score_neck_misalignment < score_border) codeBuffer.Append(PosturalAbnormalityConstants.ABNORMALITY_MILITARY_NECK);

            HabitReport.InitTime();
            HabitReport.Append(codeBuffer.ToString());
        }
    }
}