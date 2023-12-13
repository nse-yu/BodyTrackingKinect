using KinectDemo2.Custom.Service;

namespace KinectDemo2.Custom.ViewModel.Base
{
    public class KinectDataStreamBaseViewModel : BaseViewModel
    {
        protected const string IMAGE_DEFAULT_DISPLAYED = "black_color.png";
        protected const string TEXT_CONNECTED_COMMENT = "台のAzure Kinectが接続されています。";

        protected readonly IKinectService _KinectService;
        protected readonly IAlertService _AlertService;

        protected CancellationTokenSource _cancelTokenSource = null;
        protected CancellationToken _token;

        private string _textConnectedDevices;
        private ImageSource _imageRGB = ImageSource.FromFile(IMAGE_DEFAULT_DISPLAYED);

        public ImageSource ImageRGB
        {
            get => _imageRGB;
            set
            {
                if (value == _imageRGB) return;
                _imageRGB = value;
                OnPropertyChanged(nameof(ImageRGB));
            }
        }
        public string TextConnectedDevices
        {
            get => _textConnectedDevices;
            set
            {
                if (value == _textConnectedDevices) return;
                _textConnectedDevices = value;
                OnPropertyChanged(nameof(TextConnectedDevices));
            }
        }

        public KinectDataStreamBaseViewModel(IKinectService kinectService, IAlertService alertService)
        {
            _KinectService = kinectService;
            _AlertService = alertService;
        }

        protected void InitToken()
        {
            _cancelTokenSource = new();
            _token = _cancelTokenSource.Token;
        }
        protected void CancelToken() => _cancelTokenSource.Cancel();
    }
}