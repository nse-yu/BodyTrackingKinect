using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Numerics;

namespace KinectDemo2.Custom.Model.kinect
{
    public class KinectJoint : ObservableObject
    {
        private double positionX = 0;
        private double positionY = 0;
        private double positionZ = 0;
        private double quaternionW = 0;
        private double quaternionX = 0;
        private double quaternionY = 0;
        private double quaternionZ = 0;
        private JointId jointId;

        public double PositionX
        {
            get => positionX;
            set => SetProperty(ref positionX, value);
        }
        public double PositionY
        {
            get => positionY;
            set => SetProperty(ref positionY, value);
        }
        public double PositionZ
        {
            get => positionZ;
            set => SetProperty(ref positionZ, value);
        }
        public double QuaternionW
        {
            get => quaternionW;
            set => SetProperty(ref quaternionW, value);
        }
        public double QuaternionX
        {
            get => quaternionX;
            set => SetProperty(ref quaternionX, value);
        }
        public double QuaternionY
        {
            get => quaternionY;
            set => SetProperty(ref quaternionY, value);
        }
        public double QuaternionZ
        {
            get => quaternionZ;
            set => SetProperty(ref quaternionZ, value);
        }
        public JointId Id
        {
            get => jointId;
            set => SetProperty(ref jointId, value);
        }


        public KinectJoint(JointId id)
        {
            Id = id;
        }
        public KinectJoint(JointId id, Vector3 position, Quaternion quaternion) : this(id)
        {
            SetPosition(position);
            SetQuaternion(quaternion);
        }

        public KinectJoint(JointId id, Joint joint) : this(id, joint.Position, joint.Quaternion) { }

        public KinectJoint SetPosition(Vector3 position)
        {
            PositionX = position.X;
            PositionY = position.Y;
            PositionZ = position.Z;
            return this;
        }

        public KinectJoint SetQuaternion(Quaternion quaternion)
        {
            QuaternionW = quaternion.W;
            QuaternionX = quaternion.X;
            QuaternionY = quaternion.Y;
            QuaternionZ = quaternion.Z;
            return this;
        }

        public KinectJoint SetPosition(double X, double Y, double Z)
        {
            PositionX = X;
            PositionY = Y;
            PositionZ = Z;
            return this;
        }
    }
}
