using FitnessTracker.Common.Models;
using System;
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

        public async Task AddNewRunningDistanceBGVAsync(RunItem metricsRequest)
        {

            var metricsRequestAsJsonStr = JsonSerializer.Serialize(metricsRequest);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/bgv/metrics"))
            using (var content = new StringContent(metricsRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task AddNewRunningDistanceCKKSAsync(RunItem metricsRequest)
        {

            var metricsRequestAsJsonStr = JsonSerializer.Serialize(metricsRequest);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ckks/metrics"))
            using (var content = new StringContent(metricsRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;

                try
                {
                    var response = await _client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());

                }
            }
        }


        public async Task<SummaryItem> GetMetricsBGVAsync()
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/bgv/metrics"))
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


        public async Task<SummaryItem> GetMetricsCKKSAsync()
        {

            using (var request = new HttpRequestMessage(HttpMethod.Get, $"/api/ckks/metrics"))
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

        public async Task SendPublicKeyBGVAsync(PublicKeyModel publicKey)
        {
            var publicKeyRequestAsJsonStr = JsonSerializer.Serialize(publicKey);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/bgv/keys"))
            using (var content = new StringContent(publicKeyRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }

        public async Task SendPublicKeyCKKSAsync(PublicKeyModel publicKey)
        {
            var publicKeyRequestAsJsonStr = JsonSerializer.Serialize(publicKey);

            using (var request = new HttpRequestMessage(HttpMethod.Post, $"/api/ckks/keys"))
            using (var content = new StringContent(publicKeyRequestAsJsonStr, Encoding.UTF8, "application/json"))
            {
                request.Content = content;
                var response = await _client.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}