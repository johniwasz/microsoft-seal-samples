using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using System;
using System.Threading.Tasks;

namespace FitnessTrackerClient.Services
{
    public class FitnessCryptoManagerBGV : FitnessCryptoManager, IFitnessCryptoManagerBGV
    {
        public FitnessCryptoManagerBGV(IOptions<FitnessCryptoConfig> config, IFitnessTrackerApiClient apiClient, ILogger<FitnessCryptoManagerBGV> logger) :base(config, apiClient, logger)
        {
        }

        public async Task SendNewRunAsync(RunEntry<int> newRun)
        {

            var metricsRequest = new RunItem
            {
                Distance = EncryptBase64(newRun.Distance),
                Time = EncryptBase64(newRun.Time)
            };

            string logInfo = LogUtils.RunItemInfo("CLIENT", "SendNewRun", metricsRequest);

            LogInformation(logInfo);

            // Send new run to api
            await _apiClient.AddNewRunningDistanceBGVAsync(metricsRequest);
        }

        public async Task<DecryptedMetricsResponse> GetMetricsAsync()
        {
            // Get encrypted metrics
            var metrics = await _apiClient.GetMetricsBGVAsync();

            DecryptedMetricsResponse response = new DecryptedMetricsResponse();

            string logInfo = LogUtils.SummaryStatisticInfo("CLIENT", "GetMetrics", metrics);
            LogInformation(logInfo);

            // Decrypt the data
            response.TotalRuns = DecryptBase64(metrics.TotalRuns);
            response.TotalDistance = DecryptBase64(metrics.TotalDistance);
            response.TotalHours = DecryptBase64(metrics.TotalTime);
            return response;
        }
        
        public override SchemeType SchemeType => SchemeType.BGV;



    }
}
