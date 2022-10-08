using FitnessTracker.Common.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessTrackerClient
{
    internal static class FitnessTrackerClient
    {
        private static HttpClient _client = new HttpClient();

        private static readonly string BaseUri = "http://localhost:58849/api";


        internal static async Task AddNewRunningDistance(RunItem metricsRequest)
        {

            var metricsRequestAsJsonStr = JsonSerializer.Serialize(metricsRequest);
      
            using (var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUri}/metrics"))
            using (var content = new StringContent(metricsRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        internal static async Task<SummaryItem> GetMetrics()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUri}/metrics"))
            {
                var response = await _client.SendAsync(request);
                string contentText = await response.Content.ReadAsStringAsync();

                JsonSerializerOptions serOptions = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<SummaryItem>(contentText, serOptions);
            }
        }

        internal static async Task SendPublicKey(PublicKeyModel publicKey)
        {
            var publicKeyRequestAsJsonStr = JsonSerializer.Serialize(publicKey);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUri}/metrics/keys"))
            using (var content = new StringContent(publicKeyRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}