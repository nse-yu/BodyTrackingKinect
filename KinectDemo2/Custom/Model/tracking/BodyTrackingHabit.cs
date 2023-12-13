using static KinectDemo2.Custom.Helper.Processing.StringUtils;

namespace KinectDemo2.Custom.Model.tracking
{
    public partial class BodyTrackingHabit
    {
        private string _code;

        public string UserId { get; set; }
        public string PosturalAbnormalityCode
        {
            get => _code;
            set
            {
                if (_code == value) return;
                _code = AlphabetAtoC().Replace(value, "");
            }
        }
        public DateTime Time { get; set; }

        public bool IsCodePresence() => _code.Length > 0;
        public void InitTime()
        {
            Time = DateTime.Now;
        }
        public void InitCode()
        {
            PosturalAbnormalityCode = "";
        }
        public void Append(string codePart)
        {
            PosturalAbnormalityCode += codePart;
        }
    }
}