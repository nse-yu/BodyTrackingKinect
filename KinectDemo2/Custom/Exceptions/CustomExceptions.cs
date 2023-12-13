namespace KinectDemo2.Custom.Exceptions
{
    class NotHandledKinectException : Exception
            {
        public NotHandledKinectException(string message) : base(message) { }
    }
    class NotHandledPythonException : Exception
    {
        public NotHandledPythonException(string message) : base(message) { }
    }
}
