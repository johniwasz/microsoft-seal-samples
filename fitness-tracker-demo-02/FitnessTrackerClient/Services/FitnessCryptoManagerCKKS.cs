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
    public class FitnessCryptoManagerCKKS : FitnessCryptoManager, IFitnessCryptoManagerCKKS
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

            DecryptedMetricsAverageResponse response = new DecryptedMetricsAverageResponse();

            string logInfo = LogUtils.SummaryStatisticInfo("CLIENT", "GetMetrics", metrics);
            LogInformation(logInfo);

            Plaintext totalRunsText = DecryptBase64ToPlaintext(metrics.TotalRuns);

            List<double> runList = new List<double>();
            _encoder.Decode(totalRunsText, runList);

            // Decrypt the data
            response.TotalRuns = runList[0];


            List<double> distanceList = new List<double>();
            Plaintext totalDistance = DecryptBase64ToPlaintext(metrics.TotalDistance);
            _encoder.Decode(totalDistance, distanceList);

            response.TotalDistance = distanceList[0];

            List<double> timeList = new List<double>();
            Plaintext totalTime = DecryptBase64ToPlaintext(metrics.TotalTime);
            _encoder.Decode(totalTime, timeList);
            response.TotalSeconds = timeList[0];


            List<double> averageSpeedList = new List<double>();
            Plaintext averageSpeed = DecryptBase64ToPlaintext(metrics.AverageSpeed);
            _encoder.Decode(averageSpeed, averageSpeedList);
            response.AverageSpeed = averageSpeedList[0];

            return response;
        }
                

        public async Task SendNewRunAsync(RunEntry<double> runItem)
        {
            var metricsRequest = new RunItem
            {
                Distance = EncryptBase64(runItem.Distance),
                Time = EncryptBase64(runItem.Time),
                TimeReciprocal = EncryptBase64(1/runItem.Time)
            };

            string logInfo = LogUtils.RunItemInfo("CLIENT", "SendNewRun", metricsRequest);

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
