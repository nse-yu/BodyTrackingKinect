using CommunityToolkit.Mvvm.ComponentModel;
using static KinectDemo2.Custom.Model.kinect.KeyPointsWithScores;

namespace KinectDemo2.Custom.Model.kinect
{
    public enum PoseLandmarkKey
    {
        NOSE,
        LEFT_EYE_INNER,
        LEFT_EYE,
        LEFT_EYE_OUTER,
        RIGHT_EYE_INNER,
        RIGHT_EYE,
        RIGHT_EYE_OUTER,
        LEFT_EAR,
        RIGHT_EAR,
        MOUTH_LEFT,
        MOUTH_RIGHT,
        LEFT_SHOULDER,
        RIGHT_SHOULDER,
        LEFT_ELBOW,
        RIGHT_ELBOW,
        LEFT_WRIST,
        RIGHT_WRIST,
        LEFT_PINKY,
        RIGHT_PINKY,
        LEFT_INDEX,
        RIGHT_INDEX,
        LEFT_THUMB,
        RIGHT_THUMB,
        LEFT_HIP,
        RIGHT_HIP,
        LEFT_KNEE,
        RIGHT_KNEE,
        LEFT_ANKLE,
        RIGHT_ANKLE,
        LEFT_HEEL,
        RIGHT_HEEL,
        LEFT_FOOT_INDEX,
        RIGHT_FOOT_INDEX,
    }
    public class PoseLandmark : ObservableObject
    {
        public static Dictionary<MoveNetJointKey, PoseLandmarkKey> MOVENET_TO_POSE = new()
        {
            { MoveNetJointKey.NOSE, PoseLandmarkKey.NOSE },
            { MoveNetJointKey.LEFT_EYE, PoseLandmarkKey.LEFT_EYE },
            { MoveNetJointKey.RIGHT_EYE, PoseLandmarkKey.LEFT_EAR },
            { MoveNetJointKey.LEFT_EAR, PoseLandmarkKey.LEFT_EAR },
            { MoveNetJointKey.RIGHT_EAR, PoseLandmarkKey.RIGHT_EAR },
            { MoveNetJointKey.LEFT_SHOULDER, PoseLandmarkKey.LEFT_SHOULDER },
            { MoveNetJointKey.RIGHT_SHOULDER, PoseLandmarkKey.RIGHT_SHOULDER },
            { MoveNetJointKey.LEFT_ELBOW, PoseLandmarkKey.LEFT_ELBOW },
            { MoveNetJointKey.RIGHT_ELBOW, PoseLandmarkKey.RIGHT_ELBOW },
            { MoveNetJointKey.LEFT_WRIST, PoseLandmarkKey.LEFT_WRIST },
            { MoveNetJointKey.RIGHT_WRIST, PoseLandmarkKey.RIGHT_WRIST },
            { MoveNetJointKey.LEFT_HIP, PoseLandmarkKey.LEFT_HIP },
            { MoveNetJointKey.RIGHT_HIP, PoseLandmarkKey.RIGHT_HIP },
            { MoveNetJointKey.LEFT_KNEE, PoseLandmarkKey.LEFT_KNEE },
            { MoveNetJointKey.RIGHT_KNEE, PoseLandmarkKey.RIGHT_KNEE },
            { MoveNetJointKey.LEFT_ANKLE, PoseLandmarkKey.LEFT_ANKLE },
            { MoveNetJointKey.RIGHT_ANKLE, PoseLandmarkKey.RIGHT_ANKLE },
        };

        private float x = 0;
        private float y = 0;
        private float z = 0;
        private float visibility = 0;
        private float presence = 0;
        private PoseLandmarkKey key;

        public float X
        {
            get => x;
            set => SetProperty(ref x, value);
        }
        public float Y
        {
            get => y;
            set => SetProperty(ref y, value);
        }
        public float Z
        {
            get => z;
            set => SetProperty(ref z, value);
        }
        public float Visibility
        {
            get => visibility;
            set => SetProperty(ref visibility, value);
        }
        public float Presence
        {
            get => presence;
            set => SetProperty(ref presence, value);
        }
        public PoseLandmarkKey Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }
    }
}
