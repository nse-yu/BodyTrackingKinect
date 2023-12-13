using Microsoft.Maui.Controls.Platform.Compatibility;

namespace KinectDemo2.Custom.Exceptions
{
    internal class Exception5WQuestionsFactory
    {
        private readonly Exception5WQuestions questions = new();
        public static Exception5WQuestionsFactory Init() => new();
        public Exception5WQuestionsFactory What(string what)
        {
            questions.What(what);
            return this;
        }
        public Exception5WQuestionsFactory When(DateTime when)
        {
            questions.When(when);
            return this;
        }

        public Exception5WQuestions Where(string where)
        {
            questions.Where(where);
            return questions;
        }
    }
    internal class Exception5WQuestions
    {
        private Type Type = typeof(Exception);
        private DateTime when = DateTime.Now;
        private string where = "Unknown";
        private string what = "No Infomation";

        public static Exception5WQuestions Create() => new();
        public Exception5WQuestions When(DateTime when)
        {
            this.when = when;
            return this;
        }
        public Exception5WQuestions What(string what)
        {
            this.what = what;
            return this;
        }

        public Exception5WQuestions Where(string where)
        {
            this.where = where;
            return this;
        }

        public string Get(Type exceptionType)
        {
            return $"\r\nQ. What's this exceptions? -> A. {exceptionType.Name}\r\n" +
                   $"Q. Where was this exception occured? -> A. {where}\r\n" +
                   $"Q. When did this exception occur? -> A. {when}\r\n" +
                   $"Q. Please give a detailed message. -> {what}";
        }
    }
}
