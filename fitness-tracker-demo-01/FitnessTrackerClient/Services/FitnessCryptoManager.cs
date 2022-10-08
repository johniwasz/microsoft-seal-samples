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
    public class FitnessCryptoManager : IDisposable, IFitnessCryptoManager
    {
        private bool _disposed;
        private Encryptor _encryptor;
        private Decryptor _decryptor;
        private SEALContext _context;
        private KeyGenerator _keyGenerator;
        private IFitnessTrackerApiClient _apiClient;
        private PublicKey _publicKey;
        private IOptions<FitnessCryptoConfig> _config;
        private readonly ILogger<FitnessCryptoManager> _logger;

        public FitnessCryptoManager(IOptions<FitnessCryptoConfig> config, IFitnessTrackerApiClient apiClient, ILogger<FitnessCryptoManager> logger)
        {
            _config = config;
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task InitializeAsync()
        {
            _logger.LogInformation("Initializing encryption");

            _context = SEALUtils.GetContext(_config.Value.PolyModulusDegree);

            _keyGenerator = new KeyGenerator(_context);

            _keyGenerator.CreatePublicKey(out PublicKey publicKey);

            _publicKey = publicKey;

            _logger.LogInformation("Generating public key");
            PublicKeyModel keyModel = new PublicKeyModel();
            keyModel.PublicKey = SEALUtils.PublicKeyToBase64String(_publicKey);

            _logger.LogInformation("Sending public key to API");
            await _apiClient.SendPublicKey(keyModel);

            _encryptor = new Encryptor(_context, _publicKey);

            _decryptor = new Decryptor(_context, _keyGenerator.SecretKey);
        }

        public async Task SendNewRunAsync(RunEntry newRun)
        {

            // We will convert the Int value to Hexadecimal using the ToString("X") method
            var plaintext = new Plaintext($"{newRun.Distance.ToString("X")}");
            var ciphertextDistance = new Ciphertext();
            _encryptor.Encrypt(plaintext, ciphertextDistance);

            // Convert value to base64 string
            var base64Distance = SEALUtils.CiphertextToBase64String(ciphertextDistance);

            // We will convert the Int value to Hexadecimal using the ToString("X") method
            var plaintextTime = new Plaintext($"{newRun.Time.ToString("X")}");
            var ciphertextTime = new Ciphertext();
            _encryptor.Encrypt(plaintextTime, ciphertextTime);

            // Convert value to base64 string
            var base64Time = SEALUtils.CiphertextToBase64String(ciphertextTime);

            var metricsRequest = new RunItem
            {
                Distance = base64Distance,
                Time = base64Time
            };

            string logInfo = LogUtils.RunItemInfo("CLIENT", "SendNewRun", metricsRequest);

            _logger.LogInformation(logInfo);

            // Send new run to api
            await _apiClient.AddNewRunningDistance(metricsRequest);
        }

        public async Task<DecryptedMetricsResponse> GetMetricsAsync()
        {
            // Get encrypted metrics
            var metrics = await _apiClient.GetMetrics();

            DecryptedMetricsResponse response = new DecryptedMetricsResponse();

            string logInfo = LogUtils.SummaryStatisticInfo("CLIENT", "GetMetrics", metrics);
            _logger.LogInformation(logInfo);

            // Decrypt the data
            var ciphertextTotalRuns = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalRuns, _context);
            var plaintextTotalRuns = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalRuns, plaintextTotalRuns);
            response.TotalRuns = plaintextTotalRuns.ToString();

            var ciphertextTotalDistance = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalDistance, _context);
            var plaintextTotalDistance = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalDistance, plaintextTotalDistance);
            response.TotalDistance = plaintextTotalDistance.ToString();

            var ciphertextTotalHours = SEALUtils.BuildCiphertextFromBase64String(metrics.TotalHours, _context);
            var plaintextTotalHours = new Plaintext();
            _decryptor.Decrypt(ciphertextTotalHours, plaintextTotalHours);
            response.TotalHours = plaintextTotalHours.ToString();


            return response;
        }
        
        internal PublicKey PublicKey
        {
            get
            {
                return _publicKey;
            }
        }


        public void Dispose() => Dispose(true);

        public void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _encryptor?.Dispose();
                _decryptor?.Dispose();
                _keyGenerator?.Dispose();
                _publicKey?.Dispose();
                _context?.Dispose();
            }

            _disposed = true;
        }

    }
}
