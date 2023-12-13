using KinectDemo2.Custom.Constants;
using KinectDemo2.Custom.Model.tracking;
using KinectDemo2.Custom.Service.Python.ML;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using static KinectDemo2.Custom.Helper.Processing.StringUtils;

namespace KinectDemo2.Custom.Service
{
    public interface IDBService
    {
        ValueTask SaveScores(List<BodyTrackingScore> bodyTrackings);
        ValueTask SaveHabits(List<BodyTrackingHabit> bodyTrackingHabits);
        Task<List<BodyTrackingScore>> GetScoresAsync(long past);
        Task<List<BodyTrackingScore>> GetScoresAsync(DateTime start, DateTime stop);
        Task<List<BodyTrackingScore>> GetScoresAsync(string model, DateTime start, DateTime stop);
        bool IsValidModel(string model);
    }

    public class DBService : IDBService
    {
        private static readonly string MODEL = "kinect";
        private readonly HttpClient _httpClient;

        public DBService() 
        {
            _httpClient = new HttpClient();
        }
        public DBService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        ~DBService() 
        {
            _httpClient.Dispose();
        }

        public async Task<List<BodyTrackingScore>> GetScoresAsync(DateTime start, DateTime stop)
        {
            return await GetScoresAsync(MODEL, start, stop);
        }
        public async Task<List<BodyTrackingScore>> GetScoresAsync(long past)
        {
            ThrowIfInvalidClient();

            string fullURL = $@"{InfluxConstants.URI_PREFIX}/{InfluxConstants.MEASUREMENT_SCORE}/get/{past}";

            return await GetBodyTrackingScoresAsync(fullURL);
        }
        public async Task<List<BodyTrackingScore>> GetScoresAsync(string model, DateTime start, DateTime stop)
        {
            ThrowIfInvalidClient();
            var now = DateTime.Now;
            long startParam = (long)now.Subtract(start).TotalMinutes;
            long stopParam = (long)now.Subtract(stop).TotalMinutes;
            string fullURL = $@"{InfluxConstants.URI_PREFIX}/{InfluxConstants.MEASUREMENT_SCORE}/get?start={startParam}&stop={stopParam}&model={model}";

            return await GetBodyTrackingScoresAsync(fullURL);
        }
        private async Task<List<BodyTrackingScore>> GetBodyTrackingScoresAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            ThrowIfRequestFailed(response);

            var res = await response.Content.ReadFromJsonAsync<List<BodyTrackingScore>>();

            return res;
        }


        private async Task<List<BodyTrackingHabit>> GetBodyTrackingHabitsAsync(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);

            ThrowIfRequestFailed(response);

            var res = await response.Content.ReadFromJsonAsync<List<BodyTrackingHabit>>();

            return res;
        }
        public async Task<List<BodyTrackingHabit>> GetHabitsAsync(long past)
        {
            ThrowIfInvalidClient();

            string fullURL = $@"{InfluxConstants.URI_PREFIX}/{InfluxConstants.MEASUREMENT_HABIT}/get/{past}";

            return await GetBodyTrackingHabitsAsync(fullURL);
        }
        public async Task<List<BodyTrackingHabit>> GetHabitsAsync(DateTime start, DateTime stop)
        {
            ThrowIfInvalidClient();
            var now = DateTime.Now;
            long startParam = (long)now.Subtract(start).TotalMinutes;
            long stopParam = (long)now.Subtract(stop).TotalMinutes;
            string fullURL = $@"{InfluxConstants.URI_PREFIX}/{InfluxConstants.MEASUREMENT_HABIT}/get?start={startParam}&stop={stopParam}";

            return await GetBodyTrackingHabitsAsync(fullURL);
        }
        

        public async ValueTask SaveScores(List<BodyTrackingScore> trackingScores)
        {
            ThrowIfInvalidClient();

            string fullURL = $@"{InfluxConstants.URI_PREFIX}/{InfluxConstants.MEASUREMENT_SCORE}/save";
            var options = new JsonSerializerOptions(){ PropertyNamingPolicy = new LowercaseNamingPolicy() };
            var jsonData = JsonSerializer.Serialize(trackingScores, options);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync(fullURL, content);

            ThrowIfRequestFailed(res);
        }
        public async ValueTask SaveHabits(List<BodyTrackingHabit> trackingHabits)
        {
            ThrowIfInvalidClient();

            string fullURL = $@"{InfluxConstants.URI_PREFIX}/{InfluxConstants.MEASUREMENT_HABIT}/save";
            var options = new JsonSerializerOptions() { PropertyNamingPolicy = new LowercaseNamingPolicy() };
            var jsonData = JsonSerializer.Serialize(trackingHabits, options);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            var res = await _httpClient.PostAsync(fullURL, content);

            ThrowIfRequestFailed(res);
        }

        public bool IsValidModel(string model)
        {
            return ((string[])Enum.GetValues(typeof(MLModel))).Contains(model) || model == "kinect";
        }

        private void ThrowIfRequestFailed(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Failed to complete the HTTP request via \"{response.Headers.Location}\"", null, response.StatusCode);
        }
        private void ThrowIfInvalidClient()
        {
            if (_httpClient == null) throw new NullReferenceException(nameof(_httpClient));
        }
    }
}
