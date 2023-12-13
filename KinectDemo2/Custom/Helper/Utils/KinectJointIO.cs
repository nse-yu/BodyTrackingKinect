using CommunityToolkit.Maui.Storage;
using KinectDemo2.Custom.Exceptions;
using System.Text;

namespace KinectDemo2.Custom.Helper.Utils
{
    internal class KinectJointIO
    {
        public static async Task<FileSaverResult> SaveJointAsync(string query, string filename, CancellationToken token)
        {
            if (!isCsvFormat(filename)) throw new ArgumentException(Exception5WQuestionsFactory.Init()
                .What($"An invalid argument was passed: {nameof(filename)}").When(DateTime.Now).Where(nameof(SaveJointAsync)).Get(typeof(ArgumentException)));
            
            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(query));
            return await FileSaver.Default.SaveAsync(filename, stream, token);
        }

        private static bool isCsvFormat(string filename) => filename.EndsWith(".csv", StringComparison.OrdinalIgnoreCase);
    }
}
