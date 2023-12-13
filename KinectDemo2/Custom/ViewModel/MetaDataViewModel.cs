using KinectDemo2.Custom.Service;
using Microsoft.Azure.Kinect.Sensor;
using System.Windows.Input;
using KinectImageFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat;

namespace KinectDemo2.Custom.ViewModel
{
    public class MetaDataViewModel : BaseViewModel
    {
        private readonly static Dictionary<int, string> COLOR_RESOLUTIONS = new()
        {
            { (int)ColorResolution.Off, "Color camera will be turned off with this setting"},
            { (int)ColorResolution.R720p, "1280 * 720 16:9"},
            { (int)ColorResolution.R1080p, "1920 * 1080 16:9"},
            { (int)ColorResolution.R1440p, "2560 * 1440 16:9"},
            { (int)ColorResolution.R1536p, "2048 * 1536 4:3"},
            { (int)ColorResolution.R2160p, "3840 * 2160 16:9"},
            { (int)ColorResolution.R3072p, "4096 * 3072 4:3"},
        };
        private readonly static Dictionary<int, string> DEPTH_MODE = new() 
        {
            { (int)DepthMode.Off, "Depth sensor will be turned off with this setting" },
            { (int)DepthMode.NFOV_2x2Binned, "Depth and Passive IR are captured at 320×288"},
            { (int)DepthMode.NFOV_Unbinned, "Depth and Passive IR are captured at 640×576"},
            { (int)DepthMode.WFOV_2x2Binned, "Depth and Passive IR are captured at 512×512"},
            { (int)DepthMode.WFOV_Unbinned, "Depth and Passive IR are captured at 1024×1024"},
            { (int)DepthMode.PassiveIR, "Passive IR only is captured at 1024×1024"},
        };
        private readonly static Dictionary<int, string> COLOR_FORMAT = new()
        {
            { (int)KinectImageFormat.ColorMJPG, "MJPG"},
            { (int)KinectImageFormat.ColorNV12, "NV12"},
            { (int)KinectImageFormat.ColorYUY2, "YUY2"},
            { (int)KinectImageFormat.ColorBGRA32, "BGRA32"},
            { (int)KinectImageFormat.Depth16, "DEPTH16"},
            { (int)KinectImageFormat.IR16, "IR16"},
        };
        private readonly static Dictionary<int, string> CAMERA_FPS = new()
        {
            { (int)FPS.FPS5, "5 Frames per second"},
            { (int)FPS.FPS15, "15 Frames per second"},
            { (int)FPS.FPS30, "30 Frames per second"},
        };
        private static readonly IKinectService _kinectService = App.KinectService;
        private static readonly DeviceConfiguration config = new()
        {
            ColorFormat = KinectImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_Unbinned,
            SynchronizedImagesOnly = true,
        };

        private bool _isToggleed = _kinectService.GetCountOfDevices() > 0 && _kinectService.GetDefaultDevice().IsActivated;
        private string _versionColorCamera;
        private string _versionDepthCamera;
        private string _versionAudioDevice;
        private string _versionDepthSensor;
        private string _resolution;
        private string _depMode;
        private string _colorFormat;
        private string _cameraFPS;

        public bool IsToggleed
        {
            get => _isToggleed;
            set
            {
                if (_isToggleed == value) return;

                _isToggleed = _kinectService.GetCountOfDevices() == 0 ? !value : value;
                OnPropertyChanged(nameof(IsToggleed));

                if (_kinectService.GetCountOfDevices() == 0)
                {
                    App.AlertService.DisplayAlert(title: "エラー", message: "デバイスが見つかりませんでした。");
                    return;
                }

                if (_isToggleed) StartCamerasCommand.Execute(this);
                else StopCamerasCommand.Execute(this);
            }
        }
        public string VersionColorCamera
        {
            get => _versionColorCamera;
            set
            {
                if(value ==  _versionColorCamera) return;
                _versionColorCamera = value;
                OnPropertyChanged(nameof(VersionColorCamera));
            }
        }
        public string VersionDepthCamera
        {
            get => _versionDepthCamera;
            set
            {
                if(value == _versionDepthCamera) return;
                _versionDepthCamera = value;
                OnPropertyChanged(nameof(VersionDepthCamera));
            }
        }
        public string VersionAudioDevice
        {
            get => _versionAudioDevice;
            set
            {
                if(value == _versionAudioDevice) return;
                _versionAudioDevice = value;
                OnPropertyChanged(nameof(VersionAudioDevice));
            }
        }
        public string VersionDepthSensor
        {
            get => _versionDepthSensor;
            set
            {
                if (value == _versionDepthSensor) return;
                _versionDepthSensor = value;
                OnPropertyChanged(nameof(VersionDepthSensor));
            }
        }
        public string Resolution
        {
            get => _resolution;
            set
            {
                if(value == _resolution) return;
                _resolution = value;
                OnPropertyChanged(nameof(Resolution));
            }
        }
        public string DepMode
        {
            get => _depMode;
            set
            {
                if(value == _depMode) return;
                _depMode = value;
                OnPropertyChanged(nameof(DepMode));
            }
        }
        public string ColorFmt
        {
            get => _colorFormat;
            set
            {
                if (value == _colorFormat) return;
                _colorFormat = value;
                OnPropertyChanged(nameof(ColorFmt));
            }
        }
        public string CameraFPS
        {
            get => _cameraFPS;
            set
            {
                if (value == _cameraFPS) return;
                _cameraFPS = value;
                OnPropertyChanged(nameof(CameraFPS));
            }
        }
        public ICommand StartCamerasCommand { get; set; }
        public ICommand StopCamerasCommand { get; set; }
        public Action OnAppearingDelegate { get; private set; }


        public MetaDataViewModel()
        {
            // nothing connected
            if (_kinectService.GetCountOfDevices() == 0) return;

            OnAppearingDelegate = () =>
            {
                bool isActivated = false;
                if ((isActivated = _kinectService.GetDefaultDevice().IsActivated) == IsToggleed) return;

                IsToggleed = isActivated;
            };

            GetVersion();
            GetDeviceConfig();
            SetCommands();
        }

        private void SetCommands()
        {
            StartCamerasCommand = new Command(() =>
            {
                _kinectService.StartCamera();
            });

            StopCamerasCommand = new Command(() =>
            {
                _kinectService.StopCamera();
            });
        }

        private void GetVersion()
        {
            var version = _kinectService?.GetVersion();
            VersionColorCamera = version.RGB.ToString();
            VersionDepthCamera = version.Depth.ToString();
            VersionAudioDevice = version.Audio.ToString();
            VersionDepthSensor = version.DepthSensor.ToString();
        }
        private void GetDeviceConfig()
        {
            ColorFmt = COLOR_FORMAT[(int)config.ColorFormat];
            Resolution = COLOR_RESOLUTIONS[(int)config.ColorResolution];
            DepMode = DEPTH_MODE[(int)config.DepthMode];
            CameraFPS = CAMERA_FPS[(int)config.CameraFPS];
        }
    }
}
