using System.Text.Json;
using System.Text.RegularExpressions;

namespace KinectDemo2.Custom.Helper.Processing
{
    public partial class StringUtils
    {
        public class LowercaseNamingPolicy : JsonNamingPolicy
        {
            public override string ConvertName(string name) => string.Concat(name[..1].ToLower(), name.AsSpan(1));
        }

        public static bool ContainsJapanese(string input) => JapaneseWord().IsMatch(input);
        public static Tuple<double, double, double> CalculateAlphabetRatios(string input)
        {
            int countA = input.Count(c => c == 'A');
            int countB = input.Count(c => c == 'B');
            int countC = input.Count(c => c == 'C');

            int totalLength = input.Length;

            double ratioA = (double)countA / totalLength;
            double ratioB = (double)countB / totalLength;
            double ratioC = (double)countC / totalLength;

            return new Tuple<double, double, double>(ratioA, ratioB, ratioC);
        }


        [GeneratedRegex("\\p{IsHiragana}|\\p{IsKatakana}|\\p{IsCJKUnifiedIdeographs}")]
        private static partial Regex JapaneseWord();

        [GeneratedRegex("[^A-C]")]
        public static partial Regex AlphabetAtoC();
    }
}
