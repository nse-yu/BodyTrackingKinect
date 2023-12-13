using Microsoft.Azure.Kinect.BodyTracking;
using KinectFrame = Microsoft.Azure.Kinect.BodyTracking.Frame;

namespace KinectDemo2.Custom.Model.kinect
{
    internal class KinectIndexedFrame : IDisposable
    {
        private uint _bodyIndex;
        private KinectFrame _frame;
        public bool IsNoBody { get; private set; } = false;

        public KinectIndexedFrame(uint bodyIndex, KinectFrame frame)
        {
            if (frame.NumberOfBodies == 0) IsNoBody = true;

            _bodyIndex = bodyIndex;
            _frame = frame;
        }

        public KinectFrame GetFrame() => _frame;
        public uint BodyIndex() => _bodyIndex;
        public Skeleton GetSkeleton() => _frame.GetBodySkeleton(_bodyIndex);

        public void Dispose()
        {
            _frame?.Dispose();
        }
    }
}
