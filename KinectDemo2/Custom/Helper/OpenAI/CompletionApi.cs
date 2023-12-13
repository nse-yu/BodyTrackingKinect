using KinectDemo2.Custom.Constants;
using System.Text;
using System.Text.Json;
using static KinectDemo2.Custom.Helper.Processing.StringUtils;

namespace KinectDemo2.Custom.Helper.OpenAI
{
    public class CompletionApi
    {
        private readonly static string CONTENT_TYPE = "application/json";

        private HttpClient httpClient = null;
        private string _apiKey = "";

        public double Temperature { get; set; } = .7;
        public int MaxTokens { get; set; } = 100;


        public CompletionApi(string apiKey) 
        {
            if (apiKey == null) throw new ArgumentNullException(nameof(apiKey), "You should specify the API key.");
            if (apiKey.Length == 0) throw new ArgumentException("Please input the collect key.");

            _apiKey = apiKey;
            InitClient(_apiKey);
        }

        public async Task<Choice> GetCompletion(string prompt)
        {
            InitClient(_apiKey);

            var requestData = new
            {
                model = CompletionsApiConstants.API_MODEL,
                messages = new[]{ new { content = prompt, role = Role.User.ToRoleString() }},
                temperature = Temperature,
                max_tokens = MaxTokens
            };

            var options = new JsonSerializerOptions() { PropertyNamingPolicy = new LowercaseNamingPolicy() };
            var content = new StringContent(JsonSerializer.Serialize(requestData, options), Encoding.UTF8, CONTENT_TYPE);
            Choice choice = null;
            try
            {
                var response = await httpClient.PostAsync(CompletionsApiConstants.API_ENDPOINT_URL, content);
                var responseContent = await response.Content.ReadAsStringAsync();
                choice = JsonSerializer.Deserialize<CompletionsResponse>(responseContent, options).Choices[0];
            }
            catch(Exception ex)
            {
                choice = new Choice() { Finish_reason = "Error", Message = new() { Content = ex.Message, Role = Role.Assistant.ToRoleString() } };
            }
            finally
            {
                httpClient?.Dispose();
            }
            return choice;
        }


        private void InitClient(string apiKey)
        {
            if (httpClient != null) return;
            httpClient ??= new();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }
    }

    public class CompletionsResponse
    {
        public string Id {  get; set; }
        public string Object { get; set; }
        public int Created { get; set; }
        public string Model { get; set; }
        public Choice[] Choices { get; set; }
    }
}
