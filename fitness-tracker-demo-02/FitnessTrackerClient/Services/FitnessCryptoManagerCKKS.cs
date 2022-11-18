using FitnessTracker.Common;
using FitnessTracker.Common.Models;
using FitnessTracker.Common.Utils;
using FitnessTrackerClient.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Research.SEAL;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace FitnessTrackerClient.Services
{
    public class FitnessCryptoManagerCKKS : FitnessCryptoManager
    {
        private double _scale;

        // TOOD ensure this is disposed.
        private CKKSEncoder _encoder;


        public FitnessCryptoManagerCKKS(IOptions<FitnessCryptoConfig> config, IFitnessTrackerApiClient apiClient, ILogger<FitnessCryptoManagerCKKS> logger) : base(config, apiClient, logger)
        {
        }

        public override SchemeType SchemeType => SchemeType.CKKS;


        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();

            _scale = Math.Pow(2.0, 40);

            _encoder = new CKKSEncoder(_context);
        }


        public async Task<DecryptedMetricsAverageResponse> GetMetricsAverageAsync()
        {
            // Get encrypted metrics
            var metrics = await _apiClient.GetMetricsCKKSAsync();

            string logInfo = LogUtils.SummaryStatisticInfoCKKS("CLIENT", "GetMetrics", metrics);
            LogInformation(logInfo);

            Plaintext totalRunsText = DecryptBase64ToPlaintext(metrics.TotalRuns);

            List<double> runList = new();
            _encoder.Decode(totalRunsText, runList);

            // Decrypt the data
            List<double> distanceList = new();
            Plaintext totalDistance = DecryptBase64ToPlaintext(metrics.TotalDistance);
            _encoder.Decode(totalDistance, distanceList);

            List<double> timeList = new();
            Plaintext totalTime = DecryptBase64ToPlaintext(metrics.TotalTime);
            _encoder.Decode(totalTime, timeList);

            List<double> averageSpeedList = new();
            Plaintext averageSpeed = DecryptBase64ToPlaintext(metrics.AverageSpeed);
            _encoder.Decode(averageSpeed, averageSpeedList);

            return new DecryptedMetricsAverageResponse(runList[0], distanceList[0], timeList[0], averageSpeedList[0]);
        }
                

        public async Task SendNewRunAsync(RunEntry<double> runItem)
        {
            RunItemCKKS metricsRequest = new(
                EncryptBase64(runItem.Distance), 
                EncryptBase64(runItem.Time), 
                EncryptBase64(1 / runItem.Time));

            string logInfo = LogUtils.RunItemInfoCKKS("CLIENT", "SendNewRun", metricsRequest);

            LogInformation(logInfo);

            // Send new run to api
            await _apiClient.AddNewRunningDistanceCKKSAsync(metricsRequest);
        }

        

        protected string EncryptBase64(double value)
        {
            var plaintext = new Plaintext();

            _encoder.Encode(value, _scale, plaintext);

            return EncryptBase64(plaintext);

        }

    }
}
