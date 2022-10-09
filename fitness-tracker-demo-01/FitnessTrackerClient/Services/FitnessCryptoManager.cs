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

            var metricsRequest = new RunItem
            {
                Distance = EncryptBase64(newRun.Distance),
                Time = EncryptBase64(newRun.Time)
            };

            string logInfo = LogUtils.RunItemInfo("CLIENT", "SendNewRun", metricsRequest);

            _logger.LogInformation(logInfo);

            // Send new run to api
            await _apiClient.AddNewRunningDistance(metricsRequest);
        }

        private string EncryptBase64(int value)
        {
            var plaintext = new Plaintext(value.ToString("X"));
            var ciphertext = new Ciphertext();
            _encryptor.Encrypt(plaintext, ciphertext);

            // Convert value to base64 string
            return SEALUtils.CiphertextToBase64String(ciphertext);
        }

        public async Task<DecryptedMetricsResponse> GetMetricsAsync()
        {
            // Get encrypted metrics
            var metrics = await _apiClient.GetMetrics();

            DecryptedMetricsResponse response = new DecryptedMetricsResponse();

            string logInfo = LogUtils.SummaryStatisticInfo("CLIENT", "GetMetrics", metrics);
            _logger.LogInformation(logInfo);

            // Decrypt the data
            response.TotalRuns = DecryptBase64(metrics.TotalRuns);
            response.TotalDistance = DecryptBase64(metrics.TotalDistance);
            response.TotalHours = DecryptBase64(metrics.TotalHours);
            return response;
        }


        private string DecryptBase64(string encryptedText)
        {
            var cypherText = SEALUtils.BuildCiphertextFromBase64String(encryptedText, _context);
            var decryptedText = new Plaintext();
            _decryptor.Decrypt(cypherText, decryptedText);
            return decryptedText.ToString();
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
