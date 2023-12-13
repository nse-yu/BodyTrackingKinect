using KinectDemo2.Custom.Model.kinect;

namespace KinectDemo2.Custom
{
    public class KinectManager
    {
        public List<KinectDevice> Devices { get; set; } = new List<KinectDevice>();

        public KinectManager() { }

        /**Provide a solution to register multiple Azure Kinect devices. */
        public void Add(KinectDevice device) 
        {
            var isDuplicate = Devices.Any(d => d.Equals(device));
            if (isDuplicate) return;
            Devices.Add(device);
        }

        /**Returns the number of connected Azure Kinect devices. */
        public int Count => Devices == null ? 0 : Devices.Count;

        /**Shutdown all devices and remove all instances from the Device queue. */
        public void DisposeAndClearAll() 
        {
            Devices.ForEach(device =>
            {
                device?.Dispose();
            });
            Devices.Clear();
        }

        public void StopCamerasAll() => Devices.ForEach(device => device.Stop());

        public void DisposeAtIndex(int index)
        {
            Devices[index]?.Dispose();
            Devices.RemoveAt(index);
        }
    }
}
