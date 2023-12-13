using Microsoft.Azure.Kinect.Sensor;
using System.Windows.Input;
using KinectDemo2.Custom.Service;
using KinectDemo2.Custom.Exceptions;
using KinectDemo2.Custom.ViewModel.Base;
using KinectDemo2.Custom.Helper.Utils;

namespace KinectDemo2.Custom.ViewModel
{
    public class KinectSensorViewModel : KinectDataStreamBaseViewModel
    {
        private bool _isPlaying = false;
        private ImageSource _imageDepth = ImageSource.FromFile(IMAGE_DEFAULT_DISPLAYED);

        public bool IsPlaying
        {
            get => _isPlaying;
            set
            {
                //if(value == _isPlaying) return;
                _isPlaying = value;
                OnPropertyChanged(nameof(IsPlaying));
            }
        }
        public ImageSource ImageDepth
        {
            get => _imageDepth;
            set
            {
                if(value == _imageDepth) return;
                _imageDepth = value;
                OnPropertyChanged(nameof(ImageDepth));
            }
        }
        public ICommand ReloadingCommand { get; set; }
        public ICommand ReloadedCommand { get; set; }
        public ICommand BeforePlayCommand {  get; set; }
        public ICommand AfterPlayCommand { get; set; }
        public ICommand PlayingCommand { get; set; }
        public ICommand StoppedCommand { get; set; }


        public KinectSensorViewModel(IKinectService kinectService, IAlertService alertService) : base(kinectService, alertService)
        {
            SetCommands();
            try
            {
                LoadDevices();
            }catch(NotHandledKinectException e)
            {
                HandleUnLoadedDevice(e);
            }
            finally
            {
                ApplyConnectedToText();
            }
        }

        private void SetCommands()
        {
            ReloadingCommand = new Command(ReloadingMethod);
            ReloadedCommand = new Command(NotifyCountOfLoadedDevices);
            BeforePlayCommand = new Command(InitToken);
            StoppedCommand = new Command(CancelToken);
            PlayingCommand = new Command(PlayingMethod());
        }

        private Action PlayingMethod()
        {
            return async () =>
            {
                bool requestStop = await AlertIfNoDevice();
                requestStop = await AlertIfStoppedCamera();

                if (requestStop)
                {
                    UndoPlaying();
                    return;
                }

                var interval = GetAdjustedInterval();
                try
                {
                    await Task.Run(async () =>
                    {
                        // IsPlayingの変更と画像ロードからの復帰タイミングを考慮し、２箇所に脱出口を設置
                        while (true)
                        {
                            if (!IsPlaying) return;

                            LoadCaptures();

                            if (!IsPlaying) return;

                            CancelIfRequested();

                            await Task.Delay(interval);
                        }
                    }, _cancelTokenSource.Token);
                }
                catch (OperationCanceledException)
                {
                    HandleCancellation();
                }
                catch (AzureKinectException e)
                {
                    ThreadSafeNotifier.Notify($"エラー: {e.Message}", KinectService.ALERT_CAMERA_NOT_STARTED);
                }
            };
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


        private void LoadDevices()
        {
            _KinectService.LoadDevices();
        }
        private void LoadCaptures()
        {
            (ImageRGB, ImageDepth) = _KinectService.GetSourceFromKinect();
        }


        private void CancelIfRequested()
        {
            if (!_token.IsCancellationRequested) return;
            _token.ThrowIfCancellationRequested();
        }
        private void ApplyConnectedToText()
        {
            TextConnectedDevices = _KinectService.GetCountOfDevices() + TEXT_CONNECTED_COMMENT;
        }
        private void UndoPlaying() => IsPlaying = false;
        private void UndoImages() => ImageRGB = ImageDepth = ImageSource.FromFile(IMAGE_DEFAULT_DISPLAYED);


        private void HandleUnLoadedDevice(NotHandledKinectException e)
        {
            _AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: $"{e.Message}\r\nデバイスの電源を確認してください");
        }
        private void HandleCancellation()
        {
            UndoPlaying();
            UndoImages();
        }
        private void NotifyCountOfLoadedDevices()
        {
            var message = _KinectService.GetCountOfDevices() > 0 ? KinectService.ALERT_DEVICE_FOUND : KinectService.ALERT_DEVICE_NOT_FOUND;
            App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_RESULT, message: message);
        }
        private async Task<bool> AlertIfNoDevice()
        {
            if (_KinectService.GetCountOfDevices() > 0) return false;
            await App.AlertService.DisplayAlert(title: "エラー", message: KinectService.ALERT_DEVICE_NOT_FOUND);
            return true;
        }
        private async Task<bool> AlertIfStoppedCamera()
        {
            if (_KinectService.GetDefaultDevice().IsActivated) return false;
            await App.AlertService.DisplayAlert(title: AlertService.ALERT_TITLE_ERROR, message: KinectService.ALERT_CAMERA_NOT_STARTED);
            return true;
        }
    }
}