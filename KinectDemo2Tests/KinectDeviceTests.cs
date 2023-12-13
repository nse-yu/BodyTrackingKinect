using Microsoft.Azure.Kinect.Sensor;

namespace KinectDemo2Tests
{
    public class KinectDeviceTests
    {
        [Fact(DisplayName = "Confirm Kinect is connected")]
        public void ConfirmKinectConnected()
        {
            Assert.True(Device.GetInstalledCount() > 0);
        }

        [Fact(DisplayName = "Load a device.")]
        public void LoadKinect()
        {
            var device = Device.Open();

            Assert.NotNull(device);

            device.Dispose();
        }

        [Fact(DisplayName = "Confirm jack connections.")]
        public void ConfirmJackConnected()
        {
            var device = Device.Open();

            Assert.True(device.SyncInJackConnected);
            Assert.True(device.SyncOutJackConnected);

            device.Dispose();
        }
    }
}
