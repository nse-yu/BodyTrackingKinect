using CommunityToolkit.Mvvm.ComponentModel;

namespace KinectDemo2.Custom.Model.kinect
{
    public class KeyPointsWithScores : ObservableObject
    {
        public enum MoveNetJointKey
        {
            NOSE,
            LEFT_EYE,
            RIGHT_EYE,
            LEFT_EAR,
            RIGHT_EAR,
            LEFT_SHOULDER,
            RIGHT_SHOULDER,
            LEFT_ELBOW,
            RIGHT_ELBOW,
            LEFT_WRIST,
            RIGHT_WRIST,
            LEFT_HIP,
            RIGHT_HIP,
            LEFT_KNEE,
            RIGHT_KNEE,
            LEFT_ANKLE,
            RIGHT_ANKLE,
        }

        public static Dictionary<MoveNetJointKey, string> KEYPOINT_DICT = new()
        {
            { MoveNetJointKey.NOSE, "鼻" },
            { MoveNetJointKey.LEFT_EYE, "左目" },
            { MoveNetJointKey.RIGHT_EYE, "右目" },
            { MoveNetJointKey.LEFT_EAR, "左耳" },
            { MoveNetJointKey.RIGHT_EAR, "右耳" },
            { MoveNetJointKey.LEFT_SHOULDER, "左肩" },
            { MoveNetJointKey.RIGHT_SHOULDER, "右肩" },
            { MoveNetJointKey.LEFT_ELBOW, "左肘" },
            { MoveNetJointKey.RIGHT_ELBOW, "右肘" },
            { MoveNetJointKey.LEFT_WRIST, "左拳" },
            { MoveNetJointKey.RIGHT_WRIST, "右拳" },
            { MoveNetJointKey.LEFT_HIP, "左尻" },
            { MoveNetJointKey.RIGHT_HIP, "右尻" },
            { MoveNetJointKey.LEFT_KNEE, "左膝" },
            { MoveNetJointKey.RIGHT_KNEE, "右膝" },
            { MoveNetJointKey.LEFT_ANKLE, "左踝" },
            { MoveNetJointKey.RIGHT_ANKLE, "右踝" },
        };

        private double x = 0;
        private double y = 0;
        private double score = 0;
        private MoveNetJointKey key;

        public double X
        {
            get => x;
            set => SetProperty(ref x, value);
        }
        public double Y
        {
            get => y;
            set => SetProperty(ref y, value);
        }
        public double Score
        {
            get => score;
            set => SetProperty(ref score, value);
        }
        public MoveNetJointKey Key
        {
            get => key;
            set => SetProperty(ref key, value);
        }
    }
}
