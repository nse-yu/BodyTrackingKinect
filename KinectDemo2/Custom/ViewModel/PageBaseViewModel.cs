using KinectDemo2.Custom.Service;
using System.Windows.Input;

namespace KinectDemo2.Custom.ViewModel
{
    public class PageBaseViewModel : BaseViewModel
    {
        private static readonly KinectService service = (KinectService)App.KinectService;

        public ICommand MenuActivateCameraCommand { get; set; }
        public Action ContextActivateCameraDelegate { get; } = () => 
        {
            var device = service.GetDefaultDevice();
            if (device == null || device.IsActivated) return;
            device.Start();
        };
        public Action ContextStopCameraDelegate { get; } = () => 
        {
            var device = service.GetDefaultDevice();
            if (device == null || !device.IsActivated) return;
            device.Stop();
        };


        public PageBaseViewModel() { }
    }
}
