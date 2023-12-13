using Microsoft.Azure.Kinect.Sensor;
using System.Numerics;
using KinectImageFormat = Microsoft.Azure.Kinect.Sensor.ImageFormat;

namespace KinectDemo2.Custom.Model.kinect
{
    public class KinectDevice
    {
        public static readonly DeviceConfiguration DEVICE_CONFIG = new()
        {
            ColorFormat = KinectImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_Unbinned,
            SynchronizedImagesOnly = true,
        };
        public bool IsActivated { get; private set; } = false;
        public Microsoft.Azure.Kinect.Sensor.Device Device { get; set; }


        public bool Start(DeviceConfiguration configuration = null)
        {
            if (IsActivated) return IsActivated;

            try
            {
                Device.StartCameras(configuration ?? DEVICE_CONFIG);
            }
            catch (Exception)
            {
                return false;
            }
            return IsActivated = true;
        }
        public void Stop()
        {
            if (!IsActivated) return;

            try
            {
                Device.StopCameras();
            }
            catch (ObjectDisposedException)
            {
                return;
            }
            finally
            {
                IsActivated = false;
            }
        }

        public Vector2? TransformColorToDepth(Vector3 vector, CalibrationDeviceType source, CalibrationDeviceType target)
        {
            return Device.GetCalibration().TransformTo2D(vector, source, target);
        }


        public void Dispose()
        {
            Device?.Dispose();
            IsActivated = false;
        }
    }
}
