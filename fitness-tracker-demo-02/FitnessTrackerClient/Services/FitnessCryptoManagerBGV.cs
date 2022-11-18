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
    public class FitnessCryptoManagerBGV : FitnessCryptoManager
    {
        public FitnessCryptoManagerBGV(IOptions<FitnessCryptoConfig> config, IFitnessTrackerApiClient apiClient, ILogger<FitnessCryptoManagerBGV> logger) :base(config, apiClient, logger)
        {
        }

        public async Task SendNewRunAsync(RunEntry<int> newRun)
        {

            var metricsRequest = new RunItemBGV(EncryptBase64(newRun.Distance), EncryptBase64(newRun.Time), null);

            string logInfo = LogUtils.RunItemInfoBGV("CLIENT", "SendNewRun", metricsRequest);

            LogInformation(logInfo);

            // Send new run to api
            await _apiClient.AddNewRunningDistanceBGVAsync(metricsRequest);
        }

        public async Task<DecryptedMetricsResponse> GetMetricsAsync()
        {
            // Get encrypted metrics
            var metrics = await _apiClient.GetMetricsBGVAsync();

            string logInfo = LogUtils.SummaryStatisticInfoBGV("CLIENT", "GetMetrics", metrics);
            LogInformation(logInfo);

            // Decrypt the data           
            return new DecryptedMetricsResponse(
                DecryptBase64(metrics.TotalRuns),
                DecryptBase64(metrics.TotalDistance),
                DecryptBase64(metrics.TotalTime));
        }
        
        public override SchemeType SchemeType => SchemeType.BGV;



    }
}
