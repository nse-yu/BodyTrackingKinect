namespace KinectDemo2.Custom.Helper
{
    internal class RatioRect
    {
        private double width = 0.0;
        private double height = 0.0;
        private double ratio = 1.0;

        public RatioRect(double width, double height, double ratio)
        {
            this.width = width;
            this.height = height;
            this.ratio = ratio;
        }

        public double GetWidth() => width * ratio;
        public double GetHeight() => height * ratio;
    }
}
