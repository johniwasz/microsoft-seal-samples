using FitnessTracker.Common.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public class FitnessTrackerApiClient : IFitnessTrackerApiClient
    {
        private HttpClient _client;

        public FitnessTrackerApiClient(HttpClient client)
        {
            _client = client;
        }

        public async Task AddNewRunningDistance(RunItem metricsRequest)
        {

            var metricsRequestAsJsonStr = JsonSerializer.Serialize(metricsRequest);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/metrics"))
            using (var content = new StringContent(metricsRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task<SummaryItem> GetMetrics()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/metrics"))
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

        public async Task SendPublicKey(PublicKeyModel publicKey)
        {
            var publicKeyRequestAsJsonStr = JsonSerializer.Serialize(publicKey);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/metrics/keys"))
            using (var content = new StringContent(publicKeyRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}