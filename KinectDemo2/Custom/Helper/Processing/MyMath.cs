using System.Linq;
using System.Numerics;
using System.Runtime.Intrinsics.X86;

namespace KinectDemo2.Custom.Helper.Processing
{
    public class MyMath
    {
        public enum AngleOrientation
        {
            Upper,
            Left,
            Right
        }

        public static double CalculateAngleFront(Vector3 upperPoint, Vector3 rightPoint, Vector3 leftPoint, AngleOrientation angleOrientation)
        {
            Vector2 vector1, vector2;

            switch (angleOrientation)
            {
                case AngleOrientation.Upper:
                    vector1 = new Vector2((float)(rightPoint.X - upperPoint.X), (float)(rightPoint.Y - upperPoint.Y));
                    vector2 = new Vector2((float)(leftPoint.X - upperPoint.X), (float)(leftPoint.Y - upperPoint.Y));
                    break;
                case AngleOrientation.Right:
                    vector1 = new Vector2((float)(leftPoint.X - rightPoint.X), (float)(leftPoint.Y - rightPoint.Y));
                    vector2 = new Vector2((float)(upperPoint.X - rightPoint.X), (float)(upperPoint.Y - rightPoint.Y));
                    break;
                case AngleOrientation.Left:
                    vector1 = new Vector2((float)(upperPoint.X - leftPoint.X), (float)(upperPoint.Y - leftPoint.Y));
                    vector2 = new Vector2((float)(rightPoint.X - leftPoint.X), (float)(rightPoint.Y - leftPoint.Y));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid angle orientation: {angleOrientation.GetType()}");
            }

            return CalculateAngle(vector1, vector2);
        }
        public static double CalculateAngleSide(Vector3 upperPoint, Vector3 rightPoint, Vector3 leftPoint, AngleOrientation angleOrientation)
        {
            Vector2 vector1, vector2;

            switch (angleOrientation)
            {
                case AngleOrientation.Upper:
                    vector1 = new Vector2((float)(rightPoint.Z - upperPoint.Z), (float)(rightPoint.Y - upperPoint.Y));
                    vector2 = new Vector2((float)(leftPoint.Z - upperPoint.Z), (float)(leftPoint.Y - upperPoint.Y));
                    break;
                case AngleOrientation.Right:
                    vector1 = new Vector2((float)(leftPoint.Z - rightPoint.Z), (float)(leftPoint.Y - rightPoint.Y));
                    vector2 = new Vector2((float)(upperPoint.Z - rightPoint.Z), (float)(upperPoint.Y - rightPoint.Y));
                    break;
                case AngleOrientation.Left:
                    vector1 = new Vector2((float)(upperPoint.Z - leftPoint.Z), (float)(upperPoint.Y - leftPoint.Y));
                    vector2 = new Vector2((float)(rightPoint.Z - leftPoint.Z), (float)(rightPoint.Y - leftPoint.Y));
                    break;
                default:
                    throw new ArgumentOutOfRangeException($"Invalid angle orientation: {angleOrientation.GetType()}");
            }

            return CalculateAngle(vector1, vector2);
        }
        public static double CalculateAngle(Vector2 vec1, Vector2 vec2)
        {
            var innerProduct = InnerProduct(vec1, vec2);
            var vectorLength1 = VectorLength(vec1);
            var vectorLength2 = VectorLength(vec2);

            double angleRad = Math.Acos(innerProduct / (vectorLength1 * vectorLength2));
            double angleDeg = RadianToDegree(angleRad);

            return angleDeg;
        }


        public static double Scaling(double value, double max) => Math.Abs(value / max);
        public static double AbsoluteError(double value1, double value2) => Math.Abs(value1 - value2);
        public static Vector2 CompositionVectors(Vector3 vec1, Vector3 vec2) => new((float)(vec2.X - vec1.X), (float)(vec2.Y - vec1.Y));
        public static int CalculateDigitCount(double digit) => $"{(int)digit}".Length;
        public static double CalculateMedian(IEnumerable<float> numbers)
        {
            if (!numbers.Any()) return 0;

            var sortedNumbers = numbers.OrderBy(x => x).ToList();
            int count = sortedNumbers.Count;
            int midIndex = count / 2;

            return count%2 == 0 ? (sortedNumbers[midIndex-1] + sortedNumbers[midIndex]) / 2.0 : sortedNumbers[midIndex];
        }
        public static double CalculateSTD(IEnumerable<float> numbers)
        {
            if (!numbers.Any()) return 0;

            double mean = CalculateMean(numbers);
            double sumOfSquaredDifferences = numbers.Sum(x => Math.Pow(x - mean, 2));
            double variance = sumOfSquaredDifferences / numbers.Count();
            double standardDeviation = Math.Sqrt(variance);

            return standardDeviation;
        }
        public static double CalculateMean(IEnumerable<float> numbers)
        {
            if (!numbers.Any()) return 0;
            return numbers.Average();
        }
        public static List<double> CalculateEMA(IEnumerable<double> data, double alpha)
        {
            var emaValues = new List<double>();
            var ema = data.First();

            foreach (var value in data.Skip(1))
            {
                ema = alpha * value + (1 - alpha) * ema;
                emaValues.Add(ema);
            }

            return emaValues;
        }


        private static double InnerProduct(Vector2 vec1, Vector2 vec2) => vec1.X*vec2.X + vec1.Y*vec2.Y;
        private static double VectorLength(Vector2 vec) => Math.Sqrt(vec.X*vec.X + vec.Y*vec.Y);
        private static double RadianToDegree(double angleRadian) => angleRadian * (180.0 / Math.PI);
    }
}
