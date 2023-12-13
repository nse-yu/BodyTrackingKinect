using KinectDemo2.Custom.Helper.Processing;
using KinectDemo2.Custom.Model.kinect;
using Microsoft.Azure.Kinect.BodyTracking;
using System.Collections.ObjectModel;
using System.Numerics;

namespace KinectDemo2.Custom.Helper.Utils
{
    internal class PostureReviewResult
    {
        private readonly double score;
        private readonly bool isRightPosture;
        private readonly string part;

        public PostureReviewResult(double score, bool isRightPosture, string part)
        {
            this.score = score;
            this.isRightPosture = isRightPosture;
            this.part = part;
        }

        public double Score { get => score; }
        public bool IsRightPosture { get => isRightPosture; }
        public string Part { get => part; }
    }
    internal class PostureReviewer
    {
        private const double THRESHOLD_BASED_ANGLE_TRIANGLE = 47;
        private const double THRESHOLD_BASED_ANGLE_CROSSING = 90;
        private const double THRESHOLD_BASED_SHOULDER_BALANCE = 10;
        private const double THRESHOLD_BASED_NECK_DEPTH = 2.4;
        private const double THRESHOLD_BASED_NECK_MISALIGNMENT = 20;
        private const double THRESHOLD_SCORE_MEAN = 75;
        private const double SCORE_UPPER = 100.0;
        private const double SCORE_LOWER = 0.0;
        private const double RESTRICTNESS_SCALE = 1.5;

        private const string PART_TRIANGLE_CAMERA = "首の位置";
        private const string PART_HORIZONTAL_CAMERA = "上体の傾き";
        private const string PART_HORIZONTAL_KINECT = "上体の傾き";
        private const string PART_NECK_DEPTH_KINECT = "首の位置";
        private const string PART_NECK_MISALIGNMENT_KINECT = "ストレートネック";


        public static PostureReviewResult TriangleBalanceForCamera(Collection<KinectJoint> joints, double restrictness)
        {
            var nosePos = GetPositionById(joints, JointId.Nose);
            var shoulderLeftPos = GetPositionById(joints, JointId.ShoulderLeft);
            var shoulderRightPos = GetPositionById(joints, JointId.ShoulderRight);

            var topAngle = MyMath.CalculateAngleFront(nosePos, shoulderRightPos, shoulderLeftPos, MyMath.AngleOrientation.Upper);

            var score = CalculateScore(topAngle, THRESHOLD_BASED_ANGLE_TRIANGLE);
            var goodPosture = IsRightPosture(restrictness, score);

            return new(score, goodPosture, PART_TRIANGLE_CAMERA);
        }
        public static PostureReviewResult HorizontalLevelForCamera(Collection<KinectJoint> joints, double restrictness)
        {
            var shoulderLeft = GetPositionById(joints, JointId.ShoulderLeft);
            var shoulderRight = GetPositionById(joints, JointId.ShoulderRight);

            var absError = MyMath.AbsoluteError(shoulderLeft.Y * 1000, shoulderRight.Y * 1000);

            var score = CalculateScore(absError, THRESHOLD_BASED_SHOULDER_BALANCE, penalWeight: .8);
            var goodPosture = IsRightPosture(restrictness, score);

            return new(score, goodPosture, PART_HORIZONTAL_CAMERA);
        }
        public static PostureReviewResult HorizontalLevelForKinect(Collection<KinectJoint> joints, double restrictness)
        {
            var neckY = GetPositionById(joints, JointId.Neck)[1];
            var pelvis = GetPositionById(joints, JointId.Pelvis);
            var shoulderLeft = GetPositionById(joints, JointId.ShoulderLeft);
            var shoulderRight = GetPositionById(joints, JointId.ShoulderRight);

            var vectorVertical = MyMath.CompositionVectors(pelvis, new Vector3(pelvis[0], neckY, 0));
            var vectorHorizontal = MyMath.CompositionVectors(shoulderLeft, shoulderRight);

            var angle = MyMath.CalculateAngle(vectorVertical, vectorHorizontal);
 
            var score = CalculateScore(angle, THRESHOLD_BASED_ANGLE_CROSSING, allowedErrorScale: .3, penalWeight: .9);
            var goodPosture = IsRightPosture(restrictness, score);

            return new(score, goodPosture, PART_HORIZONTAL_KINECT);
        }
        public static PostureReviewResult NeckDepthForKinect(Collection<KinectJoint> joints, double restrictness)
        {
            var neckZ = GetPositionById(joints, JointId.Neck)[2];
            var clavicleLeftZ = GetPositionById(joints, JointId.ClavicleLeft)[2];
            var clavicleRightZ = GetPositionById(joints, JointId.ClavicleRight)[2];

            var neckProtrusion = Math.Min(MyMath.AbsoluteError(neckZ, clavicleLeftZ), MyMath.AbsoluteError(neckZ, clavicleRightZ));

            var score = CalculateScore(neckProtrusion, THRESHOLD_BASED_NECK_DEPTH, allowedErrorScale: 5, penalWeight: .8);
            var goodPosture = IsRightPosture(restrictness, score);

            return new(score, goodPosture, PART_NECK_DEPTH_KINECT);
        }
        public static PostureReviewResult NeckMisalignmentForKinect(Collection<KinectJoint> joints, double restrictness)
        {
            var nose = GetPositionById(joints, JointId.Nose);
            (Vector3 topGravityLine, Vector3 bottomGravityLine) = CalculateCenterOfGravity(joints);
            
            var angle = MyMath.CalculateAngleSide(nose, topGravityLine, bottomGravityLine, MyMath.AngleOrientation.Left);
            var score = CalculateScore(angle - 5, THRESHOLD_BASED_NECK_MISALIGNMENT, 1.7, .8);
            var goolPosture = IsRightPosture(restrictness, score);

            return new(score, goolPosture, PART_NECK_MISALIGNMENT_KINECT);
        }


        private static double CalculateScore(double value, double border, double allowedErrorScale = 2, double penalWeight = 1.0)
        {
            var absError = MyMath.AbsoluteError(value, border);
            var digitCount = MyMath.CalculateDigitCount(border);
            var errorScaled = MyMath.Scaling(absError, border * allowedErrorScale);
            var penal = SCORE_UPPER * errorScaled * penalWeight;
            double score = SCORE_UPPER - penal;
            return score > SCORE_LOWER ? score : SCORE_LOWER;
        }
        private static (Vector3, Vector3) CalculateCenterOfGravity(Collection<KinectJoint> joints)
        {
            // copy x and z
            var chest = GetPositionById(joints, JointId.SpineChest);
            var nose = GetPositionById(joints, JointId.Nose);
            var topGravityLine = new Vector3(chest.X, nose.Y, chest.Z);
            var bottomGravityLine = new Vector3(chest.X, chest.Y, chest.Z);

            return (topGravityLine, bottomGravityLine);
        }


        internal static bool IsRightPosture(double restrictness, params double[] scores)
        {
            return scores.Average() >= THRESHOLD_SCORE_MEAN + restrictness * RESTRICTNESS_SCALE;
        }
        private static Vector3 GetPositionById(Collection<KinectJoint> joints, JointId id) 
            => joints.ToList().Where(j => j.Id == id).Select(j => new Vector3((float)j.PositionX, (float)j.PositionY, (float)j.PositionZ)).Single();
    }
}
